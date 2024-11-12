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

                if (ValidateInputs(out var numberOfResources, out var numberOfTasks, out var taskCosts))
                {
                    viewModel.NumberOfResources = numberOfResources;
                    viewModel.NumberOfTasks = numberOfTasks;
                    viewModel.TaskCosts = taskCosts;

                    var (taskAssignments, processorCompletionTimes, processorLoadHistogram) = viewModel.SolveTaskAllocation();
                    ResultTextBlock.Inlines.Clear();

                    ResultTextBlock.Inlines.Add(new Run("Lista przydziałów:\n")
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = FontWeights.Bold
                    });

                    foreach (var assignment in taskAssignments)
                    {
                        ResultTextBlock.Inlines.Add(new Run(assignment + "\n")
                        {
                            Foreground = new SolidColorBrush(Colors.LightGreen)
                        });
                    }

                    ResultTextBlock.Inlines.Add(new Run("\nCzasy zakończenia procesorów:\n")
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = FontWeights.Bold
                    });

                    foreach (var time in processorCompletionTimes)
                    {
                        ResultTextBlock.Inlines.Add(new Run(time + "\n")
                        {
                            Foreground = new SolidColorBrush(Colors.LightSkyBlue)
                        });
                    }

                    ResultTextBlock.Inlines.Add(new Run("\nHistogram obciążenia procesorów:\n")
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = FontWeights.Bold
                    });

                    for (int i = 0; i < processorLoadHistogram.Count; i++)
                    {
                        ResultTextBlock.Inlines.Add(new Run($"P{i}: {processorLoadHistogram[i]}%\n")
                        {
                            Foreground = new SolidColorBrush(Colors.Yellow)
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

        private List<int> ParseTaskCosts(string input)
        {
            var values = input.Split(',').Select(str => int.TryParse(str.Trim(), out var cost) ? cost : (int?)null)
                                .Where(num => num.HasValue)
                                .Select(num => num.Value)
                                .ToList();

            return values;
        }
        private bool TryParseTaskCosts(string input, out List<int> taskCosts)
        {
            taskCosts = new List<int>();

            try
            {
                taskCosts = ParseTaskCosts(input);
                return taskCosts.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateInputs(out int numberOfResources, out int numberOfTasks, out List<int> taskCosts)
        {
            bool isValid = true;

            numberOfResources = 0;
            numberOfTasks = 0;
            taskCosts = new List<int>();

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

            if (string.IsNullOrWhiteSpace(TaskCostsTextBox.Text) || !TryParseTaskCosts(TaskCostsTextBox.Text, out taskCosts))
            {
                isValid = false;
                TaskCostsTextBox.BorderBrush = Brushes.Red;
                TaskCostsTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                TaskCostsTextBox.BorderBrush = Brushes.Black;
                TaskCostsTextBox.BorderThickness = new Thickness(1);
            }

            return isValid;
        }

        private void InputsChanged(object sender, TextChangedEventArgs e)
        {
            SolveButton.IsEnabled = ValidateInputs(out _, out _, out _);
        }
        private void GenerateSampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            int numberOfResources = random.Next(2, 5);
            int numberOfTasks = random.Next(1, 101);

            var taskCosts = new List<int>();

            for (int i = 0; i < numberOfTasks; i++)
                taskCosts.Add(random.Next(10, 91));

            NumberOfResourcesTextBox.Text = numberOfResources.ToString();
            NumberOfTasksTextBox.Text = numberOfTasks.ToString();

            TaskCostsTextBox.Text = string.Join(",", taskCosts);

            SolveButton.IsEnabled = true;
        }
    }
}