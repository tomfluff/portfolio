# Red Black Tree

The following is a chronological description of all members of our RBTree implementation. Each description begins with a short summary, and then describes the operations it performs or fields each member holds.

Throughout there are some optional prinIT() statements used for debugging purposes, that are not part of the insertion process and can be ingored.

## **RBNode**

A public, static class that implements the Node class, so that we can create node objects to put into our RBTree.

### **Fields:**

**key** – A int field that holds the key of each node, which is used to sort nodes in tree (initiliazed through constructor).

**value** – A string that holds the information of the node (initiliazed through constructor).

**left** – RBNode that is this node's left child, meaning it was inserted into the tree after it, not necessarily directly, and 'attached' to our node (this may change as an effect of deleting a node, when rotating).. Its key is smaller than this node's key. This field is updated constantly when inserting/deleting.

**right** – RBNode that is this node's right child, meaning it was inserted into the tree after it, not necessarily directly, and 'attached' to our node (this may change as an effect of deleting a node, when rotating). Its key is larger than this node's key. This field is updated constantly when inserting/deleting.

**parent** – RBNode that is this node's parent node, meaning it was inserted into the tree before our node, and our node was in essence "attached" to it (this may change as an effect of deleting a node, when rotating). This field is updated constantly when inserting/deleting.

**predecessor** – A field that points to a RBNode which is our node's predecessor in the tree, meaning the key that is closest and smaller than this' key. This field is updated when needed (every insert and delete) to ensure that each node always holds a pointer to its predecessor.

**successor** – A field that points to a RBNode which is our node's successor in the tree, meaning the key that is closest and larger than this' key. This field is updated when needed (every insert and delete) to ensure that each node always holds a pointer to its successor.

**isRed** - A Boolean field that if true means our node is red and if false mean's it's black. This field is updated constantly when inserting/deleting.

### **Constructors**

**RBNode(key, value)** – The default constructor which when given an int key and String value, initializes a RBNode object with the key and value assigned from the arguments. The constructor initializes all other fields; parent, left, right, predecessor, and successor, to null and isRed to true. Thus creating a node, that is currently unlinked to any other node, and is red, ready to be inserted into the tree.

**RBNode(key, value, parent)** – This constructor uses the default constructor to initialize a RBNode object, as described above and then assigns its parent field to be the given parent.

### **Getters and Setters**

- **getKey()** - standard getter that returns the key field of node used to call it (this).
- **setKey(int)** - standard setter that assigns the int given to the **key** field of the node calling it.
- **getValue()** - standard getter that returns the key field of node used to call it (this).
- **setValue(String)** - standard setter that assigns the String given to the **value** field of the node calling it.
- **getLeft()** - standard getter that returns the key field of node used to call it (this).
- **setLeft(RBNode)** - standard setter that assigns the RBNode object given to be the **left** field of the node calling it.
- **getRight()** - standard getter that returns the key field of node used to call it (this).
- **setRight(RBNode)** - standard setter that assigns the RBNode object given to be the **right** field of the node calling it.
- **getParent()** - standard getter that returns the key field of node used to call it (this).
- **setParent(RBNode)** - standard setter that assigns the RBNode object given to be the **parent** field of the node calling it.
- **getPredecessor()** - standard getter that returns the key field of node used to call it (this).
- **setPredecessor(RBNode)** - standard setter that assigns the RBNode object given to be the **predecessor** of the node calling it.
- **getSuccessor()** - standard getter that returns the key field of node used to call it (this).
- **setSuccessor(RBNode)** - standard setter that assigns the RBNode object given to be the **successor** of the node calling it.

- **setToBlack()** - custom setter that assigns false to the **isRed** field of the node calling it.

- **setToRed()** - custom setter that assigns true to the **isRed** field of the node calling it.

**Methods**

**isRed() – O(`1`)** - custom static query method that returns false if node is null (meaning a leaf), otherwise returns the value of the boolean field isRed. Built to handle the case that a node is null, which should be treated as a black node.

**hasChildren()– O(`1`)** - Boolean query that checks if a node has children by checking if either node.left or node.right are not null but rather actual RBNode objects.

**hasBothChildren()– O(`1`)** - Boolean query that checks if a node has two children(both left and right) by checking that both node.left and node.right are actual RBNode objects, and not null.

**toString()– O(`1`)** - overrides the toString method for the RBNode class. Returns a string that inside brackets has the key, the value and the color of the node.

Example: for a red node with key=1, value="abc" the return would be: [1, abc, R]

## **RBTree**

A public, class that implements the Red Black Tree, letting us initialize a tree, insert RBNode objects into it, search through, and delete them. As well as other queries to get and present information from the tree.

