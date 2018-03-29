#Skeleton file for HW4 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#You may add other utility functions to this file,
#but you may NOT change the signature of the existing ones.

#Change the name of the file to your ID number (extension .py).

############
# QUESTION 1
############
def max22(L, left, right):
    if left == right or left+1 == right:
        return max(L[left], L[right])
    l = max22(L, left, (right + left)//2)
    r = max22(L, (left + right)//2 + 1, right)
    return max(l, r)

def max_list22(L):
    return max22(L,0,len(L)-1)

############
# QUESTION 2
############
def change_fast(amount, coins):
    assert amount >= 0 and coins is not [] and type(amount) is int and type(coins) is list
    if amount == 0:
        return 1
    else:
        n = memo_change_fast(amount, coins, {})
        return n

def str_name(*params):
    return str(params)

def memo_change_fast(amount, coins, memo):
    key = str_name(amount, coins)
    if key in memo:
        return memo[key]
    elif coins == [] or amount < 0:
        memo[key] = 0
    elif amount == 0:
        memo[key] = 1
    else:
        memo[key] = memo_change_fast(amount, coins[:-1], memo) + memo_change_fast(amount - coins[-1], coins, memo)
    return memo[key]

############
# QUESTION 3
############
def win_fast(n, m, hlst, show=False):
    assert n > 0 and m > 0 and min(hlst) >= 0 and max(hlst) <= n and len(hlst) == m
    memo = {}
    return memo_win(n, m, hlst, memo, show)

def memo_win(n, m, hlst, memo, show=False):
    if str(hlst) in memo:
        return memo[str(hlst)]
    elif sum(hlst) == 0:
        memo[str(hlst)] = True
        return True
    for i in range(m):
        for j in range(hlst[i]):
            move_hlst = [n]*i+[j]*(m-i)
            new_hlst = [min(hlst[i],move_hlst[i]) for i in range(m)]
            if not memo_win(n, m, new_hlst, memo):
                if show:
                    print(new_hlst)
                memo[str(hlst)] = True
                return True
    memo[str(hlst)] = False
    return False


############
# QUESTION 4
############
def choose_sets(lst, k):
    # assert k < len(lst) or k > 0
    return memo_choose_sets(lst, k, {})

def memo_choose_sets(lst, k, memo):
    name = str_name(lst, k)
    if name in memo:
        return memo[name]
    elif k == 0 or lst == []:
        memo[name] = [[]]
        return [[]]
    elif k == 1:
        ret_lst = [[i] for i in lst]
        memo[name] = ret_lst
        return ret_lst
    elif len(lst) == k:
        memo[name] = [lst]
        return [lst]
    else:
        new_lst = lst.copy()
        item = new_lst.pop(0)
        result_01 = memo_choose_sets(new_lst, k-1, memo)
        result_02 = memo_choose_sets(new_lst, k, memo)
        for resu in result_01:
            resu.append(item)
        ret_lst = (result_01 + result_02)
        memo[name] = ret_lst
        return ret_lst

############
# QUESTION 5
############

import random

def is_prime(m,show_witness=False,sieve=False):
    """ probabilistic test for m's compositeness
    adds a trivial sieve to quickly eliminate divisibility
    by small primes """
    if sieve:
        for prime in [2,3,5,7,11,13,17,19,23,29]:
            if m % prime == 0:
                return False
    for i in range(0,100):
        a = random.randint(1,m-1) # a is a random integer in [1..m-1]
        if pow(a,m-1,m) != 1:
            if show_witness:  # caller wishes to see a witness
                print(m,"is composite","\n",a,"is a witness, i=",i+1)
            return False
    return True

def density_primes(n, times=10000):
    prime_cnt = 0
    for i in range(times):
        num = random.randrange(2**(n-1), 2**n)
        if is_prime(num, sieve=True):
            prime_cnt += 1
    return prime_cnt/times

########
# Tester
########

def test():

    # Q1 basic tests

    if max_list22([1,20,3]) != 20:
        print("error in max22()")
    if max_list22([1,20,300,400]) != 400:
        print("error in max22()")
        
    # Q2 basic tests
    if change_fast(10, [1,2,3]) != 14:
        print("error in change_fast()")

    # Q3 basic tests
    if win_fast(3, 4, [3,3,3,3]) != True:
        print("error in win_fast()")
    if win_fast(1, 1, [1]) != False:
        print("error in win_fast()")

    # Q4 basic tests
    if choose_sets([1,2,3,4], 0) != [[]]:
        print("error in choose_sets()")
    tmp = choose_sets(['a','b','c','d','e'], 4)
    if tmp == None:
        print("error in choose_sets()")
    else:
        tmp = sorted([sorted(e) for e in tmp])
        if tmp != [['a', 'b', 'c', 'd'], ['a', 'b', 'c', 'e'], ['a', 'b', 'd', 'e'], ['a', 'c', 'd', 'e'], ['b', 'c', 'd', 'e']]:
            print("error in choose_sets()")
