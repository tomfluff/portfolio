/*
 *  SPKDArray
 *  Author: Yotam
 */

#include "SPKDArray.h"
#include "SPPoint.h"
#include "SPLogger.h"
#include <assert.h>
#include <stdio.h>
#include <stdlib.h>

struct sp_kd_array {
    SPPoint *points;     // the copied points array
    int **kdArray;     // the 2-dimensional matrix of indexes
    int dim;       // the dimension of all points
    int size;      // the number for points int array
};

void generalFree(void* p1, void* p2, void* p3, void* p4, void* p5, void* p6, void* p7) {
    free(p1);
    free(p2);
    free(p3);
    free(p4);
    free(p5);
    free(p6);
    free(p7);
}

int compareCoorValues(const void *a, const void *b) {
    const double *pa = *(const double **)a;
    const double *pb = *(const double **)b;
    return (int)(pa[1] - pb[1]);
}

int spKDArrayGetIndexInArray(SPKDArray kdArr, int i, int j) {
    assert(kdArr != NULL && i >= 0 && j >= 0);
    return kdArr->kdArray[i][j];
}

int spKDArrayGetDimension(SPKDArray kdArr) {
    assert(kdArr != NULL);
    return kdArr->dim;
}

int spKDArrayGetSize(SPKDArray kdArr) {
    assert(kdArr != NULL);
    return kdArr->size;
}

SPPoint spKDArrayGetPoint(SPKDArray arr, int index) {
    assert(arr != NULL && index < arr->size);
    SPPoint p = spPointCopy(arr->points[index]);
    return p;
}

double spKDArrayGetHighestPointValueInDim(SPKDArray arr, int dim) {
    assert(arr != NULL && dim < arr->dim);
    int index = arr->kdArray[dim][arr->size-1];
    return spPointGetAxisCoor(arr->points[index], dim);
}

int spKDArrayFindMaxSpreadDim(SPKDArray arr) {
    assert(arr!=NULL);
    int coorT = 0, i;
    double spreadT = 0.0;
    for (i=0; i<arr->dim; i++) {
        if ((spPointGetAxisCoor(arr->points[arr->kdArray[i][arr->size-1]],i) - spPointGetAxisCoor(arr->points[arr->kdArray[i][0]],i)) > spreadT) {
            spreadT = spPointGetAxisCoor(arr->points[arr->kdArray[i][arr->size-1]],i) - spPointGetAxisCoor(arr->points[arr->kdArray[i][0]],i);
            coorT = i;
        }
    }
    return coorT;
}

SPPoint* spKDArrayGetPointsArray(SPKDArray spkdArr) {
    if (!spkdArr)
        return NULL;
    SPPoint* cpy = (SPPoint*)calloc(spkdArr->size, sizeof(SPPoint));
    if (!cpy) {
        spLoggerPrintWarning("Memory allocation failed", __FILE__, __func__, __LINE__);
        return NULL;
    }
    int i, j;
    for (i = 0; i < spkdArr->size; i++) {
        cpy[i] = spPointCopy((spkdArr->points)[i]);
        // check for failure
        if (!cpy[i]) {
            spLoggerPrintWarning("Point creation failed", __FILE__, __func__, __LINE__);
            for (j = 0; j < i; j++) {
                spPointDestroy(cpy[j]);
            }
            free(cpy);
            return NULL;
        }
    }
    return cpy;
}

// helper for Init
void fixTempRowPointerPlacement(double **rowTmp, double *rowTmpV, int size) {
    int j;
    // fix temp row pointer placement
    for (j = 0; j < size; j++) {
        rowTmp[j] = rowTmpV + 2 * j;
    }
}

// helper for Init
void initializeKDArrayMatrixSorted(SPKDArray spkdArr, double **rowTmp, SPPoint *arr, int *kdArrayV, int size) {
    int i, j;
    // initialize the kdArray matrix
    for (i = 0; i < spkdArr->dim; i++) {
        // calc sorted indexes by coor
        for (j = 0; j < size; j++) {
            rowTmp[j][0] = (double)j;
            rowTmp[j][1] = spPointGetAxisCoor(arr[j], i);
        }
        qsort(rowTmp, size, sizeof(double *), compareCoorValues);
        // compile row
        spkdArr->kdArray[i] = kdArrayV + i * size;
        for (j = 0; j < size; j++) {
            spkdArr->kdArray[i][j] = (int)rowTmp[j][0];
        }
    }
}