### **Fields:**

**root** - Points to the root RBNode that is the root of our tree.

**extremes** - An array point to two RBNodes, the one in index 0 being the minimum of the tree while the one in index 1 is the maximum. Updated at every insertion or deletion.

**numOfElements** - updated at every insertion or deletion, this field always keeps track of the number of valid elements in the tree at any given point.

### **Variables:**

**FLAG** - A Boolean variable that can be set to true in order to print out information about the tree in order to execute checks and debugging.

**NOT_FOUND** - A variable defining the value of a requested node not being found when calling delete, or already existing in the tree when calling insert.

### **Constructors:**

**RBTree()** - the default constructor that initializes an empty tree, meaning the root is initialized as an abstract, infinite value root that is not an actual part of the tree, the extremes are both null and there are no elements in the tree.

**RBTree(RBNode)** - this constructor given a node initializes an empty tree and then sets the RBNode given to be the infinite node's left child, this is our tree's actual root. Both of the tree's extremes are also set to be the RBNode given, since it is the only element in the tree, it is both min and max. So in essence it initializes a single element tree with the given RBNode as its root.

### **Getters and Setters:**

- **getRoot()** - custom getter that accesses the infinite root used to initialize the tree, and then returns its left child which is the actual root of the tree.
- **getNumOfElements** - standard getter that returns the int of the **NumOfElements** field.

### **Methods:**

**empty() – O(`1`)**

The boolean method, when called for with an object from class RBTree returns true if there are no active nodes in the tree and false if there is at least a root in the tree. It checks this by checking if the left child of our infinite root is null or an actual RBNode. If it is null then it returns true since this means that there are no RBNode objects in our tree, otherwise the tree has at least one node, the root, and it returns false.

**leftChild(RBNode x, RBNode y) – O(`1`)**

A command method that given two nodes sets RBNode y to be the left child node of RBNode x. The method first uses the setter to set x's **left** field to be y. It then checks to make sure that y is not null (since a null node cannot have a parent as it is not an RBNode object) and if not, then it sets y's parent field to be assigned x using the setter from RBNode class.

**rightChild(RBNode x, RBNode y) – O(`1`)**

A command method that given two nodes sets RBNode y to be the right child node of RBNode x. The method first uses the setter to set x's **right** field to be y. It then checks to make sure that y is not null (since a null node cannot have a parent as it is not an RBNode object) and if not ,then it sets y's parent field to be assigned x using the setter from RBNode class.

**transplantNodes(RBNode x, RBNode y) – O(`1`)**

A command method that given two RBNode ojects from our tree, transplants the subtree starting at y (subtree of root y) into the place currently occupied by x.

If x is the root then the process is easy, simply redefining the root, using helper method leftChild() above, which in essence removes x from the tree altogether.

If x is not the root then the method checks if it is a right or left child and then uses leftChild or RightChild with x's parent accordingly to set y instead of x, once again removing x from the tree.

**replaceNodes(RBNode x, RBNode y) – O(`1`)**

A command method used to replace one node with another.

The method uses RBNode setter methods to re-assign the value and key of node x to those of node y.

**leftRotation(RBNode x) – O(`1`)**

This command method implements the left rotation of nodes called for when fixing the tree after both insert and delete in certain cases.

The method initializes a RBNode variable to be x's right child and then uses transplant to replace x with the subtree of root y, then sets x to be y's left child using the helper method, leftChild, above, thus in essence rotating the nodes around x all to the left.

**rightRotation(RBNode x) – O(`1`)**

This command method implements the left rotation of nodes called for when fixing the tree after both insert and delete in certain cases.

The method initializes a RBNode variable to be x's left child and then uses transplant to replace x with the subtree of root y, then sets x to be y's right child using the helper method, rightChild, above, thus in essence rotating the nodes around x all to the right.

**findUncleForNode(RBNode x) – O(`1`)**

This query method is used to find the 'uncle' of the given node, meaning its parent's sibling.

Depending on whether x's parent is a left or right child, checked using isLEftChild helper method defined below, it uses setter methods of the RBNode class to get the parent's sibling.

**search(int k) – O(`log n`)**

This query method , given a key of a node returns its value (String), or null if the given key does not match any node in the tree.

The method searches for the node of the given key using helper method searchForNode defined below and then if the node returned by that method is not null, it returns its value.

**searchForNode(int k) – O(`log n`)**

This query method, given a key to search for, returns the RBNode object in the tree with that key. If no such node exists it returns null.

The method uses a while loop to go through the tree, using binary search to find the matching node in the tree. In the while loop, if the node checked has the correct key, it is returned, else we move either left or right depending on if the key is smaller or larger than that of the current node. If we reach a null node (leaf) without finding a matching key, null is returned.

