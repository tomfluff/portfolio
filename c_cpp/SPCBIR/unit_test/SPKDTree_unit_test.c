/*
 *  KDTree_unit_test test file for KDTree
 *  Author: Yotam
 */

#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include "unit_test_util.h"
#include "../SPKDTree.h"
#include "../SPKDArray.h"
#include "../SPPoint.h"
#include "../SPConfig.h"

const int DIM = 2;
const int SIZE = 5;

SPPoint* GeneratePointArray() {
  SPPoint* arr = (SPPoint*)malloc(sizeof(SPPoint)*SIZE);
  double* data = (double*)malloc(DIM*sizeof(double));
  data[0] = 1;
  data[1] = 2;
  arr[0] = spPointCreate(data, DIM, 0);
  data[0] = 123;
  data[1] = 70;
  arr[1] = spPointCreate(data,DIM,1);
  data[0] = 2;
  data[1] = 7;
  arr[2] = spPointCreate(data,DIM,2);
  data[0] = 9;
  data[1] = 11;
  arr[3] = spPointCreate(data,DIM,3);
  data[0] = 3;
  data[1] = 4;
  arr[4] = spPointCreate(data,DIM,4);
  free(data);
  return arr;
}

SPKDArray generateKDArray() {
  SPPoint* arr = GeneratePointArray();
  SPKDArray kdArr =  Init(arr, SIZE);
  for (int i=0;i<SIZE;i++) spPointDestroy(arr[i]);
  free(arr);
  return kdArr;
}

int spKDTreeCreateTest() {
  SPKDTreeNode root, t_1_0, t_1_1, t_2_0, t_2_1, t_2_2, t_2_3, t_3_0, t_3_1;
  SPKDArray arr;

  // START MAX_SPREAD
  arr = generateKDArray();
  root = spKDTreeCreate(arr, MAX_SPREAD);
  // all branches
  t_1_0 = spKDTreeNodeGetLeftSubTree(root);
  t_1_1 = spKDTreeNodeGetRightSubTree(root);
  t_2_0 = spKDTreeNodeGetLeftSubTree(t_1_0);
  t_2_1 = spKDTreeNodeGetRightSubTree(t_1_0);
  t_2_2 = spKDTreeNodeGetLeftSubTree(t_1_1);
  t_2_3 = spKDTreeNodeGetRightSubTree(t_1_1);
  t_3_0 = spKDTreeNodeGetLeftSubTree(t_2_0);
  t_3_1 = spKDTreeNodeGetRightSubTree(t_2_0);

  ASSERT_TRUE(root && t_1_0 && t_1_1 && t_2_0 && t_2_1 && t_2_2 && t_2_3 && t_3_0 && t_3_1);

  // root
  ASSERT_TRUE(spKDTreeNodeGetData(root) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(root) == 0);
  ASSERT_TRUE(spKDTreeNodeGetVal(root) == 3);
  // left child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_1_0) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_1_0) == 1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_1_0) == 4);
  // right child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_1_1) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_1_1) == 0);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_1_1) == 9);
  // left left child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_2_0) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_0) == 0);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_0) == 1);
  // left right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_1),0) == 2);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_1),1) == 7);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_1) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_1) == -1);
  // right left child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_2),0) == 9);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_2),1) == 11);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_2) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_2) == -1);
  //right right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_3),0) == 123);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_3),1) == 70);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_3) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_3) == -1);
  // left left left child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_0),0) == 1);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_0),1) == 2);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_3_0) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_3_0) == -1);
  // left left right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_1),0) == 3);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_1),1) == 4);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_3_0) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_3_0) == -1);

  spKDTreeDestroy(root);
  // END MAX_SPREAD

  // START INCREMENTAL
  arr = generateKDArray();
  root = spKDTreeCreate(arr, INCREMENTAL);
  // all branches
  t_1_0 = spKDTreeNodeGetLeftSubTree(root);
  t_1_1 = spKDTreeNodeGetRightSubTree(root);
  t_2_0 = spKDTreeNodeGetLeftSubTree(t_1_0);
  t_2_1 = spKDTreeNodeGetRightSubTree(t_1_0);
  t_2_2 = spKDTreeNodeGetLeftSubTree(t_1_1);
  t_2_3 = spKDTreeNodeGetRightSubTree(t_1_1);
  t_3_0 = spKDTreeNodeGetLeftSubTree(t_2_0);
  t_3_1 = spKDTreeNodeGetRightSubTree(t_2_0);

  ASSERT_TRUE(root && t_1_0 && t_1_1 && t_2_0 && t_2_1 && t_2_2 && t_2_3 && t_3_0 && t_3_1);

  // root
  ASSERT_TRUE(spKDTreeNodeGetData(root) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(root) == 0);
  ASSERT_TRUE(spKDTreeNodeGetVal(root) == 3);
  // left child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_1_0) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_1_0) == 1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_1_0) == 4);
  // right child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_1_1) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_1_1) == 1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_1_1) == 11);
  // left left child (root)
  ASSERT_TRUE(spKDTreeNodeGetData(t_2_0) == NULL);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_0) == 0);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_0) == 1);
  // left right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_1),0) == 2);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_1),1) == 7);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_1) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_1) == -1);
  // right left child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_2),0) == 9);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_2),1) == 11);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_2) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_2) == -1);
  //right right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_3),0) == 123);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_2_3),1) == 70);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_2_3) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_2_3) == -1);
  // left left left child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_0),0) == 1);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_0),1) == 2);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_3_0) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_3_0) == -1);
  // left left right child (root)
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_1),0) == 3);
  ASSERT_TRUE(spPointGetAxisCoor(spKDTreeNodeGetData(t_3_1),1) == 4);
  ASSERT_TRUE(spKDTreeNodeGetDim(t_3_0) == -1);
  ASSERT_TRUE(spKDTreeNodeGetVal(t_3_0) == -1);

  spKDTreeDestroy(root);
  // END INCREMENTAL

  // START RANDOM
  arr = generateKDArray();
  root = spKDTreeCreate(arr, RANDOM);
  // all branches
  t_1_0 = spKDTreeNodeGetLeftSubTree(root);
  t_1_1 = spKDTreeNodeGetRightSubTree(root);
  t_2_0 = spKDTreeNodeGetLeftSubTree(t_1_0);
  t_2_1 = spKDTreeNodeGetRightSubTree(t_1_0);
  t_2_2 = spKDTreeNodeGetLeftSubTree(t_1_1);
  t_2_3 = spKDTreeNodeGetRightSubTree(t_1_1);
  t_3_0 = spKDTreeNodeGetLeftSubTree(t_2_0);
  t_3_1 = spKDTreeNodeGetRightSubTree(t_2_0);

  ASSERT_TRUE(root && t_1_0 && t_1_1 && t_2_0 && t_2_1 && t_2_2 && t_2_3 && t_3_0 && t_3_1);

  spKDTreeDestroy(root);
  // END RANDOM

  return true;
}

