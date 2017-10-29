
# import the necessary packages
from pyimagesearch import imutils
from sklearn.cluster import MiniBatchKMeans
from sklearn.cluster import KMeans
import math
import numpy as np
import argparse
import cv2

def runExamples():
    for i in range(1, 15):
        inP = "examples/" + str(i) + ".jpg"
        outS = "examples/" + str(i) + "_a_skin.jpg"
        outK = "examples/" + str(i) + "_b_kmean.jpg"
        outA = "examples/" + str(i) + "_c_agc.jpg"
        getSkinColor(inP, outS, outK, outA)

def mark_largest_contours(image):
    # prepare matrix for the edges
    accumEdged = np.zeros(image.shape[:2], dtype="uint8")
    # loop over the blue, green, and red channels, respectively
    for chan in cv2.split(image):
        # blur the channel, extract edges from it, and accumulate the set
        # of edges for the image
        chan = cv2.medianBlur(chan, 5)
        edged = cv2.Canny(chan, 50, 200)
        accumEdged = cv2.bitwise_or(accumEdged, edged)
    # find contours in the accumulated image, keeping only the largest one
    (cnts, _) = cv2.findContours(accumEdged.copy(), cv2.RETR_EXTERNAL,
                                 cv2.CHAIN_APPROX_SIMPLE)
    cnts = sorted(cnts, key=cv2.contourArea, reverse=True)[:4]
    # mark all contours
    for cnt in cnts:
        x, y, w, h = cv2.boundingRect(cnt)
        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)

    return image

def cluster_and_parse(skin):
    (h, w) = skin.shape[:2]
    # convert color to L*a*b for euclid cluster calculations
    # with the k-means clostering algorithm
    skin = cv2.cvtColor(skin, cv2.COLOR_BGR2LAB)
    # reshape image into a vector so clustering can be applied
    skin = skin.reshape((skin.shape[0] * skin.shape[1], 3))
    # apply k-means using the specified number of clusters and
    # then create the quantized image based on the predictions
    clt = MiniBatchKMeans(4)
    labels = clt.fit_predict(skin)
    quant = clt.cluster_centers_.astype("uint8")[labels]
    # reshape the feature vectors to images
    quant = quant.reshape((h, w, 3))
    # convert from L*a*b* to HSV
    quant = cv2.cvtColor(quant, cv2.COLOR_LAB2BGR)
    # convert color back to rgb and crop black bounds
    #quant = extract_largest_contour(quant)
    return quant

def centroid_histogram(clt):
    # grab the number of different clusters and create a histogram
    # based on the number of pixels assigned to each cluster
    numLabels = np.arange(0, len(np.unique(clt.labels_)) + 1)
    (hist, _) = np.histogram(clt.labels_, bins=numLabels)

    # normalize the histogram, such that it sums to one
    hist = hist.astype("float")
    hist /= hist.sum()

    # return the histogram
    return hist

def plot_colors(hist, centroids):
    # initialize the bar chart representing the relative frequency
    # of each of the colors
    bar = np.zeros((50, 300, 3), dtype="uint8")
    startX = 0

    # loop over the percentage of each cluster and the color of
    # each cluster
    for (percent, color) in zip(hist, centroids):
        # plot the relative percentage of each cluster
        endX = startX + (percent * 300)
        cv2.rectangle(bar, (int(startX), 0), (int(endX), 50),
                      color.astype("uint8").tolist(), -1)
        startX = endX

    # return the bar chart
    return bar

def cluster_and_categorize(image):
    # reshape the image to be a list of pixels
    image = image.reshape((image.shape[0] * image.shape[1], 3))

    # cluster the pixel intensities
    clt = KMeans(n_clusters=4)
    clt.fit(image)

    # build a histogram of clusters and then create a figure
    # representing the number of pixels labeled to each color
    hist = centroid_histogram(clt)
    bar = plot_colors(hist, clt.cluster_centers_)
    return bar

def distance_between_colors(color_a, color_b):
    vec = color_a - color_b
    return cv2.norm(vec)

def categorize_skin_color(color):
    color_hsv = cv2.cvtColor(color, cv2.COLOR_BGR2HSV)
    buckets = [[42,20,255],[38,34,249],[36,65,243], [36,100,214], [32,147,141], [21,174,41]]
    best_v = 1024
    best_i = 0
    i = 0
    for buc in buckets:
        i += 1
        tmp_v = distance_between_colors(color_hsv[0,0], buc)
        if tmp_v < best_v:
            best_v = tmp_v
            best_i = i
    return best_i

