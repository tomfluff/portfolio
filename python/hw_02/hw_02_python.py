#Skeleton file for HW2 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#You may add other utility functions to this file,
#but you may NOT change the signature of the existing ones.

#Change the name of the file to your ID number (extension .py).

import math

############
# QUESTION 1
############

# 1c
def reverse_sublist(lst, start, end):
    end_at = round((end+start)/2)
    for i in range(start, end_at):
        to_i = end-(i-start+1)
        tmp_obj = lst[i]
        lst[i] = lst[to_i]
        lst[to_i] = tmp_obj

# 1d
def rotate1(lst):
    lst.insert(0, lst.pop())

# 1e
def rotatek_v1(lst, k):
    for i in range(0, k % len(lst)):
        rotate1(lst)

# 1f
def rotatek_v2(lst, k):
    k %= len(lst)
    reverse_sublist(lst, 0, len(lst))
    reverse_sublist(lst, 0, k)
    reverse_sublist(lst, k, len(lst))

############
# QUESTION 2b
############

def power_new(a,b):
    """ computes a**b using iterated squaring """
    result = 1
    b_bin = bin(b)[2:]
    reverse_b_bin = b_bin[: :-1]
    for bit in reverse_b_bin: 
        if bit == '1':
            result = result*a
        a = a*a
    return result


############
# QUESTION 3b
############

def add_hex(A,B):
    base_16 = '0123456789abcdef'
    if len(A) > len(B):
        B = B.zfill(len(A))
    else:
        A = A.zfill(len(B))
    rev_A = A[::-1]
    rev_B = B[::-1]
    carry = 0
    result = ''
    for i in range(len(A)):
        tmp_sum = base_16.index(rev_A[i]) + base_16.index(rev_B[i]) + carry
        if tmp_sum >= 16:
            carry = 1
        else:
            carry = 0
        result += base_16[tmp_sum % 16]
    if carry != 0:
        result += '1'
    return result[::-1]


############
# QUESTION 4b
############

def sum_divisors(n):
    num_lst = [0]
    for i in range(1, math.floor(math.sqrt(n)) + 1):
        if n % i == 0 and i < n:
            num_lst.append(i)
            if num_lst.count(n//i) == 0 and n//i < n:
                num_lst.append(n//i)
    return sum(num_lst)

def is_finite(n):
    lst_sums = []
    isit = True
    while isit and n != 1:
        if lst_sums.count(n) != 0:
            isit = False
        else:
            lst_sums.append(n)
        n = sum_divisors(n)
    return isit

def cnt_finite(limit):
    sum_it = 0
    for n in range(1, limit + 1):
        if is_finite(n):
            sum_it += 1
    return sum_it

############
# QUESTION 5
############

def altsum_digits(n, d):
    str_n = str(n)
    tmp_sum = 0
    for i in range(0, d):
        tmp_sum += (-1)**i*int(str_n[i])
    max_sum = tmp_sum
    for j in range(1, len(str_n) - d):
        tmp_sum = -1 * tmp_sum + int(str_n[j-1]) + (-1)**(d-1)*int(str_n[d+j-1])
        if tmp_sum > max_sum:
            max_sum = tmp_sum
    return max_sum
    
########
# Tester
########

def test():

    lst = [1,2,3,4,5]
    reverse_sublist (lst,0,4)
    if lst != [4, 3, 2, 1, 5]:
        print("error in reverse_sublist()")        
    lst = ["a","b"]
    reverse_sublist (lst,0,1)
    if lst != ["a","b"]:
        print("error in reverse_sublist()")        

    lst = [1,2,3,4,5]
    rotate1(lst)
    if lst != [5,1,2,3,4]:
        print("error in rotate1()")        

    lst = [1,2,3,4,5]
    rotatek_v1(lst,2)
    if lst != [4,5,1,2,3]:
        print("error in rotatek_v1()")        
    
    lst = [1,2,3,4,5]
    rotatek_v2(lst,2)
    if lst != [4,5,1,2,3]:
        print("error in rotatek_v2()")        

    if power_new(2,3) != 8:
        print("error in power_new()")

    if add_hex("a5","17")!="bc":
        print("error in add_hex()")
    
    if sum_divisors(6)!=6 or \
       sum_divisors(4)!=3:        
        print("error in sum_divisors()")

    if is_finite(6) or \
       not is_finite(4):
        print("error in is_finite()")

    if cnt_finite(6) != 5:
        print("error in cnt_finite()")
        
    if altsum_digits(5**36,12)!=18:
        print("error in altsum_digits()")        