int spKDTreeFindKNearestNeighborsTest() {
  SPPoint p;
  SPKDTreeNode root;
  SPBPQueue bpq;
  SPListElement el;
  root = spKDTreeCreate(generateKDArray(), MAX_SPREAD);

  double data[] = {1.0,1.0};
  p = spPointCreate(data, 2, 0);

  bpq = spKDTreeFindKNearestNeighbors(root, 3, p);
  ASSERT_TRUE(bpq);

  el = spBPQueuePeek(bpq);
  spBPQueueDequeue(bpq);
  ASSERT_TRUE(spListElementGetIndex(el) == 0);
  ASSERT_TRUE(spListElementGetValue(el) == 1);
  spListElementDestroy(el);

  el = spBPQueuePeek(bpq);
  spBPQueueDequeue(bpq);
  ASSERT_TRUE(spListElementGetIndex(el) == 4);
  ASSERT_TRUE(spListElementGetValue(el) == 13);
  spListElementDestroy(el);

  el = spBPQueuePeek(bpq);
  spBPQueueDequeue(bpq);
  ASSERT_TRUE(spListElementGetIndex(el) == 2);
  ASSERT_TRUE(spListElementGetValue(el) == 37);
  spListElementDestroy(el);

  spPointDestroy(p);
  spKDTreeDestroy(root);
  spBPQueueDestroy(bpq);
  return true;
}

int main() {
  RUN_TEST(spKDTreeCreateTest);
  RUN_TEST(spKDTreeFindKNearestNeighborsTest);
  return 0;
}
