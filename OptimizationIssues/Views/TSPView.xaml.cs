using OptimizationIssues.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OptimizationIssues.Views
{
    /// <summary>
    /// Interaction logic for TSPView.xaml
    /// </summary>
    public partial class TSPView : UserControl
    {
        public TSPView()
        {
            InitializeComponent();
            DataContext = new TSPViewModel();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (TSPViewModel)DataContext;
            viewModel.NumberOfCities = int.Parse(NumberOfCitiesTextBox.Text);
            viewModel.DistanceMatrix = ParseDistanceMatrix(DistanceMatrixTextBox.Text);

            int result = viewModel.SolveTravelingSalesmanProblem();
            MessageBox.Show($"Minimalna długość trasy: {result}");
        }

        private static List<List<int>> ParseDistanceMatrix(string input)
        {
            var rows = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var distanceMatrix = new List<List<int>>();

            foreach (var row in rows)
            {
                var values = row.Split(',').Select(int.Parse).ToList();
                distanceMatrix.Add(values);
            }

            return distanceMatrix;
        }
    }
}