**insert(k,v) – O(`log n`)**

The method is given a key and corresponding value and creates a node object to match, which it then inserts into the tree, fixing it accordingly so that it remains balanced. The method 'counts' and returns the amount of color switches that were necessary to rebalance the tree following the insertion of the new node, returning that number at the end. If the given key belongs to a node that already exists in the tree then the method returns -1 and does not create a new node nor insert it into the tree.

First we check if the tree is empty, if so we must insert a root node.

1. We check using getRoot() if the root of our tree is null:

  1. We assign the left child of our infinite 'root' to be a new RBNode which we initialize with the given (k,v)
  2. Set root to black using setToBlack from RBNode class.
  3. Set both indexes of our tree's **extremes** field to be the node we have just made the root of our tree (It is currently both max and min element).
  4. We set our **numOfElements** field to be 1.
  5. Return 1, since one color switch has taken place.

  If not, and the root exists we must find the proper insertion place, and insert it into the tree, but before we do so, we must check that a node with the same key does not already exist in the tree.

2. We initialize a new RBNode called parent, and use helper method findInsertionPlace, defined below, to find it.

3. If parent's key matches the given key then a node with this key already exists and we return NOT_FOUND value (-1).
4. Otherwise (key does not exist in tree):

  1. We initialize a new RBNode with our given (k,v, parent) called node. (This uses our custom constructor, initializing a node whose **parent** field is already defined).
  2. If parent's key is larger than k, we set parent's **left** field to be node.
  3. Otherwise: we set parent's **right** field to be node.

  Now after insertion, we must re-balance our tree, using fixTreeInsertion() helper method defined below, however before doing so we must handle our tree's fields.

5. We increment **numOfElements** field of our tree (+1).

6. Now we handle our **extremes** field:

  1. We check if the inserted node's key is smaller than that of the node in extremes index 0, if so then we re-assign it. Such that extremes[0] now points to our newly inserted node.
  2. We check if the inserted node's key is larger than that of the node in extremes index 1, if so then we re-assign it. Such that extremes[1] now points to our newly inserted node.

7. We define a int num to equal the value returned by fixTreeInsertion(node).

  After re-balancing we must also handle the **successor** and **predecessor** fields of both our new node and its new successor and predecessor.

8. If node is not the new minimum (meaning has no predecessor):

  1. Sets predecessor using findPredecessor method (defined below).
  2. Sets its predecessor's **successor** field to be the node itself.

9. If node is not the new maximum (meaning has no successor):

  1. Sets sucessor using findSuccessor method (defined below).
  2. Sets its successor's **predecessor** field to be the node itself.

Returns the number of color switches (num).

**findInsertionPlace(int k) – O(`log n`)**

Before inserting a node, we must find the appropriate place to insert it that will maintain the binary search properties of our tree. This query method searches through the tree, using binary search, to find that place.

We define a temporary variable called sub_node and initialize it to be the root of the tree using getRoot(). We use a do-while loop, that while sub_node is not null (meaning reaches a leaf):

1. Defines node to equal sub_node
2. Checks if the sub_node's key matches the given key, if so we return it, if not then we continue down the tree, re-defining sub_node left or right depending if its key is larger or smaller than k.
3. When we reach a leaf, meaning our sub_node was defined to null in the last while loop and will not re-enter the loop, our node will still be defined to the previous value of sub_node, meaning our leaf's parent, which is the location of insertion and is now returned.

*If the key exists in the tree then the node with that key is returned.

**fixTreeInsertion(RBNode x) – O(`log n`)**

A recursive command method that re-balances our tree after insertion.

We use cases, checked using helper methods defined below, each of which has a certain matching fix, to rebalance our tree.

Since any insertion is that of a red node, we first check if the node has been attached to a black node (its parent is black), if so no fix is required.

1. We use isRed from the RBNode class to check if x's parent is black, if so we return 0.
2. If not we initialize a variable int num = 0.

  Check if we have either case 1a or 1b, both of which are handled the same and if so handle them, setting x's parent and uncle to black and its grandfather to red, then re-assigns x to continue treatment.

3. If either the checkInsCase1a/1b() return true:

  1. We use setToBlack on both x.getParent and findUncleNode(x) (hepler methods – setToBlack from RBNode class, and findUncleNode one defined above)

  2. We use RBNode class' setToRed to set x's grandfather to red.

  3. We increment num by 3 (3 color switches have been made).
  4. We redefine x to be its grandfather using getters, and make an additional recursive call.

  Otherwise if neither case 1a, or 1b is true: We check which case, 2 or 3 (normal or mirror) we are in and fix it.

