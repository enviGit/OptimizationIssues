using System.Diagnostics;

namespace OptimizationIssues.Models
{
    public class TaskAllocationProblem : ProblemBase
    {
        public readonly double[] ProcessorMultipliers = { 1.0, 1.25, 1.5, 1.75 };

        public int NumberOfResources { get; set; }
        public int NumberOfTasks { get; set; }
        public int[] TaskCosts { get; set; }

        public TaskAllocationProblem(int numberOfResources, int numberOfTasks, int[] taskCosts)
        {
            NumberOfResources = numberOfResources;
            NumberOfTasks = numberOfTasks;
            TaskCosts = taskCosts;
        }

        public override int Solve(int maxTrials, int maxGenerationsWithoutImprovement, int maxTime)
        {
            var populationSize = Math.Max(100, NumberOfTasks * 2);
            var maxGenerations = Math.Max(10000, NumberOfTasks * 100);
            var mutationRate = 0.01;
            var eliteSize = 20;
            var generationsWithoutImprovement = 0;

            List<int[]> population = InitializePopulation(populationSize);
            int[] bestSolution = population.First();
            int bestFitness = CalculateFitness(bestSolution);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int generation = 0; generation < maxGenerations && generationsWithoutImprovement < maxGenerationsWithoutImprovement; generation++)
            {
                population = Evolve(population, eliteSize, mutationRate);

                int[] currentBest = population.First();
                int currentBestFitness = CalculateFitness(currentBest);

                if (currentBestFitness < bestFitness)
                {
                    bestFitness = currentBestFitness;
                    bestSolution = currentBest;
                    generationsWithoutImprovement = 0;
                }
                else
                    generationsWithoutImprovement++;

                if (stopwatch.ElapsedMilliseconds >= maxTime)
                    break;

                if (generation >= maxTrials)
                    break;
            }

            stopwatch.Stop();

            return bestFitness;
        }

        private List<int[]> InitializePopulation(int populationSize)
        {
            var population = new List<int[]>();

            Random random = new Random();

            for (int i = 0; i < populationSize; i++)
            {
                int[] individual = new int[NumberOfTasks];

                for (int j = 0; j < NumberOfTasks; j++)
                    individual[j] = random.Next(0, NumberOfResources);

                population.Add(individual);
            }

            return population;
        }

        private List<int[]> Evolve(List<int[]> population, int eliteSize, double mutationRate)
        {
            List<int[]> newPopulation = new List<int[]>();
            List<int[]> elite = population.OrderBy(ind => CalculateFitness(ind)).Take(eliteSize).ToList();
            newPopulation.AddRange(elite);

            Random random = new Random();

            while (newPopulation.Count < population.Count)
            {
                int[] parent1 = SelectParent(population);
                int[] parent2 = SelectParent(population);

                int[] child = Crossover(parent1, parent2);

                if (random.NextDouble() < mutationRate)
                    Mutate(child);

                newPopulation.Add(child);
            }

            return newPopulation;
        }

        private int[] SelectParent(List<int[]> population)
        {
            Random random = new Random();
            int tournamentSize = 5;
            var tournament = population.OrderBy(x => CalculateFitness(x)).Take(tournamentSize).ToList();

            return tournament[random.Next(tournamentSize)];
        }

        private int[] Crossover(int[] parent1, int[] parent2)
        {
            Random random = new Random();
            int crossoverPoint = random.Next(1, NumberOfTasks - 1);

            int[] child = new int[NumberOfTasks];
            Array.Copy(parent1, 0, child, 0, crossoverPoint);
            Array.Copy(parent2, crossoverPoint, child, crossoverPoint, NumberOfTasks - crossoverPoint);

            return child;
        }

        private void Mutate(int[] individual)
        {
            Random random = new Random();
            int mutationPoint = random.Next(0, NumberOfTasks);
            individual[mutationPoint] = random.Next(0, NumberOfResources);
        }

        private int CalculateFitness(int[] solution)
        {
            int[] processorCompletionTime = new int[NumberOfResources];

            for (int i = 0; i < NumberOfTasks; i++)
            {
                int processor = solution[i];
                processorCompletionTime[processor] += (int)(TaskCosts[i] * ProcessorMultipliers[processor]);
            }

            return processorCompletionTime.Max();
        }
    }
}
