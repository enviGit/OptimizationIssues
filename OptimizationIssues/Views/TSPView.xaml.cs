#pragma warning disable CS8629
using OptimizationIssues.ViewModels;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
            ValidateInputs(out _, out _);
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = (TSPViewModel)DataContext;

                if (ValidateInputs(out var numberOfCities, out var distanceMatrix))
                {
                    viewModel.NumberOfCities = int.Parse(NumberOfCitiesTextBox.Text);
                    viewModel.DistanceMatrix = ParseDistanceMatrix(DistanceMatrixTextBox.Text);

                    var watch = Stopwatch.StartNew();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    (int result, List<int> path) = viewModel.SolveTravelingSalesmanProblem();
                    path.Add(path[0]);
                    string pathString = string.Join(" -> ", path);
                    ResultTextBlock.Inlines.Clear();

                    watch.Stop();

                    ResultTextBlock.Inlines.Add(new Run("Minimalna długość trasy: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    ResultTextBlock.Inlines.Add(new Run(result.ToString() + "\n")
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#98FF98"))
                    });

                    ResultTextBlock.Inlines.Add(new Run("Trasa: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    var pathParts = pathString.Split(new string[] { " -> " }, StringSplitOptions.None);

                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        if (i > 0)
                            ResultTextBlock.Inlines.Add(new Run(" -> ")
                            {
                                Foreground = new SolidColorBrush(Colors.White)
                            });

                        ResultTextBlock.Inlines.Add(new Run(pathParts[i])
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"))
                        });
                    }

                    ResultTextBlock.Inlines.Add(new Run($"\nCzas obliczeń: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    string colorHex = watch.ElapsedMilliseconds < 500 ? "#98FF98" : watch.ElapsedMilliseconds < 3000 ? "#FFFF66" : "#FF9898";

                    ResultTextBlock.Inlines.Add(new Run($"{watch.ElapsedMilliseconds} ms")
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex))
                    });
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

        private bool ValidateInputs(out int numberOfCities, out List<List<int>> distanceMatrix)
        {
            bool isValid = true;

            numberOfCities = 0;
            distanceMatrix = new List<List<int>>();

            if (string.IsNullOrWhiteSpace(NumberOfCitiesTextBox.Text) || !int.TryParse(NumberOfCitiesTextBox.Text, out numberOfCities) || numberOfCities <= 1)
            {
                isValid = false;
                NumberOfCitiesTextBox.BorderBrush = Brushes.Red;
                NumberOfCitiesTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                NumberOfCitiesTextBox.BorderBrush = Brushes.Black;
                NumberOfCitiesTextBox.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(DistanceMatrixTextBox.Text) || !TryParseDistanceMatrix(DistanceMatrixTextBox.Text, out distanceMatrix, numberOfCities))
            {
                isValid = false;
                DistanceMatrixTextBox.BorderBrush = Brushes.Red;
                DistanceMatrixTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                DistanceMatrixTextBox.BorderBrush = Brushes.Black;
                DistanceMatrixTextBox.BorderThickness = new Thickness(1);
            }

            return isValid;
        }

        private bool TryParseDistanceMatrix(string input, out List<List<int>> distanceMatrix, int expectedSize)
        {
            distanceMatrix = new List<List<int>>();
            var rows = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (rows.Length != expectedSize)
                return false;

            try
            {
                foreach (var row in rows)
                {
                    var values = row.Split(',').Select(str => int.TryParse(str.Trim(), out var number) ? number : (int?)null)
                                    .Where(num => num.HasValue)
                                    .Select(num => num.Value)
                                    .ToList();

                    if (values.Count != expectedSize)
                        return false;

                    distanceMatrix.Add(values);
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
            SolveButton.IsEnabled = ValidateInputs(out _, out _);
        }

        private void GenerateSampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();

            int numberOfCities = rand.Next(3, 11);
            NumberOfCitiesTextBox.Text = numberOfCities.ToString();

            var distanceMatrix = GenerateRandomDistanceMatrix(numberOfCities);
            DistanceMatrixTextBox.Text = FormatDistanceMatrix(distanceMatrix);

            SolveButton.IsEnabled = true;
        }

        private List<List<int>> GenerateRandomDistanceMatrix(int size)
        {
            Random rand = new Random();
            var matrix = new List<List<int>>();

            for (int i = 0; i < size; i++)
            {
                var row = new List<int>();

                for (int j = 0; j < size; j++)
                    row.Add(i == j ? 0 : rand.Next(10, 101));

                matrix.Add(row);
            }

            return matrix;
        }

        private string FormatDistanceMatrix(List<List<int>> matrix)
        {
            var result = new StringBuilder();

            foreach (var row in matrix)
                result.AppendLine(string.Join(",", row));

            return result.ToString();
        }
    }
}