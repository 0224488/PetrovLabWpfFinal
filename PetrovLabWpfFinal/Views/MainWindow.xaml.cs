using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PetrovLabWpfFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModels.MainWindowViewModel();
        }
	}

    /// <summary>
    /// конвертер для отображения операции в виде строки
    /// </summary>
    [ValueConversion(typeof(Operations), typeof(string))]
    public class OperationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Operations operation)
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
                    case Operations.Substract:
                        return "-";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
