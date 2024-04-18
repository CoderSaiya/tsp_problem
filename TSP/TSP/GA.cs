using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    internal class GA
    {
        private int n;
        private int popSize;
        private int maxGenerations;
        private double mutationRate;
        List<List<double>> distances = new List<List<double>>();
        List<List<int>> population;
        public GA(int popSize, int maxGen, double mutRate)
        {
            this.popSize = popSize;
            this.maxGenerations = maxGen;
            this.mutationRate = mutRate;
        }
        public List<List<double>> readFile(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    n = int.Parse(reader.ReadLine());
                    for (int i = 0; i < n; i++)
                    {
                        string[] rowValues = reader.ReadLine().Split('\t');
                        List<double> row = new List<double>();
                        foreach (string value in rowValues)
                        {
                            row.Add(double.Parse(value));
                        }
                        distances.Add(row);
                    }
                }
                return distances;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc file: {ex.Message}");
                return null;
            }
        }


        private List<int> CreateIndividual()
        {
            List<int> path = Enumerable.Range(0, n).ToList();
            Random rng = new Random();
            for (int i = 0; i < n - 1; i++)
            {
                int randomIndex = rng.Next(i, n);
                int temp = path[i];
                path[i] = path[randomIndex];
                path[randomIndex] = temp;
            }
            return path;
        }

        static double CalculateFitness(List<int> path, List<List<double>> distances)
        {
            int numCities = path.Count;
            double totalDistance = 0.0;
            for (int i = 0; i < numCities - 1; i++)
            {
                totalDistance += distances[path[i]][path[i + 1]];
            }
            totalDistance += distances[path.Last()][path.First()]; // Quay trở lại thành phố bắt đầu
            return totalDistance;
        }

        static List<int> Crossover(List<int> parent1, List<int> parent2)
        {
            List<int> child = new List<int>(parent1);
            int crossoverPoint = new Random().Next(2, child.Count);
            HashSet<int> used = new HashSet<int>(parent1.GetRange(0, crossoverPoint));
            for (int i = crossoverPoint; i < child.Count; i++)
            {
                int j = 1;
                while (j < parent2.Count && used.Contains(parent2[j]))
                {
                    j++;
                }
                if (j < parent2.Count)
                {
                    child[i] = parent2[j];
                    used.Add(parent2[j]);
                }
                else
                {
                    for (int k = 1; k < parent1.Count; k++)
                    {
                        if (!used.Contains(parent1[k]))
                        {
                            child[i] = parent1[k];
                            used.Add(parent1[k]);
                            break;
                        }
                    }
                }
            }
            return child;
        }

        static void Mutate(List<int> path)
        {
            int pos1 = new Random().Next(1, path.Count);
            int pos2 = new Random().Next(1, path.Count);
            int temp = path[pos1];
            path[pos1] = path[pos2];
            path[pos2] = temp;
        }

        public void Evolve()
        {
            population = Enumerable.Range(0, popSize).Select(_ => CreateIndividual()).ToList();
            for (int gen = 0; gen < maxGenerations; gen++)
            {
                var fitnessValues = new List<(double, int)>();
                for (int i = 0; i < popSize; i++)
                {
                    fitnessValues.Add((CalculateFitness(population[i], distances), i));
                }
                fitnessValues.Sort();

                var newPopulation = new List<List<int>>();
                for (int i = 0; i < popSize / 2; i++)
                {
                    newPopulation.Add(new List<int>(population[fitnessValues[i].Item2]));
                }

                while (newPopulation.Count < popSize)
                {
                    int idx1 = new Random().Next(popSize / 2);
                    int idx2 = new Random().Next(popSize / 2);
                    List<int> child = Crossover(population[idx1], population[idx2]);
                    if (new Random().NextDouble() < mutationRate)
                    {
                        Mutate(child);
                    }
                    newPopulation.Add(child);
                }

                population = newPopulation;
            }
        }

        public void PrintBestIndividual()
        {
            double bestFitness = CalculateFitness(population[0], distances);
            int bestIdx = 0;
            for (int i = 1; i < popSize; i++)
            {
                double curFitness = CalculateFitness(population[i], distances);
                if (curFitness < bestFitness)
                {
                    bestFitness = curFitness;
                    bestIdx = i;
                }
            }
            Console.WriteLine($"Chi phi: {bestFitness}");
            Console.Write("Duong di: ");
            List<int> bestPath = population[bestIdx];
            foreach (int cityIdx in bestPath)
            {
                Console.Write($"{cityIdx} -> ");
            }
            Console.WriteLine(0);
            
        }
    }
}
