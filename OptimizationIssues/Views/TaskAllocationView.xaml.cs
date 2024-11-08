#pragma warning disable CS8629
using OptimizationIssues.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OptimizationIssues.Views
{
    /// <summary>
    /// Interaction logic for TaskAllocationView.xaml
    /// </summary>
    public partial class TaskAllocationView : UserControl
    {
        public TaskAllocationView()
        {
            InitializeComponent();
            DataContext = new TaskAllocationViewModel();
            ValidateInputs(out _, out _, out _);
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = (TaskAllocationViewModel)DataContext;

                if (ValidateInputs(out var numberOfResources, out var numberOfTasks, out var costMatrix))
                {
                    viewModel.NumberOfResources = numberOfResources;
                    viewModel.NumberOfTasks = numberOfTasks;
                    viewModel.CostMatrix = costMatrix;

                    var (minCost, maxValue) = viewModel.SolveTaskAllocation();
                    ResultTextBlock.Text = $"Minimalny koszt: {minCost}\nMaksymalna wartość: {maxValue}";
                }
                else
                    ResultTextBlock.Text = "Podano błędne dane. Upewnij się, że wszystkie pola są poprawnie wypełnione.";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = "Program napotkał błąd. Sprawdź czy podane przez Ciebie dane są prawidłowe.";
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private List<List<int>> ParseCostMatrix(string input)
        {
            var rows = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var costMatrix = new List<List<int>>();

            foreach (var row in rows)
            {
                var values = row.Split(',').Select(int.Parse).ToList();
                costMatrix.Add(values);
            }

            return costMatrix;
        }
        private bool ValidateInputs(out int numberOfResources, out int numberOfTasks, out List<List<int>> costMatrix)
        {
            bool isValid = true;

            numberOfResources = 0;
            numberOfTasks = 0;
            costMatrix = new List<List<int>>();

            if (string.IsNullOrWhiteSpace(NumberOfResourcesTextBox.Text) || !int.TryParse(NumberOfResourcesTextBox.Text, out numberOfResources) || numberOfResources <= 0)
            {
                isValid = false;
                NumberOfResourcesTextBox.BorderBrush = Brushes.Red;
                NumberOfResourcesTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                NumberOfResourcesTextBox.BorderBrush = Brushes.Black;
                NumberOfResourcesTextBox.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(NumberOfTasksTextBox.Text) || !int.TryParse(NumberOfTasksTextBox.Text, out numberOfTasks) || numberOfTasks <= 0)
            {
                isValid = false;
                NumberOfTasksTextBox.BorderBrush = Brushes.Red;
                NumberOfTasksTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                NumberOfTasksTextBox.BorderBrush = Brushes.Black;
                NumberOfTasksTextBox.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(CostMatrixTextBox.Text) || !TryParseCostMatrix(CostMatrixTextBox.Text, out costMatrix))
            {
                isValid = false;
                CostMatrixTextBox.BorderBrush = Brushes.Red;
                CostMatrixTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                CostMatrixTextBox.BorderBrush = Brushes.Black;
                CostMatrixTextBox.BorderThickness = new Thickness(1);
            }

            return isValid;
        }
        private bool TryParseCostMatrix(string input, out List<List<int>> costMatrix)
        {
            costMatrix = new List<List<int>>();
            var rows = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var row in rows)
                {
                    var values = row.Split(',').Select(str => int.TryParse(str.Trim(), out var number) ? number : (int?)null)
                                    .Where(num => num.HasValue)
                                    .Select(num => num.Value)
                                    .ToList();

                    if (values.Count == 0)
                        return false;

                    costMatrix.Add(values);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void InputsChanged(object sender, TextChangedEventArgs e)
        {
            SolveButton.IsEnabled = ValidateInputs(out _, out _, out _);
        }
    }
}