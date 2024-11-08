using OptimizationIssues.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OptimizationIssues.Views
{
    /// <summary>
    /// Interaction logic for KnapsackView.xaml
    /// </summary>
    public partial class KnapsackView : UserControl
    {
        public KnapsackView()
        {
            InitializeComponent();
            DataContext = new KnapsackViewModel();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (KnapsackViewModel)DataContext;
            viewModel.KnapsackCapacity = int.Parse(CapacityTextBox.Text);
            viewModel.Weights = WeightsTextBox.Text.Split(',').Select(int.Parse).ToList();
            viewModel.Values = ValuesTextBox.Text.Split(',').Select(int.Parse).ToList();

            int result = viewModel.SolveKnapsack();
            //MessageBox.Show($"Wynik rozwiązania: {result}");
            ResultTextBlock.Text = $"Maksymalna wartość: {result}";
        }
    }
}