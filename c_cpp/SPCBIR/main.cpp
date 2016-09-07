/*
 *  Main file
 *  Author: Yotam
 */

#include <cstdlib>
#include <cassert>
#include <cstring>
#include <cstdio>
#include <cstdbool>
#include "SPImageProc.h"

extern "C" {
#include "SPConfig.h"
#include "SPImageExtraction.h"
#include "SPPoint.h"
#include "SPLogger.h"
#include "SPKDTree.h"
#include "SPKDArray.h"
#include "SPBPQueue.h"
#include "main_aux.c"
}

using namespace sp;

int main(int argc, char const *argv[]) {
    const char* filename, *queryMsg = "Please enter an image path:\n";
    int i, j, numOfFeats, featsCount = 0, numOfImages, queryCount = 0, spKNN, spSimIm;
    char* imagePath, *featsPath, *loggerPath, *queryPath = NULL, *logTestMsg;
    ImageProc* proc = NULL;
    SPConfig conf;
    SPPoint* featsT, *feats;
    SPKDArray kdarr;
    SPKDTreeNode tree;
    SPBPQueue bpq;
    int** imgFeatCount_p, *imgFeatCount_d;
    bool extraction_mode, minimalGui, sanity = true;
    SP_SPLIT_METHOD split_method;
    SP_LOGGER_LEVEL loggerLevel;
    SP_EXTRACTION_MSG ext_msg;
    SP_CONFIG_MSG conf_msg;;
    SP_LOGGER_MSG log_msg;

    // get filename from argv[2] (or use default)
    filename = spcbirGetConfigFilename(argc, argv);
    if (!filename) {
        exit(0);
    }
    // open the configuration file
    conf = spcbirOpenConfigFile(filename);
    if (!conf) {
        exit(0);
    }

    // get attributes from config file for program use
    conf_msg = spcbirGetValuesFromConfig(conf, &numOfImages, &numOfFeats, &spKNN, &spSimIm, &extraction_mode, &split_method, &loggerPath, &loggerLevel, &minimalGui);
    if (conf_msg != SP_CONFIG_SUCCESS) {
        printf("An error occured while getting data from %s\n", filename);
        spConfigDestroy(conf);
        exit(0);
    }
    log_msg = spLoggerCreate(loggerPath, loggerLevel);
    if (log_msg != SP_LOGGER_SUCCESS) {
        switch (log_msg) {
        case SP_LOGGER_DEFINED:
            printf("The logger already exists\n");
            break;
        case SP_LOGGER_OUT_OF_MEMORY:
            printf("Logger memory allocation failure\n");
            break;
        case SP_LOGGER_CANNOT_OPEN_FILE:
            printf("Error occured while trying to open/create %s\n", loggerPath);
            break;
        default:
            printf("An unknown logger error occured\n");
        }
        spConfigDestroy(conf);
        exit(0);
    }
    // Logger created path can be freed
    free(loggerPath);

    // create a big enough SPPoint array for all features from all images.
    // the spNumOfFeatures attricute from the config file states the MAX num of features per image,
    // but since opencv sometimes extracts (spNumOfFeatures+1) features, it's been addad.
    feats = (SPPoint*)calloc(numOfImages*(numOfFeats + 1), sizeof(SPPoint));
    queryPath = (char*)calloc(CONFIG_LINE_MAX_SIZE+1, sizeof(char));
    imgFeatCount_p = (int**)calloc(numOfImages, sizeof(int*));
    imgFeatCount_d = (int*)calloc(2*numOfImages, sizeof(int));
    imagePath = (char*)malloc(1+CONFIG_LINE_MAX_SIZE); // create a big enough buffer
    featsPath = (char*)malloc(1+CONFIG_LINE_MAX_SIZE); // create a big enough buffer
    logTestMsg = (char*)malloc(1+CONFIG_LINE_MAX_SIZE); // create a big enough buffer
    proc = new ImageProc(conf); // creat ImageProc ovject
    if (logTestMsg == NULL || featsPath == NULL || imagePath == NULL || proc == NULL || imgFeatCount_d == NULL || imgFeatCount_p == NULL || feats == NULL || queryPath == NULL) {
        spLoggerPrintError("Error allocating enough memory", __FILE__, __func__, __LINE__);
        freeSPPointsArray(feats, numOfFeats);
        simpleFree(queryPath, imgFeatCount_d, imgFeatCount_p, imagePath, featsPath, logTestMsg);
        spConfigDestroy(conf);
        spLoggerDestroy();
        delete proc;
        exit(0);
    }
    spLoggerPrintInfo("All allocations were successful, configuration file recieved and logger created");
    // printf("0. General memory allocation, and preparation successful\n");
    // fix pointers for results array
    for (i=0; i<numOfImages; i++) {
        imgFeatCount_p[i] = imgFeatCount_d + 2*i;
    }

    if (extraction_mode) {
        // Extract mode
        sprintf(logTestMsg, "Working in 'Extraction Mode' - %d images to process", numOfImages);
        spLoggerPrintInfo(logTestMsg);
        // go over all pictures and store data in file and in the feats array
        for (i = 0; i < numOfImages; i++) {
            if (spConfigGetFeaturesFilePath(featsPath, conf, i) != SP_CONFIG_SUCCESS || spConfigGetImagePath(imagePath, conf, i) != SP_CONFIG_SUCCESS) {
                spLoggerPrintError("Error creating 'Image File Path' and/or 'Image Features File path'", __FILE__, __func__, __LINE__);
                freeSPPointsArray(feats, numOfFeats);
                simpleFree(queryPath, imgFeatCount_d, imgFeatCount_p, imagePath, featsPath, logTestMsg);
                spConfigDestroy(conf);
                spLoggerDestroy();
                delete proc;
                exit(0);
            }
            featsT = proc->getImageFeatures(imagePath, i, &numOfFeats); // get features for image
            ext_msg = spExtractFromImage(featsT, numOfFeats, i, featsPath); // save features to file
            if (!featsT || ext_msg != SP_EXTRACTION_SUCCESS) {
                if (!featsT) sprintf(logTestMsg, "Error extracting features from %s", imagePath);
                else sprintf(logTestMsg, "Error sacing features to file %s", featsPath);
                spLoggerPrintError(logTestMsg, __FILE__, __func__, __LINE__);
                freeSPPointsArray(feats, numOfFeats);
                simpleFree(queryPath, imgFeatCount_d, imgFeatCount_p, imagePath, featsPath, logTestMsg);
                spConfigDestroy(conf);
                spLoggerDestroy();
                delete proc;
                exit(0);
            }
            // move features to generall array and destroy the temp array
            for (j=0; j < numOfFeats; j++) {
                feats[featsCount] = featsT[j];
                featsCount++;
            }
            free(featsT);
        }
    } else {
        // Non-Extract mode
        for (i = 0; i < numOfImages; i++) {
            conf_msg = spConfigGetFeaturesFilePath(featsPath, conf, i);
            featsT = spExtractFromFiles(featsPath, &numOfFeats, &ext_msg);
            if (conf_msg != SP_CONFIG_SUCCESS || ext_msg != SP_EXTRACTION_SUCCESS) {
                if (conf_msg != SP_CONFIG_SUCCESS) sprintf(logTestMsg, "Error recieving features file path for image #%d", i);
                else sprintf(logTestMsg, "Error extracting features from %s", featsPath);
                spLoggerPrintError(logTestMsg, __FILE__, __func__, __LINE__);
                freeSPPointsArray(feats, featsCount);
                simpleFree(queryPath, imgFeatCount_d, imgFeatCount_p, imagePath, featsPath, logTestMsg);
                spConfigDestroy(conf);
                spLoggerDestroy();
                delete proc;
                exit(0);
            }
            for (j=0; j<numOfFeats; j++) {
                feats[featsCount] = featsT[j];
                featsCount++;
            }
            free(featsT);
        }
    }
    sprintf(logTestMsg, "%d images processed successfuly - a total of %d features", numOfImages, featsCount);
    spLoggerPrintInfo(logTestMsg);
    simpleFree(featsPath, NULL, NULL, NULL, NULL, NULL);
    kdarr = Init(feats, featsCount); // no need to to destroy KDArray (handles in tree creation)
    if (!kdarr) {
        spLoggerPrintError("Error occured while creating the KDArray", __FILE__, "Init", __LINE__);
        simpleFree(imagePath, queryPath, imgFeatCount_d, imgFeatCount_p, logTestMsg, NULL);
        freeSPPointsArray(feats, featsCount);
        spConfigDestroy(conf);
        spLoggerDestroy();
        delete proc;
        exit(0);
    }
    freeSPPointsArray(feats, featsCount); // free SPPoints array (un-needed after KDArray creation)
    tree = spKDTreeCreate(kdarr, split_method);
    if (!tree) {
        spLoggerPrintError("Error occured while creating the KDTree", __FILE__, "spKDTreeCreate", __LINE__);
        simpleFree(imagePath, queryPath, imgFeatCount_d, imgFeatCount_p, logTestMsg, NULL);
        spKDArrayDestroy(kdarr);
        spConfigDestroy(conf);
        spLoggerDestroy();
        delete proc;
        exit(0);
    }
    // main query loop
    do {
        // sanity explained:
        // ---------------------
        // It's a mechanism for making sure nothing went wrong without a large amout of exit().
        // sanity = true -> everything is ok
        // sanity = false -> something went wrong
        // Therefor every stage in the program checks is sanity == true.
        if (getLine(queryMsg, queryPath, CONFIG_LINE_MAX_SIZE) != OK) {
            spLoggerPrintWarning("Invalid query input entered (empty)", __FILE__, __func__, __LINE__);
            sanity = false;
        } else if (strcmp(queryPath,"<>") != 0) {
            queryCount++;
            sprintf(logTestMsg, "Recieved query %s (#%d)", queryPath, queryCount);
            spLoggerPrintInfo(logTestMsg);
            featsT = proc->getImageFeatures(queryPath, numOfImages+1, &numOfFeats);
            if (!featsT) {
                sprintf(logTestMsg, "Error while extracting features for the query %s", queryPath);
                spLoggerPrintError(logTestMsg, __FILE__, __func__, __LINE__);
                sanity = false;
            }
            // reset image result counter array
            for (i=0; i<numOfImages; i++) {
                imgFeatCount_p[i][0] = i;
                imgFeatCount_p[i][1] = 0;
            }
            // count image features per feature
            for (i=0; i<numOfFeats && sanity; i++) {
                bpq = spKDTreeFindKNearestNeighbors(tree, spKNN, featsT[i]);
                if (!bpq) {
                    sprintf(logTestMsg, "Error occured while trying to find nearest neighbors for feature #%d", i);
                    spLoggerPrintError(logTestMsg, __FILE__, __func__, __LINE__);
                    sanity = false;
                } else {
                    if (spcbirCountImageProximity(bpq, imgFeatCount_p) < 0) {
                        spLoggerPrintError("Error occured while calculating proximity for images", __FILE__, __func__, __LINE__);
                        sanity = false;
                    }
                    spBPQueueDestroy(bpq);
                }
            }
            if (featsT) freeSPPointsArray(featsT, numOfFeats);     // free the query image features array when needed
            if (sanity) {
                // sort results to get best images
                qsort(imgFeatCount_p, numOfImages, sizeof(int*), compare2DInt);
                if (!minimalGui) printf("Best candidates for - %s - are:\n", queryPath);
                // loop over the needed results and display them
                for (i=0; sanity && i < spSimIm; i++) {
                    if (spConfigGetImagePath(imagePath, conf, imgFeatCount_p[i][0]) != SP_CONFIG_SUCCESS) {
                        sprintf(logTestMsg, "Error creating file name for image #%d", imgFeatCount_p[i][0]);
                        spLoggerPrintError(logTestMsg, __FILE__, __func__, __LINE__);
                        sanity = false;
                    } else {
                        if (minimalGui) {
                            proc->showImage(imagePath);
                        } else {
                            printf("%s\n", imagePath);
                        }
                    }
                }
            }
        } else {
            // go to exit phase
        }
    } while (sanity && strcmp(queryPath,"<>") != 0);

    // exit phase
    spLoggerPrintInfo("Exiting");
    printf("Exiting...\n");
    simpleFree(imagePath, queryPath, imgFeatCount_d, imgFeatCount_p, logTestMsg, NULL);
    spKDTreeDestroy(tree);
    spConfigDestroy(conf);
    spLoggerDestroy();
    delete proc;
    return 0;
}
