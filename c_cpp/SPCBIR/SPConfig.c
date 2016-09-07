
/*
 *  SPConfig
 *  Author: Yotam
 */

#include <stdbool.h>
#include <stdlib.h>
#include <stdio.h>
#include <ctype.h>
#include <assert.h>
#include <string.h>
#include "SPConfig.h"
#include "SPLogger.h"

struct sp_config_t {
    char* spImagesDirectory;
    char* spImagesPrefix;
    char* spImagesSuffix;
    char* spLoggerFilename;
    char* spPCAFilename;
    int spNumOfImages;
    int spPCADimension;
    int spNumOfFeatures;
    int spNumOfSimilarImages;
    int spKNN;
    bool spMinimalGUI;
    bool spExtractionMode;
    SP_LOGGER_LEVEL spLoggerLevel;
    SP_SPLIT_METHOD spKDTreeSplitMethod;
};

char* ignoreWhitespaces(char* source) {
    // Ignore whitespaces in the beginning
    while(isspace(*source)) {
        source++;
    }
    return source;
}

int extractParamNameFromString(char* source, char* dest) {
    while (!isspace(*source) && *source != '=') {
        *dest++ = *source++;
    }
    source = ignoreWhitespaces(source);
    if (*source != '=') {
        return -1;
    }
    if (*dest != '\0') {
        *dest = '\0';
    }
    return 1;
}

bool checkForWhitespaceInString(char* str) {
    char* strT = str;
    strT = ignoreWhitespaces(strT); // Just in case the beginning has whitespaces
    while (!isspace(*strT) && *strT != '\0') {
        strT++;
    }
    strT = ignoreWhitespaces(strT);
    if (*strT != '\0') {
        return true;
    }
    return false;
}

void removeTailingWhitespces(char* str) {
    while (!isspace(*str) && *str != '\0') {
        str++;
    }
    *str = '\0';
}

bool isAllDigits(char* str) {
    char* strT = str;
    while (*strT != '\0') {
        if (!isdigit(*strT)) return false;
        strT++;
    }
    return true;
}

void printConstraintsNotMet(const char* filename, int lineNum){
    printf("File: %s\nLine: %d\nMessage: Invalid value - constraint not met\n", filename, lineNum);
}

void printInvalidLine(const char* filename, int lineNum){
    printf("File: %s\nLine: %d\nMessage: Invalid configuration line\n", filename, lineNum);
}

void printParamNotSet(const char* filename, int lineNum, char* paramName){
    printf("File: %s\nLine: %d\nMessage: Parameter %s is not set\n", filename, lineNum, paramName);
}

int storeParamValueFromFile(const char* filename, SPConfig res, char* paramName, char* value, SP_CONFIG_MSG* msg, int lineNum) {
    // Handle invalid line
    if (checkForWhitespaceInString(value)) {
        return -2;
    } else {
        // Line is valid, removing spaces at end of value
        removeTailingWhitespces(value);
    }
    if (strcmp(paramName, "spImagesDirectory") == 0) {
        strcpy(res->spImagesDirectory,value);
        return 0;
    } else if (strcmp(paramName, "spImagesPrefix") == 0) {
        strcpy(res->spImagesPrefix, value);
        return 1;
    } else if (strcmp(paramName, "spImagesSuffix") == 0) {
        strcpy(res->spImagesSuffix, value);
        if (strcmp(res->spImagesSuffix, ".jpg") != 0 && strcmp(res->spImagesSuffix, ".png") != 0 && strcmp(res->spImagesSuffix, ".bmp") != 0 && strcmp(res->spImagesSuffix, ".gif") != 0) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_STRING;
            return -1;
        }
        return 2;
    } else if (strcmp(paramName, "spNumOfImages") == 0) {
        sscanf(value,"%d",&res->spNumOfImages);
        if (!isAllDigits(value)|| res->spNumOfImages <= 0) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
        return 3;
    } else if (strcmp(paramName, "spPCADimension") == 0) {
        sscanf(value,"%d",&(res->spPCADimension));
        if (!isAllDigits(value) || res->spPCADimension < 10 || res->spPCADimension > 28) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
    } else if (strcmp(paramName, "spPCAFilename") == 0) {
        strcpy(res->spPCAFilename, value);
    } else if (strcmp(paramName, "spNumOfFeatures") == 0) {
        sscanf(value,"%d",&(res->spNumOfFeatures));
        if (!isAllDigits(value) || res->spNumOfFeatures <= 0) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
    } else if (strcmp(paramName, "spExtractionMode") == 0) {
        if (strcmp(value,"true") == 0) {
            res->spExtractionMode = true;
        } else if (strcmp(value,"false") == 0) {
            res->spExtractionMode = false;
        } else {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_STRING;
            return -1;
        }
    } else if (strcmp(paramName, "spNumOfSimilarImages") == 0) {
        if (!isAllDigits(value) || sscanf(value,"%d",&(res->spNumOfSimilarImages)) == EOF || res->spNumOfSimilarImages <= 0) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
    } else if (strcmp(paramName, "spKDTreeSplitMethod") == 0) {
        if (strcmp(value, "RANDOM") == 0) {
            res->spKDTreeSplitMethod = RANDOM;
        } else if (strcmp(value, "MAX_SPREAD") == 0) {
            res->spKDTreeSplitMethod = MAX_SPREAD;
        } else if (strcmp(value, "INCREMENTAL") == 0) {
            res->spKDTreeSplitMethod = INCREMENTAL;
        } else {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_STRING;
            return -1;
        }
    } else if (strcmp(paramName, "spKNN") == 0) {
        sscanf(value,"%d",&(res->spKNN));
        if (!isAllDigits(value) || res->spNumOfFeatures <= 0) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
    } else if (strcmp(paramName, "spMinimalGUI") == 0) {
        if (strcmp(value,"true") == 0) {
            res->spMinimalGUI = true;
        } else if (strcmp(value,"false") == 0) {
            res->spMinimalGUI = false;
        } else {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_STRING;
            return -1;
        }
    } else if (strcmp(paramName, "spLoggerLevel") == 0) {
        sscanf(value,"%u",&(res->spLoggerLevel));
        if (!isAllDigits(value) || res->spLoggerLevel < 0 || res->spLoggerLevel > 4) {
            printConstraintsNotMet(filename, lineNum);
            *msg = SP_CONFIG_INVALID_INTEGER;
            return -1;
        }
    } else if (strcmp(paramName, "spLoggerFilename") == 0) {
        strcpy(res->spLoggerFilename, value);
    } else {
        return -2;
    }
    return 4;
}

