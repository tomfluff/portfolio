#Binomial Heaps

##**Intro:**
An implementation of a non-lazy binomial heap, of non-negative, non-unique whole numbers.

The following is a chronological description of all members of our Binomial Heap implementation. Each description begins with a short summary, and then describes the operations it performs or fields each member holds. Methods meant for debugging purposes or presentation purposes are explained less thoroughly as they are not strictly part of the implementation.

Throughout the codes there are some optional prinIT()  statements used solely for debugging purposes, that are not part of the implementation and thus are not mentioned in the documentation. These will only execute when FLAG is set to true.

##**BinomialHeap Class**

**Contains all member of this implementation, including a class that defines the HeapNode objects which will be the nodes in our heaps.**

**FLAG** - A Boolean variable that can be set to true in order to print out information about the tree in order to execute checks and debugging.


##**HeapNode Class**

Our BinomialHeap implementation utilizes HeapNode Objects created and inserted into it, the binomial heap itself is compromised of connected HeapNodes.  The implementation occurs using a data structure similar to a doubly linked list, where the BinomialHeap has a list of root HeapNodes, each linked one to the other, each of which points to its offspring nodes, also held in doubly linked lists. Further explanation on the build of the BinomialHeap will follow below.

Each HeapNode object holds information about itself, as well as holding pointers to its neighbors, offspring list and parent, allowing us easy access to our entire BinomialHeap, using only a pointer to the startNode (detailed below) or head root of our tree.

###**Fields:**

**key** – A int field that holds the key of each node, which is used not only to insert it into the correct place in the tree, and thus later to find it, but in our implementation is also the node&#39;s value (initiliazed through constructor and never changed, necessary field to initialize a node).

**degree** – A int field that signifies how many children the given node has, meaning it is (in our implementation) a root of a binomial tree of that degree. If this value is 0, then our node is a leaf, or in other words, the root of a binomial tree of root 0.

**next** – A field pointing to the &#39;next&#39; HeapNode in our root list or offspring list (detailed below) this and **prev** allows us to create a doubly linked list of sorts and gives us easy access to any node, allowing us to go over all nodes easily. If this field is set to null then the node is the last in the list meaning either the last (largest) root or the last (smallest) offspring of another HeapNode.

**prev** – A field pointing to the previous HeapNode in our root list or offspring list (detailed below).

**parent** – A field pointing to the &#39;parent&#39; HeapNode of our node, meaning the root of the binomial tree it belongs to. If this field is set to null then our HeapNode object is a root of our BinomialHeap.

**leftChild** – A field pointing to the beginning HeapNode of our node&#39;s offspring list, meaning the largest degree HeapNode object that is a part of the binomial tree that our current node is a root of.

###**Constructor:** 

**HeapNode(int value) -  O(`1`)**

There is only one, standard, contructor for HeapNode. It accepts an int as its argument, which is then set to be the key of our HeapNode. All HeapNodes are initiatied with degree 0 as they have no children upon being initialized, as well as all pointers being set to null, as the object is currently a stand alone HeapNode.

###**Getters and Setters: All of time complexity -  O(`1`)**

- **getKey()** - standard getter that returns the key field of node used to call it (this).
- **setKey(int)** - standard setter that assigns the int given to the **key** field of the node calling it.
- **getDegree()** - standard getter that returns the degree field of node used to call it (this).
- **setDegree(int)** - standard setter that assigns the int given to the **degree** field of the node calling it.
- **getNext()** - standard getter that returns the **next** field of node used to call it (this), meaning a pointer to the **&#39;next&#39;** node in the list of roots or offspring.
- **setNext(HeapNode next)** - standard setter that sets the next field of node used to call it (this), meaning sets it to point to the **next** node given as an argument.
- **getPrev()** - standard getter that returns the **prev** field of node used to call it (this), meaning a pointer to the **&#39;previous&#39;** node in the list of roots or offspring..
- **setPrev (HeapNode prev)** - standard setter that sets the **prev** field of node used to call it (this), meaning sets it to point to the **prev** node given as an argument.
- **getPrev()** - standard getter that returns the **parent** field of node used to call it (this), meaning a pointer to the **&#39;parent** node of the offspring list to which it belongs – the root of the binomial tree or node belongs to.
- **setPrev (HeapNode prev)** - standard setter that sets the **parent** field of node used to call it (this), meaning sets it to point to the **parent** node given as an argument.
- **getLeftChild()** - standard getter that returns the l **eftChild** field of node used to call it (this), meaning a pointer to the starting node (the ** ** node with the highest degree) of its offspring list.
- **setLeftChild(HeapNode prev)** - standard setter that sets the **leftChild** field of node used to call it (this), meaning sets it to point to the startingnode of its offspring list. This should always be the node of the highest degree from its offspring.