4. If the checkInsCase2N() returns true:

  1. We use helper method leftRotation() using x's parent, thus rotating left around x's parent.
  2. We re-assign x to be its left child.

5. If the checkInsCase2M() returns true:

  1. We use helper method rightRotation() using x's parent, thus rotating left around x's parent.
  2. We re-assign x to be its right child.

6. If the checkInsCase3N() returns true:

  1. We use helper method rightRotation() using x's grandfather, thus rotating right around x's parent's parent.
  2. We use helper method from RBNode class setToBlack to set x's parent to be black.
  3. We use helper method from RBNode class setToRed to set x's parent's right child, meaning x's sibling, to be a red node.
  4. We increment num by 2 (2 color switches have been made).

7. If the checkInsCase3M() returns true:

  1. We use helper method leftRotation() using x's grandfather, thus rotating left around x's parent's parent.
  2. We use helper method from RBNode class setToBlack to set x's parent to be black.
  3. We use helper method from RBNode class setToRed to set x's parent's left child, meaning x's sibling, to be a red node.
  4. We increment num by 2 (2 color switches have been made).

  The last thing we need to do is handle the case that we have rolled up such that we now have a red root, this is easily fixed, as changing the root's color will not upset the balance of the tree.

8. If x is the root and is red (Checked using RBNode.isRed to avoid NullPointerException)

  1. We set x to black
  2. We increment num
  3. Return num (this means we are done with our rebalance)

9. As long as x is not our root we continue making recursive calls returning our current num added to value returned by fixTreeInsertion() with our current x.

_The following methods are all boolean query methods meant to verify which case of insertion is applicable in order to apply the corresponding fix._

In each case the relevant conditions are defined as boolean variables and then all conditions are returned in a `&&` statement such that only if all conditions are met then the general statement returns true, otherwise it returns false.

**checkInsCase1a(RBNode x) – O(`1`)**

Checks if the node is a right child whose 'uncle' node is red.

First condition - checks if x is its parent's right child using getter methods from RBNode class.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class.

**checkInsCase1b(RBNode x) – O(`1`)**

