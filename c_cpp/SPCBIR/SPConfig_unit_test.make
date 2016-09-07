CC = gcc
OBJS = SPConfig_unit_test.o SPConfig.o
EXEC = sp_config_unit_test
TESTS_DIR = ./unit_test
COMP_FLAG = -std=c99 -Wall -Wextra \
-Werror -pedantic-errors

$(EXEC): $(OBJS)
	$(CC) $(OBJS) -o $@
SPConfig_Unit_Test.o: $(TESTS_DIR)/SPConfig_Unit_Test.c $(TESTS_DIR)/unit_test_util.h SPConfig.h
	$(CC) $(COMP_FLAG) -c $(TESTS_DIR)/$*.c
SPConfig.o: SPConfig.c SPConfig.h SPLogger.h
	$(CC) $(COMP_FLAG) -c $*.c
clean:
	rm -f $(OBJS) $(EXEC)
