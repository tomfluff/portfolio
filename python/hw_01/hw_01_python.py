#Skeleton file for HW1 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#Change the name of the file to your ID number (extension .py).


#Question 3
in_file = "hw_01_sample_text.txt"
out_file = "hw_01_output_text.txt"
#Add the rest of your code here.
#Assume the file in_file exists in the same folder as the current file

txtFile = open(in_file, 'r')
txtOutput = open(out_file, 'w')

for line in txtFile:
    if len(line) == 1:
        txtOutput.write('0\n'),
    else:
        txtOutput.write(str(len(line.split())) + '\n'),

txtOutput.close()

#**************************************************************
#Question 5
k = 3
n = 100
#Add the rest of your code here.

for n in range(1, n+1):
    if n%k == 0 and str(k) in str(n):
        print('boom-boom!')
    elif n%k == 0:
        print ('boom!')
    elif str(k) in str(n):
        print('boom!')
    else:
        print(n)

#**************************************************************
#Question 6
input_str = input("Please enter a positive integer: ")
#Add the rest of your code here.
#It should handle any positive integer or an arithmetic expression
#at the end, length, start and seq should hold the answers

input_int = int(eval(input_str))  # Evaluates input number

temp_len = 0
temp_seq = ''
index = 0

length = 0
seq = None
start = -1

for n in str(input_int):
    index += 1
    if int(n) % 2 != 0:
        temp_len += 1
        temp_seq += n
    else:
        temp_len = 0
        temp_seq = ''
    if temp_len > length:
        length = temp_len
        seq = temp_seq
        start = index - temp_len

print("The maximal length is", length)
print("Sequence starts at", start)
print("Sequence is", seq)

#**************************************************************
