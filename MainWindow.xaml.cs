
using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Globalization;

namespace EngineeringCalculator
{
    public partial class MainWindow : Window
    {
        private string currentInput = "";
        private string expression = "";
        private bool lastInputWasOperator = false;
        private bool lastInputWasDecimal = false;
        private bool lastInputWasFunction = false;
        private bool calculated = false;
        private int openParenthesesCount = 0;
        private bool waitingForSecondOperand = false;
        private string pendingOperation = "";

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            ResultDisplay.Text = currentInput;
            ExpressionDisplay.Text = expression;
        }

        private void ClearAll()
        {
            currentInput = "";
            expression = "";
            lastInputWasOperator = false;
            lastInputWasDecimal = false;
            lastInputWasFunction = false;
            calculated = false;
            openParenthesesCount = 0;
            waitingForSecondOperand = false;
            pendingOperation = "";
            UpdateDisplays();
        }

        private void Backspace()
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                if (currentInput.EndsWith(","))
                {
                    lastInputWasDecimal = false;
                }
                currentInput = currentInput.Remove(currentInput.Length - 1);
                UpdateDisplays();
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            var button = (Button)sender;
            currentInput += button.Content.ToString();
            lastInputWasOperator = false;
            lastInputWasFunction = false;
            UpdateDisplays();
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            if (!lastInputWasDecimal && !currentInput.Contains(","))
            {
                if (string.IsNullOrEmpty(currentInput)) currentInput = "0";
                currentInput += ",";
                lastInputWasDecimal = true;
                lastInputWasOperator = false;
                lastInputWasFunction = false;
                UpdateDisplays();
            }
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                if (currentInput.StartsWith("-"))
                {
                    currentInput = currentInput.Substring(1);
                }
                else
                {
                    currentInput = "-" + currentInput;
                }
                UpdateDisplays();
            }
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                expression = "";
                calculated = false;
            }

            var button = (Button)sender;
            string op = button.Content.ToString();

            if (!string.IsNullOrEmpty(currentInput))
            {
                expression += currentInput + " " + op + " ";
                currentInput = "";
                lastInputWasOperator = true;
                lastInputWasDecimal = false;
                lastInputWasFunction = false;
            }
            else if (!lastInputWasOperator && !string.IsNullOrEmpty(expression))
            {
                expression = expression.TrimEnd();
                if (expression.Length > 0)
                {
                    expression = expression.Remove(expression.Length - 1);
                    expression += op + " ";
                    lastInputWasOperator = true;
                    lastInputWasDecimal = false;
                    lastInputWasFunction = false;
                }
            }

            UpdateDisplays();
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            var button = (Button)sender;
            string function = button.Content.ToString();

            if (string.IsNullOrEmpty(currentInput))
            {
                expression += function + "(";
                openParenthesesCount++;
                lastInputWasFunction = true;
                lastInputWasOperator = false;
                lastInputWasDecimal = false;
            }
            else
            {
                expression += function + "(" + currentInput + ")";
                currentInput = "";
                lastInputWasFunction = true;
                lastInputWasOperator = false;
                lastInputWasDecimal = false;
            }

            UpdateDisplays();
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            var button = (Button)sender;
            string operation = button.Content.ToString();

            if (!string.IsNullOrEmpty(currentInput))
            {
                switch (operation)
                {
                    case "x²":
                        expression += "(" + currentInput + ")²";
                        currentInput = "";
                        break;
                    case "1/x":
                        expression += "1/(" + currentInput + ")";
                        currentInput = "";
                        break;
                    case "|x|":
                        expression += "|" + currentInput + "|";
                        currentInput = "";
                        break;
                    case "n!":
                        expression += currentInput + "!";
                        currentInput = "";
                        break;
                    case "√x":
                        expression += "√(" + currentInput + ")";
                        currentInput = "";
                        break;
                    case "x^y":
                        waitingForSecondOperand = true;
                        pendingOperation = "^";
                        expression = currentInput + " ^ ";
                        currentInput = "";
                        break;
                    case "10^x":
                        expression += "10^(" + currentInput + ")";
                        currentInput = "";
                        break;
                }
            }
            else if (operation == "x^y")
            {
                waitingForSecondOperand = true;
                pendingOperation = "^";
                expression += " ^ ";
                currentInput = "";
            }

            UpdateDisplays();
        }

        private long Factorial(int n)
        {
            return n <= 1 ? 1 : n * Factorial(n - 1);
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            var button = (Button)sender;
            string parenthesis = button.Content.ToString();

            if (parenthesis == "(")
            {
                expression += "(";
                openParenthesesCount++;
                lastInputWasOperator = false;
                lastInputWasFunction = false;
            }
            else if (parenthesis == ")" && openParenthesesCount > 0)
            {
                expression += string.IsNullOrEmpty(currentInput) ? ")" : currentInput + ")";
                currentInput = "";
                openParenthesesCount--;
                lastInputWasOperator = false;
                lastInputWasFunction = false;
            }

            UpdateDisplays();
        }

        private void ConstantButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculated)
            {
                ClearAll();
                calculated = false;
            }

            var button = (Button)sender;
            string constant = button.Content.ToString();

            currentInput = constant == "π" ?
                Math.PI.ToString(CultureInfo.InvariantCulture) :
                Math.E.ToString(CultureInfo.InvariantCulture);

            lastInputWasOperator = false;
            lastInputWasFunction = false;
            UpdateDisplays();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            Backspace();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (waitingForSecondOperand && !string.IsNullOrEmpty(currentInput))
            {
                try
                {
                    expression += currentInput;
                    string fullExpression = expression;
                    fullExpression = fullExpression.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture))
                                                .Replace("×", "*")
                                                .Replace("÷", "/")
                                                .Replace(",", ".")
                                                .Replace("^", "Math.Pow")
                                                .Replace("(", "(")
                                                .Replace(")", ")");

                    // Обработка степеней - добавляем запятую между аргументами
                    if (fullExpression.Contains("Math.Pow"))
                    {
                        fullExpression = fullExpression.Replace("Math.Pow ", "Math.Pow(")
                                                     .Replace(" ", ",") + ")";
                    }

                    var result = Evaluate(fullExpression);
                    expression += " =";
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    waitingForSecondOperand = false;
                    pendingOperation = "";
                    calculated = true;
                }
                catch
                {
                    currentInput = "Ошибка";
                }
                UpdateDisplays();
                return;
            }

            if (!string.IsNullOrEmpty(currentInput) || !string.IsNullOrEmpty(expression))
            {
                try
                {
                    string fullExpression = expression + currentInput;

                    while (openParenthesesCount > 0)
                    {
                        fullExpression += ")";
                        openParenthesesCount--;
                    }

                    fullExpression = fullExpression.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture))
                                                .Replace("×", "*")
                                                .Replace("÷", "/")
                                                .Replace(",", ".")
                                                .Replace("sin(", "Math.Sin(")
                                                .Replace("cos(", "Math.Cos(")
                                                .Replace("tg(", "Math.Tan(")
                                                .Replace("log(", "Math.Log10(")
                                                .Replace("ln(", "Math.Log(")
                                                .Replace("√(", "Math.Sqrt(")
                                                .Replace(")²", "^2)")
                                                .Replace("!", "")
                                                .Replace("|", "");

                    // Обработка факториала
                    if (fullExpression.Contains("!"))
                    {
                        int factIndex = fullExpression.IndexOf("!");
                        string beforeFact = fullExpression.Substring(0, factIndex);
                        string afterFact = fullExpression.Substring(factIndex + 1);
                        double number = double.Parse(beforeFact, CultureInfo.InvariantCulture);
                        if (number >= 0 && number <= 20 && number == Math.Floor(number))
                        {
                            fullExpression = Factorial((int)number).ToString(CultureInfo.InvariantCulture) + afterFact;
                        }
                        else
                        {
                            throw new Exception("Неверное число факториала");
                        }
                    }

                    var result = Evaluate(fullExpression);
                    expression = expression + currentInput + " =";
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    calculated = true;
                }
                catch
                {
                    currentInput = "Ошибка";
                }
                UpdateDisplays();
            }
        }

        private double Evaluate(string expression)
        {
            DataTable dt = new DataTable();
            var result = dt.Compute(expression, null);
            return Convert.ToDouble(result);
        }
    }
}