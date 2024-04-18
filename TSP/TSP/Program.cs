using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TSP
{
    public class TSP
    {
        private static int[] visited;
        private static int[,] distance;
        private static int[] result;
        private static int[] a;
        private static int sumD;
        private static int n;
        private static int min;
        private static TimeSpan timeLimit = TimeSpan.FromSeconds(60);
        private static DateTime startTime;
        private static Boolean flag = false;
        public TSP()
        {
            sumD = 0;
            visited = new int[0];
            distance = new int[0, 0];
            result = new int[0];
            a = new int[0];
            min = int.MaxValue;
            n = 0;
            flag = false;
        }
        public void setST(DateTime dt) { startTime = dt; }
        public Boolean getFlag() { return flag; }
        public void readFile(string file_name)
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
                            n = int.Parse(parts[0]);
                            distance = new int[n, n];
                            visited = new int[n];
                            result = new int[n];
                            a = new int[n];
                        }
                        else
                        {
                            for (int i = 0; i < n; i++)
                            {
                                distance[lineNumber - 1, i] = int.Parse(parts[i]);
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
        public void printMatrix()
        {
            for (int i = 0; i < distance.GetLength(0); i++)
            {
                for (int j = 0; j < distance.GetLength(1); j++)
                {
                    Console.Write(distance[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        private int calculateDistance()
        {
            int sum = 0;
            for(int i = 0; i < result.Length - 1; i++)
            {
                sum += distance[result[i], result[i + 1]];
            }
            sum += distance[result[result.Length - 1], result[0]];
            return sum;
        }
        public void PrintSolution()
        {
            if (flag)
            {
                Console.WriteLine("Chi phi: " + calculateDistance());
                Console.Write("Duong di: ");
                foreach (int id in result)
                {
                    Console.Write(id + " -> ");
                }
                Console.WriteLine(0);
            }
            else Console.WriteLine("Khong giai duoc!");
        }
        public void vetCan(int i)
        {
            for (int j = 1; j < n; j++)
            {
                if (DateTime.Now - startTime > timeLimit)
                {
                    Console.WriteLine("Qua thoi gian!");
                    flag = false;
                    return;
                }
                else if (visited[j] == 0)
                {
                    flag = true;
                    a[i] = j;
                    visited[j] = 1; // Đánh dấu thành phố j đã được thăm
                    sumD += distance[a[i - 1], a[i]];
                    if (i == n - 1)
                    {
                        if (sumD + distance[a[i], 0] < min) // Thêm distance[a[i], 0] để hoàn thành vòng
                        {
                            Array.Copy(a, result, a.Length);
                            min = sumD + distance[a[i], 0]; // Thêm distance[a[i], 0] để hoàn thành vòng
                        }
                    }
                    else
                    {
                        vetCan(i + 1);
                    }
                    sumD -= distance[a[i - 1], a[i]];
                    visited[j] = 0; // Đánh dấu thành phố j chưa được thăm lại
                }
            }
        }
        public void thamLam()
        {
            result[0] = 0;
            visited[0] = 1;

            for (int i = 1; i < n; i++)
            {
                int nearestCity = -1;
                min = int.MaxValue;
                for (int j = 0; j < n; j++)
                {
                    if (visited[j] == 0 && distance[result[i - 1], j] < min)
                    {
                        nearestCity = j;
                        min = distance[result[i - 1], j];
                    }
                }

                if (nearestCity != -1)
                {
                    result[i] = nearestCity;
                    visited[nearestCity] = 1;
                    sumD += min;
                }
            }
            sumD += distance[result[n - 1], 0];
            if (DateTime.Now - startTime <= timeLimit)
            {
                flag = true;
            }
        }
    }

    public class Program
    {
        public static void WriteToCSV(string filePath, double vetCanTime, double thamLamTime, double acoTime)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write($"{vetCanTime},{thamLamTime},{acoTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV file: {ex.Message}");
            }
        }

        public static void Main(string[] args)
        {
            string[] dataset = { "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset1.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset2.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset3.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset4.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset5.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset6.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset7.txt",
                                 "C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset8.txt" };
            TSP vetCan;
            TSP thamLam;
            mathModel mm;
            ACO aco;
            GA gen;
            for (int i = 0; i < dataset.Length; i++)
            {
                Console.WriteLine($">>>>>>>DATASET {i + 1}<<<<<<<");
                //vet can
                Stopwatch stopwatch = new Stopwatch();
                vetCan = new TSP();
                vetCan.readFile(dataset[i]);
                vetCan.printMatrix();
                Console.WriteLine("\nTHUAT TOAN VET CAN\n");
                vetCan.setST(DateTime.Now);
                stopwatch.Start();
                vetCan.vetCan(1);
                vetCan.PrintSolution();
                stopwatch.Stop();
                Console.WriteLine($"Thoi gian chay: {stopwatch.Elapsed}");

                //tham lam
                Console.WriteLine("\nTHUAT TOAN THAM LAM\n");
                thamLam = new TSP();
                thamLam.setST(DateTime.Now);
                thamLam.readFile(dataset[i]);
                stopwatch = Stopwatch.StartNew();
                thamLam.thamLam();
                thamLam.PrintSolution();
                stopwatch.Stop();
                Console.WriteLine($"Thoi gian chay: {stopwatch.Elapsed}");

                //mathmodel
                Console.WriteLine("\nMATH MODEL\n");
                mm = new mathModel();
                int[,] matrix = mm.ReadCostMatrixFromFile(dataset[i]);
                stopwatch = Stopwatch.StartNew();
                (List<int>, int) result = mm.SolveTSP(matrix);
                mm.printResult(result);
                stopwatch.Stop();
                Console.WriteLine($"Thoi gian chay: {stopwatch.Elapsed}");

                // ACO
                int numberOfAnts = 10;
                double alpha = 1.0;
                double beta = 2.0;
                double evaporationRate = 0.5;

                Console.WriteLine("\nTHUAT TOAN ACO\n");
                aco = new ACO(numberOfAnts, alpha, beta, evaporationRate);
                aco.readFile(dataset[i]);
                stopwatch = Stopwatch.StartNew();
                int[] bestTour = aco.Solve(100);

                Console.WriteLine("Chi phi: " + aco.CalculateTourDistance(bestTour));
                Console.Write("Duong di: ");
                foreach (int city in bestTour)
                {
                    Console.Write(city + " -> ");
                }
                Console.WriteLine(bestTour[0]);
                stopwatch.Stop();
                Console.WriteLine($"Thoi gian chay: {stopwatch.Elapsed}");

                //genetic
                int popSize = 50;
                int maxGenerations = 1000;
                double mutationRate = 0.2;

                Console.WriteLine("\nTHUAT TOAN DI TRUYEN\n");
                gen = new GA(popSize, maxGenerations, mutationRate);
                gen.readFile(dataset[i]);
                stopwatch = Stopwatch.StartNew();
                gen.Evolve();
                gen.PrintBestIndividual();
                stopwatch.Stop();
                Console.WriteLine($"Thoi gian chay: {stopwatch.Elapsed}");
            }
        }
    }
}