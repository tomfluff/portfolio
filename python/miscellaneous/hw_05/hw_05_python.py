#Skeleton file for HW5 - Spring 2015 - extended intro to CS

#Add your implementation to this file

#You may add other utility functions to this file,
#but you may NOT change the signature of the existing ones.

#Change the name of the file to your ID number (extension .py).

############
# QUESTION 1
############

class Polynomial():
    def __init__(self, coeffs_lst):
        self.coeffs = coeffs_lst

    def __repr__(self):
        terms = [" + ("+str(self.coeffs[k])+"*x^" + \
                 str(k)+")" \
                 for k in range(len(self.coeffs)) \
                 if self.coeffs[k]!=0]
        terms = "".join(terms)
        if terms == "":
            return "0"
        else:
            return terms[3:] #discard leftmost '+'

    def degree(self):
        return len(self.coeffs)-1

    def evaluate(self, x):
        res = 0
        last_times = 1
        for i in range(self.degree()+1):
            res += self.coeffs[i]*last_times
            last_times *= x
        return res

    def derivative(self):
        der = Polynomial([])
        for i in range(1,self.degree()+1):
            der.coeffs.append(self.coeffs[i]*i)
        if der.coeffs == []:
            return Polynomial([0])
        return der

    def __eq__(self, other):
        assert isinstance(other, Polynomial)
        if self.degree() != other.degree():
            return False
        else:
            for i in range(self.degree()+1):
                if self.coeffs[i] != other.coeffs[i]:
                    return False
        return True

    def __lt__(self, other):
        assert isinstance(other, Polynomial)
        pass #replace this with your code

    def __add__(self, other):
        assert isinstance(other, Polynomial)
        res = Polynomial([])
        if self.degree() >= other.degree():
            for i in range(self.degree()+1):
                try:
                    res.coeffs.append(self.coeffs[i]+other.coeffs[i])
                except IndexError:
                    res.coeffs.append(self.coeffs[i])
            while res.degree() > 0 and res.coeffs[-1] == 0:
                list.pop(res.coeffs)
            return res
        else:
            return other.__add__(self)

    def __neg__(self):
        res = Polynomial([])
        for i in range(self.degree()+1):
            res.coeffs.append(self.coeffs[i]*(-1))
        return res

    def __sub__(self, other):
        assert isinstance(other, Polynomial)
        res = Polynomial([])
        deg_s, deg_o = self.degree(), other.degree()
        neg_other = other.__neg__()
        for i in range(min(deg_o, deg_s)+1):
            res.coeffs.append(self.coeffs[i]+neg_other.coeffs[i])
        if deg_o > deg_s:
            res.coeffs.extend(neg_other.coeffs[deg_s+1:])
        elif deg_s > deg_o:
            res.coeffs.extend(self.coeffs[deg_o+1:])
        while res.degree() > 0 and res.coeffs[-1] == 0:
            list.pop(res.coeffs)
        return res

    def __mul__(self, other):
        assert isinstance(other, Polynomial)
        deg_s, deg_o = self.degree(), other.degree()
        res = Polynomial([])
        for i in range(deg_s+1):
            if self.coeffs[i] == 0:
                res.coeffs.append(0)
            else:
                for j in range(deg_o+1):
                    if res.degree() < i+j:
                        res.coeffs.append(self.coeffs[i]*other.coeffs[j])
                    else:
                        res.coeffs[i+j] += self.coeffs[i]*other.coeffs[j]

        while res.degree() > 0 and res.coeffs[-1] == 0:
            list.pop(res.coeffs)
        return res

    def find_root(self):
        return NR(lambda x: self.evaluate(x), lambda x: self.derivative().evaluate(x))


## code for Newton Raphson, needed in find_root ##
from random import *

def diff_param(f,h=0.001):
    return (lambda x: (f(x+h)-f(x))/h)


def NR(func, deriv, epsilon=10**(-8), n=100, x0=None):
    if x0 is None:
        x0 = uniform(-100.,100.)
    x=x0; y=func(x)
    for i in range(n):
        if abs(y)<epsilon:
            #print (x,y,"convergence in",i, "iterations")
            return x
        elif abs(deriv(x))<epsilon:
            #print ("zero derivative, x0=",x0," i=",i, " xi=", x)
            return None
        else:
            #print(x,y)
            x = x- func(x)/deriv(x)
            y = func(x)
    #print("no convergence, x0=",x0," i=",i, " xi=", x)
    return None