###**Methods:**

**equals(Object obj) -  O(`1`)**

A boolean method overriding java.lang&#39;s equals method that corresponds with the == operator.  The method checks if two HeapNode objects are equal by checking (after initializing a HeapNode other from casting obj as HeapNode)  if all of the fields of the two objects are equal as well as if they have the same OuterType (see below).

**toString - O(`1`)**

A method overriding java.lang&#39;s toString method for the type HeapNode. This method will return a string version of any object for which its called. For our purposes we chose to return a string with the HeapNode&#39;s key and degree.

**toLongString - O(`1`)**

A method  used for debugging purposes that expands on the shorter toString() method, returning a string with all fields of the HeapNode.

**printSubTree - O(`k`)**

*(k being the number of nodes in the subtree including root)*

A method  used for debugging purposes returns a string that contains information about the entire sub tree of a given node. The method uses recursive calls to go down and add all nodes in the subtree to the string before returning it.

**getOuterType() - O(`1`)**

A method used in the overwritten equals method to check if the two HeapNodes being compared are both part of the same BinomialHeap.

**printNodeListFromStartNode(boolean longString) - O(`k`)**

*(k  = number of nodes in list that the node the method is called for is the head of)*

A method  used for debugging purposes returns a string that contains information about the list of nodes that the node which calls the method is the start of. Uses either the toString() or toLongString() methods defined above depending on the value of the Boolean variable given to it.                }

##**BinomialHeap class:**

###**Fields:**

**startNode** - A field that holds a pointer to our BinomialHeap object&#39;s start node, meaning the first root in its root list, the smallest root of the heap. This node has a **prev** field of null.

This field is initialized either in the constructor or upon the first insertion into the BinomialHeap and is maintained any time we delete anything from our heap, as well as anytime we meld two heaps together (including any insertion which uses meld).

**minNode** - A field that holds a pointer to our BinomialHeap object&#39;s minimal node, meaning the node with the smallest (minimal) key. This field is initialized either in the constructor or upon the first insertion into the BinomialHeap and is maintained any time we delete anything from our heap (since we could be deleting the minNode), as well as anytime we meld two heaps together (including any insertion which uses meld).

**size** - An int field that gives us information about the number of nodes currently in our tree. The size of our tree is the exact number of nodes that it currently contains including all root nodes. This field is initialized either in the constructor or upon the first insertion into the BinomialHeap and is maintained any time we delete anything from our heap,as well as anytime we meld two heaps together (including any insertion which uses meld).

###**Constructors**

**BinomialHeap() - O(`1`)**– The default constructor called without arguments, initializes a BinomialHeap object with all HeapNode fields; startNode,and minNode initialized to null and size to 0. Thus creating an empty BinomialHeap.

**BinomialHeap(HeapNode startNode) - O(`logn`)**– This constructor uses the default constructor to initialize a BinomialHeap object, as described above and then assigns its startNode field to point to the given argument, thus creating a BinomialHeap from the startNode and whatever it points to. This could be a heap with one node, or many. The constructor then uses helper function **findMinForHeap()** defined below to find the minimal node in our new heap and sets the **minNode** field to point at that node. Lastly the constructor uses helper function **getSizeForHeap()**, defined below, to find how many nodes are in our linked lists data structure that we have just used to initialize our BinomialHeap. We assume that the startNode given to our constructor is a valid head root for a binomial heap and do not check the input.

###**Getters and Setters**

There are no straightforward setters nor getters for our BinomialHeap fields, but other methods that serve the same or a similar purpose will be discussed below.

###**Methods**

**toString() - O(`logn`)**

A method that overrides java.lang&#39;s toString() method for the BinomialHeap class, offering us a method to return a String that contains our BinomialHeaps desired information. The method initializes a String with certain characters for clarity, then after initializing a node variable to our BinomiaHeap&#39;s startNode uses a while loop to go over the entire root list, calling HeapNode&#39;s toString() method for each, and then updating node to be node.getNext(), meaning the next root in the root list, each time adding to our string which is returned in the end containing all the root nodes of the tree.

**toLongString() - O(`logn`)**

