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
            SetDarkTheme();
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

        private static void SetDarkTheme()
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary
            {
                Source = new Uri("Styles/DarkTheme.xaml", UriKind.Relative)
            };
        }

        private void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString().Contains("DarkTheme"))
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary
                {
                    Source = new Uri("Styles/LightTheme.xaml", UriKind.Relative)
                };
            else
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary
                {
                    Source = new Uri("Styles/DarkTheme.xaml", UriKind.Relative)
                };
        }
    }
}