/*
    KDArray
    Author: Yotam
 */

#ifndef SPKDARRAY_H_
#define SPKDARRAY_H_

#include "SPPoint.h"

typedef struct sp_kd_array* SPKDArray;

/*
 *  Allocates the needed memory, copies the points
 *  and creates the KDArray from them, with the given size.
 *  The method assumes the size param (size) is valid for the points array (arr).
 *
 *  arr - the SPPoint array
 *  size - the amount of points in the array
 *
 *  @return
 *  NULL if error occured, otherwise returns the newly created SPKDArray.
 */
SPKDArray Init(SPPoint* arr, int size);

/*
 *  Splits the given SPKDArray based on a given coordinate.
 *  This function assumes that the coordinate param (coor) is valid for the given SPKDArray.
 *
 *  kdArr - the SPKDArray to split
 *  coor - the coordinate to split by
 *
 *  @return
 *  NULL if error occured , otherwise returns two SPKDArray (kdLeft, kdRight).
 */
SPKDArray* Split(SPKDArray spkdArr, int coor);

/**
 * Returns the index of the coordinate with the maximal spread
 * @method spKDArrayFindMaxSpreadDim
 * @param  arr                       The KDArray to check
 * @return                           An integer representing the index of the coordinate
 */
int spKDArrayFindMaxSpreadDim(SPKDArray arr);

/**
 * Returns a copy of the point at an index in the KDArray
 * @method spKDArrayGetPoint
 * @param  arr               The KDArray to check
 * @param  index             The index of the point
 * @return                   NULL if any error occured, otherwise an SPPoint representing a copy of the point
 */
SPPoint spKDArrayGetPoint(SPKDArray arr, int index);

/**
 * Returns a copy of the SPPoints array of a KDArray
 * @method spKDArrayGetPointsArray
 * @param  spkdArr                 The KDArray to copy from
 * @return                         NULL if any error occured, otherwise an SPPoint array (copyof original)
 */
SPPoint* spKDArrayGetPointsArray(SPKDArray spkdArr);

/**
 * Returns the p[dim]->value of the largest point (according to the given dimension)
 * @method spKDArrayGetHighestPointValueInDim
 * @param  arr                                The KDArray to check
 * @param  dim                                The dimension to check by
 * @return                                    A double representing p[dim]->value
 */
double spKDArrayGetHighestPointValueInDim(SPKDArray arr, int dim);

/**
 * Gets the point index at position [i,j] in the KDArray matrix
 * @method spKDArrayGetIndexInArray
 * @param  spkdArr                  The KDArray to check
 * @param  i                        The i-th index
 * @param  j                        The j-th index
 * @return                          An integer representing the index at the given position
 */
int spKDArrayGetIndexInArray(SPKDArray spkdArr, int i, int j);

/**
 * Gets the dimension of the KDArray
 * @method spKDArrayGetDimension
 * @param  spkdArr               The KDArray to check
 * @return                       An integer representing the dimension
 */
int spKDArrayGetDimension(SPKDArray spkdArr);

/**
 * Gets the size of the KDArray (= the number of points in it)
 * @method spKDArrayGetSize
 * @param  spkdArr          The KDArray to check
 * @return                  An integer representing the number of points
 */
int spKDArrayGetSize(SPKDArray spkdArr);

/**
 * Destroys the KDArray and frees all memory attached to it
 * @method spKDArrayDestroy
 * @param  spkdArr          The KDArray to Destroy
 */
void spKDArrayDestroy(SPKDArray spkdArr);

/**
 * Fully print a KDArray to a file
 * @method spKDArrayFullPrintToFile
 * @param  spkdArr                  The KDArray to print
 * @param  filepath                 The print file path
 */
void spKDArrayFullPrintToFile(SPKDArray spkdArr, const char* filepath);

#endif // SPKDRAAY_H_
