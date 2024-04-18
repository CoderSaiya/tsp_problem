import csv
import matplotlib.pyplot as plt

def read_data_file(file_path):
    with open(file_path, 'r') as file:
        data = file.read().strip().split(',')
        data = [int(item) for item in data]
    return data

def plot_comparison(dataset1, dataset2, dataset3):
    algorithms = ['vetCan', 'thamLam', 'ACO']
    datasets = ['Dataset 1', 'Dataset 2', 'Dataset 3']

    plt.figure(figsize=(10, 5))

    bar_width = 0.2
    index = range(len(algorithms))

    for i, dataset in enumerate([dataset1, dataset2, dataset3], start=1):
        dataset_values = [dataset[algo] for algo in algorithms]
        plt.bar([pos + i * bar_width for pos in index], dataset_values, bar_width, label=datasets[i - 1])

    plt.xlabel('Algorithms')
    plt.ylabel('Values')
    plt.title('Comparison of Algorithms')
    plt.xticks([pos + bar_width for pos in index], algorithms)
    plt.legend()
    plt.tight_layout()
    plt.show()

def main():
    dataset1 = {}
    dataset2 = {}
    dataset3 = {}

    data_dataset1 = read_data_file(r'C:\Users\admin\Documents\WorkSpace\TKPTGT\btl\TSP\TSP\result_dataset1.txt')
    dataset1['vetCan'], dataset1['thamLam'], dataset1['ACO'] = data_dataset1

    data_dataset2 = read_data_file(r'C:\Users\admin\Documents\WorkSpace\TKPTGT\btl\TSP\TSP\result_dataset2.txt')
    dataset2['vetCan'], dataset2['thamLam'], dataset2['ACO'] = data_dataset2

    data_dataset3 = read_data_file(r'C:\Users\admin\Documents\WorkSpace\TKPTGT\btl\TSP\TSP\result_dataset3.txt')
    dataset3['vetCan'], dataset3['thamLam'], dataset3['ACO'] = data_dataset3

    plot_comparison(dataset1, dataset2, dataset3)

if __name__ == "__main__":
    main()
