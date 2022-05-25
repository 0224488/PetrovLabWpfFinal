using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PetrovLabWpfFinal;

namespace PetrovLabWpfFinal.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		#region Constructor
		public MainWindowViewModel()
		{
			_calculator = new Models.CalculatorModel();
			_calculator.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
			
			NumberPressCommand = new RelayCommand(OnNumberPressExecute);
			DeleteSymbolCommand = new RelayCommand(OnDeleteSymbolExecute, CanExecuteDeleteSymbol);
			BinaryOperationPressCommand = new RelayCommand(OnBinaryOperationPressExecute);
			UnaryOperationPressCommand = new RelayCommand(OnUnaryOperationPressExecute);
		}
		#endregion

		#region Private members
		private readonly Models.CalculatorModel _calculator;
		private string _display = "0";
		private string _expression = string.Empty;
		private string _firstNumberExpression;
		private string _secondNumberExpression;
		private Operations _lastOperation = Operations.Unknown;
		private string _lastSymbol;
		/// <summary>
		/// необходимость очистки дисплея при следующем вводе символа
		/// </summary>
		private bool _newDisplayRequired;
		#endregion

		#region Public properties
		/// <summary>
		/// первый операнд
		/// </summary>
		public string FirstNumber
		{
			get { return _calculator.FirstNumber; }
			set 
			{ 
				_calculator.FirstNumber = value; 
				OnPropertyChanged(nameof(FirstNumber), nameof(FirstNumberExpression)); 
			}
		}

		/// <summary>
		/// поле выражение
		/// </summary>
		public string Expression
		{
			get { return _expression; }
			set
			{
				_expression = value;
				if(string.IsNullOrEmpty(value))
				{
					FirstNumberExpression = value;
					SecondNumberExpression = value;
				}
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// второй операнд
		/// </summary>
		public string SecondNumber
		{
			get { return _calculator.SecondNumber; }
			set 
			{
				_calculator.SecondNumber = value; 
				OnPropertyChanged(nameof(SecondNumber), nameof(SecondNumberExpression)); 
			}
		}

		/// <summary>
		/// первая часть для вывода в поле выражение
		/// </summary>
		public string FirstNumberExpression
		{
			get { return string.IsNullOrEmpty(_firstNumberExpression) ? FirstNumber : _firstNumberExpression; }
			set { _firstNumberExpression = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// вторая часть для вывода в поле выражение
		/// </summary>
		public string SecondNumberExpression
		{
			get { return string.IsNullOrEmpty(_secondNumberExpression) ? SecondNumber : _secondNumberExpression; }
			set { _secondNumberExpression = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// поле дисплея
		/// </summary>
		public string Display
		{
			get { return _display; }
			protected set
			{
				bool previousCanExecuteDelete = CanExecuteDeleteSymbol(null);

				if(SetProperty(ref _display, value))
				{
					if(previousCanExecuteDelete != CanExecuteDeleteSymbol(null))
						DeleteSymbolCommand.RaiseCanExecuteChanged();
				}
			}
		}

		/// <summary>
		/// текущая операция
		/// </summary>
		public Operations Operation
		{
			get { return _calculator.Operation; }
			set { _calculator.Operation = value; }
		}

		/// <summary>
		/// последняя операция
		/// </summary>
		public Operations LastOperation
		{
			get { return _lastOperation; }
			set
			{
				_lastOperation = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// результат вычисления
		/// </summary>
		public string Result
		{
			get { return _calculator.Result; }
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		#region Commands
		/// <summary>
		/// команда нажатия кнопок чисел
		/// </summary>
		public IRelayCommand NumberPressCommand { get; }

		/// <summary>
		/// команда нажатия кнопки стереть
		/// </summary>
		public IRelayCommand DeleteSymbolCommand { get; }

		/// <summary>
		/// команда нажатия кнопок операций
		/// </summary>
		public IRelayCommand BinaryOperationPressCommand { get; }

		/// <summary>
		/// команда нажатия кнопок унарных операторов
		/// </summary>
		public IRelayCommand UnaryOperationPressCommand { get; }
		#endregion

		//обработка нажатия кнопок цифрового блока
		private void OnNumberPressExecute(object param)
		{
			_lastSymbol = param.ToString();
			switch(param.ToString())
			{
				case "CE":
					if(LastOperation == Operations.Unknown || LastOperation == Operations.Equal)
						FirstNumber = "0";
					else
						SecondNumber = "0";

					Display = "0";

					if(LastOperation == Operations.Equal)
					{
						FirstNumber = string.Empty;
						SecondNumber = string.Empty;
						Expression = string.Empty;
						LastOperation = Operations.Unknown;
						Operation = Operations.Unknown;
					}
					break;
				case "C":
					Display = "0";
					FirstNumber = string.Empty;
					SecondNumber = string.Empty;
					Operation = LastOperation = Operations.Unknown;
					Expression = string.Empty;
					break;
				case "+/-":
					if(Display.StartsWith("-"))
						Display = Display.Remove(0, 1);
					else if(Display != "0")
						Display = $"-{Display}";
					break;
				case ",":
					if(_newDisplayRequired)
					{
						Display = "0,";
					}
					else if(!Display.Contains(","))
						Display += ",";
					break;
				default:
					if(Display == "0" || _newDisplayRequired)
						Display = param.ToString();
					else
						Display += param;
					if(LastOperation == Operations.Equal)
						Expression = string.Empty;
					break;
			}
			_newDisplayRequired = false;
		}

		//обработка нажатия кнопки стирания последнего символа
		private void OnDeleteSymbolExecute(object param)
		{
			if(!Helper.IsOperation(_lastSymbol))
				Display = Display.Length > 1 ? Display.Remove(Display.Length - 1) : "0";
		}

		private bool CanExecuteDeleteSymbol(object param) => Display.Length > 0;

		//обработка нажатия кнопок бинарных операций (+, -, *, /)
		private void OnBinaryOperationPressExecute(object param)
		{
			var currentOperation = Helper.GetOperation(param.ToString());
			if(Helper.IsOperation(_lastSymbol))
			{
				var lastSymbolOperation = Helper.GetOperation(_lastSymbol);
				
				if((lastSymbolOperation == Operations.Add || lastSymbolOperation == Operations.Divide ||
					lastSymbolOperation == Operations.Multiply || lastSymbolOperation == Operations.Substract) && 
					(currentOperation == Operations.Add || currentOperation == Operations.Divide ||
					currentOperation == Operations.Multiply || currentOperation == Operations.Substract))
				{
					// при повторном нажатии клавиши операции, просто заменяем знак операции
					LastOperation = currentOperation;
					Expression = LastOperation == Operations.Equal ?
						GetExpression(Operation) : FirstNumberExpression;
					return;
				}
			}
			_lastSymbol = param.ToString();
			
			try
			{
				if(string.IsNullOrEmpty(FirstNumber) || LastOperation == Operations.Equal)
				{
					if(LastOperation == Operations.Equal && currentOperation == Operations.Equal)
					{
						FirstNumberExpression = FirstNumber;
						_calculator.CalculateResult();

						LastOperation = currentOperation;
						Expression = LastOperation == Operations.Equal ?
							GetExpression(Operation) : FirstNumberExpression;
						if(FirstNumber != Display)
							FirstNumber = Display;
					}
					else
					{
						FirstNumber = FirstNumberExpression = Display;
						SecondNumber = string.Empty;
						Operation = Operations.Unknown;
						LastOperation = currentOperation;
						Expression = GetExpression(Operation);
					}
				}
				else
				{
					if(LastOperation != Operations.Unknown && SecondNumber != Display)
					{
						SecondNumber = SecondNumberExpression = Display;
					}
					Operation = LastOperation;
					_calculator.CalculateResult();

					LastOperation = currentOperation;
					if(LastOperation != Operations.Equal && FirstNumber != Display)
					{
						FirstNumber = FirstNumberExpression = Display;
					}

					Expression = LastOperation == Operations.Equal ? GetExpression(Operation) : FirstNumberExpression;
					if(FirstNumber != Display)
						FirstNumber = Display;
				}
				_newDisplayRequired = true;
			}
			catch(Exception)
			{
				Display = Result;
			}
		}

		//обработка нажатия кнопок унарных операций (%, 1/x, x², √x)
		private void OnUnaryOperationPressExecute(object param)
		{
			_lastSymbol = param.ToString();
			try
			{
				if(string.IsNullOrEmpty(FirstNumber))
				{
					FirstNumber = Display;
				}
				else if(LastOperation != Operations.Unknown && string.IsNullOrEmpty(SecondNumber))
				{
					SecondNumber = SecondNumberExpression = Display;
				}
				Operation = Helper.GetOperation(param.ToString());
				if(Operation == Operations.Percent)
				{
					if(string.IsNullOrEmpty(SecondNumber))
						SecondNumber = "0";
					if(LastOperation == Operations.Multiply || LastOperation == Operations.Divide)
					{
						// first * second / 100
						var fisrtNumber = FirstNumber;
						FirstNumber = 1.ToString();
						_calculator.CalculateResult();
						FirstNumber = fisrtNumber;
					}
					else
					{
						// first +/- (first * second / 100)
						_calculator.CalculateResult();
					}
					if(SecondNumber == "0")
						SecondNumber = string.Empty;
				}
				else
				{
					_calculator.CalculateResult();

					// формируем строку для вывода выражения
					string expr = string.IsNullOrEmpty(SecondNumber) ? FirstNumberExpression : SecondNumberExpression;
					switch(Operation)
					{
						case Operations.Square:
							expr = $"sqr({expr})";
							break;
						case Operations.SquareRoot:
							expr = $"√({expr})";
							break;
						case Operations.PartOfWhole:
							expr = $"1/({expr})";
							break;
					}
					if(string.IsNullOrEmpty(SecondNumber))
						FirstNumberExpression = expr;
					else
						SecondNumberExpression = expr;
				}

				if(string.IsNullOrEmpty(SecondNumber) && FirstNumber != Display)
					FirstNumber = Display;
				else if(SecondNumber != Display)
					SecondNumber = Display;
				Expression = GetExpression(LastOperation);
				_newDisplayRequired = true;
			}
			catch(Exception)
			{
				Display = Result;
			}
		}

		// вычисление выражение для вывода
		private string GetExpression(Operations operation)
		{
			var result = FirstNumberExpression;
			if(!string.IsNullOrEmpty(Helper.OperationToString(operation)) && !string.IsNullOrEmpty(SecondNumberExpression))
			{
				result += $" {Helper.OperationToString(operation)} {SecondNumberExpression}";
			}
			return result;
		}

		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if(Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
			if(PropertyName == nameof(Result))
			{
				Display = Result;
			}
		}

		protected void OnPropertyChanged(params string[] propertyNames)
		{
			if(PropertyChanged != null)
			{
				foreach(var propertyName in propertyNames)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
