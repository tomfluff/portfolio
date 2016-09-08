#Skeleton file for HW3 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#You may add other utility functions to this file,
#but you may NOT change the signature of the existing ones.

#Change the name of the file to your ID number (extension .py).

############
# QUESTION 2
############

def find_root(f, a, b, EPS=0.001):
    if f(a)*f(b) > 0:
        return None
    while True:
        if -EPS <= f(a) <= EPS:
            return a
        if -EPS <= f(b) <= EPS:
            return b
        point = (a+b)/2
        if f(point)*f(a) <= 0:
            b = point
        elif f(point)*f(b) <= 0:
            a = point

############
# QUESTION 3
############

# b
def multi_merge_v2(lst_of_lsts):
    min_i_lst = []
    for i in range(len(lst_of_lsts)):
        if lst_of_lsts[i] != []:
            min_i_lst.append(0)
        else:
            min_i_lst.append(-1)
    all_num = []
    while sum(min_i_lst) != (-1)*len(min_i_lst):
        min_lst = []
        for i in range(len(min_i_lst)):
            if min_i_lst[i] != -1:
                min_lst.append(lst_of_lsts[i][min_i_lst[i]])
            else:
                min_lst.append('-')
        min_num = min([x for x in min_lst if x != '-'])
        all_num.append(min_num)
        min_i_lst[min_lst.index(min_num)] += 1
        if min_i_lst[min_lst.index(min_num)] >= len(lst_of_lsts[min_lst.index(min_num)]):
            min_i_lst[min_lst.index(min_num)] = -1
    return all_num

def merge(lst1, lst2):
    """ merging two ordered lists using
        the two pointer algorithm """
    n1 = len(lst1)
    n2 = len(lst2)
    lst3 = [0 for i in range(n1 + n2)]  # alocates a new list
    i = j = k = 0  # simultaneous assignment
    while (i < n1 and j < n2):
        if (lst1[i] <= lst2[j]):
            lst3[k] = lst1[i]
            i = i +1
        else:
            lst3[k] = lst2[j]
            j = j + 1
        k = k + 1  # incremented at each iteration
    lst3[k:] = lst1[i:] + lst2[j:]  # append remaining elements
    return lst3

# c
def multi_merge_v3(lst_of_lsts):
    m = len(lst_of_lsts)
    merged = []

    for i in range(m):
        merged = merge(merged, lst_of_lsts[i])

    return merged



############
# QUESTION 5
############

# a
def steady_state(lst):
    start = 0
    end = len(lst) - 1
    while end != start+1:
        mid = (start+end)//2
        if start == lst[start]:
            return start
        elif end == lst[end]:
            return end
        if mid == lst[mid]:
            return mid
        elif mid < lst[mid]:
            end = mid
        elif mid > lst[mid]:
            start = mid


# d
def cnt_steady_states(lst):
    if lst[0] == 0 and lst[-1] == len(lst)-1:
        return len(lst)
    elif steady_state(lst) is not None:
        anchor = steady_state(lst)
        start = anchor
        end = len(lst)-1
        while end != start+1:
            mid = (start+end)//2
            if lst[mid] == mid:
                start = mid
            else:
                end = mid
        return end - anchor
    else:
        return 0


############
# QUESTION 6
############
def sort_num_list(lst):
    k = round(max([max(lst), (min(lst)**2)**0.5]))
    k_parts = [[] for i in range(4*k+1)]
    new_lst = []
    for e in lst:
        i = int((e+k)*2)
        k_parts[i].append(e)
    for e in k_parts:
        new_lst.extend(e)
    return new_lst
    
########
# Tester
########

def test():
    
    f1 = lambda x : x - 1
    res = find_root(f1 , -10, 10)
    EPS=0.001
    if res == None or abs(f1(res)) > EPS  or \
       find_root(lambda x : x**2  , -10, 10) != None:
        print("error in find_root")
        
   
    if multi_merge_v2([[1,2,2,3],[2,3,5],[5]]) != [1, 2, 2, 2, 3, 3, 5, 5] :
        print("error in multi_merge_v2")

    if multi_merge_v3([[1,2,2,3],[2,3,5],[5]]) != [1, 2, 2, 2, 3, 3, 5, 5] :
        print("error in multi_merge_v3")

    if steady_state([-4,-1,0,3,5]) != 3 or \
       steady_state([-4,-1,2,3,5]) not in [2,3] or \
       steady_state([-4,-1,0,4,5]) != None:
        print("error in steady_state")
        
    if cnt_steady_states([-4,-1,0,3,5]) != 1 or \
       cnt_steady_states([-4,-1,2,3,5]) != 2 or \
       cnt_steady_states([-4,-1,0,4,5]) != 0:
        print("error in cnt_steady_states")

    if sort_num_list([10, -2.5, 0, 12.5, -30, 0]) \
       != [-30, -2.5, 0, 0, 10, 12.5]:
        print("error in sort_num_list")
