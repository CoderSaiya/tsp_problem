using System;
using System.Collections.Generic;
using Google.OrTools.LinearSolver;

public class mathModel
{
    public (List<int>, int) SolveTSP(int[,] costMatrix)
    {
        int numCities = costMatrix.GetLength(0);

        Solver solver = Solver.CreateSolver("CBC");
        if (solver is null)
        {
            Console.WriteLine("Solver initialization failed.");
            return (null, -1);
        }

        // Decision Variables
        Variable[,] x = new Variable[numCities, numCities];
        for (int i = 0; i < numCities; i++)
        {
            for (int j = 0; j < numCities; j++)
            {
                x[i, j] = solver.MakeBoolVar($"x[{i},{j}]");
            }
        }
        Dictionary<int, Variable> tourPosition = new Dictionary<int, Variable>();
        for (int i = 0; i < numCities; i++)
        {
            tourPosition[i] = solver.MakeIntVar(0, numCities - 1, $"tour_position[{i}]");
        }

        // Constraints
        for (int i = 0; i < numCities; i++)
        {
            solver.Add((from j in Enumerable.Range(0, numCities) select x[i, j]).ToArray().Sum() == 1); // Outgoing edges from city i
            solver.Add((from j in Enumerable.Range(0, numCities) select x[j, i]).ToArray().Sum() == 1); // Incoming edges to city i
        }

        for (int i = 0; i < numCities; i++)
        {
            solver.Add(x[i, i] == 0); // No self-loops
        }

        for (int i = 0; i < numCities; i++)
        {
            for (int j = 0; j < numCities; j++)
            {
                if (i != j)
                {
                    solver.Add(x[i, j] + x[j, i] <= 1); // Ensure no duplicate paths between any pair of cities
                }
            }
        }


        // Ràng buộc: Hành trình phải bắt đầu từ thành phố 0.
        solver.Add(tourPosition[0] == 0);

        // Ràng buộc: Hành trình kết thúc tại thành phố ban đầu.
        solver.Add((from i in Enumerable.Range(0, numCities) select x[i, 0]).ToArray().Sum() == 1);

        // Ràng buộc: Xây dựng vị trí của mỗi thành phố trong hành trình dựa trên biến x.
        for (int i = 0; i < numCities; i++)
        {
            for (int j = 1; j < numCities; j++)
            {
                solver.Add(tourPosition[i] - tourPosition[j] + numCities * x[i, j] <= numCities - 1);
            }
        }


        // Objective Function
        Objective objective = solver.Objective();
        for (int i = 0; i < numCities; i++)
        {
            for (int j = 0; j < numCities; j++)
            {
                objective.SetCoefficient(x[i, j], costMatrix[i, j]);
            }
        }
        objective.SetMinimization();

        // Solve
        Solver.ResultStatus resultStatus = solver.Solve();

        // Print solution
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            List<int> tour = new List<int>();
            int nextCity = 0;
            int totalCost = 0;
            while (true)
            {
                tour.Add(nextCity);
                for (int j = 0; j < numCities; j++)
                {
                    if (x[nextCity, j].SolutionValue() > 0)
                    {
                        totalCost += costMatrix[nextCity, j]; // Add cost from current city to next city to total cost
                        nextCity = j;
                        break;
                    }
                }
                if (nextCity == 0)
                {
                    break;
                }
            }
            return (tour, totalCost);
        }
        else
        {
            return (null, -1);
        }
    }

    //public void Main(string[] args)
    //{
    //    int[,] costMatrix = ReadCostMatrixFromFile("C:\\Users\\admin\\Documents\\WorkSpace\\TKPTGT\\btl\\TSP\\TSP\\dataset5.txt");
    //    if (costMatrix != null)
    //    {
    //        SolveTSP(costMatrix);
    //    }
    //    else
    //    {
    //        Console.WriteLine("Failed to read cost matrix from file.");
    //    }
    //}

    public int[,] ReadCostMatrixFromFile(string filePath)
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
            int numCities = int.Parse(lines[0]);
            int[,] costMatrix = new int[numCities, numCities];
            for (int i = 0; i < numCities; i++)
            {
                string[] values = lines[i + 1].Split('\t');
                for (int j = 0; j < numCities; j++)
                {
                    costMatrix[i, j] = int.Parse(values[j]);
                }
            }
            return costMatrix;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading cost matrix from file: " + ex.Message);
            return null;
        }
    }
    public void printResult((List<int>, int) result)
    {
        Console.WriteLine($"Chi phi: {result.Item2}");
        Console.Write("Duong di: ");
        for (int i = 0; i < result.Item1.Count; i++)
        {
            Console.Write($"{result.Item1[i]} -> ");
        }
        Console.WriteLine(0);
    }
}