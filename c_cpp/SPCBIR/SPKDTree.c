
/*
 *  SPKDTree
 *  Author: Yotam
 */

 #include <stdlib.h>
 #include <assert.h>
 #include <stdbool.h>
 #include <stdio.h>
 #include <time.h>
 #include "SPKDArray.h"
 #include "SPKDTree.h"
 #include "SPConfig.h"
 #include "SPBPQueue.h"
 #include "SPListElement.h"
 #include "SPLogger.h"

struct sp_kdtree_node_t {
    int dim;     // the splitting dimension
    double val;     // the median value of the splitting dimension
    SPKDTreeNode left;     // left subtree
    SPKDTreeNode right;     // right subtree
    SPPoint data;     // has the point data only if this is a leaf
};

bool isLeaf(SPKDTreeNode node) {
    assert(node != NULL);
    if (node->data != NULL) return true;
    return false;
}

double calcL2Distance(double a, double b) {
    return (a-b)*(a-b);
}

SPKDTreeNode kdTreeCreate(SPKDArray arr, SP_SPLIT_METHOD split_method, int prevDim) {
    // allocation
    SPKDTreeNode res_node = (SPKDTreeNode)malloc(sizeof(*res_node));
    if (!res_node) return NULL;

    int dimT = -1;
    double valT = -1;
    SPKDTreeNode leftT = NULL;
    SPKDTreeNode rightT = NULL;
    SPPoint dataT = NULL;
    SPKDArray* splitT = NULL;     // will hoast {left, right}

    if (spKDArrayGetSize(arr) == 1) {
        dataT = spKDArrayGetPoint(arr, 0);
        if (!dataT) {
          spLoggerPrintWarning("Point creation failed", __FILE__, __func__, __LINE__);
          free(res_node);
          return NULL;
        }
    } else {
        switch (split_method) {
        case MAX_SPREAD:
            dimT = spKDArrayFindMaxSpreadDim(arr);
            break;
        case RANDOM:
            dimT = rand() % spKDArrayGetDimension(arr);
            break;
        case INCREMENTAL:
            dimT = (prevDim + 1) % spKDArrayGetDimension(arr);
            break;
        default:
            break;
        }
        splitT = Split(arr, dimT);
        if (!splitT) {
          free(res_node);
          return NULL;
        }
        valT = spKDArrayGetHighestPointValueInDim(splitT[0], dimT);
        leftT = kdTreeCreate(splitT[0],split_method, dimT);
        rightT = kdTreeCreate(splitT[1], split_method, dimT);
        if (!leftT || !rightT) {
          spKDTreeDestroy(leftT);
          spKDTreeDestroy(rightT);
          free(res_node);
          free(splitT);
          return NULL;
        }
    }
    // end of calculations
    res_node->dim = dimT;
    res_node->val = valT;
    res_node->left = leftT;
    res_node->right = rightT;
    res_node->data = dataT;

    free(splitT);
    spKDArrayDestroy(arr);
    return res_node;
}

void kdTreeFindKNearestNeighbors(SPKDTreeNode curr, SPBPQueue bpq, SPPoint p) {
    SPListElement el;
    bool other; // true = other is right | false = other is left

    if (!curr) return;

    if (isLeaf(curr)) {
        el = spListElementCreate(spPointGetIndex(curr->data), spPointL2SquaredDistance(p, curr->data));
        spBPQueueEnqueue(bpq, el);
        spListElementDestroy(el);
        return;
    }
    if (spPointGetAxisCoor(p,curr->dim) <= curr->val) {
        kdTreeFindKNearestNeighbors(curr->left, bpq, p);
        other = true;
    } else {
        kdTreeFindKNearestNeighbors(curr->right, bpq, p);
        other = false;
    }

    if (!spBPQueueIsFull(bpq) || calcL2Distance(curr->val, spPointGetAxisCoor(p,curr->dim)) < spBPQueueMaxValue(bpq)) {
        if (other) {
            kdTreeFindKNearestNeighbors(curr->right, bpq, p);
        } else {
            kdTreeFindKNearestNeighbors(curr->left, bpq, p);
        }
    }
}

int spKDTreeNodeGetDim(SPKDTreeNode tree) {
    assert(tree != NULL);
    return tree->dim;
}

int spKDTreeNodeGetVal(SPKDTreeNode tree) {
    assert(tree != NULL);
    return tree->val;
}

SPPoint spKDTreeNodeGetData(SPKDTreeNode tree) {
    assert(tree != NULL);
    SPPoint p = spPointCopy(tree->data);
    return p;
}

SPKDTreeNode spKDTreeNodeGetLeftSubTree(SPKDTreeNode tree) {
    assert(tree !=NULL);
    return tree->left;
}

SPKDTreeNode spKDTreeNodeGetRightSubTree(SPKDTreeNode tree) {
    assert(tree !=NULL);
    return tree->right;
}

SPKDTreeNode spKDTreeCreate(SPKDArray arr, SP_SPLIT_METHOD split_method) {
    if (!arr || spKDArrayGetSize(arr) < 1) return NULL;

    time_t t;     // used for rand()
    srand((unsigned) time(&t));

    SPKDTreeNode tree = kdTreeCreate(arr, split_method, -1);
    return tree;
}

SPBPQueue spKDTreeFindKNearestNeighbors(SPKDTreeNode root, int max_size, SPPoint p) {
    SPBPQueue bpq = spBPQueueCreate(max_size);
    if (!bpq) {
        spLoggerPrintWarning("Queue creation failure", __FILE__, __func__, __LINE__);
        return NULL;
    }
    kdTreeFindKNearestNeighbors(root, bpq, p);
    return bpq;
}

void spKDTreeDestroy(SPKDTreeNode tree) {
    if (tree == NULL) {
        // nothing to free
        return;
    }
    spKDTreeDestroy(tree->left);
    spKDTreeDestroy(tree->right);
    spPointDestroy(tree->data);
    free(tree);
}

void spKDTreeNodePrint(SPKDTreeNode tree) {
    int j;
    printf("--------------\n\tDim: %d\n\tVal: %.2f\n", tree->dim, tree->val);
    if (tree->dim == -1 && tree->val== -1) {
        printf("\tData: (");
        for (j = 0; j < spPointGetDimension(tree->data); j++) {
            printf("%.0f", spPointGetAxisCoor(tree->data, j));
            if (j < spPointGetDimension(tree->data) - 1)
                printf(", ");
        }
        printf(")\n\n");
    } else {
        printf("\tData: NULL\n\n");
    }
}

void printTreeToFile(SPKDTreeNode tree, FILE* fp) {
    int j;
    fprintf(fp, "--------------\n\tDim: %d\n\tVal: %.2f\n", tree->dim, tree->val);
    if (tree->dim == -1 && tree->val== -1) {
        fprintf(fp, "\tData: (");
        for (j = 0; j < spPointGetDimension(tree->data); j++) {
            fprintf(fp, "%.0f", spPointGetAxisCoor(tree->data, j));
            if (j < spPointGetDimension(tree->data) - 1)
                fprintf(fp, ", ");
        }
        fprintf(fp, ")\n\n");
    } else {
        fprintf(fp, "\tData: NULL\n\n");
        printTreeToFile(tree->left, fp);
        printTreeToFile(tree->right, fp);
    }
}

void spKDTreeFullPrint(SPKDTreeNode tree, const char* filepath) {
    FILE* fp = fopen(filepath, "w");
    if (!fp) {
      spLoggerPrintWarning("Failed to open/create print file, print canceled", __FILE__, __func__, __LINE__);
      return;
    }
    printTreeToFile(tree, fp);
    fclose(fp);
}
