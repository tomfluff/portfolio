
/*
 *  SPKDTree
 *  Author: Yotam
 */

 #ifndef SPKDTREE_H_
 #define SPKDTREE_H_

#include "SPKDArray.h"
#include "SPBPQueue.h"
#include "SPConfig.h"


typedef enum sp_kdtree_other_t {
  LEFT, RIGHT, NONE
} SP_KDTREE_OTHER;

typedef struct sp_kdtree_node_t* SPKDTreeNode;

/**
 * Creates a KDTreeNode recursivly (thus creating a tree)
 *
 * @method spKDTreeCreate
 * @param  arr            The KDArray containing all the data to be included in the tree
 * @param  split_method   The split method to create the tree by
 * @return                The root SPKDTreeNode representing the tree
 */
SPKDTreeNode spKDTreeCreate(SPKDArray arr, SP_SPLIT_METHOD split_method);

/**
 * Finds the K-Nearest-Neighbors (recursivly) for a given point p in the tree
 * @method spKDTreeFindKNearestNeighbors
 * @param  root                          The tree root element
 * @param  max_size                      The maximal size of the results queue
 * @param  p                             The query point
 * @return                               A BPQueue containing the K-Nearest-Neighbors of point p in the tree
 */
SPBPQueue spKDTreeFindKNearestNeighbors(SPKDTreeNode root, int max_size, SPPoint p);

/**
 * Gets the dimension of a KDTreeNode
 *
 * @method spKDTreeNodeGetDim
 * @param  tree               The KDTreeNode to extract from
 * @return                    An integer representing the dimension of the KDTreeNode
 */
int spKDTreeNodeGetDim(SPKDTreeNode tree);

/**
 * Gets the value of a KDTreeNode
 * @method spKDTreeNodeGetVal
 * @param  tree               The KDTreeNode to extract from
 * @return                    An integer representing the value of the KDTreeNode
 */
int spKDTreeNodeGetVal(SPKDTreeNode tree);

/**
 * Gets a copy of the SPPoint stored in the data attribute of a KDTreeNode
 * @method spKDTreeNodeGetData
 * @param  tree                The KDTreeNode to extract from
 * @return                     The SPPoint stored in the data of the KDTreeNode
 */
SPPoint spKDTreeNodeGetData(SPKDTreeNode tree);

/**
 * Gets the left KDTreeNode of a KDTreeNode
 * @method spKDTreeNodeGetLeftSubTree
 * @param  tree                       The KDTreeNode to extract from
 * @return                            The left KDTreeNode
 */
SPKDTreeNode spKDTreeNodeGetLeftSubTree(SPKDTreeNode tree);

/**
 * Gets the right KDTreeNode of a KDTreeNode
 * @method spKDTreeNodeGetRightSubTree
 * @param  tree                       The KDTreeNode to extract from
 * @return                            The right KDTreeNode
 */
SPKDTreeNode spKDTreeNodeGetRightSubTree(SPKDTreeNode tree);

/**
 * Destroy (recursivly) the KDTree
 * @method spKDTreeDestroy
 * @param  tree            The root KDTreeNode of the tree
 */
void spKDTreeDestroy(SPKDTreeNode tree);

/**
 * Print (stdout) a string representation of a given KDTreeNode
 * @method spKDTreeNodePrint
 * @param  tree              The KDTreeNode to print
 */
void spKDTreeNodePrint(SPKDTreeNode tree);

/**
 * Print (to a file) a representation of a KDTree (recursivly)
 * @method spKDTreeFullPrint
 * @param  tree              The root KDTreeNode of the tree
 * @param  filepath          The file path to print to
 */
void spKDTreeFullPrint(SPKDTreeNode tree, const char* filepath);

#endif
