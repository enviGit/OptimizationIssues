using OptimizationIssues.Models;

namespace OptimizationIssues.ViewModels
{
    public class TaskAllocationViewModel
    {
        public int NumberOfResources { get; set; }
        public int NumberOfTasks { get; set; }
        public List<List<int>> CostMatrix { get; set; }

        public TaskAllocationViewModel()
        {
            CostMatrix = new List<List<int>>();
        }

        public (int MinCost, int MaxValue) SolveTaskAllocation()
        {
            int[,] costMatrix = new int[NumberOfResources, NumberOfTasks];

            for (int i = 0; i < NumberOfResources; i++)
            {
                for (int j = 0; j < NumberOfTasks; j++)
                {
                    costMatrix[i, j] = CostMatrix[i][j];
                }
            }

            int minCost = CalculateMinCost(costMatrix, NumberOfResources, NumberOfTasks);

            int maxValue = CalculateMaxValue(costMatrix, NumberOfResources, NumberOfTasks);

            return (minCost, maxValue);
        }

        private int CalculateMinCost(int[,] costMatrix, int resources, int tasks)
        {
            int[,] dp = new int[resources + 1, tasks + 1];

            for (int i = 0; i <= resources; i++)
            {
                for (int j = 0; j <= tasks; j++)
                {
                    if (i == 0 || j == 0)
                        dp[i, j] = 0;
                    else
                        dp[i, j] = Math.Min(dp[i - 1, j], dp[i, j - 1]) + costMatrix[i - 1, j - 1];
                }
            }
            return dp[resources, tasks];
        }

        private int CalculateMaxValue(int[,] costMatrix, int resources, int tasks)
        {
            int[,] dp = new int[resources + 1, tasks + 1];

            for (int i = 0; i <= resources; i++)
            {
                for (int j = 0; j <= tasks; j++)
                {
                    if (i == 0 || j == 0)
                        dp[i, j] = 0;
                    else
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]) + costMatrix[i - 1, j - 1];
                }
            }
            return dp[resources, tasks];
        }
    }

}