void setDefaults(SPConfig config) {
    config->spPCADimension = 20;
    strcpy(config->spPCAFilename,"pca.yml");
    config->spNumOfFeatures = 100;
    config->spExtractionMode = true;
    config->spNumOfSimilarImages = 1;
    config->spKDTreeSplitMethod = MAX_SPREAD;
    config->spKNN = 1;
    config->spMinimalGUI = false;
    config->spLoggerLevel = 3;
    strcpy(config->spLoggerFilename,"stdout");
}

bool checkAnyMissingValues(const char* filename, int lineCount, bool* validation, SP_CONFIG_MSG* msg) {
    if (!validation[0]) {
        printParamNotSet(filename, lineCount, "spImagesDirectory");
        *msg = SP_CONFIG_MISSING_DIR;
        return true;
    }
    else if (!validation[1]) {
        printParamNotSet(filename, lineCount, "spImagesPrefix");
        *msg = SP_CONFIG_MISSING_PREFIX;
        return true;
    }
    else if (!validation[2]) {
        printParamNotSet(filename, lineCount, "spImagesSuffix");
        *msg = SP_CONFIG_MISSING_SUFFIX;
        return true;
    }
    else if (!validation[3]) {
        printParamNotSet(filename, lineCount, "spNumOfImages");
        *msg = SP_CONFIG_MISSING_NUM_IMAGES;
        return true;
    }
    else *msg = SP_CONFIG_SUCCESS;
    return false;
}

SPConfig spConfigCreate(const char* filename, SP_CONFIG_MSG* msg){
    assert( msg != NULL );
    FILE *fp;
    SPConfig res;
    char* line, *lineM, *paramName, *paramNameM;
    int lineCount, valueErrorFlag, paramErrorFlag;
    // validation for no default values
    // spImagesDirectory , spImagesPrefix , spImagesSuffix , spNumOfImages , General
    bool validation[] = {false, false, false, false, true};

    // Allocation
    res = (SPConfig)malloc(sizeof(struct sp_config_t));
    if (!res) {
        *msg = SP_CONFIG_ALLOC_FAIL;
        return NULL;
    }
    lineM = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    paramNameM = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    res->spImagesDirectory = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    res->spImagesPrefix = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    res->spImagesSuffix = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    res->spLoggerFilename = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    res->spPCAFilename = (char*)calloc(CONFIG_LINE_MAX_SIZE+1,sizeof(char));
    // Verify allocation didn't fail
    if (!lineM || !paramNameM || !res->spImagesDirectory || !res->spImagesPrefix || !res->spImagesSuffix || !res->spLoggerFilename || !res->spPCAFilename) {
        free(lineM);
        free(paramNameM);
        spConfigDestroy(res);
        *msg = SP_CONFIG_ALLOC_FAIL;
        return NULL;
    }
    // at this point all allocations are valid
    if ((fp = fopen(filename, "r")) == NULL) {
        free(lineM);
        free(paramNameM);
        spConfigDestroy(res);
        *msg = SP_CONFIG_CANNOT_OPEN_FILE;
        return NULL;
    }
    // file opened successfuly
    setDefaults(res);
    lineCount = 0;

    line = lineM;
    paramName = paramNameM;

    while(fgets(line, CONFIG_LINE_MAX_SIZE, fp) != NULL && feof(fp) == 0) {
        lineCount++;
        // remove whitespaces from beginning of line
        line = ignoreWhitespaces(line);
        //Skips comments and 'newline's
        if (*line == '#'||*line == '\n' || *line =='\0') {
            continue;
        }
        paramErrorFlag = extractParamNameFromString(line, paramName);
        // find value start;
        while (*line != '=' && *line != '\0') {
            line++;
        }
        if (*line != '\0') {
            line++; // skip '='
            line = ignoreWhitespaces(line);
        }
        valueErrorFlag = storeParamValueFromFile(filename, res, paramName, line, msg, lineCount);
        if (paramErrorFlag < 0 || valueErrorFlag < 0 || *line == '\0') {
            free(lineM);
            free(paramNameM);
            spConfigDestroy(res);
            fclose(fp);
            if (paramErrorFlag < 0 || valueErrorFlag != -1) {
                *msg = SP_CONFIG_INVALID_LINE;
                printInvalidLine(filename, lineCount);
            }
            return NULL;
        }
        // At this point paramName and value are correct and line is correct
        validation[valueErrorFlag] = true;
        // at this point the parameter was extracted
    }
    // check if all attributes exist
    if (checkAnyMissingValues(filename, lineCount, validation, msg)) {
        spConfigDestroy(res);
        res = NULL;
    }
    // after finishing the whole file, freeing everything and closing file
    free(lineM);
    free(paramNameM);
    fclose(fp);
    return res;
}

