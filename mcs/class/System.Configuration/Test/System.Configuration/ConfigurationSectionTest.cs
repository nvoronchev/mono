//
// System.Configuration.ConfigurationSectionTest.cs - Unit tests
//
// Author:
//	Greg Smolyn
//	Gonzalo Paniagua Javier <gonzalo@novell.com
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Configuration;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace MonoTests.System.Configuration {
	[TestFixture]
	public class ConfigurationElementCollectionTest
	{
		static readonly string TestTempDir = Path.GetFullPath (Path.Combine("Test", "runtime-tmp"));
		static readonly string TestAppExePath = Path.Combine(TestTempDir, "app.exe");
		static readonly string TestAppExeConfigPath = TestAppExePath + ".config";

		[SetUp]
		public void Setup ()
		{
			var config = @"
<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <configSections>
       <section name=""DefaultConfigSection"" type=""System.Configuration.DefaultSection"" />
    </configSections>
    <DefaultConfigSection>
    </DefaultConfigSection>
</configuration>".Trim ();

			Directory.CreateDirectory (TestTempDir);
			File.WriteAllText (TestAppExePath, String.Empty);  // Fake exe-file.
			File.WriteAllText (TestAppExeConfigPath, config);
		}

		[Test]
		public void TwoConfigElementsInARow () // Bug #521231
		{
			string config = @"<fooconfig><foos><foo id=""1"" /></foos><bars><bar id=""1"" /></bars></fooconfig>";
			var fooSection = new FooConfigSection ();
			fooSection.Load (config);
		}

		[Test]
		public void GetRawXmlTest ()
		{
			var config = ConfigurationManager.OpenExeConfiguration (TestAppExePath);
			var section = config.Sections["DefaultConfigSection"] as DefaultSection;
			var rawXml = section.SectionInformation.GetRawXml ();

			Assert.IsNotNull (rawXml, "#1: : GetRawXml() returns null");
			Assert.IsFalse (string.IsNullOrEmpty (rawXml), "#2: GetRawXml() returns String.Empty");
		}
	
		class FooConfigSection : ConfigurationSection
		{
			public void Load (string xml) 
			{ 
				Init (); 
				using (StringReader sr = new StringReader(xml))  
					using (XmlReader reader = new XmlTextReader(sr)) { 
						DeserializeSection (reader); 
					} 
			}

			[ConfigurationProperty("foos")]
			[ConfigurationCollection(typeof(FooConfigElementCollection), AddItemName="foo")]
			public FooConfigElementCollection Foos {
				get { return (FooConfigElementCollection)base["foos"]; }
				set { base["foos"] = value; }
			}

			[ConfigurationProperty("bars")]
			[ConfigurationCollection(typeof(BarConfigElementCollection), AddItemName="bar")]
			public BarConfigElementCollection Bars {
				get { return (BarConfigElementCollection)base["bars"]; }
				set { base["bars"] = value; }
			}			
		}

		class FooConfigElementCollection : ConfigurationElementCollection
		{
			protected override ConfigurationElement CreateNewElement ()
			{
				return new FooConfigElement();
			}

			protected override object GetElementKey (ConfigurationElement element)
			{
				return ((FooConfigElement)element).Id;
			}
		}

		class FooConfigElement : ConfigurationElement
		{
			[ConfigurationProperty("id")]
			public int Id {
				get { return (int)base["id"]; }
				set { base["id"] = value; }
			}

		}

		class BarConfigElementCollection : ConfigurationElementCollection
		{
			protected override ConfigurationElement CreateNewElement ()
			{
				return new BarConfigElement();
			}

			protected override object GetElementKey (ConfigurationElement element)
			{
				return ((BarConfigElement)element).Id;
			}
		}

		class BarConfigElement : ConfigurationElement
		{
			[ConfigurationProperty("id")]
			public int Id {
				get { return (int)base["id"]; }
				set { base["id"] = value; }
			}
		}
	}
}

