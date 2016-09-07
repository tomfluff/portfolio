
/*
 *  SPImageExtraction
 *  Author: Yotam
 */

#ifndef SPIMAGEEXTRACTION_H_
#define SPIMAGEEXTRACTION_H_

#include "SPConfig.h"
#include "SPPoint.h"

typedef enum sp_extraction_msg {
    SP_EXTRACTION_INVALID_ARGUMENT,
    SP_EXTRACTION_ALLOC_FAIL,
    SP_EXTRACTION_FILE_ERROR,
    SP_EXTRACTION_SUCCESS
} SP_EXTRACTION_MSG;

/**
 * Saves a list of SPPoints to a file for later usage. If any errors occured, prints to the log file.
 * (Assumes a logger element has been created prior to usage)
 *
 * @method spExtractFromImage
 * @param  feats              The SPPoints list
 * @param  numOfFeatures      The number of SPPoints in the list
 * @param  index              The index to attach to the points
 * @param  filename           The file to save to
 * @return                    An SP_EXTRACTION_MSG enum with the process message
 *                            SP_EXTRACTION_INVALID_ARGUMENT - One argument is invalid
 *                            SP_EXTRACTION_ALLOC_FAIL - An allocation error occured
 *                            SP_EXTRACTION_FILE_ERROR - Error opening/creating file
 *                            SP_EXTRACTION_SUCCESS - Operation successful
 */
SP_EXTRACTION_MSG spExtractFromImage(SPPoint *feats, int numOfFeatures, int index, const char *filename);

/**
 * Retries an SPPoints array from a file. Places the correct SP_EXTRACTION_MSG in the given message pointer.
 *
 * @method spExtractFromFiles
 * @param  featsPath          The file path containing the points
 * @param  numOfFeatures      A pointer to an integer to be updated later with the number of points extracted from the file
 * @param  msg                The SP_EXTRACTION_MSG message pointer to update
 * @return                    An array of SPPoints
 */
SPPoint* spExtractFromFiles(const char* featsPath, int* numOfFeatures, SP_EXTRACTION_MSG* msg);

#endif // SPIMAGEEXTRACTION_H_
