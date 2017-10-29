# import the necessary packages
from pyimagesearch import imutils
from sklearn.cluster import MiniBatchKMeans
from sklearn.cluster import KMeans
import math
import numpy as np
import argparse
import cv2


def hsv_color_test(imgIP):
    img = cv2.imread(imgIP)
    imgHSV = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
    crop = imgHSV[250:256,70:76]
    print crop

if __name__ == '__main__':
    hsv_color_test("./1.jpg")
    print "\nFinished!"