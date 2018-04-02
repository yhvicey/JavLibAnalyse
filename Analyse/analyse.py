import pandas as pd
import numpy
import matplotlib.pyplot as plt

# Variables
PATH = "../Data/data_cn_2018_04_02.tsv"

data = pd.read_csv(PATH, sep='\t', header=0)
