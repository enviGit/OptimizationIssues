namespace OptimizationIssues.Models
{
    public class TaskAllocationProblem : ProblemBase
    {
        public int NumberOfResources { get; set; }
        public int NumberOfTasks { get; set; }
        public int[,] CostMatrix { get; set; }

        public TaskAllocationProblem(int numberOfResources, int numberOfTasks, int[,] costMatrix)
        {
            NumberOfResources = numberOfResources;
            NumberOfTasks = numberOfTasks;
            CostMatrix = costMatrix;
        }

        public override int Solve()
        {
            return 0;
        }
    }
}