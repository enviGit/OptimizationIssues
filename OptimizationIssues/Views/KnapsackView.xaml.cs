#pragma warning disable CS8629
using OptimizationIssues.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using static OptimizationIssues.Views.KnapsackView;

namespace OptimizationIssues.Views
{
    /// <summary>
    /// Interaction logic for KnapsackView.xaml
    /// </summary>
    public partial class KnapsackView : UserControl
    {
        public class KnapsackItem
        {
            public int Index { get; set; }
            public int Weight { get; set; }
            public int Value { get; set; }
        }
        public KnapsackView()
        {
            InitializeComponent();
            DataContext = new KnapsackViewModel();
            ValidateInputs(out _, out _, out _);

            var plotModel = new PlotModel
            {
                Title = "Wyniki optymalizacji",
                Background = OxyColor.FromRgb(46, 46, 46)
            };

            plotModel.TextColor = OxyColor.FromRgb(255, 255, 255);
            PlotView.Model = plotModel;
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

                    var watch = Stopwatch.StartNew();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    var (maxValue, selectedItems, usedCapacity) = viewModel.SolveKnapsackWithDetails();
                    ResultTextBlock.Inlines.Clear();

                    watch.Stop();

                    ResultTextBlock.Inlines.Add(new Run("Maksymalna wartość: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    ResultTextBlock.Inlines.Add(new Run(maxValue.ToString())
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#98FF98"))
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

                    ResultTextBlock.Inlines.Add(new Run($"\nCzas obliczeń: ")
                    {
                        Foreground = new SolidColorBrush(Colors.White)
                    });

                    string colorHex = watch.ElapsedMilliseconds < 20 ? "#98FF98" : watch.ElapsedMilliseconds < 50 ? "#FFFF66" : "#FF9898";

                    ResultTextBlock.Inlines.Add(new Run($"{watch.ElapsedMilliseconds} ms")
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex))
                    });

                    ResultTextBlock.Inlines.Add(new Run("\n\nWybrane przedmioty:\n")
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = System.Windows.FontWeights.Bold
                    });

                    selectedItems.Reverse();

                    foreach (var item in selectedItems)
                    {
                        int itemNumber = item.Index + 1;

                        ResultTextBlock.Inlines.Add(new Run($"Przedmiot ")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });

                        ResultTextBlock.Inlines.Add(new Run($"{itemNumber}")
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"))
                        });

                        ResultTextBlock.Inlines.Add(new Run($" - Waga: ")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });

                        ResultTextBlock.Inlines.Add(new Run(item.Weight.ToString())
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A8D7"))
                        });

                        ResultTextBlock.Inlines.Add(new Run(", Wartość: ")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });

                        ResultTextBlock.Inlines.Add(new Run(item.Value.ToString())
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADD8E6"))
                        });

                        ResultTextBlock.Inlines.Add(new Run("\n")
                        {
                            Foreground = new SolidColorBrush(Colors.White)
                        });
                    }

                    var knapsackItems = selectedItems.Select(item => new KnapsackItem
                    {
                        Index = item.Index,
                        Weight = item.Weight,
                        Value = item.Value
                    }).ToList();

                    UpdateChart(knapsackItems);
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

        private void UpdateChart(List<KnapsackItem> selectedItems)
        {
            var plotModel = new PlotModel
            {
                Title = "Wyniki optymalizacji",
                Background = OxyColor.FromRgb(46, 46, 46)
            };

            plotModel.TextColor = OxyColor.FromRgb(255, 255, 255);

            var valueSeries = new LineSeries
            {
                Title = "Wartości przedmiotów",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                StrokeThickness = 2,
                Color = OxyColor.FromRgb(0, 255, 0)
            };

            var weightSeries = new LineSeries
            {
                Title = "Wagi przedmiotów",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                StrokeThickness = 2,
                Color = OxyColor.FromRgb(255, 0, 0)
            };

            for (int i = 0; i < selectedItems.Count; i++)
            {
                var item = selectedItems[i];
                valueSeries.Points.Add(new DataPoint(i, item.Value));
                weightSeries.Points.Add(new DataPoint(i, item.Weight));
            }

            plotModel.Series.Add(valueSeries);
            plotModel.Series.Add(weightSeries);

            foreach (var item in selectedItems)
            {
                var priorityColor = OxyColor.FromRgb(
                    (byte)(255 - item.Value * 2),
                    (byte)(255 - item.Weight * 2),
                    0
                );

                var annotation = new TextAnnotation
                {
                    Text = $"{item.Value}",
                    TextPosition = new DataPoint(selectedItems.IndexOf(item), item.Value),
                    FontSize = 12,
                    TextColor = OxyColor.FromRgb(255, 255, 255)
                };

                plotModel.Annotations.Add(annotation);
            }

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = selectedItems.Max(item => item.Value) + 10,
                Title = "Wartość przedmiotu",
                TitleColor = OxyColor.FromRgb(255, 255, 255),
                TextColor = OxyColor.FromRgb(255, 255, 255)
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Minimum = 0,
                Maximum = selectedItems.Max(item => item.Weight) + 10,
                Title = "Waga przedmiotu",
                TitleColor = OxyColor.FromRgb(255, 255, 255),
                TextColor = OxyColor.FromRgb(255, 255, 255)
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = selectedItems.Count,
                MajorStep = 1,
                MinorStep = 1,
                Title = "Numer przedmiotu",
                LabelFormatter = value => (value + 1).ToString(),
                TitleColor = OxyColor.FromRgb(255, 255, 255),
                TextColor = OxyColor.FromRgb(255, 255, 255)
            });

            PlotView.Model = plotModel;
        }
    }
}