Checks if the node is a left child whose 'uncle' node is red. (1a's mirror case)

First condition - checks if x is its parent's left child using getter methods from RBNode class.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class.

**checkInsCase2N(RBNode x) – O(`1`)**

Checks if x is a right child as well as its parent being a left child, and its uncle being black.

First condition - checks if x is its parent's right child using getter methods from RBNode class, and if its parent is a left child by checking if its **parent** field matches the left child of x's grandfather node using getters.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class, checking that it returns false.

**checkInsCase3N (RBNode) – O(`1`)**

Checks if x is a right child as well as its parent being a left child, and its uncle being black.

First condition - checks if x is its parent's right child using getter methods from RBNode class, and if its parent is a left child by checking if its **parent** field matches the left child of x's grandfather node using getters.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class, checking that it returns false.

**checkInsCase2M(RBNode) – O(`1`)**

Checks if x is a left child as well as its parent being a right child, and its uncle being black.

First condition - checks if x is its parent's left child using getter methods from RBNode class, and if its parent is a right child by checking if its **parent** field matches the right child of x's grandfather node using getters.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class, checking that it returns false.

**checkInsCase3M(RBNode) – O(`1`)**

Checks if x is a right child as well as its parent being a right child, and its uncle being black.

First condition - checks if x is its parent's right child using getter methods from RBNode class, and if its parent is a right child by checking if its **parent** field matches the right child of x's grandfather node using getters.

Second condition – finds x's uncle using the findUncleForNode method and checks if x's uncle is red using the isRed method from RBNode class, checking that it returns false.

**delete(int k)**

This command method, is given an int, and if that int matches a key existing in the RBNode tree, using helper methods deletes it from the tree and then re-balances the tree. The method holds an int counter that measures how many color switches are executed during the deletion and rebalancing, returning that number. If the tree does not contain a node whose key matches the int given, then the NOT_FOUND (-1) value is returned.

The first thing we do is use helper method searchForNode to assure if a node with matching key exists in the tree.

1. We define a RBNode variable called node to be the result of searchForNode() with our given k.
2. If node is null:

  1. Return NOT_FOUND value.

  Othwerwise: A node exists in our tree that matches the int given and we will now proceed to remove it:

3. We decrement our tree's **numOfElements** field by one, as we are removing a node.

  Next we wish to handle the case in which we are deleting one of our extremes:

4. If our node is the 0 index in our extremes array:

  1. Re-assign extremes[0] to point to node's **successor** field, which will now be the minimal element since it is closest in size to the deleted node's key.

5. If our node is the 1 index in our extremes array:

  1. Re-assign extremes[1] to point to node's **predecessor** field, which will now be the maximal element since it is closest in size to the deleted node's key.

  Now we use a helper method delete(node) to remove our node from the tree, and rebalance it.

6. We define an int variable called colorSwitches to be the return value of delete(node) which removes the node from the tree and rebalances it, measuring color switches along the way.

  Following step 6 our node is removed, and our tree is rebalanced, except for perhaps the root.

  The last thing we need to do is handle the case that we have rolled up such that we now have a red root, this is easily fixed, as changing the root's color will not upset the balance of the tree.

7. If x is the root and is red (Checked using RBNode.isRed to avoid NullPointerException)

  1. We set x to black
  2. We increment colorSwitches

In our final step we return colorSwitches.

**delete(RBNode node) – O(`log n`)**

A command method that given a node removes it from the tree, fixing the connextions so that no subtree is left disconnected upon removal. The method initializes a int counter colorSwitches that measures the amount of color switches executed and returns that number.

First of all we initialize a int variable called colorSwitches.

We then initialize a RBNode variable called repNode that we will use to replace our deleted node, we will assign repNode below according to cases.

First case, if our given node has two children.

In this case we will use node's successor to replace it, since we know that this will be legal in the sense that all of node's right offspring (except perhaps successor itself) will be larger than successor and all of its left offspring will be smaller.

1. We check if our node has two children using hasBothChildren helper method if so:

  1. We re-assign repNode to be the given (arg) node's successor.
  2. We use the helper method replaceNodes() to replace our node with repNode, what we are actually doing is transferring repNode's info into node and then removing repNode from the tree leaving node in place, but 'turning it into' repNode.
  3. Now we handle the successor and predecessor fields:

    1. We set node's successor to be that of repNode's.
    2. If repNode has a successor (isn't null) then we set its (the successor's) predecessor to be node, thus updating both fields.

  4. We check if repNode and node had different colors, and if so increment coloSwitches by one since in our replacement we would have then in essence changed repNode's color to that of node.
  5. We now return colorSwitches + the value of delete(RBNode) called for our repNode, removing it from the tree.

2. Otherwise: Our node has only one or no children nodes. In this case we will be removing node from the tree, 'connecting' between its parent and repNode instead.

  1. First of all we handle our successor and predecessor fields.

    1. If node has a predecessor (isn't null) then we set its (the predecessor's) successor to be node's successor.
    2. If node has a successor (isn't null) then we set its (the successor's) predecessor to be node's predecessor.

    Now we have two cases, if node has a single child or is a leaf.

  2. We check if node has a single child using hasChildren() helper method and then set repNode to be node's child.

    We check using a getter method if node's **left** field is null, if not then repNode is set to node.left, otherwise to node.right (since we have already asserted that it has exactly one child).

  3. If node has no children then repNode is set to null.

  4. If our node is red, then we simply 'cut' it out of the tree connecting its parent with repNode and we need not rebalance the tree, since removing a red node from a certain level will not throw off the balance (black depth).

    1. We check using RBNode class' isRed if our node is red, and if so:
    2. Use helper method transplantNodes to 'cut' out node and connect its parent to repNode, which will either be its (only) child or a null node ( meaning our deleted node was a leaf)
    3. We return colorSwitches.

3. If our node is black, but our repNode, with which we are now in essence replace node's place is red, we can easily fix the balance of the tree by turning repNode black and also do not need to rebalance our tree.

  1. We check using RBNode class' isRed if our node is black as well as repNode being red, and if so:
  2. We set repNode to black using setToBlack().
  3. We increment colorSwitches.
  4. Use helper method transplantNodes to 'cut' out node and connect its parent to repNode, which will either be its (only) child or a null node ( meaning our deleted node was a leaf)
  5. We return colorSwitches.

4. If neither d nor e hold true then we will have to rebalance our tree using fixTreeDeletion() helper method.

  1. Before doing so we still remove our node, 'replacing' it with repNode but before we use transplantNodes() we first 'save' node's parent field in a RBNode variable we initialize called parent, that holds our deleted node's parent. We will need this for fixing the tree.
  2. Use helper method transplantNodes to 'cut' out node and connect its parent to repNode, which will either be its (only) child or a null node ( meaning our deleted node was a leaf)

5. Now we must rebalance our tree:

  1. Return colorSwitches + the returned value of fixTreeDeletion(), fixTreeDeletion is given : repNode, which has now taken our deleted node's place in the tree. the sibling of our repNode, found using findSiblingForNode helper method . parent, which matches repNode's **parent** field. And a Boolean variable returned from isLeftChild() helper method which tells us if the normal or mirror case will apply to us.

**findSiblingForNode(RBNode parent, RBNode node) – O(`1`)**

Query method that given a node and its parent, find's the node's sibling.

(Parent is given to avoid NullPointerExceeption in the case that our node is the root.)

First of all checks if the parent is our inifinite root, meaning the node is the root of the tree, if so returns null.

Checks if node isLeftChild (method defined below) and if so returns the right child of the parent, otherwise (if loop not entered) returns the left child.

**isLeftChild(RBNode, RBNode)**

Query method that given a node and its parent, checks if the node is a left child.

Uses a getter method on parent to get its left child and checks if it's the given node, if so returns true, otherwise false.

**fixTreeDeletion(RBNode, RBNode, RBNode) – O(`log n`)**

This command method rebalances a tree after the deletion of a certain element. . The method initializes a int counter colorSwitches that measures the amount of color switches executed and returns that number.

Our method uses a while loop that continues to 'roll' up the tree as long as node is not the root of the tree, when node is the root of the tree then we know we are done rebalancing and return colorSwitches.

1. We initialize an int variable called switchColors in order to measure how many color changes we make.

  Now we check which case is applicable, fixing each accordingly. We use a while loop that continues to run while node is NOT equal to root (checked with getRoot())

2. Case 1: Checks using RBNode class' isRed() if node's sibling is red.

  1. We set node's parent node to red using setToRed helper method from RBNode class.
  (The parent must have been black since it has a red child – node's sibling)
  2. We set sibling to black using setToBlack helper method from RBNode class.
  3. We increment colorSwitches by 2
  4. If normal case ( meaning if the Boolean parameter given (isLeft) was true):
    1. We use leftRotation() helper method on parent, rotating all our elements to the left around node.
    2. We set sibling to be parent's new right node, (using getter on parent).
  5. If mirror case ( meaning if the Boolean parameter given (isLeft) was false):
    1. We use rightRotation() helper method on parent, rotating all our elements to the right around node.
    2. We set sibling to be parent's new left node, (using getter on parent).

3. Case 2a: Checks using RBNode class' isRed() to assure that parent is red while sibling is black, and both its children are black.
  1. We use setToBlack to make parent black.
  2. We use setToRed to make sibling red.
  3. We increment colorSwitches by 2
  4. We return colorSwitches

  Since in this case we have finished rebalancing our tree, fixing the black depth on our sibling's side by switching it's color with parent's (which was red)

4. Before moving on, we must ensure that sibling is not null, if it is then case 2 should have been called to fix it, but could not have and so our tree still needs fixing. TO prepare for the continuation we must roll up our problem – our parent now will be our node and it will 'carry double balck weight'.

  1. If sibling si null:
    1. node is re-assigned to be parent
    2. sibling is re-assigned to be praent's sibling using findSinlingForNode() helper method.
    3. parent is re-assigned to be the node in parent's **parent** field.
    4. normal, the Boolean variable in charge of telling us if we are in normal or mirror case is re-assigned using isLeftChild() helper method.

5. Case 2b: Checks using RBNode class' isRed() to assure that parent is black, sibling is black, and both its children are black.

  1. We use setToRed to make sibling red. In this case we continue to 'roll up' with our parent now 'carrying' double black weight.
  2. node is re-assigned to be parent
  3. sibling is re-assigned to be praent's sibling using findSinlingForNode() helper method.
  4. parent is re-assigned to be the node in parent's **parent** field.
  5. normal, the Boolean variable in charge of telling us if we are in normal or mirror case is re-assigned using isLeftChild() helper method.

  (The same checks performed after 2a)

6. Cases 3 and 4: Checks using RBNode class' isRed() to assure that the sibling is black but has a red son.

  1. Case 3: Checks using isLeftChild() helper method and RBNode class' isRed() if sibling is a right child and has a right black node, (meaning left child is red)

    1. Using setToRed() turns sibling into a red node.
    2. Using setToBlack() turns sibling's left child into a black node.
    3. Increments color switches by 2.
    4. Uses rightRotation on sibling to rotate the nodes into their new positon.
    5. We re-assign sibling to be sibling's parent (the node from sibling's **parent** field, using a getter).

  2. Case 3 Mirror: Checks using isLeftChild() helper method and RBNode class' isRed() if sibling is a left child and has a left black node, (meaning right child is red)

    1. Using setToRed() turns sibling into a red node.
    2. Using setToBlack() turns sibling's left child into a black node.
    3. Increments color switches by 2.
    4. Uses leftRotation on sibling to rotate the nodes into their new positon.
    5. We re-assign sibling to be sibling's parent (the node from sibling's **parent** field, using a getter).
    
  3. Case 4: We reach case 4 after any version of case 3 (and some versions of case 1) in any case, sibling's right or left child will be red (normal or mirror).

  If normal is true:
    1. We turn sibling's right child to black using setToBlack().

  Otherwise we have a mirror case and:

    2. We turn sibling's right child to black using setToBlack().

  Either way after doing so:
  
    3. Increment colorSwitches

  We know check to see ifour parent node isRed, in which case we set it to black using RBNode class' setToBlack and then set sibling to red using setToRed. In this case we must increment our color switches by 2.

  To finish treatment we must rotate the tree left or right (normal or mirror) and then we will have completed our rebalancing, restoring the black depth to be equal at all nodes.

  If normal is true: use leftRotation() helper method on parent.

  Otherwise: use rightRotation() helper method on parent.

  Return colorSwitches.