A method very similar to toString that returns a string of the same nodes as toString, simply with more information about each node. Works in the same manner as toString() using HeapNode&#39;s toLongString() method instead of its toString() method.

**getSizeForHeap() - O(`logn`)**

A private method used to return an int of the number of nodes contained in the BinomialHeap at a given time.

1. Initializes int variable size to 0.
2. Initializes HeapNode variable node to be the startNode of our binomial heap.
3. Uses a while loop to go over all root nodes, checking each time if node&#39;s **next** field is null, and if not then adding 2node.degree to the size, and then updating node to be node.next.
4. When node.next == null, we exit the while loop since this means we are at the end of our root list.
5. Return size.

\*We add 2 to the power of each root node&#39;s degree to the size, since by the properties of binomial heaps, each root node is the root of a binomial tree of size2degree. This was proved by induction in class.

**findMinForHeap() - O(`logn`)**

A private method used to return an int of the number of nodes contained in the BinomialHeap at a given time. Since one of the fundamental properties of binomial heaps is that each node is greater than or equal to that of its parent (meaning the key), we know that the minimal node will be in the root list. Thus suffice to go over the root list to check which is the minimal node (has minimal key) in order to find the tree&#39;s minNode.

1. Initialize HeapNode variable node to our first root (startNode).
2. Initialize HeapNode variable min to be our startNode as well.
3. Check if node is null – if so then there is no minNode, since our heap is empty and we return null.
4. Use  while loop with condition that node not be null to go over entire root list as follows:
  1. Update node to be node.getNext(), meaning the next node in our root list.
  2. Check if our (newly updated) node&#39;s key is smaller than that of min.
    1. If so update min to be our current node.
5. Once node ==null, this means that we have reached the end of our list, and our while loop is complete.
6. return min.

**empty() - O(`1`)**

A public, boolean method used to check if a BinomialHeap object contains HeapNodes or not (is empty). The method simply checks if the **startNode** field of the BinomialHeap is null, if it is then the BinomialHeap must be empty and true is returned, if not, then it cannot be empty as it has at least one node (startNode) and false is returned.

**insert(int value) - O(`logn`)**

A public, boolean command method used to insert a new node into our binomial heap. The method, given an int to be the new node&#39;s key creates a new HeapNode with that key, and then inserts it into the binomial heap by creating a new BinomialHeap with just that node and then melding that with our existing BinomialHeap.

1. Initializes a new HeapNode node with our given key using HeapNode class&#39; default constructor. This is the node we want to insert into our heap.
2. Initializes a new BinomialHeap variable heap using the constructor that accepts a HeapNode, giving it our node. This creates a BinomialHeap with node as its only element (size 1).
3. We call our **meld()** function, defined below, melding the two heaps together, thus inserting our node into the heap correctly while maintaining all relevant fields and connections.

Our binomial heap now contains a node (or an additional node) with the given int argument as its key.

**deleteMin(int value) - O(`logn`)**

A public command method that deletes the node with the minimal key from our BinomialHeap. Reminder – the minNode of our tree will always be a root, meaning a meber of the root list, from the fundamental properties of the binomial heap. Our method initializes a variable to be the current minNode, then removes it from the tree by disconnecting it from the two HeapNodes to the &#39;left&#39; and &#39;right&#39; of it in the root list. After doing so we take all of minNode&#39;s offspring and initialize a new BinomialHeap with these nodes by reversing the list minNode&#39;s direct offspring and then initializing a BinomialHeap using the (newly reversed) first node of the list as the BinomialHeap&#39;s new startNode. We then merge this new heap with our existing BinomialHeap with which the method was called and we have obtained our original BinomialHeap updated to be without the deleted node, with all fields updated and all properties maintained.

1. Initializes a new HeapNode variable min to be the current **minNode** f our BinomialHeap.
2. The next thing we do is disconnect min from our root list, maintain a pointer to it, but in essence removing it and all its offspring nodes from our BinomialHeap (used to call method).
3. For removal, we use getters and setters from the HeapNode class, setting the node previous to min to have its **next** field be min&#39;s next, and setting it to be the **prev** field of the node that is min&#39;s next.  This in essence removes min from the root list, since there is now no node that points to it. When doing so we check some specific cases that require special treatement:
  1. If our min is the last node in the root list, meaning min.getNext() == null:
    1. Then the **prev** field of min&#39;s next node need not be handled.
  2. If our min is the first node in the root list (startNode), meaning min.getPrev == null:
    1. Then the **next** field of min&#39;s prev node need not be handled.
    2. We must update our BinomialHeap&#39;s **startNode** field to be min.getNext(), since we are removing the current startNode from the heap.
