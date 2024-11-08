using OptimizationIssues.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (TaskAllocationViewModel)DataContext;
            viewModel.NumberOfResources = int.Parse(NumberOfResourcesTextBox.Text);
            viewModel.NumberOfTasks = int.Parse(NumberOfTasksTextBox.Text);
            viewModel.CostMatrix = ParseCostMatrix(CostMatrixTextBox.Text);

            var (minCost, maxValue) = viewModel.SolveTaskAllocation();
            ResultTextBlock.Text = $"Minimalny koszt: {minCost}\nMaksymalna wartość: {maxValue}";
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
    }
}