using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetrovLabWpfFinal.Models
{
    /// <summary>
    /// Класс модели калькулятора
    /// </summary>
	public class CalculatorModel : INotifyPropertyChanged
	{
        #region Private members
        private string _result;
        #endregion

        #region Constructors
        public CalculatorModel()
        {
            FirstNumber = string.Empty;
            SecondNumber = string.Empty;
            Operation = Operations.Unknown;
            _result = string.Empty;
        }

        public CalculatorModel(string firstOperand, string secondOperand, Operations operation)
        {
            ValidateNumber(firstOperand);
            ValidateNumber(secondOperand);
            //ValidateOperation(operation);

            FirstNumber = firstOperand;
            SecondNumber = secondOperand;
            Operation = operation;
            _result = string.Empty;
        }

        public CalculatorModel(string firstOperand, string operation) : this(firstOperand, operation, Operations.Unknown)
        {
        }
        #endregion

        #region Public properties
        /// <summary>
        /// первый операнд
        /// </summary>
        public string FirstNumber { get; set; }

        /// <summary>
        /// второй операнд
        /// </summary>
        public string SecondNumber { get; set; }

        /// <summary>
        /// операция
        /// </summary>
        public Operations Operation { get; set; }

        /// <summary>
        /// результат
        /// </summary>
        public string Result 
        {
            get { return _result; } 
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Public methods
        /// <summary>
        /// вычисление результата
        /// </summary>
        public void CalculateResult()
        {
            ValidateData();

            try
            {
                switch(Operation)
                {
                    case Operations.Add:
                        _result = (Convert.ToDouble(FirstNumber) + Convert.ToDouble(SecondNumber)).ToString();
                        break;
                    case Operations.Substract:
                        _result = (Convert.ToDouble(FirstNumber) - Convert.ToDouble(SecondNumber)).ToString();
                        break;
                    case Operations.Multiply:
                        _result = (Convert.ToDouble(FirstNumber) * Convert.ToDouble(SecondNumber)).ToString();
                        break;
                    case Operations.Divide:
						_result = (Convert.ToDouble(FirstNumber) / Convert.ToDouble(SecondNumber)).ToString();
                        break;
                    case Operations.Percent:
						_result = (Convert.ToDouble(FirstNumber) * Convert.ToDouble(SecondNumber) / 100).ToString();
						break;
                    case Operations.SquareRoot:
						_result = string.IsNullOrEmpty(SecondNumber) ?
							Math.Sqrt(Convert.ToDouble(FirstNumber)).ToString() : 
                            Math.Sqrt(Convert.ToDouble(SecondNumber)).ToString();
                        break;
                    case Operations.Square:
                        _result = string.IsNullOrEmpty(SecondNumber) ?
                            Math.Pow(Convert.ToDouble(FirstNumber), 2).ToString() :
                            Math.Pow(Convert.ToDouble(SecondNumber), 2).ToString();
                        break;
                    case Operations.PartOfWhole:
						_result = string.IsNullOrEmpty(SecondNumber) ? 
                            (1.0 / Convert.ToDouble(FirstNumber)).ToString() :
                            (1.0 / Convert.ToDouble(SecondNumber)).ToString();
						break;
                }
            }
            catch(Exception)
            {
                _result = "Ошибка вычисления";
                throw;
            }
			finally
			{
                OnPropertyChanged(nameof(Result));
            }
        }
        #endregion

        #region Private methods
        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        // проверка числа на правильность
		private void ValidateNumber(string number)
		{
			if(!double.TryParse(number, out _))
            {
                _result = $"Неверное число: {number}";
                throw new ArgumentException(_result);
            }
        }

        // проверка данных на правильность
        private void ValidateData()
        {
            //ValidateOperation(Operation);
            switch(Operation)
            {
                case Operations.Add:
                case Operations.Divide:
                case Operations.Multiply:
                case Operations.Substract:
                    ValidateNumber(FirstNumber);
                    ValidateNumber(SecondNumber);
                    break;
                case Operations.PartOfWhole:
                case Operations.Percent:
                case Operations.Square:
                case Operations.SquareRoot:
                    ValidateNumber(FirstNumber);
                    break;
            }
            switch(Operation)
			{
                case Operations.Divide:
                    if(Convert.ToDouble(SecondNumber) == 0)
                    {
                        _result = "Ошибка деления на 0";
                        throw new DivideByZeroException(_result);
                    }
                    break;
                case Operations.PartOfWhole:
                    if(Convert.ToDouble(FirstNumber) == 0)
                    {
                        _result = "Ошибка деления на 0";
                        throw new DivideByZeroException(_result);
                    }
                    break;
			}
        }
        #endregion
    }
}
