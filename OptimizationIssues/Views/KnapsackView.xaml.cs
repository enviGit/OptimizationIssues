#pragma warning disable CS8629
using OptimizationIssues.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
            ValidateInputs(out _, out _, out _);
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = (KnapsackViewModel)DataContext;

                if (ValidateInputs(out var capacity, out var weights, out var values))
                {
                    viewModel.KnapsackCapacity = capacity;
                    viewModel.Weights = weights;
                    viewModel.Values = values;

                    var (maxValue, selectedItems, usedCapacity) = viewModel.SolveKnapsackWithDetails();
                    ResultTextBlock.Inlines.Clear();

                    ResultTextBlock.Inlines.Add(new Run("Maksymalna wartość: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    ResultTextBlock.Inlines.Add(new Run(maxValue.ToString())
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"))
                    });

                    ResultTextBlock.Inlines.Add(new Run("\nZużyta pojemność plecaka: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    double fillPercentage = ((double)usedCapacity / capacity) * 100;

                    ResultTextBlock.Inlines.Add(new Run($"{usedCapacity}/{capacity} ({fillPercentage:F2}%)")
                    {
                        Foreground = new SolidColorBrush(usedCapacity == capacity
                        ? (Color)ColorConverter.ConvertFromString("#98FF98")
                        : (Color)ColorConverter.ConvertFromString("#FF9898"))
                    });

                    ResultTextBlock.Inlines.Add(new Run("\n\nWybrane przedmioty:\n")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    var sortedItems = selectedItems
                        .Select((value, index) => new { value, originalIndex = index + 1 })
                        .OrderBy(item => weights.IndexOf(item.value.Weight))
                        .ToList();

                    foreach (var item in sortedItems)
                    {
                        int itemNumber = weights.IndexOf(item.value.Weight) + 1;

                        ResultTextBlock.Inlines.Add(new Run($"Przedmiot {itemNumber} - Waga: ")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });

                        ResultTextBlock.Inlines.Add(new Run(item.value.Weight.ToString())
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A8D7"))
                        });

                        ResultTextBlock.Inlines.Add(new Run(", Wartość: ")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });

                        ResultTextBlock.Inlines.Add(new Run(item.value.Value.ToString())
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADD8E6"))
                        });

                        ResultTextBlock.Inlines.Add(new Run("\n")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });
                    }
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

        private bool ValidateInputs(out int capacity, out List<int> weights, out List<int> values)
        {
            bool isValid = true;

            capacity = 0;
            weights = new List<int>();
            values = new List<int>();

            if (string.IsNullOrWhiteSpace(CapacityTextBox.Text) || !int.TryParse(CapacityTextBox.Text, out capacity) || capacity <= 0)
            {
                isValid = false;
                CapacityTextBox.BorderBrush = Brushes.Red;
                CapacityTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                CapacityTextBox.BorderBrush = Brushes.Black;
                CapacityTextBox.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(WeightsTextBox.Text) || !TryParseList(WeightsTextBox.Text, out weights))
            {
                isValid = false;
                WeightsTextBox.BorderBrush = Brushes.Red;
                WeightsTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                WeightsTextBox.BorderBrush = Brushes.Black;
                WeightsTextBox.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(ValuesTextBox.Text) || !TryParseList(ValuesTextBox.Text, out values))
            {
                isValid = false;
                ValuesTextBox.BorderBrush = Brushes.Red;
                ValuesTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                ValuesTextBox.BorderBrush = Brushes.Black;
                ValuesTextBox.BorderThickness = new Thickness(1);
            }

            if (weights.Count != values.Count)
            {
                isValid = false;
                WeightsTextBox.BorderBrush = Brushes.Red;
                WeightsTextBox.BorderThickness = new Thickness(2);
                ValuesTextBox.BorderBrush = Brushes.Red;
                ValuesTextBox.BorderThickness = new Thickness(2);
            }

            return isValid;
        }

        private bool TryParseList(string input, out List<int> list)
        {
            list = input.Split(',').Select(str =>
            {
                return int.TryParse(str.Trim(), out var number) ? number : (int?)null;
            })
                .Where(num => num.HasValue)
                .Select(num => num.Value)
                .ToList();

            return list.Count > 0 && list.Count == input.Split(',').Length;
        }

        private void InputsChanged(object sender, TextChangedEventArgs e)
        {
            SolveButton.IsEnabled = ValidateInputs(out _, out _, out _);
        }
        private void GenerateSampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            int capacity = random.Next(10, 101);
            CapacityTextBox.Text = capacity.ToString();

            int itemCount = random.Next(3, 8);
            List<int> weights = new List<int>();
            List<int> values = new List<int>();

            for (int i = 0; i < itemCount; i++)
            {
                weights.Add(random.Next(1, capacity / 2));
                values.Add(random.Next(10, 101));
            }

            WeightsTextBox.Text = string.Join(", ", weights);
            ValuesTextBox.Text = string.Join(", ", values);

            CapacityTextBox.BorderBrush = Brushes.Black;
            CapacityTextBox.BorderThickness = new Thickness(1);

            WeightsTextBox.BorderBrush = Brushes.Black;
            WeightsTextBox.BorderThickness = new Thickness(1);

            ValuesTextBox.BorderBrush = Brushes.Black;
            ValuesTextBox.BorderThickness = new Thickness(1);

            SolveButton.IsEnabled = ValidateInputs(out _, out _, out _);
        }
    }
}