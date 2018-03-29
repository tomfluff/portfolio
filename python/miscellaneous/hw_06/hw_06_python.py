#Skeleton file for HW6 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#You may add other utility functions to this file,
#but you may NOT change the signature of the existing ones.

#Change the name of the file to your ID number (extension .py).

from hw_06_matrix import *

############
# QUESTION 1
############

def fingerprint(mat):
    assert isinstance(mat,Matrix)
    k,makesure = mat.dim()
    assert k==makesure

    return sum(mat[i,j] for i in range(k) for j in range(k))

def move_right(mat, i, j, k, fp):
    left_cul = right_cul = 0
    for l in range(k):
        left_cul += mat[i+l,j]
        right_cul += mat[i+l,j+k]
    return fp - left_cul + right_cul

def move_down(mat, i, j, k, fp):
    bot_row = top_row = 0
    for l in range(k):
        top_row += mat[i,j+l]
        bot_row += mat[i+k,j+l]
    return fp - top_row + bot_row


def has_repeating_subfigure(mat, k):
    n,m = mat.dim()
    fp = fingerprint(mat[0:k,0:k])
    dic = {str(fp):'True'}
    r_move = d_move = fp
    for i in range(n-k+1):
        for j in range(m-k+1):
            if j+k < m:
                r_move = move_right(mat,i,j,k,r_move)
                if str(r_move) in dic:
                    return True
                else:
                    dic[str(r_move)] = True
        if i+k < n:
            d_move = move_down(mat, i,0,k,d_move)
            if str(d_move) in dic:
                return True
            else:
                dic[str(d_move)] = True
            r_move = d_move
    return False

########
# Tester
########

def test():
    #Question 1
    im = Matrix.load("./hw_06_sample.bitmap")
    k = 2
    if fingerprint(im[:k,:k]) != 384 or \
       fingerprint(im[1:k+1,1:k+1]) != 256 or \
       fingerprint(im[0:k,1:k+1]) != 511:
        print("error in fingerprint()")
    if move_right(im, 0, 0, k, 384) != 511:
        print("error in move_right()")
    if move_down(im, 0, 1, k, 511) != 256:
        print("error in move_down()")
    if has_repeating_subfigure(im, k) != True or\
       has_repeating_subfigure(im, k=3) != False:
        print("error in repeating_subfigure()")