The method returns colorswitches at the end, in any case when the while loop is finished.

**findSuccessor(RBNode x) – O(`log n`)**

Query method that given a node, finds its successor in the tree, meaning the node with the key closest to its key and size and larger than it.

Before we explain how the code works let us examine the algorithm for finding a successor in any tree with binary search properties:

If x has a right subtree: According to the properties of binary search trees, the next larger key must be in the right subtree. All keys in a right subtree are larger than the key of x, and so the successor will be minimal key in this subtree, making it the closest in size to x's key.

Else:

If x has no right child then its successor will be above it on the tree, since any left offspring will have smaller keys than that of x. Looking up the tree we check if x is a left child of its parent, if so then its parent is its successor, however if it is a right child then we must continue to check upwards, since if it is a right child its key is larger than that of its parent, and so we continue to check upwards until we find the first left child, whose parent will then be x's successor as x is part of its left offspring, we know that its key will be larger than that of x.

Our method works as follows:

1. Checks if x is alone in the tree, meaning a leaf and the root using NOT hasChildren() and checking if it equals getRoot(). If so returns null, since there is no successor.
2. If x is equal to our tree's extremes array in index 1 (this.extremes[1]), it is the maximal element and has no successor in the tree, and so we return null.
3. If x has a right subtree (even a single right child), uses our getMinInSubTree() method to find the minimal node in that tree and returns it.
4. Defines a temporary node hold, initializing it as x, and uses a while loop to roll up the tree each time redefining hold as it's parent until a left child is found, then returns its parent.

