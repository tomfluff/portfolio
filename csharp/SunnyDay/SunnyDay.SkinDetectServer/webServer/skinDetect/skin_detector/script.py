
# import the necessary packages
from pyimagesearch import imutils
import numpy as np
import argparse
import cv2

def runExamples():
    for i in range(1, 5):
        getSkinColor("examples/" + str(i) + ".jpg", "examples/" + str(i) + "out_skin.jpg", "examples/" + str(i) + "out_avg.jpg")

def computeAvg(skin, outpath_acg):
    maskSkin = getMask(skin)
    skin = np.ma.masked_array(skin, mask=maskSkin)
    average_color_per_row = np.ma.average(skin, axis = 0)
    average_color = np.ma.average(average_color_per_row, axis = 0)
    average_color = np.uint8(average_color)
    average_color_img = np.array([[average_color]*100]*100, np.uint8)
    cv2.imwrite(outpath_acg, average_color_img)
    return average_color

def getMask(skin):
    res = np.zeros(skin.shape)
    li_cnt = -1
    pic_cnt = -1
    for li in skin:
        li_cnt += 1
        pic_cnt = -1
        for pixel in li:
            pic_cnt += 1
            if pixel[2] == 0:
                res[li_cnt][pic_cnt]=1
    return res

def findNearestBucket(bucketList, pixel):
    minLen = None
    minBuc = None
    for buc in bucketList:
        dist = ((buc[0] - pixel[0]) ** 2) + ((buc[1] - pixel[1]) ** 2) + ((buc[2] - pixel[2]) ** 2)
        if minLen == None or minLen > dist:
            minLen = dist
            minBuc = buc
    return minBuc
    
def bucketsavg(skin, outpath_acg, bucketList):
    hist = {}
    for buc in bucketList:
        hist[buc] = 0
    for li in skin:
        for pixel in li:
            if pixel[2] != 0:
                hist[findNearestBucket(bucketList, pixel)] += 1
    maxBuc = None
    maxVal =  -1
    for buc in hist:
        val = hist[buc]
        if maxVal < val:
            maxBuc = buc
            maxVal = val
    average_color_img = np.array([[maxBuc]*100]*100, np.uint8)
    cv2.imwrite(outpath_acg, average_color_img)
    return maxBuc
    
        
def getSkinColor(pathToImage, outpath_onlySkin, outpath_acg):
    # define the upper and lower boundaries of the HSV pixel
    # intensities to be considered 'skin'
    #lower = np.array([0, 48, 80], dtype = "uint8")
    lower = np.array([0, 5, 75], dtype = "uint8")
    upper = np.array([20, 255, 255], dtype = "uint8")

    frame = cv2.imread(pathToImage)

    # resize the frame, convert it to the HSV color space,
    # and determine the HSV pixel intensities that fall into
    # the speicifed upper and lower boundaries
    frame = imutils.resize(frame, width = 400)
    converted = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    skinMask = cv2.inRange(converted, lower, upper)

    # apply a series of erosions and dilations to the mask
    # using an elliptical kernel
    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (11, 11))
    skinMask = cv2.erode(skinMask, kernel, iterations = 2)
    skinMask = cv2.dilate(skinMask, kernel, iterations = 2)

    # blur the mask to help remove noise, then apply the
    # mask to the frame
    skinMask = cv2.GaussianBlur(skinMask, (3, 3), 0)
    skin = cv2.bitwise_and(frame, frame, mask = skinMask)

    # show the skin in the image along with the mask
    cv2.imwrite(outpath_onlySkin, skin)
    #average_color = computeAvg(skin, outpath_acg)
    #print average_color
    #average_color = findNearestBucket([(40, 1.2, 98.04),(70,2,99),(49.09,34.38,87.84),(37.5,46.64,87.45),(13.91,68.32,39.61),(272.73,25.58,16.86)], average_color)
    #average_color = bucketsavg(skin, outpath_acg, [(250,249,247),(251,252,246),(224,210,147),(223,184,119),(101,48,32),(38,32,43)])
    #average_color = bucketsavg(skin, outpath_acg, [(40, 1.2, 98.04),(70,2,99),(49.09,34.38,87.84),(37.5,46.64,87.45),(13.91,68.32,39.61),(272.73,25.58,16.86)])
    #average_color = bucketsavg(skin, outpath_acg, [(280, 3, 245), (40,3,236), (40,3,250),(54,23,253),(41,23,253),(43,25,254),(5,11,250),(21,14,243),(32,10,244),(67,8,252),(44,15,252),(43,29,254),(48,30,255),(48,30,255),(46,48,241),(48,70,239),(49,88,224),(49,66,242),(43,82,235),(49,111,235),(45,139,227),(43,135,225),(42,114,223),(37,118,222),(38,127,199),(35,122,188),(])
    
#print "The skin color in HSV scale is: "
    #print average_color
    return average_color

