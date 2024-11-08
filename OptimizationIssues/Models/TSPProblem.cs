namespace OptimizationIssues.Models
{
    public class TSPProblem
    {
        public int NumberOfCities { get; set; }
        public int[,] DistanceMatrix { get; set; }

        public TSPProblem(int numberOfCities, int[,] distanceMatrix)
        {
            NumberOfCities = numberOfCities;
            DistanceMatrix = distanceMatrix;
        }

        public int Solve()
        {
            var cities = Enumerable.Range(0, NumberOfCities).ToArray();
            return FindShortestPath(cities);
        }

        private int FindShortestPath(int[] cities)
        {
            int minPathLength = int.MaxValue;
            var cityPermutations = GetPermutations(cities, cities.Length);

            foreach (var permutation in cityPermutations)
            {
                int pathLength = CalculatePathLength(permutation);

                if (pathLength < minPathLength)
                    minPathLength = pathLength;
            }

            return minPathLength;
        }

        private int CalculatePathLength(int[] path)
        {
            int totalLength = 0;

            for (int i = 0; i < path.Length - 1; i++)
                totalLength += DistanceMatrix[path[i], path[i + 1]];

            totalLength += DistanceMatrix[path[path.Length - 1], path[0]];

            return totalLength;
        }

        private IEnumerable<int[]> GetPermutations(int[] cities, int length)
        {
            if (length == 1)
                return cities.Select(t => new int[] { t });

            return GetPermutations(cities, length - 1).SelectMany(t => cities.Where(e => !t.Contains(e)), (t, e) => t.Concat(new int[] { e }).ToArray());
        }
    }

}
