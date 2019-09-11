using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rekenmachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        private string _rightOperand;
        private string _leftOperand;
        private string _operator;
        private string _number;
        private string _command;
        private string _prevCommand;
        string[] operators = new[] { "+", "-", "*", "/", };
        string[] numbers = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", };
        private Stack<string> _expression = new Stack<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EvalExpression()
        {
            if (_expression.Count == 3)//there is an expression in the stack that can and must be evaluated
            {
                _rightOperand = _expression.Pop();
                _operator = _expression.Pop();
                _leftOperand = _expression.Pop();

                string sum = "";
                switch (_operator)
                {

                    case "+":
                        sum = (float.Parse(_leftOperand) + float.Parse(_rightOperand)).ToString();
                        break;

                    case "-":
                        sum = (float.Parse(_leftOperand) - float.Parse(_rightOperand)).ToString();
                        break;

                    case "*":
                        sum = (float.Parse(_leftOperand) * float.Parse(_rightOperand)).ToString();
                        break;

                    case "/":
                        sum = (float.Parse(_leftOperand) / float.Parse(_rightOperand)).ToString();
                        break;

                    default:
                        Output.Text = _number;
                        break;
                }

                Output.Text = sum;
                _expression.Push(sum);
                _number = null;
            }

        }
        private void Button_Is_Click(object sender, RoutedEventArgs e)
        {
            _expression.Push(_number);
            EvalExpression();
        }

        private void Button_Op_Click(object sender, RoutedEventArgs e)
        {
            Button passedButton = sender as Button;
            _command = passedButton.Content.ToString();

            if (!string.IsNullOrEmpty(_number))
            {
                _expression.Push(_number);
            }

            EvalExpression();

            if (operators.Contains(_command))
            {
                if (operators.Contains(History.Text.Substring(History.Text.Length - _command.Length)))
                {
                    History.Text = History.Text.Remove(History.Text.Length - _command.Length);
                    _expression.Pop();
                }
                History.Text += _command;
                _expression.Push(_command);
            }
            _number = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button passedButton = sender as Button;
            _command = passedButton.Content.ToString();

            if ("CE".Equals(_command))
            {
                if (_expression.Count > 0) { _expression.Clear(); }
                _number = null;
                _command = null;
                _prevCommand = null;
                Output.Text = "";
                History.Text = "";
            }
            else
            {
                if (numbers.Contains(_command))
                {
                    _number += _command;
                    Output.Text = _number;
                    History.Text += _command;
                }
                else
                {
                    if (_expression.Count > 0)
                    {
                        _expression.Pop();
                        History.Text = History.Text.Remove(History.Text.Length - _command.Length);
                        History.Text += _command;
                    }
                    _expression.Push(_command);
                }
            }
            _prevCommand = _command;
        }
    }
}
