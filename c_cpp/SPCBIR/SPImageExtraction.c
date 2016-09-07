
/*
 *  SPImageExtraction
 *  Author: Yotam
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "SPConfig.h"
#include "SPPoint.h"
#include "SPLogger.h"
#include "SPImageExtraction.h"

SP_EXTRACTION_MSG spExtractFromImage(SPPoint *feats, int numOfFeatures, int index, const char *filename) {
    // input check
    if (!feats || !filename) {
        spLoggerPrintWarning("Invalid Arguments", __FILE__, __func__, __LINE__);
        return SP_EXTRACTION_INVALID_ARGUMENT;
    }

    int i,j,fDim;

    fDim = spPointGetDimension(*feats);
    FILE *fp = fopen(filename, "w");

    if (fp == NULL) {
        spLoggerPrintWarning("Error creating feattures file", __FILE__, __func__, __LINE__);
        return SP_EXTRACTION_FILE_ERROR;
    }
    // Write image index and features number
    if (fprintf(fp, "%d %d %d\n",index, numOfFeatures, fDim) < 0) {
        spLoggerPrintWarning("Error writing feattures to file", __FILE__, __func__, __LINE__);
        fclose(fp);
        return SP_EXTRACTION_FILE_ERROR;
    }
    for (i=0; i<numOfFeatures; i++) {
        for (j=0; j<fDim; j++) {
            if (fprintf(fp, "%lf ", spPointGetAxisCoor(feats[i], j)) < 0) {
                spLoggerPrintWarning("Error writing feattures to file", __FILE__, __func__, __LINE__);
                fclose(fp);
                return SP_EXTRACTION_FILE_ERROR;
            }
            // File write was successful
        }
        fprintf(fp, "\n");
    }
    // Finished writing all features
    fclose(fp);
    return SP_EXTRACTION_SUCCESS;

}

SPPoint* spExtractFromFiles(const char* featsPath, int* numOfFeatures, SP_EXTRACTION_MSG* msg) {
    if (!featsPath) {
        spLoggerPrintWarning("Invalid Arguments", __FILE__, __func__, __LINE__);
        *msg = SP_EXTRACTION_INVALID_ARGUMENT;
        return NULL;
    }
    int i,j,k,index,fDim;
    double* pData;
    SPPoint* arr;

    FILE *fp = fopen(featsPath, "r");
    if (fp == NULL) {
        spLoggerPrintWarning("Error opening feattures file", __FILE__, __func__, __LINE__);
        *msg = SP_EXTRACTION_FILE_ERROR;
        return NULL;
    }
    // Read image index and features number
    if (fscanf(fp, "%d %d %d\n",&index, numOfFeatures, &fDim) == 0) {
        spLoggerPrintWarning("Error writing feattures to file", __FILE__, __func__, __LINE__);
        fclose(fp);
        *msg = SP_EXTRACTION_FILE_ERROR;
        return NULL;
    }
    arr = (SPPoint*)calloc(*numOfFeatures,sizeof(SPPoint));
    pData = (double*)calloc(fDim,sizeof(double));
    if (!arr || !pData) {
        spLoggerPrintWarning("Error allocating memory for features", __FILE__, __func__, __LINE__);
        free(arr);
        free(pData);
        fclose(fp);
        *msg = SP_EXTRACTION_ALLOC_FAIL;
        return NULL;
    }
    // Allocation successful
    for (i=0; i<*numOfFeatures; i++) {
        for (j=0; j<fDim; j++) {
            if (fscanf(fp,"%lf ", &(pData[j])) == 0) {
                spLoggerPrintWarning("Error extracting features from file", __FILE__, __func__, __LINE__);
                free(arr);
                free(pData);
                fclose(fp);
                *msg = SP_EXTRACTION_FILE_ERROR;
                return NULL;
            }
            // Read successful
        }
        arr[i] = spPointCreate(pData, fDim, index);
        if (!arr[i]) {
            spLoggerPrintWarning("Error creating SPPoint for extracted feature", __FILE__, __func__, __LINE__);
            for (k=0; k<i; k++) {
                spPointDestroy(arr[k]);
            }
            free(arr);
            free(pData);
            fclose(fp);
            *msg = SP_EXTRACTION_ALLOC_FAIL;
            return NULL;
        }
        // Point created successfuly
    }
    // Finished reading
    free(pData);
    fclose(fp);
    *msg = SP_EXTRACTION_SUCCESS;
    return arr;
}