If during the while loop we reach the root without finding a left child then we know that there is no successor in the tree for our node x and return null.

**findPredecessor(RBNode) – O(`log n`)**

Query method that given a node, finds its predecessor in the tree, meaning the node with the key closest to its key and size and smaller than it.

Before we explain how the code works let us examine the algorithm for finding a predecessor in any tree with binary search properties (similar but opposite to successor):

If x has a left subtree: According to the properties of binary search trees, the next smaller key must be in the left subtree. All keys in a left subtree are smaller than the key of x, and so the predecessor will be maximal key in this subtree, making it the closest in size to x's key.

Else:

If x has no left child then its successor will be above it on the tree, since any right offspring will have larger keys than that of x. Looking up the tree we check if x is a right child of its parent, if so then its parent is its predecessor, however if it is a left child then we must continue to check upwards. Since if it is a left child its key is smaller than that of its parent, and so we continue to check upwards until we find the first right child, whose parent will then be x's predecessor as x is part of its right offspring, we know that its key will be smaller than that of x.

Our method works as follows (similar but opposite to Successor):

1. Checks if x is alone in the tree, meaning a leaf and the root using NOT hasChildren() and checking if it equals getRoot(). If so returns null, since there is no predecessor.
2. If x is equal to our tree's extremes array in index 0 (this.extremes[0]), it is the minimal element and has no predecessor in the tree, and so we return null.
3. If x has a left subtree (even a single left child), uses our getMaxInSubTree() method to find the maximal node in that tree and returns it.
4. Defines a temporary node hold, initializing it as x, and uses a while loop to roll up the tree each time redefining hold as its parent until a right child is found, then returns its parent.

If during the while loop we reach the root without finding a right child then we know that there is no predecessor in the tree for our node x and return null.

**min() – O(`1`)**

Query method that returns the value of the node with the minimal key of the tree.

First checks if index 0 of our extremes array of our tree is null, if not returns its value. If so, returns null.

**checkMin()– O(`log n`)**

Query method that returns the minimal node of the tree, used for checks and debugging.

The method defines a temporary RBNode variable as the root of the tree and then uses getMinSubTree(), defined below, on the root, thus finding our tree's minimal node and returning it.

**getMinInSubTree(RBNode node) – O(`log n`)**

Query method, that given a RBNode, returns the node with the minimal key from all of its offspring.

