import matplotlib.pyplot as plt
import pandas as pd
import csv
import datetime

csv_file = open("C:/Users/seigo/Desktop/Programing Test/BTCData/20180728/BTC20180728.csv", "r", encoding="ms932", errors="", newline="")
f = csv.reader(csv_file, delimiter=",", doublequote=False, lineterminator="\r\n")
data = [ e for e in f]
#data2 = pd.read_csv("C:/Users/seigo/Desktop/Programing Test/BTCData20180728/test.csv").values.tolist()
print(data[1])
print(data[1][1])
print([e[1] for e in data[1:100]])
x=[ datetime.datetime.strptime(e[0] + e[1], '%Y/%m/%d%H:%M:%S') for e in data[1:]]
y1=[int(e[2]) for e in data[1:]]
y2=[int(e[3]) for e in data[1:]]

plt.plot(x,y1,color='red',marker='o',label='setosa')
plt.plot(x,y2,color='blue',marker='o',label='setosa')
#plt.scatter(X[50:100,0],X[50:100,1],color='blue',marker='x',label='versicolor')
plt.show()
