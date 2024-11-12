using OptimizationIssues.Models;

namespace OptimizationIssues.ViewModels
{
    public class TaskAllocationViewModel
    {
        public int NumberOfResources { get; set; }
        public int NumberOfTasks { get; set; }
        public List<int> TaskCosts { get; set; }
        public int MaxTrials { get; set; }
        public int MaxGenerationsWithoutImprovement { get; set; }
        public int MaxTime { get; set; }

        public TaskAllocationViewModel()
        {
            TaskCosts = new List<int>();
        }

        public (List<string> TaskAssignments, List<string> ProcessorCompletionTimes, List<int> ProcessorLoadHistogram, int LastProcessorCompletionTime) SolveTaskAllocation()
        {
            var problem = new TaskAllocationProblem(NumberOfResources, NumberOfTasks, TaskCosts.ToArray());
            int bestFitness = problem.Solve(MaxTrials, MaxGenerationsWithoutImprovement, MaxTime);

            List<string> taskAssignments = new List<string>();
            List<string> processorCompletionTimes = new List<string>();
            List<int> processorLoadHistogram = new List<int>();

            int[] processorCompletionTime = new int[NumberOfResources];
            for (int i = 0; i < NumberOfResources; i++)
                processorCompletionTime[i] = 0;

            for (int task = 0; task < NumberOfTasks; task++)
            {
                int minTimeProcessor = -1;
                int minCompletionTime = int.MaxValue;

                for (int resource = 0; resource < NumberOfResources; resource++)
                {
                    int taskTime = (int)(TaskCosts[task] * problem.ProcessorMultipliers[resource]);

                    if (processorCompletionTime[resource] + taskTime < minCompletionTime)
                    {
                        minCompletionTime = processorCompletionTime[resource] + taskTime;
                        minTimeProcessor = resource;
                    }
                }

                taskAssignments.Add($"Zadanie {task + 1} - Procesor P{minTimeProcessor}, Czas: {TaskCosts[task] * problem.ProcessorMultipliers[minTimeProcessor]} ms");
                processorCompletionTime[minTimeProcessor] += (int)(TaskCosts[task] * problem.ProcessorMultipliers[minTimeProcessor]);
            }

            for (int i = 0; i < NumberOfResources; i++)
                processorCompletionTimes.Add($"P{i}: zakończenie pracy w {processorCompletionTime[i]} ms");

            int maxCompletionTime = processorCompletionTime.Max();

            foreach (var completionTime in processorCompletionTime)
            {
                int loadPercentage = (int)((double)completionTime / maxCompletionTime * 100);
                processorLoadHistogram.Add(loadPercentage);
            }

            return (taskAssignments, processorCompletionTimes, processorLoadHistogram, maxCompletionTime);
        }
    }
}