Defines a temporary RBNOde variable **elem** , initializing it as node, and then uses a while loop to find the leftmost node of the tree. This will always be the minimal node of any BST (or subtree).

Our while loop's conditions are that node is NOT null and **elem** has a left child, and if entered it re-assigns elem to be its own left child. When elem has no left child (meaning it is the leftmost node) the method returns elem.

**max() – O(`1`)**

Query method that returns the value of the node with the maximal key of the tree.

First checks if index 1 of our extremes array of our tree is null, if not returns its value. If so, returns null.

**checkMax() – O(`log n`)**

Query method that returns the maximal node of the tree, used for checks and debugging.

The method defines a temporary RBNode variable as the root of the tree and then uses getMaxSubTree(), defined below, on the root, thus finding our tree's maximal node and returning it.

**getMaxInSubTree(RBNode) – O(`log n`)**

Query method, that given a RBNode, returns the node with the maximal key from all of its offspring.

Defines a temporary RBNode variable **elem** , initializing it as node, and then uses a while loop to find the rightmost node of the tree. This will always be the largest node of any BST (or subtree).

Our while loop's conditions are that node is NOT null and **elem** has a right child, and if entered it re-assigns elem to be its own right child. When elem has no right child (meaning it is the rightmost node) the method returns elem.

**keysToArray()– O(`n`)**

Query method that returns an array containing all of the keys in our tree, sorted in ascending order.

1. We initialize an int array (result array) to the size of our tree's **numOfElements** field.
2. We use getNodeArray (defined below) to build an array of all of our keys.
3. Using a temp variable **index** =0, we use a for loop, and for all nodes in our array (see 2), we put that node's key in the corresponding **index** in our result array, then increment **index**.
4. Return the result array

\* If the tree is empty, an empty array will be returned.

**valuesToArray() – O(`n`)**

Query method that returns an array containing all of the values in our tree, sorted in ascending order by their keys.

1. We initialize an int array (result array) to the size of our tree's **numOfElements** field.
2. We use getNodeArray (defined below) to build an array of all of our keys.
3. Using a temp variable **index** =0, we use a for loop, and for all nodes in our array (see 2), we put that node's value in the corresponding **index** in our result array, then increment **index**.
4. Return the result array.

\* If the tree is empty, an empty array will be returned.

**getNodeArray() – O(`n`)**

Query method that builds and returns an array containing all of the nodes in our tree, sorted in ascending order by their keys, used as a helper function in both valuesToArray and keysToArray.

1. We initialize an int array (result array) to the size of our tree's **numOfElements** field.
2. We define a temporary RBNode variable to be the node at our tree's elements array in index 0, meaning our minimal element.
3. We use a for loop from 0 to our array length, that each time adds the temp node to our result array at index i, and then re-assigns our temp node to be its successor. (This can happen in O(`1`) since each node holds a field with its successor at all times)

4. Return result array.

**size() – O(`1`)**

Query method that returns our tree's **numOfElements** field.

**findParent(RBNode child) – O(`log n`)**

Query method used if necessary to check if x's parent field is correct.

1. Defines elem as the root of the tree.
2. If our child is equal to elem (the root) then returns null since x has no parent.
3. Uses a while loop to roll down the tree, each time checking if elem is a parent of x, and if re-assigning elem to be its right or left child depending on wheter child's key is smaller or larger than its key.
4. Once one of elem's "child" fields (elem.left or elem.right - checked with getters) is equal to child we do not re-enter our while loop and return elem.

**toString() – O(`n`)**

Query method that overrides the toString method, defining it for RBTree.

1. Initializes an empty result string.
2. If the root of our tree is not null, (meaning our tree is not empty):

  1. Defines temp RBNode variable node as the root of the tree.
  2. Reassigns the result string, adding all our nodes to it using toString(RBNode) defined directly below).

3. If the the tree is empty, assigns our result string to be: "Tree is empty"
4. Returns the finished string.

**toString(RBNode node) – O(`n`)**

A recursive query method that given an RBNode prints its entire subtree (meaning subtree that has it as a root).

1. Initializes an empty result string.
2. If node is null, meaning our subtree is empty, returns an empty string.
3. Re-assigns result string to have "{ " then the node itself, then ":"

  1. Adds: "(left)" and calls toString again with node's left child.
  2. Adds: "(right)" and calls toString again with node's right child.

4. Returns the finished string.

The recursion will continue on each subtree (first left then right) until a leaf is reached.

**printIt(String)**

A method that if FLAG is true, prints its given arguments, used for debugging and retrieving information about different processes throughout the program.

If FLAG is true, uses System.out.println() to print argument.
