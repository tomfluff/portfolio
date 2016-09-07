
/*
 *  SPConfig
 *  Author: Yotam
 */

#ifndef SPCONFIG_H_
#define SPCONFIG_H_

#include <stdbool.h>
#include "SPLogger.h"

#define CONFIG_LINE_MAX_SIZE 1024

/**
 * A data-structure which is used for configuring the system.
 */

typedef enum sp_config_msg_t {
    SP_CONFIG_ALLOC_FAIL,
    SP_CONFIG_CANNOT_OPEN_FILE,
    SP_CONFIG_INVALID_LINE,
    SP_CONFIG_INVALID_ARGUMENT,
    SP_CONFIG_INVALID_INTEGER,
    SP_CONFIG_INVALID_STRING,
    SP_CONFIG_INDEX_OUT_OF_RANGE,
    SP_CONFIG_MISSING_DIR,
    SP_CONFIG_MISSING_PREFIX,
    SP_CONFIG_MISSING_SUFFIX,
    SP_CONFIG_MISSING_NUM_IMAGES,
    SP_CONFIG_SUCCESS
} SP_CONFIG_MSG;

typedef enum sp_split_method_t {
    MAX_SPREAD, RANDOM, INCREMENTAL, INVALID
} SP_SPLIT_METHOD;

typedef struct sp_config_t* SPConfig;

/**
 * Creates a new system configuration struct. The configuration struct
 * is initialized based on the configuration file given by 'filename'.
 *
 * @param filename - the name of the configuration file
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return NULL in case an error occurs. Otherwise, a pointer to a struct which
 *      contains all system configuration.
 *
 * The resulting value stored in msg is as follow:
 * - SP_CONFIG_INVALID_ARGUMENT - if filename == NULL
 * - SP_CONFIG_CANNOT_OPEN_FILE - if the configuration file given by filename cannot be open
 * - SP_CONFIG_ALLOC_FAIL - if an allocation failure occurred
 * - SP_CONFIG_INVALID_INTEGER - if a line in the config file contains invalid integer
 * - SP_CONFIG_INVALID_STRING - if a line in the config file contains invalid string
 * - SP_CONFIG_MISSING_DIR - if spImagesDirectory is missing
 * - SP_CONFIG_MISSING_PREFIX - if spImagesPrefix is missing
 * - SP_CONFIG_MISSING_SUFFIX - if spImagesSuffix is missing
 * - SP_CONFIG_MISSING_NUM_IMAGES - if spNumOfImages is missing
 * - SP_CONFIG_SUCCESS - in case of success
 *
 *
 */
SPConfig spConfigCreate(const char* filename, SP_CONFIG_MSG* msg);

/*
 * Returns true if spExtractionMode = true, false otherwise.
 *
 * @param config - the configuration structure
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return true if spExtractionMode = true, false otherwise.
 *
 * - SP_CONFIG_INVALID_ARGUMENT - if config == NULL
 * - SP_CONFIG_SUCCESS - in case of success
 */
bool spConfigIsExtractionMode(const SPConfig config, SP_CONFIG_MSG* msg);

/*
 * Returns true if spMinimalGUI = true, false otherwise.
 *
 * @param config - the configuration structure
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return true if spExtractionMode = true, false otherwise.
 *
 * - SP_CONFIG_INVALID_ARGUMENT - if config == NULL
 * - SP_CONFIG_SUCCESS - in case of success
 */
bool spConfigMinimalGui(const SPConfig config, SP_CONFIG_MSG* msg);

/*
 * Returns the number of images set in the configuration file, i.e the value
 * of spNumOfImages.
 *
 * @param config - the configuration structure
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return positive integer in success, negative integer otherwise.
 *
 * - SP_CONFIG_INVALID_ARGUMENT - if config == NULL
 * - SP_CONFIG_SUCCESS - in case of success
 */
int spConfigGetNumOfImages(const SPConfig config, SP_CONFIG_MSG* msg);

/*
 * Returns the number of features to be extracted. i.e the value
 * of spNumOfFeatures.
 *
 * @param config - the configuration structure
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return positive integer in success, negative integer otherwise.
 *
 * - SP_CONFIG_INVALID_ARGUMENT - if config == NULL
 * - SP_CONFIG_SUCCESS - in case of success
 */
