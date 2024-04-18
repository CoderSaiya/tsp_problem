from ortools.linear_solver import pywraplp
import time
import codecs

def solve_tsp(cost_matrix):
    num_cities = len(cost_matrix)
    solver = pywraplp.Solver.CreateSolver('GLOP')
    if not solver:
        return
    
    # Decision Variables
    x = {}
    for i in range(num_cities):
        for j in range(num_cities):
            x[i, j] = solver.BoolVar(f'x[{i},{j}]')
    
    # Constraints
    for i in range(num_cities):
        solver.Add(sum(x[i, j] for j in range(num_cities)) == 1)  # Outgoing edges from city i
        solver.Add(sum(x[j, i] for j in range(num_cities)) == 1)  # Incoming edges to city i
    
    for i in range(num_cities):
        solver.Add(x[i, i] == 0)  # No self-loops
    
    for i in range(num_cities):
        for j in range(num_cities):
            if i != j:
                solver.Add(x[i, j] + x[j, i] <= 1)  # Ensure no duplicate paths between any pair of cities

    for i in range(num_cities):
        solver.Add(sum(x[i, j] for j in range(num_cities)) == 1)
    
    # Objective Function
    objective = solver.Objective()
    for i in range(num_cities):
        for j in range(num_cities):
            objective.SetCoefficient(x[i, j], cost_matrix[i][j])
    objective.SetMinimization()
    
    status = solver.Solve()
    
    if status == pywraplp.Solver.OPTIMAL:
        tour = []
        next_city = 0
        total_cost = 0
        while True:
            tour.append(next_city)
            for j in range(num_cities):
                if x[next_city, j].solution_value() > 0:
                    total_cost += cost_matrix[next_city][j]  # Add cost from current city to next city to total cost
                    next_city = j
                    break
            if next_city == 0:
                break
        return tour, total_cost
    else:
        return None, None

def read_cost_matrix_from_file(file_path):
    with codecs.open(file_path, 'r', encoding='utf-8-sig') as file:
        file.readline()
        cost_matrix = []
        for line in file:
            row = list(map(int, line.strip().split()))
            cost_matrix.append(row)

    return cost_matrix

def print_matrix(matrix):
    for row in matrix:
        print("\t".join(map(str, row)))

def main():
    file_path = r"C:\Users\admin\Documents\WorkSpace\TKPTGT\btl\TSP\TSP\Dataset2.txt"
    cost_matrix = read_cost_matrix_from_file(file_path)

    print("Ma tran:")
    print_matrix(cost_matrix)
    print("\n")
    
    optimal_tour, total_cost = solve_tsp(cost_matrix)
    if optimal_tour:
        print("Duong di:", optimal_tour)
        print("Tong chi phi:", total_cost)
    else:
        print("Khong tim thay!")

if __name__ == "__main__":
    start_time = time.time()
    main()
    end_time = time.time()
    execution_time = end_time - start_time
    print("Thoi gian:", execution_time, "s")