def get_skin_color_from_hist(hist):
    # sum[i][0] = the color, sum[i][1] = amount
    sum = [[hist[0,0],0]]

    for col in hist[0]:
        b = False
        for s in sum:
            if s[0][0] is col[0] and s[0][1] is col[1] and s[0][2] is col[2]:
                s[1] += 1
                b = True
        if b is False:
            sum.extend([[col,1]])
    # wdth = hist.shape[1]
    # if hist.item(0,wdth-1,0) < 6 and hist.item(0,wdth-1,1) < 6 and hist.item(0,wdth-1,2) < 6:
    #     return hist[0,0]
    # return hist[0,wdth-1]
    maxV = 0
    maxC = None
    for s in sum:
        if maxV >= s[1] and s[0] is not [0,0,0]:
            maxV = s[1]
            maxC = s[0]
    return maxC

def getSkinColor(pathToImage, outpath_onlySkin, outpath_Kmean, outpath_AGC):
    # define the upper and lower boundaries of the HSV pixel
    # intensities to be considered 'skin'
    # lower = np.array([0, 48, 80], dtype = "uint8")
    lower = np.array([0, 5, 40], dtype="uint8")
    upper = np.array([55, 255, 255], dtype = "uint8")

    # get the image from the file path
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

    # do the action money shot!
    quant = cluster_and_categorize(skin)

    # save outputs of k-mean clustering
    cv2.imwrite(outpath_Kmean, quant)

    # get solid 150x150 image of the detected skin color
    k_color = np.zeros((150, 150, 3), np.uint8)
    k_color[:] = get_skin_color_from_hist(quant)
    cv2.imwrite(outpath_AGC, k_color)

    skin_type = categorize_skin_color(k_color[0:1,0:1])

    print pathToImage + "\n---------------"
    print "Your skin color is: "
    print k_color[0,0]
    print "Your skin type is: "
    print skin_type
    print ""


    # convert color to HSVcalc best match skin color and save a sample
    # quant = cv2.cvtColor(quant, cv2.COLOR_BGR2HSV)
    # bucketList = [(280, 3, 245), (40,3,236), (40,3,250),(54,23,253),(41,23,253),(43,25,254),(5,11,250),(21,14,243),(32,10,244),(67,8,252),(44,15,252),(43,29,254),(48,30,255),(48,30,255),(46,48,241),(48,70,239),(49,88,224),(49,66,242),(43,82,235),(49,111,235),(45,139,227),(43,135,225),(42,114,223),(37,118,222),(38,127,199),(35,122,188)]
    # avgC,avgI = bucketsavg(quant, outpath_AGC, bucketList)

    #print "Your skin type is: "
    # print avgI
    # print " and the HSV color is: "
    # print avgC
    #print "Finished!"

    #average_color = computeAvg(skin, outpath_acg)
    #print average_color
    #average_color = findNearestBucket([(40, 1.2, 98.04),(70,2,99),(49.09,34.38,87.84),(37.5,46.64,87.45),(13.91,68.32,39.61),(272.73,25.58,16.86)], average_color)
    #average_color = bucketsavg(skin, outpath_acg, [(250,249,247),(251,252,246),(224,210,147),(223,184,119),(101,48,32),(38,32,43)])
    #average_color = bucketsavg(skin, outpath_acg, [(40, 1.2, 98.04),(70,2,99),(49.09,34.38,87.84),(37.5,46.64,87.45),(13.91,68.32,39.61),(272.73,25.58,16.86)])
    #average_color = bucketsavg(skin, outpath_acg, [(280, 3, 245), (40,3,236), (40,3,250),(54,23,253),(41,23,253),(43,25,254),(5,11,250),(21,14,243),(32,10,244),(67,8,252),(44,15,252),(43,29,254),(48,30,255),(48,30,255),(46,48,241),(48,70,239),(49,88,224),(49,66,242),(43,82,235),(49,111,235),(45,139,227),(43,135,225),(42,114,223),(37,118,222),(38,127,199),(35,122,188)])
    
#print "The skin color in HSV scale is: "
    #print average_color
    #return average_color

if __name__ == '__main__':
    runExamples()
