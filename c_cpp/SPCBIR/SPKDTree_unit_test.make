CC = gcc
OBJS = SPKDTree_unit_test.o SPKDTree.o SPKDArray.o SPBPQueue.o SPList.o SPListElement.o SPPoint.o SPConfig.o
EXEC = sp_kdtree_unit_test
TESTS_DIR = ./unit_test
COMP_FLAG = -std=c99 -Wall -Wextra \
-Werror -pedantic-errors

$(EXEC): $(OBJS)
	$(CC) $(OBJS) -o $@
SPKDTree_unit_test.o: $(TESTS_DIR)/SPKDTree_unit_test.c $(TESTS_DIR)/unit_test_util.h SPPoint.h SPKDArray.h SPConfig.h SPKDTree.h
	$(CC) $(COMP_FLAG) -c $*.c
SPKDTree.o: SPKDTree.c SPKDTree.h SPKDArray.h SPConfig.h SPBPQueue.h SPListElement.h
	$(CC) $(COMP_FLAG) -c $*.c
SPKDArray.o: SPKDArray.c SPPoint.h
	$(CC) $(COMP_FLAG) -c $*.c
SPPoint.o: SPPoint.c SPPoint.h
	$(CC) $(COMP_FLAG) -c $*.c
SPConfig.o: SPConfig.c SPConfig.h
	$(CC) $(COMP_FLAG) -c $*.c
SPBPQueue.o: SPBPQueue.c SPBPQueue.h SPList.h SPListElement.h
	$(CC) $(COMP_FLAG) -c $*.c
SPList.o: SPList.c SPList.h SPListElement.h
	$(CC) $(COMP_FLAG) -c $*.c
SPListElement.o: SPListElement.c SPListElement.h
	$(CC) $(COMP_FLAG) -c $*.c
clean:
	rm -f $(OBJS) $(EXEC)