SPKDArray Init(SPPoint *arr, int size) {
    if (!arr || size <= 0)
        return NULL;         // invalid input

    int i, j;
    // memory allocation
    SPKDArray spkdArr = (SPKDArray)malloc(sizeof(struct sp_kd_array));
    if (!spkdArr)
        return NULL;         // exit on SPKDArray allocation error
    // dim and size
    spkdArr->dim = spPointGetDimension(arr[0]);
    spkdArr->size = size;
    //
    spkdArr->points = (SPPoint *)calloc(spkdArr->size, sizeof(SPPoint));
    spkdArr->kdArray = (int **)calloc(spkdArr->dim, sizeof(int *));
    int *kdArrayV = (int *)calloc((spkdArr->dim) * spkdArr->size, sizeof(int));
    double **rowTmp = (double **)calloc(spkdArr->size, sizeof(double *));     // freed
    double *rowTmpV = (double *)calloc(spkdArr->size * 2, sizeof(double));     // freed

    // if any allocation failed free all and return NULL
    if (!spkdArr->points || !spkdArr->kdArray || !kdArrayV || !rowTmp ||
        !rowTmpV) {
        spLoggerPrintWarning("Memory allocation failed", __FILE__, __func__, __LINE__);
        free(spkdArr->points);
        free(kdArrayV);
        free(spkdArr->kdArray);
        free(spkdArr);
        free(rowTmp);
        free(rowTmpV);
        return NULL;
    }

    // copy the points
    for (i = 0; i < size; i++) {
        spkdArr->points[i] = spPointCopy(arr[i]);
        // check for failure
        if (!spkdArr->points[i]) {
            spLoggerPrintWarning("Point creation failed", __FILE__, __func__, __LINE__);
            for (j = 0; j < i; j++) {
                spPointDestroy(spkdArr->points[j]);
            }
            free(spkdArr->points);
            free(kdArrayV);
            free(spkdArr->kdArray);
            free(spkdArr);
            free(rowTmp);
            free(rowTmpV);
            return NULL;
        }
    }
    // end memory allocation
    fixTempRowPointerPlacement(rowTmp, rowTmpV, size);
    initializeKDArrayMatrixSorted(spkdArr, rowTmp, arr, kdArrayV, size);

    // free the temporary row
    free(rowTmpV);
    free(rowTmp);
    return spkdArr;
}

// helper for Split
void updateXArrayforSPKDArray(SPKDArray kdArr, int *xArr, int midP, int coor) {
    // update X-Array
    int i;
    for (i = 0; i < kdArr->size; i++) {
        if (i <= midP) {
            xArr[(kdArr->kdArray)[coor][i]] = 0;
        } else {
            xArr[(kdArr->kdArray)[coor][i]] = 1;
        }
    }
}

// helper for Split
void updateMapsAndAddPointsToLists(SPKDArray kdArr, SPKDArray leftArr, SPKDArray rightArr, SPPoint *pointTmp, int *xArr, int *mapL, int *mapR) {
    // update the maps and add point to correct list
    int i, j, k;
    j = 0;
    k = 0;
    for (i = 0; i < kdArr->size; i++) {
        if (xArr[i] == 0) {
            // left point
            leftArr->points[j] = pointTmp[i];
            mapL[i] = j;
            mapR[i] = -1;
            j++;
        } else {
            // right point
            rightArr->points[k] = pointTmp[i];
            mapL[i] = -1;
            mapR[i] = k;
            k++;
        }
    }
}

// helper for Split
void fixPointerPlacementForKDArrayMatrix(SPKDArray leftArr, SPKDArray rightArr, int *kdArrayVL, int *kdArrayVR) {
    int i;
    // fix placement for kdArrays
    for (i = 0; i < leftArr->dim; i++) {
        leftArr->kdArray[i] = kdArrayVL + i * leftArr->size;
    }
    for (i = 0; i < rightArr->dim; i++) {
        rightArr->kdArray[i] = kdArrayVR + i * rightArr->size;
    }
}

// helper for Split
void recreateSortedKDArrayMatrixes(SPKDArray kdArr, SPKDArray leftArr, SPKDArray rightArr, int *xArr, int *mapL, int *mapR) {
    int i, j, k, l;
    // recreate sorted matrix
    for (i = 0; i < kdArr->dim; i++) {
        k = 0;
        l = 0;
        for (j = 0; j < kdArr->size; j++) {
            if (xArr[(kdArr->kdArray)[i][j]] == 0) {
                // in leftArr
                leftArr->kdArray[i][k] = mapL[(kdArr->kdArray)[i][j]];
                k++;
            } else {
                // in rightArr
                (rightArr->kdArray)[i][l] = mapR[(kdArr->kdArray)[i][j]];
                l++;
            }
        }
    }
}