############
# QUESTION 2
############

### Tree node class - code from lecture, You need to add a field ###

class Tree_node():
    def __init__(self,key,val):
        self.key=key
        self.val=val
        self.left=None
        self.right=None
        self.h_p = []

    def __repr__(self):
        return "[" + str(self.left) + " " + str(self.key) + " " + \
                    str(self.val) + " " + str(self.right) + "]"

### Binary search tree - code from lecture - DO NOT CHANGE ! ###

def insert(root,key,val):
    if root==None:
        root = Tree_node(key,val)
    elif key==root.key:
        root.val = val     # update the val for this key
    elif key<root.key:
        root.left = insert(root.left,key,val)
    elif key>root.key:
        root.right = insert(root.right,key,val)
    return root

def lookup(root,key):
    if root==None:
        return None
    elif key==root.key:
        return root.val
    elif key < root.key:
        return lookup(root.left,key)
    else:
        return lookup(root.right,key)


### End code from lecture ###

# a
def weight(node):
    if node is None:
        return 0
    elif node.left == node.right is None:
        node.h_p = [node.key]
        return node.val
    w_l = node.val + weight(node.left)
    w_r = node.val + weight(node.right)
    if node.left is None:
        w_l = w_r -1
    elif node.right is None:
        w_r = w_l -1
    node.h_p = [node.key]
    if w_l >= w_r and node.left is not None:
        node.h_p.extend(node.left.h_p)
    elif w_r >= w_r and node.right is not None:
        node.h_p.extend(node.right.h_p)
    return max(w_l, w_r)

# b
def heavy_path(node):
    if node is None:
        return None
    weight(node)
    return node.h_p

# c
def find_closest_key(node, k):
    if node == None:
        return None
    elif node.key == k:
        return node.key
    elif node.left == node.right == None:
        return node.key
    else:
        l_k = find_closest_key(node.left, k)
        r_k = find_closest_key(node.right, k)
        args = [x for x in [l_k,r_k,node.key] if x is not None]
        min_key = min(args, key=lambda x: abs(k-x))
        return min_key


############
# QUESTION 3
############



#########################################
### SimpleDict CODE ###
#########################################

class SimpleDict:
    def __init__(self, m, hash_func=hash):
        """ initial hash table, m empty entries """
        self.table = [ [] for i in range(m)]
        self.hash_mod = lambda x: hash_func(x) % m

    def __repr__(self):
        L = [self.table[i] for i in range(len(self.table))]
        return "".join([str(i) + " " + str(L[i]) + "\n" for i in range(len(self.table))])

    def __eq__(self, other):#for testing
        return self.table == other.table

    def items(self):
        return [item for chain in self.table for item in chain]

    def values(self):
        return [val for item in self.table for key,val in item]

    def find(self, key):
        """ returns value if key in hashtable, None otherwise  """
        h_k = self.hash_mod(key)
        for item in self.table[h_k]:
            if item[0] == key:
                return item[1]
        return None

    def insert(self, key, value):
        """ insert an item into table
            if key already exists - update value
            key must be hashable """
        h_k = self.hash_mod(key)
        for item in self.table[h_k]:
            if item[0] == key:
                self.table[h_k].remove(item)
        self.table[h_k].append((key,value))

#########################################
### SimpleDict CODE - end ###
#########################################

from urllib.request import urlopen

def download(url):
    ''' url should be a string containing the full path, incl. http://  '''
    f=urlopen(url)
    btext=f.read()
    text = btext.decode('utf-8')
    #read from the object, storing the page's contents in text.
    f.close()
    return text

def clean(text):
    ''' converts text to lower case, then replaces all characters except
       letters, spaces, newline and carriage return by spaces '''
    letter_set = "abcdefghijklmnopqrstuvwxyz \n\r"
    text = str.lower(text)
    cleaned = ""
    for letter in text:
        if letter in letter_set:
            cleaned += letter
        else:
            cleaned += " "
    return cleaned

def count_words(words):
    d = SimpleDict(200)
    for word in words:
        if d.find(word) is None:
            d.insert(word, 1)
        else:
            d.insert(word, d.find(word)+1)
    return d

def sort_by_cnt(count_dict):
    return sorted(count_dict.items(), key=lambda x: x[1], reverse=True)


