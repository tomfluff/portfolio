CC = gcc
CPP = g++
OBJS = main.o SPImageProc.o SPPoint.o SPConfig.o SPImageExtraction.o SPLogger.o SPKDTree.o SPKDArray.o SPBPQueue.o SPList.o SPListElement.o
EXEC = SPCBIR
INCLUDEPATH=/usr/local/lib/opencv-3.1.0/include/
LIBPATH=/usr/local/lib/opencv-3.1.0/lib/
LIBS=-lopencv_xfeatures2d -lopencv_features2d \
-lopencv_highgui -lopencv_imgcodecs -lopencv_imgproc -lopencv_core

CPP_COMP_FLAG = -std=c++11 -Wall -Wextra \
-Werror -pedantic-errors -DNDEBUG

C_COMP_FLAG = -std=c99 -Wall -Wextra \
-Werror -pedantic-errors -DNDEBUG

$(EXEC): $(OBJS)
	$(CPP) $(OBJS) -L$(LIBPATH) $(LIBS) -o $@
main.o: main.cpp main_aux.c SPImageProc.h SPConfig.h SPImageExtraction.h SPPoint.h SPLogger.h SPKDTree.h SPKDArray.h SPBPQueue.h SPListElement.h
	$(CPP) $(CPP_COMP_FLAG) -I$(INCLUDEPATH) -c $*.cpp
SPImageProc.o: SPImageProc.cpp SPImageProc.h SPConfig.h SPPoint.h SPLogger.h
	$(CPP) $(CPP_COMP_FLAG) -I$(INCLUDEPATH) -c $*.cpp
SPConfig.o: SPConfig.c SPConfig.h SPLogger.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPImageExtraction.o: SPImageExtraction.c SPImageExtraction.h SPPoint.h SPConfig.h SPLogger.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPLogger.o: SPLogger.c SPLogger.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPPoint.o: SPPoint.c SPPoint.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPKDTree.o: SPKDTree.c SPKDTree.h SPKDArray.h SPConfig.h SPBPQueue.h SPListElement.h SPLogger.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPBPQueue.o: SPBPQueue.c SPBPQueue.h SPList.h SPListElement.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPKDArray.o: SPKDArray.c SPPoint.h SPLogger.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPList.o: SPList.c SPList.h SPListElement.h
	$(CC) $(C_COMP_FLAG) -c $*.c
SPListElement.o: SPListElement.c SPListElement.h
	$(CC) $(C_COMP_FLAG) -c $*.c
clean:
	rm -f $(OBJS) $(EXEC)
