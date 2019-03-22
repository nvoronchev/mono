//
// TestUtil.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MonoTests.System.Configuration.Util {
	
	public static class TestUtil {

		public static void RunWithTempFile (Action<string> action)
		{
			var filename = Path.GetTempFileName ();
			
			try {
				File.Delete (filename);
				action (filename);
			} finally {
				if (File.Exists (filename))
					File.Delete (filename);
			}
		}

		// Action<T1,T2> doesn't exist in .NET 2.0
		public delegate void MyAction<T1,T2> (T1 t1, T2 t2);

		public static void RunWithTempFiles (MyAction<string,string> action)
		{
			var file1 = Path.GetTempFileName ();
			var file2 = Path.GetTempFileName ();
			
			try {
				File.Delete (file1);
				File.Delete (file2);
				action (file1, file2);
			} finally {
				if (File.Exists (file1))
					File.Delete (file1);
				if (File.Exists (file2))
					File.Delete (file2);
			}
		}

		public static string ThisApplicationPath {
			get {
				var asm = Assembly.GetEntryAssembly ();
				return asm.Location;
			}
		}

		public static string ThisConfigFileName {
			get {
				var exe = Path.GetFileName (ThisApplicationPath);
				return exe + ".config";
			}
		}

		public static global::System.Configuration.Configuration WriteXmlToFileAndOpenConfiguration (string rawXmlConfig, string filePath)
		{
			File.WriteAllText (filePath, rawXmlConfig.Trim ());
			return OpenConfiguration (filePath);
		}

		public static global::System.Configuration.Configuration OpenConfiguration (string filePath)
		{
			var map = new ExeConfigurationFileMap {
				ExeConfigFilename = filePath
			};
			return ConfigurationManager.OpenMappedExeConfiguration (map, ConfigurationUserLevel.None);
		}
	}
}

