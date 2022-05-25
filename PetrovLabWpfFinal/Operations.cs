using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovLabWpfFinal
{
	public enum Operations
	{
		Unknown,
		Add,
		Divide,
		Equal,
		Multiply,
		PartOfWhole,
		Percent,
		Square,
		SquareRoot,
		Substract
	}

	public static class Helper
	{
		public static Operations GetOperation(string operation)
		{
			switch(operation)
			{
				case "+":
					return Operations.Add;
				case "÷":
					return Operations.Divide;
				case "=":
					return Operations.Equal;
				case "x":
					return Operations.Multiply;
				case "1/x":
					return Operations.PartOfWhole;
				case "%":
					return Operations.Percent;
				case "x²":
					return Operations.Square;
				case "√x":
					return Operations.SquareRoot;
				case "-":
					return Operations.Substract;
				default:
					return Operations.Unknown;
			}
		}

		public static bool IsOperation(string symbol)
		{
			return GetOperation(symbol) != Operations.Unknown;
		}

		public static string OperationToString(Operations operation)
		{
			switch(operation)
			{
				case Operations.Add:
					return "+";
				case Operations.Divide:
					return "÷";
				case Operations.Equal:
					return "=";
				case Operations.Multiply:
					return "x";
				/*case "1/x":
					return Operations.PartOfWhole;
				case "%":
					return Operations.Percent;
				case "x²":
					return Operations.Square;
				case "√x":
					return Operations.SquareRoot;*/
				case Operations.Substract:
					return "-";
				default:
					return string.Empty;
			}
		}
	}
}
