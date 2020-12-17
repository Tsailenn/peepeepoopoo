import numpy as np
import matplotlib.pyplot as plt
import random as rnd
import math
import pandas as pd


class KMeans:
    def __init__(self):
        self.data = pd.read_csv("country_and_states.csv", sep=",")
        self.R = 6371

        self.totalCentroids = 6

        self.xCoords = []
        self.yCoords = []

        self.oldCentroidX = []
        self.oldCentroidY = []

        self.centroidX = []
        self.centroidY = []

        self.membership = []
        self.randomInd = []
        for i in range(self.data.shape[0]):
            self.Convert(self.data.iloc[i][1], self.data.iloc[i][2])
        
        while (len(self.randomInd) < self.totalCentroids):
            ind = rnd.randint(0, len(self.xCoords))
            if (not (ind in self.randomInd)):
                self.randomInd.append(ind)

        for i in self.randomInd:
            self.centroidX.append(self.xCoords[i])
            self.centroidY.append(self.yCoords[i])

        print(self.centroidX)
        print(self.centroidY)
        plt.scatter(self.centroidX, self.centroidY)
        plt.show()
    
    def RecalculateCentroid(self):
        for i in range(len(self.membership)):
            totalX = 0
            totalY = 0
            for j in self.membership[i]:
                if (math.isnan(j[0]) or math.isnan(j[1])):
                    totalX += 0
                    totalY += 0
                else:
                    totalX += j[0]
                    totalY += j[1]
                
            meanX = totalX / len(self.membership[i])
            meanY = totalY / len(self.membership[i])
            self.centroidX[i] = meanX
            self.centroidY[i] = meanY
    
    def Convert(self, lat, lon):
        x = self.R * np.cos(lat) * np.cos(lon)
        y = self.R * np.cos(lat) * np.sin(lon)
        self.xCoords.append(x)
        self.yCoords.append(y)

    def EuclideanDist(self, x0, y0, x1, y1):
        return np.sqrt((x0-x1)**2 + (y0-y1)**2)

    def CompareCentroids(self):
        for i in range(self.totalCentroids):
            if (self.oldCentroidX[i] != self.centroidX[i] or self.oldCentroidY[i] != self.centroidY[i]):
                return False
        return True


    def PushCentroid(self):
        self.oldCentroidX = self.centroidX.copy()
        self.oldCentroidY = self.centroidY.copy()

    def ClearCentroid(self):
        self.centroidX = []
        self.centroidY = []
    
    def UpdateMembership(self):
        self.membership = []
        for i in range(self.totalCentroids):
            self.membership.append([])
        for i in range(len(self.xCoords)):
            chosenInd = 0
            shortestDist = 999999999999
            for j in range(len(self.centroidX)):
                if (self.EuclideanDist(self.centroidX[j], self.centroidY[j], self.xCoords[i], self.yCoords[i]) < shortestDist):
                    chosenInd = j
                    shortestDist = self.EuclideanDist(self.centroidX[j], self.centroidY[j], self.xCoords[i], self.yCoords[i])
            self.membership[chosenInd].append([self.xCoords[i], self.yCoords[i]])



    def Begin(self):
        self.PushCentroid()
        self.UpdateMembership()
        self.RecalculateCentroid()
        while (not self.CompareCentroids()):
            self.PushCentroid()
            self.UpdateMembership()
            self.RecalculateCentroid()

    def Plot(self):
        plt.scatter(self.xCoords, self.yCoords)
        plt.scatter(self.centroidX, self.centroidY)
        plt.show()

    def Announce(self):
        cluster = []
        for i in range(self.totalCentroids):
            cluster.append([[], []])

        for i in range(len(self.xCoords)):
            chosenInd = 0
            shortestDist = 999999999999
            for j in range(len(self.centroidX)):
                if (self.EuclideanDist(self.centroidX[j], self.centroidY[j], self.xCoords[i], self.yCoords[i]) < shortestDist):
                    chosenInd = j
                    shortestDist = self.EuclideanDist(self.centroidX[j], self.centroidY[j], self.xCoords[i], self.yCoords[i])
            a = self.xCoords[i]
            b = self.yCoords[i]
            if (math.isnan(a) or math.isnan(b)):
                a = 0
                b = 0
            cluster[chosenInd][0].append(a)
            cluster[chosenInd][1].append(b)
            
        print(self.centroidX)
        print(self.centroidY)    

        for i in cluster:
            plt.scatter(i[0], i[1])
        plt.scatter(self.centroidX, self.centroidY)
        plt.show()
        

kMeansObj = KMeans()
kMeansObj.Begin()
kMeansObj.Announce()