############
# QUESTION 4
############

# a
def next_row(lst):
    new_row = [1]
    for i in range(1,len(lst)):
        new_row.append(lst[i] + lst[i-1])
    new_row.append(1)
    return new_row

# b
def generate_pascal():
    base = [1]
    while True:
        yield base
        base = next_row(base)

# c
def generate_bernoulli():
    pasc = generate_pascal()
    while True:
        ber_line = []
        ber_sum = 0
        for i in next(pasc):
            ber_sum += i
            ber_line.append(ber_sum)
        yield ber_line



############
# QUESTION 5
############

##In order to test Q5 uncomment the following line
#from matrix import * #matrix.py needs to be at the same directory

from hw_05_matrix import *

# a
def upside_down(im):
    n,m = im.dim()
    im2 = Matrix(n,m)
    for i in range((n+1)//2):
        im2.rows[i] = im.rows[n-i-1]
        im2.rows[n-i-1] = im.rows[i]
    return im2

# b
def reconstruct_image(m):
    all_bords = get_all_img_borders(m)
    curr_im = find_top_left(all_bords)
    n,k = curr_im.dim()
    full_img = Matrix(1+(n-1)*m, 1+(k-1)*m)
    for i in range(m):
        full_img[0:full_img.dim()[0], i*(k-1):(i+1)*(k-1)+1] = rebuild_column(m, all_bords, curr_im)
        curr_im = find_next_right(curr_im,all_bords)
    return full_img

def rebuild_column(m, bords, im):
    '''
    The function re-builds a column of segments based on a starting segment
    :param m: The number of segments
    :param bords: The list of border tuples for all segments
    :param im: The base segment for the column (should be the starting segment)
    :return: Returnes the re-built column (type Matrix)
    '''
    n,k = im.dim()
    column = Matrix(1+(n-1)*m,k)
    column[0:n,0:k] = im
    cnt = 1
    while cnt < m:
        im = find_next_bottom(im, bords)
        column[cnt*(n-1):(cnt+1)*(n-1)+1, 0:k] = im
        cnt += 1
    return column

# ---------------------------------------------------------------
# I'm not using the following function, yet I decided to leave it
# ---------------------------------------------------------------

def rebuild_row(m, bords, im):
    '''
    The function re-builds a row of segments based on a starting segment
    :param m: The number of segments
    :param bords: The list of border tuples for all segments
    :param im: The base segment for the row (should be the starting segment)
    :return: Returns the re-built row (type Matrix)
    '''
    n,k = im.dim()
    row = Matrix(n,1+(k-1)*m)
    row[0:n,0:k] = im
    cnt = 1
    while cnt < m:
        im = find_next_right(im,bords)
        row[0:n, cnt*(k-1):(cnt+1)*(k-1)+1] = im
        cnt += 1
    return row

# ---------------------------------------------------------------

def find_next_bottom(im, bords):
    '''
    Finds the next segment from the bottom
    :param im: The base segment
    :param bords: The list of border tuples for all segments
    :return: Returns the next segment (type Matrix)
    '''
    for i in range(len(bords)):
        if get_img_borders(im)[1] == bords[i][0]:
            new_im = Matrix.load(bords[i][4])
            bords.pop(i)
            return new_im
    return None

def find_next_right(im, bords):
    '''
    Finds the next segment from the right
    :param im: The base segment
    :param bords: The list of border tuples for all segments
    :return: Returns the next segment (type Matrix)
    '''
    for i in range(len(bords)):
        if get_img_borders(im)[3] == bords[i][2]:
            new_im = Matrix.load(bords[i][4])
            bords.pop(i)
            return new_im
    return None

def find_top_left(bord_lst):
    '''
    The function finds the top left segment
    (the one that nothing connects to from the top or left border)
    :param bord_lst: The list of border tuples for all segments
    :return: Returns the top-left segment (type Matrix)
    '''
    for i in range(len(bord_lst)):
        flag = True
        for j in range(len(bord_lst)):
            if i != j:
                if bord_lst[i][0] == bord_lst[j][1] or bord_lst[i][2] == bord_lst[j][3]:
                    flag = False
                    break
        if flag:
            t_l = Matrix.load(bord_lst[i][4])
            bord_lst.pop(i)
            return t_l

def get_all_img_borders(m):
    '''
    The function computes a list of tuples containing the borders of every segment
    :param m: The number of segments
    :return: Returns a list of tuples containing the top, bottom, left, right borders (in that order)
    '''
    bord_lst = []
    for i in range(1,m**2 +1):
        str_path = './hw_05_puzzle/im'+str(i)+'.bitmap'
        im = Matrix.load(str_path)
        t,b,l,r = get_img_borders(im)
        bord_lst.append((t,b,l,r, str_path))
    return bord_lst

def get_img_borders(im):
    '''
    The function calculates the 1px top, bottom, left, right edges of an image
    :param im: The image to calculate the edges for
    :return: A tuple of the top, bottom, left, right edges (in that order)
    '''
    n,m = im.dim()
    im_r_t, im_r_b = im.rows[0], im.rows[n-1]
    im_c_l, im_c_r = [im[i,0] for i in range(n)], [im[i,m-1] for i in range(n)]
    return im_r_t, im_r_b, im_c_l, im_c_r


########
# Tester
########

def test():

    #Question 1
    q = Polynomial([0, 0, 0, 6])
    if str(q) != "(6*x^3)":
        print("error in Polynomial.__init__ or Polynomial.in __repr__")
    if q.degree() != 3:
        print("error in Polynomial.degree")
    p = Polynomial([3, -4, 1])
    if p.evaluate(10) != 63:
        print("error in Polynomial.evaluate")
    dp = p.derivative()
    ddp = p.derivative().derivative()
    if ddp.evaluate(100) != 2:
        print("error in Polynomial.derivative")
    if not p == Polynomial([3, -4, 1]) or p==q:
        print("error in Polynomial.__eq__")
    r = p+q
    if r.evaluate(1) != 6:
        print("error in Polynomial.__add__")
    if not (q == Polynomial([0, 0, 0, 5]) + Polynomial([0, 0, 0, 1])):
        print("error in Polynomial.__add__ or Polynomial.__eq__")
    if (-p).evaluate(-10) != -143:
        print("error in Polynomial.__neg__")
    if (p-q).evaluate(-1) != 14:
        print('',p,'\n',q,'\n',(p-q),'\n',(p-q).evaluate(-1))
        print("error in Polynomial.__sub__")
    if (p*q).evaluate(2) != -48:
        print("error in Polynomial.__mult__")
    if (Polynomial([0])*p).evaluate(200) != 0:
        print("error in Polynomial class")
    root = p.find_root()
    if root-3 > 10**-7 and root-1 > 10**-7:
        print("error in Polynomial.find_root")


    #Question 2
    t = None
    t = insert(t, 1, 85) #the first time we change t from None to a "real" Node
    insert(t, 2.3, -30)
    insert(t, -10, 7.5)
    insert(t, 2, 10.3)
    if weight(t) != 92.5:
        print("error in weight()")
    if heavy_path(t) != [1, -10]:
        print("error in heavy path()")

    if find_closest_key(t, -5) != -10:
        print("error in find_closest_key()")
    if find_closest_key(t, 2.2) != 2.3:
        print("error in find_closest_key()")


    #Question 3
    h = SimpleDict(200)
    h.insert("ab", 2)
    h.insert("ef", 1)
    h.insert("cd", 3)
    if len(h.items()) != 3:
        print("error in insert()")
    if h.find("ab") != 2:
        print("error in find()")
    if h.find("ef") != 1:
        print("error in find()")
    if h.find("cd") != 3:
        print("error in find()")

    d = count_words(["ab", "cd", "cd", "ef", "cd", "ab"])
    if d is None:
        print("error in count_words()")
    if len(d.items()) != 3:
        print("error in count_words()")
    if d.find("ab") != 2:
        print("error in count_words()")
    if d.find("ef") != 1:
        print("error in count_words()")
    if d.find("cd") != 3:
        print("error in count_words()")

    if sort_by_cnt(d) != [['cd', 3], ['ab', 2], ['ef', 1]] and sort_by_cnt(d) != [('cd', 3), ('ab', 2), ('ef', 1)]:
        print("error in sort_by_cnt()")


    # Question 4
    gp = generate_pascal()
    if gp == None:
        print("error in generate_pascal()")
    elif next(gp)!=[1] or next(gp)!=[1,1] or next(gp)!=[1,2,1]:
        print("error in generate_pascal()")
