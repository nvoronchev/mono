Imports System
Module DecimalLiteral
	Sub Main()
		Try
			Dim a As Decimal=1.23E+40D
			     		                                                                                   	     
		Catch e As Exception
			Console.WriteLine(e.Message)
		End Try
	End Sub
End Module
