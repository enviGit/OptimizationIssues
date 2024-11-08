using OptimizationIssues.Models;

namespace OptimizationIssues.ViewModels
{
    public class TSPViewModel
    {
        public int NumberOfCities { get; set; }
        public List<List<int>> DistanceMatrix { get; set; }

        public TSPViewModel()
        {
            DistanceMatrix = new List<List<int>>();
        }

        public (int, List<int>) SolveTravelingSalesmanProblem()
        {
            int[,] matrix = new int[NumberOfCities, NumberOfCities];

            for (int i = 0; i < NumberOfCities; i++)
            {
                for (int j = 0; j < NumberOfCities; j++)
                    matrix[i, j] = DistanceMatrix[i][j];
            }

            var tspProblem = new TSPProblem(NumberOfCities, matrix);
            return tspProblem.Solve();
        }
    }
}