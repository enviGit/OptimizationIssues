namespace OptimizationIssues.Models
{
    public class TaskAllocationProblem : ProblemBase
    {
        public int NumberOfResources { get; set; }
        public int NumberOfTasks { get; set; }
        public int[,] TaskCosts { get; set; }

        public TaskAllocationProblem(int numberOfResources, int numberOfTasks, int[,] taskCosts)
        {
            NumberOfResources = numberOfResources;
            NumberOfTasks = numberOfTasks;
            TaskCosts = taskCosts;
        }

        public override int Solve()
        {
            return 0;
        }
    }
}