bool spConfigIsExtractionMode(const SPConfig config, SP_CONFIG_MSG* msg){
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return false;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spExtractionMode;
}

bool spConfigMinimalGui(const SPConfig config, SP_CONFIG_MSG* msg){
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return false;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spMinimalGUI;
}

int spConfigGetNumOfImages(const SPConfig config, SP_CONFIG_MSG* msg){
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spNumOfImages;
}

int spConfigGetNumOfFeatures(const SPConfig config, SP_CONFIG_MSG* msg){
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spNumOfFeatures;
}

int spConfigGetPCADim(const SPConfig config, SP_CONFIG_MSG* msg){
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spPCADimension;
}

int spConfigGetKNN(const SPConfig config, SP_CONFIG_MSG* msg) {
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spKNN;
}

int spConfigGetNumOfSimilarImages(const SPConfig config, SP_CONFIG_MSG* msg) {
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spNumOfSimilarImages;
}

SP_SPLIT_METHOD spConfigGetSplitMethod(const SPConfig config, SP_CONFIG_MSG* msg) {
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return INVALID;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spKDTreeSplitMethod;
}

SP_LOGGER_LEVEL spConfigGetLoggerLevel(const SPConfig config, SP_CONFIG_MSG* msg) {
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return -1;
    }
    *msg = SP_CONFIG_SUCCESS;
    return config->spLoggerLevel;
}

char* spConfigGetLoggerFilename(const SPConfig config, SP_CONFIG_MSG* msg) {
    assert(msg != NULL);
    if (!config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        *msg = SP_CONFIG_INVALID_ARGUMENT;
        return NULL;
    }
    *msg = SP_CONFIG_SUCCESS;
    char* res = (char*)malloc(strlen(config->spLoggerFilename)+1);
    strcpy(res, config->spLoggerFilename);
    return res;
}

SP_CONFIG_MSG spConfigGetImagePath(char* imagePath, const SPConfig config,int index){
    if (!imagePath || !config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        return SP_CONFIG_INVALID_ARGUMENT;
    }
    if (index >= config->spNumOfImages) {
        spLoggerPrintWarning("Index is out of range", __FILE__, __func__, __LINE__);
        return SP_CONFIG_INDEX_OUT_OF_RANGE;
    }
    sprintf(imagePath, "%s%s%d%s", config->spImagesDirectory, config->spImagesPrefix, index, config->spImagesSuffix);
    return SP_CONFIG_SUCCESS;
}

SP_CONFIG_MSG spConfigGetPCAPath(char* pcaPath, const SPConfig config){
    if (!config || !pcaPath) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        return SP_CONFIG_INVALID_ARGUMENT;
    }
    sprintf(pcaPath, "%s%s", config->spImagesDirectory, config->spPCAFilename);
    return SP_CONFIG_SUCCESS;
}

SP_CONFIG_MSG spConfigGetFeaturesFilePath(char* featsPath, const SPConfig config, int index) {
    if (!featsPath || !config) {
        spLoggerPrintWarning("Invalid arguments entered", __FILE__, __func__, __LINE__);
        return SP_CONFIG_INVALID_ARGUMENT;
    }
    if (index >= config->spNumOfImages) {
        spLoggerPrintWarning("Index is out of range", __FILE__, __func__, __LINE__);
        return SP_CONFIG_INDEX_OUT_OF_RANGE;
    }
    sprintf(featsPath, "%s%s%d.feats", config->spImagesDirectory, config->spImagesPrefix, index);
    return SP_CONFIG_SUCCESS;
}

void spConfigDestroy(SPConfig config){
    if (config == NULL) return;
    free(config->spImagesDirectory);
    free(config->spImagesPrefix);
    free(config->spImagesSuffix);
    free(config->spLoggerFilename);
    free(config->spPCAFilename);
    free(config);
    return;
}
