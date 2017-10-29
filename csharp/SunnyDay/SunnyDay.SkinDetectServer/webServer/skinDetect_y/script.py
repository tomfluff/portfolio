
# import the necessary packages
from pyimagesearch import imutils
from sklearn.cluster import MiniBatchKMeans
from sklearn.cluster import KMeans
import math
import numpy as np
import argparse
import cv2

def runExamples():
    for i in range(1, 38):
        inP = "examples/" + str(i) + ".jpg"
        outS = "examples/" + str(i) + "_a_oskin.jpg"
        outK = "examples/" + str(i) + "_b_kmean.jpg"
        outA = "examples/" + str(i) + "_c_avgc.jpg"
        (a,b) = getSkinColor(inP, outS, outK, outA)

def mark_largest_contours(image):
    # prepare matrix for the edges
    # accumEdged = np.zeros(image.shape[:2], dtype="uint8")
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    ret, binary = cv2.threshold(gray,5,255,cv2.THRESH_BINARY)
    accumEdged = cv2.Canny(binary, 100, 200)
    # find contours in the accumulated image, keeping only the largest one
    (cnts, _) = cv2.findContours(accumEdged.copy(), cv2.RETR_EXTERNAL,
                                 cv2.CHAIN_APPROX_SIMPLE)
    cnts = sorted(cnts, key=cv2.contourArea, reverse=True)[:5]
    # mark all contours
    for cnt in cnts:
        x, y, w, h = cv2.boundingRect(cnt)
        cv2.rectangle(image, (x, y), (x + w, y + h), (255, 0, 0), 2)

    return image

def cluster_and_mark(skin):
    (h, w) = skin.shape[:2]
    # convert color to L*a*b for euclid cluster calculations
    # with the k-means clostering algorithm
    skin = cv2.cvtColor(skin, cv2.COLOR_BGR2LAB)
    # reshape image into a vector so clustering can be applied
    skin = skin.reshape((skin.shape[0] * skin.shape[1], 3))
    # apply k-means using the specified number of clusters and
    # then create the quantized image based on the predictions
    clt = MiniBatchKMeans(5)
    labels = clt.fit_predict(skin)
    quant = clt.cluster_centers_.astype("uint8")[labels]
    # reshape the feature vectors to images
    quant = quant.reshape((h, w, 3))
    # convert from L*a*b* to HSV
    quant = cv2.cvtColor(quant, cv2.COLOR_LAB2BGR)
    # convert color back to rgb and crop black bounds
    quant = mark_largest_contours(quant)
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
    clt = KMeans(5)
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
    buckets = [[16, 10, 253], [11, 29, 239], [13, 53, 235], [16, 108, 231], [14, 138, 199], [16, 153, 142],[10, 136, 67]]
    best_v = 1024
    best_i = 0
    i = -1
    for buc in buckets:
        i += 1
        tmp_v = distance_between_colors(color_hsv[0,0], buc)
        if tmp_v < best_v:
            best_v = tmp_v
            best_i = i
    return best_i

def get_skin_color_from_hist(hist):
    sum = [[hist[0,0],0]]

    for col in hist[0]:
        b = False
        for s in sum:
            if s[0][0] == col[0] and s[0][1] == col[1] and s[0][2] == col[2]:
                s[1] += 1
                b = True
                break
        if b is False:
            sum.extend([[col,1]])
    maxV = 0
    maxC = None
    for s in sum:
        if (s[1] >= maxV) and (s[0][0] > 5 and s[0][1] > 5 and s[0][2] > 5):
            maxV = s[1]
            maxC = s[0]
    return maxC

def getSkinColor(pathToImage, outpath_onlySkin, outpath_Kmean, outpath_AGC):
    # define the upper and lower boundaries of the HSV pixel
    # intensities to be considered 'skin'
    lower_a = np.array([0, 5, 40], dtype="uint8")
    upper_a = np.array([25, 255, 255], dtype = "uint8")

    lower_b = np.array([160, 10, 230], dtype="uint8")
    upper_b = np.array([179, 30, 250], dtype="uint8")

    lower_c = np.array([0, 70, 70], dtype="uint8")
    upper_c = np.array([3, 85, 98], dtype="uint8")

    # get the image from the file path
    frame = cv2.imread(pathToImage)

    # resize the frame, convert it to the HSV color space,
    # and determine the HSV pixel intensities that fall into
    # the speicifed upper and lower boundaries
    frame = imutils.resize(frame, width = 300, height=300)
    frame = cv2.medianBlur(frame, 9)
    converted = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    skinMask_a = cv2.inRange(converted, lower_a, upper_a)
    skinMask_b = cv2.inRange(converted, lower_b, upper_b)
    skinMask_c = cv2.inRange(converted, lower_c, upper_c)

    # combine the masks (edge cases handled here)
    skinMask = skinMask_a + skinMask_b + skinMask_c

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
    # quant = cluster_and_mark(skin)
    quant = cluster_and_categorize(skin)

    # save outputs of k-mean clustering
    cv2.imwrite(outpath_Kmean, quant)

    # get solid 150x150 image of the detected skin color
    k_color = np.zeros((200, 200, 3), np.uint8)
    k_color[:] = get_skin_color_from_hist(quant)

    skin_type = categorize_skin_color(k_color[50:51,50:51])

    print "Image - ", pathToImage, "\n---------------"
    print "Your skin color is: ", k_color[50,50]
    print "Your skin type is: ", skin_type, "\n"

    cv2.putText(k_color, '#{}'.format(skin_type), (10, 60), cv2.FONT_HERSHEY_COMPLEX_SMALL, 3, (255,255,255),4)
    cv2.imwrite(outpath_AGC, k_color)

    return (k_color[50,50], skin_type)

if __name__ == '__main__':
    runExamples()
    print "\nFinished!"


# Skin color buckets (36):
# Full 36 buckets HSV (H) [[280, 3, 245], [40,3,236], [40,3,250],[54,23,253],[41,23,253],[43,25,254],[5,11,250],[21,14,243],[32,10,244],[67,8,252],[44,15,252],[43,29,254],[48,30,255],[48,30,255],[46,48,241],[48,70,239],[49,88,224],[49,66,242],[43,82,235],[49,111,235],[45,139,227],[43,135,225],[42,114,223],[37,118,222],[38,127,199],[35,122,188]]
# Full 36 buckets HSV (H/2) [[140, 3, 245], [20,3,236], [20,3,250],[27,23,253],[20,23,253],[22,25,254],[2,11,250],[11,14,243],[16,10,244],[34,8,252],[22,15,252],[21,29,254],[24,30,255],[24,30,255],[23,48,241],[24,70,239],[25,88,224],[25,66,242],[21,82,235],[25,111,235],[22,139,227],[21,135,225],[21,114,223],[18,118,222],[18,127,199],[17,122,188]]
# Full 7 buckets HSV (H) [[42,20,255],[38,34,249],[36,65,243], [36,100,214], [32,147,141], [21,174,41]]
# Full 7 buckets HSV (H/2) [[16, 10, 253], [11, 29, 239], [13, 53, 235], [16, 108, 231], [14, 138, 199], [16, 153, 142],[10, 136, 67]]