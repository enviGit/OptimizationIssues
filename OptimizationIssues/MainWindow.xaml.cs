using OptimizationIssues.Views;
using System.Windows;
using System.Windows.Controls;

namespace OptimizationIssues
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ProblemSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedProblem = ((ComboBoxItem)ProblemSelector.SelectedItem)?.Content?.ToString() ?? string.Empty;
            ProblemDetails.Content = null;

            switch (selectedProblem)
            {
                case "Problem Plecakowy":
                    ProblemDetails.Content = new KnapsackView();
                    break;
                case "Problem Alokacji Zadań":
                    ProblemDetails.Content = new TaskAllocationView();
                    break;
                case "Problem Komiwojażera":
                    ProblemDetails.Content = new TSPView();
                    break;
            }
        }
    }
}