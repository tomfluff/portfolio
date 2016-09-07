/*
 *  KDArray_unit_test test file for KDArray
 *  Author: Yotam
 */

#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include "unit_test_util.h"
#include "../SPConfig.h"


bool spConfigCreateTest() {
  char* filename = "./spcbir.config";
  SP_CONFIG_MSG* msg = (SP_CONFIG_MSG*)malloc(sizeof(SP_CONFIG_MSG));
  SPConfig conf = spConfigCreate(filename, msg);
  ASSERT_TRUE(conf != NULL);
  ASSERT_TRUE(spConfigIsExtractionMode(conf,msg) == true);
  ASSERT_TRUE(spConfigMinimalGui(conf, msg) == false);
  ASSERT_TRUE(spConfigGetNumOfImages(conf,msg) == 17);
  ASSERT_TRUE(spConfigGetNumOfFeatures(conf,msg) == 100);
  ASSERT_TRUE(spConfigGetPCADim(conf,msg) == 20);
  char* imagePath = (char*)calloc(CONFIG_LINE_MAX_SIZE+1, sizeof(char));
  ASSERT_TRUE(spConfigGetImagePath(imagePath, conf, 10) == SP_CONFIG_SUCCESS);
  ASSERT_TRUE(strcmp(imagePath, "./images/img10.png") == 0);
  char* pcaPath = (char*)calloc(CONFIG_LINE_MAX_SIZE+1, sizeof(char));
  ASSERT_TRUE(spConfigGetPCAPath(pcaPath, conf) == SP_CONFIG_SUCCESS);
  ASSERT_TRUE(strcmp(pcaPath, "./images/pca.yml") == 0);
  free(msg);
  free(imagePath);
  free(pcaPath);
  spConfigDestroy(conf);
  return true;
}

int main() {
  RUN_TEST(spConfigCreateTest);
  return 0;
}
