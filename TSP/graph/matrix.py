import random

def generate_random_cost_matrix(num_cities, min_cost=1, max_cost=50):
    cost_matrix = []
    for i in range(num_cities):
        row = []
        for j in range(num_cities):
            if i == j:
                row.append(0)
            elif i > j:
                row.append(cost_matrix[j][i])
            else:
                row.append(random.randint(min_cost, max_cost))
        cost_matrix.append(row)
    return cost_matrix

def write_matrix_to_file(matrix, file_path):
    with open(file_path, 'w') as file:
        file.write(str(len(matrix)) + '\n')
        for row in matrix:
            file.write('\t'.join(map(str, row)) + '\n')

matrix = generate_random_cost_matrix(7)
for row in matrix:
    print("\t".join(map(str, row)))
write_matrix_to_file(matrix,r"C:\Users\admin\Documents\WorkSpace\TKPTGT\btl\TSP\TSP\dataset5.txt")