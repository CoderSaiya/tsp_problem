using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    internal class genetic
    {
        private int N;
        private double[,] matrix;
        private int[][] population;
        private Random rng = new Random();
        public int[][] getP()
        {
            return population;
        }

        public void readFile (string file_name)
        {
            using (StreamReader sr = new StreamReader(file_name))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file_name))
                    {
                        string line;
                        int lineNumber = 0;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split('\t');
                            if (lineNumber == 0)
                            {
                                N = int.Parse(parts[0]);
                                matrix = new double[N, N];
                            }
                            else
                            {
                                for (int i = 0; i < N; i++)
                                {
                                    this.matrix[lineNumber - 1, i] = int.Parse(parts[i]);
                                }
                            }
                            lineNumber++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public void Initialization()
        {
            population = new int[N][];
            for (int i = 0; i < N; i++)
            {
                population[i] = CreateRandomList();
            }
        }
        private int[] CreateRandomList()
        {
            int[] temp = new int[N];
            for (int i = 0; i < N; i++)
            {
                temp[i] = i + 1;
            }
            Shuffle(temp);
            return temp;
        }
        private void Shuffle(int[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
        public int[] FindBest(int[][] generation)
        {
            int[] tour = generation[0];
            double bestCost = CalculateCost(tour);
            for (int n = 1; n < generation.Length; n++)
            {
                double currentCost = CalculateCost(generation[n]);
                if (currentCost < bestCost)
                {
                    tour = generation[n];
                    bestCost = currentCost;
                }
            }
            return tour;
        }
        public double CalculateCost(int[] chromosome)
        {
            double cost = 0;
            for (int j = 0; j < chromosome.Length - 1; j++)
            {
                cost += matrix[chromosome[j] - 1,chromosome[j + 1] - 1];
            }
            return cost;
        }
        public int[][] CreateNewGeneration(int[][] previous_generation, double mutation_rate)
        {
            int[][] new_generation = new int[previous_generation.Length][];
            new_generation[0] = FindBest(previous_generation);

            for (int a = 0; a < previous_generation.Length / 2; a++)
            {
                int[] parent_1 = Selection(previous_generation);
                int[] parent_2 = Selection(previous_generation);

                int[] child_1, child_2;
                CrossoverMix(parent_1, parent_2, out child_1, out child_2);

                if (rng.NextDouble() < mutation_rate)
                {
                    Mutation(child_1);
                }
                if (rng.NextDouble() < mutation_rate)
                {
                    Mutation(child_2);
                }

                new_generation[2 * a + 1] = child_1;
                new_generation[2 * a + 2] = child_2;
            }

            return new_generation;
        }
        private int[] Selection(int[][] population)
        {
            int ticket_1 = rng.Next(0, population.Length);
            int ticket_2 = rng.Next(0, population.Length);
            int ticket_3 = rng.Next(0, population.Length);
            int ticket_4 = rng.Next(0, population.Length);

            int[] candidate_1 = population[ticket_1];
            int[] candidate_2 = population[ticket_2];
            int[] candidate_3 = population[ticket_3];
            int[] candidate_4 = population[ticket_4];

            int[] winner = candidate_1;
            if (CalculateCost(candidate_2) < CalculateCost(winner))
                winner = candidate_2;
            if (CalculateCost(candidate_3) < CalculateCost(winner))
                winner = candidate_3;
            if (CalculateCost(candidate_4) < CalculateCost(winner))
                winner = candidate_4;

            return winner;
        }
        private void CrossoverMix(int[] p_1, int[] p_2, out int[] child_1, out int[] child_2)
        {
            int point_1 = rng.Next(1, p_1.Length - 1);
            int point_2 = rng.Next(1, p_1.Length - 1);
            int begin = Math.Min(point_1, point_2);
            int end = Math.Max(point_1, point_2);

            int[] child_1_1 = new int[begin];
            int[] child_1_2 = new int[p_1.Length - end];
            Array.Copy(p_1, 0, child_1_1, 0, begin);
            Array.Copy(p_1, end, child_1_2, 0, p_1.Length - end);
            child_1 = new int[begin + child_1_2.Length];
            Array.Copy(child_1_1, child_1, begin);
            Array.Copy(child_1_2, 0, child_1, begin, child_1_2.Length);

            child_2 = new int[end - begin + 1];
            Array.Copy(p_2, begin, child_2, 0, end - begin + 1);

            int[] child_1_remain = new int[p_2.Length];
            int[] child_2_remain = new int[p_1.Length];

            int j = 0, k = 0;
            for (int i = 0; i < p_2.Length; i++)
            {
                if (Array.IndexOf(child_1, p_2[i]) == -1)
                {
                    child_1_remain[j] = p_2[i];
                    j++;
                }
            }

            for (int i = 0; i < p_1.Length; i++)
            {
                if (Array.IndexOf(child_2, p_1[i]) == -1)
                {
                    child_2_remain[k] = p_1[i];
                    k++;
                }
            }

            Array.Resize(ref child_1, begin + child_1_2.Length + j);
            Array.Resize(ref child_2, end - begin + 1 + k);

            Array.Copy(child_1_remain, 0, child_1, begin + child_1_2.Length, j);
            Array.Copy(child_2_remain, 0, child_2, end - begin + 1, k);
        }
        private void Mutation(int[] chromosome)
        {
            int mutation_index_1 = rng.Next(1, chromosome.Length - 1);
            int mutation_index_2 = rng.Next(1, chromosome.Length - 1);

            // Kiểm tra chỉ số để đảm bảo chúng không vượt quá kích thước của mảng
            if (mutation_index_1 >= 0 && mutation_index_1 < chromosome.Length &&
                mutation_index_2 >= 0 && mutation_index_2 < chromosome.Length)
            {
                int temp = chromosome[mutation_index_1];
                chromosome[mutation_index_1] = chromosome[mutation_index_2];
                chromosome[mutation_index_2] = temp;
            }
        }
    }
}
