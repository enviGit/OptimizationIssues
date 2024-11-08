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
            int n = NumberOfResources;
            int m = NumberOfTasks;
            int[,] dp = new int[n + 1, 1 << m];

            for (int i = 0; i <= n; i++)
                for (int j = 0; j < (1 << m); j++)
                    dp[i, j] = 0;

            for (int i = 1; i <= n; i++)
                for (int mask = 0; mask < (1 << m); mask++)
                    for (int task = 0; task < m; task++)
                        if ((mask & (1 << task)) == 0) 
                        {
                            int newMask = mask | (1 << task);
                            dp[i, newMask] = Math.Max(dp[i, newMask], dp[i - 1, mask] + CostMatrix[i - 1, task]);
                        }

            return dp[n, (1 << m) - 1];
        }
    }

}