4. We update our BinomialHeap&#39;s **minNode** field using findMinForHeap() helper method, defined above, since we have just removed the minimal node. We do this to make it easier for us to update the minNode in meld() that will be used shortly after.
5. We now want to switch the list of min&#39;s children so that we can use it to create a new binomial heap to be melded with ours.
  1. First we check (using HeapNode class&#39;s getLeftChild(), checking if it&#39;s null) and handle the specific case of min not having any children, meaning being a tree of degree 0:
    1. If so then all we have to do is decrement **size** field of our BinomialHeap and we are finished since we have already removed the desired node, and no other nodes from our heap, we are done.
  2. We define four HeapNode variables to hold temporary pointers to nodes to assist with the reverse. We initialize all four, last, tempNext, tempPrev and firstItem to be the current beginning of the list, min.getLeftChild.
  3. We use a while loop to go through the list, with condition that last not be null.
    1. If last.getNext() is null, meaning we have reached the end of our list, then we set firstItem to be last, since it is the head of our new root list.
    2. For each node we set its parent to be null, disconnecting it from min.
    3. We set the temp nodes, to be the fields of last (tempNext= last.getNext() and same for prev).
    4. Then use those temp nodes to reassign the values of last reversed (last.setNext(tempPrev)).
    5. Lastly we update last to be tempNext, meaning the next node after last in the original non-reversed list. Thus traversing over the entire list.
6. Now that we have reversed the list, we initialize a new BinomialHeap using the constructor that receives a HeapNode as an argument, giving it firstItem, which will now be the startNode of our new BinomialHeap.
7. We now meld our new BinomialHeap with our existing (original) BinomialHeap and update the minimum, and we are done. We are left with a binomial heap that has all the same nodes except for the one node we wanted to delete.

**findMin(int value) – O(`1`)**

A public query method that returns the value (or key) of the minimal node. Since we always hold a pointer to our minNode, all we have to do is return the key of that node, using a getter from the HeapNode class.

**meld(BinomialHeap heap2) - O(`logn`)**

The meld method is the most critical method of the BinomialHeap implementation. This method is used for the operation of melding two heaps together and is utilized in both insert and delete. Almost any operation that makes any changes to our binomial heap will have meld() as its most critical component.

Meld uses two helper methods **merge()** and **link()** that are detailed below in its implementation. **merge()** takes the two root lists of the BinomialHeaps we wish to merge and returns the pointer to the head (startNode) of a new root list containing all the roots of both lists sorted by size. **link()** takes two of the same degree root, connecting them such that they form a new tree with a degree that is one higher than that of both beforehand, giving us a new root for our root list, that is one of the previous ones, which is now a root of a tree of a degree one larger than before.  Our meld takes two BinomialHeaps, andafter checking for and handling specific cases uses merge() to unite the root lists and then link to handle any discrepancies (such as two roots with the same degree). Since the method calls for melding our existing BinomialHeap, let us call it original, calling the method with heap2 and we are creating a new BinomialHeap, we then redefine original to be that new heap, which is a union of the two.

1. First we check and handle some specific cases:
  1. If heap2 is empty (checked using **empty()** method from above) then our meld has no work to do and can return, leaving our riginal BinomialHeap as is.
  2. If our riginal BinomialHeap is empty (checked once again with **empty()**) then we simply have to redefine it to be heap2 and are then done. We do so by updating all of riginal&#39;s fields to be those of heap2 and tehn return.
2. We create a new BinomialHeap varieable newHeap using the BinomialHeap constructor that takes a HeapNode as an argument giving it the node returned by **merge()** called by riginal n our heap2, meaning the starting node of the new root list.

We now have a BinomialHeap newHeap that is almost definitely an invalid BinomialHeap, as it might have two root nodes of the same degree.We update our newHeap &#39;s field and then handle it, suing predefined cases:

1. We first update the minNode field of our newHeap by checking which of the two melded heaps had a smaller min and setting our newHeap&#39;s minNode to that one. (This will work even if they have the same minNode, as then these two will be linked and the resulting root will still be the minimal node which we are still pointing to – this works in our case since our key is the same as our value – see case3 below).We update this using if statements to check which key is bigger, using getters and setters from our BInomialHeap class.
2. We Initialize a new HeapNode variable x to be the startNode of our newHeap. We will now traverse over our current root list using a while loop with condition that x.getNext() is not null, meaning x is not the last root node, fixing all discrepancies. We use four cases (based on cases given in Cormen&#39;s [Introduction to Algotrithims](http://www.mif.vu.lt/~valdas/ALGORITMAI/LITERATURA/Cormen/Cormen.pdf) second Edition):
  1. Cases 1 and 2:
    1. If x&#39;s degree is different from the degree of the root node after it (checked using x.getNext().getDegree()), **or** x has two nodes to the right of it, both of which have the same degree as it: we update x to be x.getNext().

    In Case 1, x and it&#39;s next node, are different from one another and need not be fixed, and so we move forward.

    In Case 2 x and its two partners are all of the same degree, meaning x must have been created using a previous **link()**  when fixing other nodes. In this case we also move forward since doing so will mean linking the next two nodes (x.getNext() and x.getNext().getNext()) thus changing their degree and leaving x as the only node with that degree.

    If Case 1 or 2 is not entered then we have two nodes asjacent to each other with the same degree (x and x.getNext()) . We check which node x or x.getNext() has a lower key, and then create a new binomial tree building it by connecting the two trees  that the nodes are a root of, leavin the node with the smaller key to be the root of our new tree. The new tree will have a degree that is one larger than the two nodes&#39; previous degree and will still maintain the minheap property.

  2. Case 3: Only entered if case 2 does not hold true (meaning could be passed on to from Case 1 or 2 after treatment only – in next loop). This means that x and the next node in the root list (x.getNext()) both have the same degree, and that x has a smaller (or equal to) key then x.getNext() (condition for case3)  and this must be handled.
    1. If x has a smaller (or equal to) key then x.getNext()&#39;s key:
      1. Remove x.getNext() from the root node list by updating the prev and next fields of nodes x and x.getNext().getNext(). Since we need the fields to stay as they are for link, which uses them, we simply initialize temporary HeapNode variables to save the fields and change them only after link is executed.
      2. We use **link(x.getNext(), x)** to &#39;hang&#39; x.getNext() on x, making x the root of a now one larger (degree+1) binomial tree.
      3. We reassign x &#39;s fields to the appropriate nodes (saved in temp variables in step 1) and then x.getNext()&#39;s **prev** field to be x. Note that if x&#39;s **next** field is null then the x.getNext()&#39;s **prev** field need not be taken care of (doesn&#39;t exist).
  3. Case 4 – Once again this is the case that both x and x.getNext() are of the same degree, however in this case x has a degree larger than x.getNext(). Meaning that the new binomial tree will be created by hanging x on x.getNext() and x.getNext() will be the node of the new tree that is left in the root node list of our BinomialHeap. In this case we have a specific case that we need to check that need special handling:
    1. If x is the startNode (checked using getPrev() of HeapNode class, to check it it&#39;s null) then we must set the startNode of our newHeap to be x.getNext()

  Otherwise we perform actions very similar to case 3, sort of mirroring it, removing x from the root list and &#39;hanging&#39; it on x.getNext() thuss making x.getNext() a root to a binomial tree that is of degree one larger then the two node&#39;s original degree. We also have to update x to be x.getParent() (the former x.getNext()) after link is executed, the new root in our root list, from which we continue to traverse the list.

  4. Once our while loop finishes we know that we have finished our list, since the last node cannot create a problem that has not already been fixed, since it cannot be the same degree as the next node as one does not exist.
  5. Finally we re-assign the fields of our riginal BinomialHeap to be those of our newHeap, thus replacing our riginal BinomialHeap with this new melded heap of the riginal BinomialHeap and heap2.  When updating size, we use method **sizeForHeap()**, defined above, in order to ensure that our new heap has a correct size.

Short explanation on time complexity:

The time complexity of **meld()** is O(`log n`), n being the total nodes of the new BinomialHeap that is obtained after melding the two. Say our two heaps before has logm and logk nodes, then **O(`log(n)`) = O(`log(m)+log(k)`)**, and   **`n = m+k`**.

Since our original heap had m nodes it contains at most `⌊log m⌋+1` roots as detailed above,  and by the same reasoning heap2 contains at most `⌊log k⌋+1` roots.  Our new heap then can have AT MOST `⌊log m⌋+⌊log k⌋+2 ≤ 2⌊log n⌋+2` = O(`log n`) roots at the end of our **merge()** operation (defined directly below). Since **merge()** takes O(`log n`) time, and after calling merge we simply traverse over the O(`log n`) sized root list using a while loop that performs O(`1`) operations each time (one of the four cases), the total running time is then O(`log n+ log n`) = O(`log n`).

**merge() - O(`logn`)**

A private helper method used in **meld()** that given two BinomialHeap objects (one calling the method -  let us call it original BinomialHeap) , returns a pointer to the starting node of a new root list created from uniting the root lists of the two BinomialHeaps given in such a way that the list is sorted in ascending order by degree.

1. We define temporary helper HeapNode variables, a,b,c and firstNode.
2. We check which heap has a startNode with a lower degree, assigning a to be the startNode of the BinomialHeap that has the lower startNode.getDegree(), and b to be the startNode with the larger degree.
3. We assign variable firstNode to be a, as it will be the start of our new root list, being the root with the lowest degree of all the two list&#39;s roots. (If the two startNode&#39;s have the same degree a will be assigned to be the startNode of heap2 – this is arbitrary).
  We now traverse both lists, each time adding the appropriate node (lower degree) to root list ( the one which is started by firstNode), setting it as a.getNext(), until one of the lists is finished.

4. While loop with condition that both a.getNext() and b.getNext() are not null, meaning neither list is at its last element:
  1. If a.getNext() has a smaller degree then b we simply update a, moving it one forward (a.getNext()), meaning moving forward in the root list, and re-enter our loop.
  2. Otherwise, this means that b has a smaller degree then the next node in our root list, and b must be inserted between a and a.getNext().  We do so by defining our temp variable c to be b.getNext(), this will serve us to update b later, and then insert b into our root list after a by updating all of the fields of a,b and a.getNext() using getters and setters from the HeapNode class such that b is now place after a and before a.getNext().
  3. We then update c&#39;s **prev** field to be null, as this our new first element of the to-be-inserted root list (since all elements before it have been inserted) and then re-assign b to be c, in order to continue traversing over both root lists.
5. Once our while loop exists, then we know that one of the root lists, either our updated root list traversed by a, or our to-be-inserted root list traversed by b has ended. We use helper method **handleEndOfListForMerge()** to correctly insert all remaining elements in b to our updated root list, and are then done merging our root lists.
6. return firstNode, which is the starting node of our updated and sorted root list.

**handleEndOfListForMerge(HeapNode a, HeapNode b) – O(`log n`)**

This private helper method is used in merge() to handle the case in which one of the lists being traversed while uniting the two lists has ended. The method correctly completes the merge of the lists by checking where the remaining elements should be and &#39;inserting&#39; them as a chunk into that place.

1. Define a helper HeapNode variable c.
2. If a is the list that has ended (check by checking if a.getNext() ==null) Then we need to insert all remaining elements of to-be-inserted list, traversed by b, into our updated list (traversed by a) however before doing so must check which elements should be entered before a, and which after it (larger degree).
  1. We use a while loop to traverse over all remaining elements of to-be-inserted root list, each time at the end of our loop updating b to b.getNext() using a condition for our while loop that b not be null.
    1. If a&#39;s degree is larger than b&#39;s degree (checked using getters from HeapNode class) then b must be inserted into the list before a.
      1. We do so by first setting our temp variable c to b.getNext(), in order to later update b.
      2. We check if a is the firstNode, and if not then update the **next** field of a.getPrev to be b (if a is firstNode then this need not be done).
      3.  We update all other **prev** and **next** fields accordingly such that b is inserted into the root list that a traverses before a.
      4. We update b to be c.&#39;
  2. Our while loop ends when the list traversed by b ends or when b has a degree larger than that of a, meaning it needs to be inserted after a.
  3. In this case, we simply set a&#39;s **next** field to be b, and if b is not null, set its **prev** field to be a. We are then done merging the lists.
3. In the  other case, where b is the list that has ended (meaning has one node left) then we must check where that node needs to go, insert it and we are done.
  1. We initialize our temp variable c to be a.
  2. We use a while loop to traverse our updated list, using the condition that a not be null and moving a forward (getNext()) at the end of each loop.
    1. Each time we check if b has a degree larger than a:
      1. If so we redefine c to be our current a and move a forward to a.getNExt().
      2. If not then we break our loop.
  3. At this point we know that b must be inserted right before a, meaning between c and a.
    1. To insert we first check that c.getNext() is not null, and update c.getNext()&#39;s prev field to b. If it is null then it need not be handled.
    2. We update all other **prev** and **next** fields accordingly such that b is inserted into the root list that a traverses right after c, which is our last(pre-update in while loop) non-null a that has a degree smaller than b.
4. We have thus finished merging our lists sorted by degree.

**link(HeapNode y, HeapNode z) – O(`1`)**

A helper method used in meld() to &#39;hang&#39; root node y on the other root node z, thus creating a new binomial tree of degree +1 with z as its root, that will still be part of the BinomialHeap&#39;s root list.

Since each root node has a **leftChild** field that points to the first node in its list of children, which must be the node of the largest degree which is one less than its own degree, we need to set y to be that first node in z&#39;s children list.

1. We set the parent of y to be z.
2. We set y&#39;s **next** field to be the node pointed to by z&#39;s **leftChild** field.
3. We set z&#39;s **prev** field to be **y**&#39;s **prev** field as it has now been removed from the list. (This is handled in merge in the case that z is not to the right of y).
4. If the newly updated z. **getPrev()** is not null then we must update the **next** field of the node before z to be z.
5. We set y&#39;s **prev** field to be null, as it is the first node of z&#39;s children list.
6. If z previously had a left child (meaning the  roots are not of degree 0) then we set the **prev** field of z&#39;s **leftChild** (soon to be changed) to y, thus connecting all of z&#39;s previous children list to y as its continuation.
7. We set z&#39;s **leftChild** field to be y, thus &#39;hanging&#39; y on the root z, making it its largest child – tree of degree z.(new)degree-1.
8. Lastly we update z&#39;s degree to the new degree by adding one to it.

We have thus created a new binomial tree of degree k+1 by connecting two k degree binomial tree&#39;s, &#39;hanging&#39; one on the other, thus making it the root of a k+1 binomial tree by giving it a k degreed child, along with all its other children.


**size()**

A public query method that returns the size of the BinomialHeap calling it. Since we hold a field **size** that holds this information at all times, this is simply done by returning that field.

**minTreeRank() – O(`1`)**

A public query method that returns the degree of the smallest root in the BinomialHeap calling it. Since the root with the smallest degree will always be our startNode, as the root list is built in ascending order, we simply return the degree of our startNode using the getter from HeapNode class.

**binaryRep() – O(`log n`)**

A public query method that returns an array that represents which degree trees our BinomialHeap contains. The array returned is a Boolean array, containing the value true in every index corresponding to a degree that a root (hence a tree) in our BinomialHeap has, and false for every index that does not.

1. First we initialize a Boolean array to the appropriate size. To calculate the right size we divide `log(size`) by `log(2)` to obtain the log base 2 of our size, since java&#39;s log uses base e. We round down to obtain a whole number and then add one. This is important as we need not `log(size)` indexes but rather `log(size)+1`. This is so since, roots of a binomial heap with n nodes will always ascend from degree 0 (not necessarily existing in tree) to degree `log(size)`, the largest degree in a n node tree is always `floor(log(n))`, since otherwise their could not be n nodes in the tree. (For proof see Corollary 19.2 in Cormen&#39;s [Introduction to Algotrithims](http://www.mif.vu.lt/~valdas/ALGORITMAI/LITERATURA/Cormen/Cormen.pdf) pg. 391). Our array is initialized with all indexes set to false (default).
2. We initialize a HeapNode variable node to startNode.
3. We use a while loop to traverse through our root list, with condition that node not be null, each time re-assigning node to node.getNext() at the end of our loop, thus going over the entire root list, since when the last node is reached node.getNext will reassign node to null, thus finishing our while loop.
4. Inside our while loop we simple check the degree (using getter from HeapNode class) of our root node currently assigned to node and then change our array at that index to be true. Since we traverse the entire root list, this ensures that any degree belonging to a root node in our BinomialHeap will then be set to true in our array, while all other indexes will be false.
5. return our array, which is a binary (true/false) representation of our BinomialHeap.

**arrayToHeap(int[] array) – O(`n`)**

*(n being the number of elements in the array = size of the BinomialHeap created)*

A public command method that creates a new BinomialHeap, replacing the current one if there is one, from the elements of the given array. The method first creates a new BinomialHeap giving it a HeapNode as an argument which it creates in the method line using the HeapNode constructor on the first element of the array.  The method then continues to insert all other elements of the array into that new BinomialHeap, using a for loop to traverse the list, each time calling **insert()**, see above, to create a node and insert it into the BinomialHeap with the value of the current element of the array. Finally the method re-assigns our existing&#39;s startNode, minNode and size fields to be those of the new BinomialHeap created, thus replacing our existing one with the new one created only from the elements of our given array.

**isValid() – O(`n`)**

A public Boolean query method that checks our BinomialHeap&#39;s validity according to the properties that any binomial heap must maintain.

The properties checked to ensure validity are the following:

1. **minheap property:** The key of any node is greater than or equal to the key of its parent (note that in our case the key is also the value).
2. **No two roots with the same degree:** For any (&lt;0) integer, there is no more than one binomial tree (root node) with the degree equal to that integer.

The method checks the two properties separately but in the same while loop.

1. Initializes a new HeapNode variable node to be the startNode of our BinomialHeap.
2. Checks if our BinomialHeap is empty, if so, it is by default valid, and we only need to check that it truly is empty and not just has a faulty startNode pointer (done below – see step 5) and we are done. Otherwise:
3. Initializes a int array called degrees to the same size as our binaryRep array (see explanation for size above), meaning one larger than the floor() of log2(size). Upon initialization all indexes are set to 0 (default). This will serve as a counter array, we will once again traverse our root list, checking all degrees, and incrementing any index that matches a degree of a root. If any index in our array grows to be larger than one then our BinomialHeap is in violation of the second property and is not valid.
4. We start a while loop with the condition of node not being null.  At the end of each loop we re-assign node to node.getNext() (using getter from HeapNode class), thus when node is null, we know that we have traversed over our entire root list, and hence our entire tree.
  1. We increment the element at index node.getDegree() by one.
  2. We check, using an if statement, if the newly updated element is larger than one. If so return false.
  3. We now use helper method **checkTreeForValid(),** defined below, to check our first property. Returning false if its return value is false. The method checks each sub tree of each root for validity recursively.
  4. We re-assign node to node.getNext() using getter from HeapNode class.
5. At the end we take care of the case where empty() returned true, ensuring that our BinomialHeap is indeed empty by checking that its size is 0 and that startNode  is indeed null, otherwise returning false.
6. If at no point during our method did we encounter an error and return false, then our BinomialHeap is valid and we return true.



**checkTreeForValid(HeapNode node) – O(`n`)**

A public boolean query helper method used to check each root node from our BinomialHeap for validity according to the first property (**minheap property**). The method is a recursive one, traversing down the subtree of a given root as described below. Our method is called in **isValid()** for each node in our BinomialHeap&#39;s root list.

1. First we check if our given node is a leaf, by checking if its left child is null (using getLeftChild() from HeapNode class), if so then it is a binomial tree of degree 0 and is valid by default. This is our base case. Otherwise:
2. Initialize a int variable index to be one less than our given node&#39;s degree (using getter from HeapNode calss).
3. Initialize a HeapNode variable child to be node&#39;s leftChild, meaning the beginning of it&#39;s children list, or the largest binomial tree that it contains. This node should have a degree that is one less than that of node, since ant binomial tree of degree k is built from two trees of degree k-1, and thus our node&#39;s largest child should be a tree of degree k-1 (if it has degree k).
4. We check if child&#39;s key is smaller than that of its parent (node), if so return false since this violates the fundamental minheap property.
5. We check if child&#39;s degree (using getter from HeapNode calss) is equal to index, if not return false (see explanation above).
6. We use a recursive call, calling **checkTreeForValid(child)** thus traversing down node&#39;s subtree.
7. We then decrement index, since we will now be moving on to the next node in node&#39;s children list, which should be of degree that is one less than the previous one, once again from the properties of how binomial trees, of degree k, our compromised to have exactly one child of each degree rom 0 to k-1, since they are built from two tree&#39;s of degree k-1 (proof by induction done in class).
8. After decrementing index we update child to be the node in child &#39;s **next** field, meaning the next (degree smaller by one) node in node&#39;s children list. At this point we have already checked our previous child&#39;s entire subtree using a recursive call on this method, and now proceed to do so for the newly updated
9. We continue to do this as long as child is not null, meaning we traverse throughout the entire children list of our original called node, and thus through all of its offspring (subtree).
10. If at no point during our method did we encounter an error and return false, then our BinomialHeap is valid and we return true.

**printIt() – O(`1`)**

A private query method used for debugging purposes linked to the FLAG variable that allows us to put print statements throughout the code that give us information about the process, that will only execute if FLAG is &#39;turned on&#39; (set to true).

**printHeapDetailed() – O(`n`)**

A private query method used for  presentation purposes that returns a string that contains information about all nodes in the BinomialHeap, using a while loop to traverse over the root list, each time using HeapNode&#39;s printSubTreeOfNode() method to add to the string information about the entire subtree of that node.