SPKDArray* Split(SPKDArray kdArr, int coor) {
    // check correct input
    if (!kdArr || coor < 0) {
        spLoggerPrintWarning("Invalid arguments", __FILE__, __func__, __LINE__);
        return NULL;
    }

    int midP = 0;
    if (kdArr->size > 2)
        midP = (int)((kdArr->size) / 2.0);

    // memory allocation
    SPKDArray *resArray = (SPKDArray *)calloc(2, sizeof(SPKDArray));
    if (!resArray) return NULL;
    SPKDArray leftArr = (SPKDArray)malloc(sizeof(struct sp_kd_array));
    SPKDArray rightArr = (SPKDArray)malloc(sizeof(struct sp_kd_array));
    if (!leftArr || !rightArr) {
        spLoggerPrintWarning("Memory allocation failure", __FILE__, __func__, __LINE__);
        generalFree(leftArr, rightArr, resArray, NULL, NULL, NULL, NULL);
        return NULL;
    }
    leftArr->size = midP + 1; // left
    leftArr->dim = kdArr->dim; // left
    rightArr->size = (kdArr->size) - (midP + 1); // right
    rightArr->dim = kdArr->dim; // right
    leftArr->points = (SPPoint*)calloc(leftArr->size, sizeof(SPPoint)); // left
    leftArr->kdArray = (int**)calloc(leftArr->dim, sizeof(int *)); // left
    int *kdArrayVL = (int*)calloc((leftArr->size) * (leftArr->dim), sizeof(int)); // left
    rightArr->points = (SPPoint*)calloc(rightArr->size, sizeof(SPPoint)); // right
    rightArr->kdArray = (int**)calloc(rightArr->dim, sizeof(int *)); // right
    int *kdArrayVR = (int*)calloc((rightArr->size) * (rightArr->dim), sizeof(int)); // right
    // general allocation
    int *xArr = (int *)calloc(kdArr->size, sizeof(int));     // freed
    int *mapL = (int *)calloc(kdArr->size, sizeof(int));     // freed
    int *mapR = (int *)calloc(kdArr->size, sizeof(int));     // freed
    // copy the points from the kdArr
    SPPoint *pointTmp = spKDArrayGetPointsArray(kdArr);     // freed

    // check that no allocation errors occured
    if (!leftArr->points || !leftArr->kdArray || !kdArrayVL ||
        !rightArr->points || !rightArr->kdArray || !kdArrayVR || !xArr || !mapL ||
        !mapR || !pointTmp) {
        spLoggerPrintWarning("Memory allocation failure", __FILE__, __func__, __LINE__);
        generalFree(leftArr->points, leftArr->kdArray, kdArrayVL, rightArr->points, rightArr->kdArray, kdArrayVR, leftArr);
        generalFree(rightArr, resArray, xArr, mapL, mapR, pointTmp, NULL);
        return NULL;
    }
    // end memory allocation
    updateXArrayforSPKDArray(kdArr, xArr, midP, coor);
    updateMapsAndAddPointsToLists(kdArr, leftArr, rightArr, pointTmp, xArr, mapL, mapR);
    fixPointerPlacementForKDArrayMatrix(leftArr, rightArr, kdArrayVL, kdArrayVR);
    recreateSortedKDArrayMatrixes(kdArr, leftArr, rightArr, xArr, mapL, mapR);

    // place results
    resArray[0] = leftArr;
    resArray[1] = rightArr;

    generalFree(xArr, mapL, mapR, pointTmp, NULL, NULL, NULL);
    return resArray;
}

void spKDArrayDestroy(SPKDArray spkdArr) {
    if (!spkdArr) return;
    int i;
    for (i = 0; i < spkdArr->size; i++)
        spPointDestroy(spkdArr->points[i]);
    free(spkdArr->points);
    free(*(spkdArr->kdArray));
    free(spkdArr->kdArray);
    free(spkdArr);
    return;
}

void spKDArrayFullPrintToFile(SPKDArray spkdArr, const char* filepath) {
    FILE* fp = fopen(filepath, "w");
    char *m1 = "--------------\n";
    char *m2 = "- Points:\n";
    char *m3 = "- Size:\t";
    char *m4 = "- Dim:\t";
    char *m5 = "- Matrix:";

    fprintf(fp, "%s", m1);
    fprintf(fp, "%s%d\n", m3, spkdArr->size);
    fprintf(fp, "%s%d\n", m4, spkdArr->dim);
    fprintf(fp, "%s", m2);
    int i, j;
    for (i = 0; i < spkdArr->size; i++) {
        fprintf(fp, "\t%d\t-\t(", i);
        for (j = 0; j < spkdArr->dim; j++) {
            fprintf(fp, "%.0f", spPointGetAxisCoor((spkdArr->points)[i], j));
            if (j < spkdArr->dim - 1)
                fprintf(fp, ", ");
        }
        fprintf(fp, ")\n");
    }
    fprintf(fp, "%s", m5);
    for (i = 0; i < spkdArr->dim; i++) {
        fprintf(fp, "\n\t");
        for (j = 0; j < spkdArr->size; j++) {
            fprintf(fp, "%d\t", (spkdArr->kdArray)[i][j]);
        }
    }
    fprintf(fp, "\n\n");
    fclose(fp);
    return;
}