int spConfigGetNumOfFeatures(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the dimension of the PCA. i.e the value of spPCADimension.
 *
 * @param config - the configuration structure
 * @assert msg != NULL
 * @param msg - pointer in which the msg returned by the function is stored
 * @return positive integer in success, negative integer otherwise.
 *
 * - SP_CONFIG_INVALID_ARGUMENT - if config == NULL
 * - SP_CONFIG_SUCCESS - in case of success
 */
int spConfigGetPCADim(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the split method from the configuration element. Updates the SP_CONFIG_MSG element given to update if any errors occured
 * @method spConfigGetSplitMethod
 * @param  config                 The configuration element
 * @param  msg                    An SP_CONFIG_MSG message pointer to be updated
 * @return                        The SP_SPLIT_METHOD enum element
 */
SP_SPLIT_METHOD spConfigGetSplitMethod(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the KNN element from the config file. Updates the SP_CONFIG_MSG element given to update if any errors occured
 * @method spConfigGetKNN
 * @param  config         The configuration element
 * @param  msg            An SP_CONFIG_MSG message pointer to be updated
 * @return                An integer represanting the KNN value
 */
int spConfigGetKNN(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the spNumberOfSimilarImages from the config param. Updates the SP_CONFIG_MSG element given to update if any errors occured
 * @method spConfigGetNumOfSimilarImages
 * @param  config                        The Config param
 * @param  msg                           An SP_CONFIG_MSG message pointer to be updated
 * @return                               The spNumberOfSimilarImages value
 */
int spConfigGetNumOfSimilarImages(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the spLoggerLevel param from the config file. Updates the SP_CONFIG_MSG element given to update if any errors occured
 * @method spConfigGetLoggerLevel
 * @param  config                 The Config param
 * @param  msg                    An SP_CONFIG_MSG message pointer to be updated
 * @return                        The spLoggerLevel param
 */
SP_LOGGER_LEVEL spConfigGetLoggerLevel(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Returns the spLoggerFilename param from the config file. Updates the SP_CONFIG_MSG element given to update if any errors occured
 * @method SPConfigGetLoggerFilename
 * @param  config                    The config file
 * @param  msg                       An SP_CONFIG_MSG message pointer to be updated
 * @return                           The spLoggerFilename param
 */
char* spConfigGetLoggerFilename(const SPConfig config, SP_CONFIG_MSG* msg);

/**
 * Given an index 'index' the function stores in imagePath the full path of the
 * ith image.
 *
 * For example:
 * Given that the value of:
 *  spImagesDirectory = "./images/"
 *  spImagesPrefix = "img"
 *  spImagesSuffix = ".png"
 *  spNumOfImages = 17
 *  index = 10
 *
 * The functions stores "./images/img10.png" to the address given by imagePath.
 * Thus the address given by imagePath must contain enough space to
 * store the resulting string.
 *
 * @param imagePath - an address to store the result in, it must contain enough
 * space.
 * @param config - the configuration structure
 * @param index - the index of the image.
 *
 * @return
 * - SP_CONFIG_INVALID_ARGUMENT - if imagePath == NULL or config == NULL
 * - SP_CONFIG_INDEX_OUT_OF_RANGE - if index >= spNumOfImages
 * - SP_CONFIG_SUCCESS - in case of success
 */
SP_CONFIG_MSG spConfigGetImagePath(char* imagePath, const SPConfig config, int index);

/**
 * The function stores in pcaPath the full path of the pca file.
 * For example given the values of:
 *  spImagesDirectory = "./images/"
 *  spPcaFilename = "pca.yml"
 *
 * The functions stores "./images/pca.yml" to the address given by pcaPath.
 * Thus the address given by pcaPath must contain enough space to
 * store the resulting string.
 *
 * @param imagePath - an address to store the result in, it must contain enough space.
 * @param config - the configuration structure
 * @return
 *  - SP_CONFIG_INVALID_ARGUMENT - if imagePath == NULL or config == NULL
 *  - SP_CONFIG_SUCCESS - in case of success
 */
SP_CONFIG_MSG spConfigGetPCAPath(char* pcaPath, const SPConfig config);

/**
 * Places the features file path into the path pointer. Returns the SP_CONFIG_MSG element.
 * @method spConfigGetFeaturesFilePath
 * @param  featsPath                   The pre-allocated features file path string
 * @param  config                      The config file
 * @param  index                       The image index to get the path for
 * @return                             An SP_CONFIG_MSG enum representing the result of the function
 */
SP_CONFIG_MSG spConfigGetFeaturesFilePath(char* featsPath, const SPConfig config, int index);

/**
 * Destroys the configuration file element and frees all allocated memory
 * @method spConfigDestroy
 * @param  config          The config file to destroy
 */
void spConfigDestroy(SPConfig config);

#endif /* SPCONFIG_H_ */
