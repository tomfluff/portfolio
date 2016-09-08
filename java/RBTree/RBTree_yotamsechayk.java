
/**
 * Public Static Class RBTree
 *
 * @author Yotam Sechayk
 *
 */
public class RBTree {
	public static class RBNode {
		private int key;
		private String value;
		private RBNode left;
		private RBNode right;
		private RBNode parent;
		private RBNode predecessor;
		private RBNode successor;
		private boolean isRed;

		// Constructors
		public RBNode(int key, String value) {
			this.key = key;
			this.value = value;
			this.parent = null;
			this.left = null;
			this.right = null;
			this.predecessor = null;
			this.successor = null;
			this.isRed = true;
		}

		public RBNode(int key, String value, RBNode parent) {
			this(key, value);
			this.parent = parent;
		}

		// Getters Setters:
		public int getKey() {
			return key;
		}

		public void setKey(int key) {
			this.key = key;
		}

		public String getValue() {
			return value;
		}

		public void setValue(String value) {
			this.value = value;
		}

		public RBNode getLeft() {
			return left;
		}

		public void setLeft(RBNode node) {
			this.left = node;
		}

		public RBNode getRight() {
			return right;
		}

		public void setRight(RBNode node) {
			this.right = node;
		}

		public RBNode getParent() {
			return parent;
		}

		public void setParent(RBNode node) {
			this.parent = node;
		}

		public RBNode getPredecessor() {
			return this.predecessor;
		}

		public void setPredecessor(RBNode node) {
			this.predecessor = node;
		}

		public RBNode getSuccessor() {
			return this.successor;
		}

		public void setSuccessor(RBNode node) {
			this.successor = node;
		}

		public void setToBlack() {
			this.isRed = false;
		}

		public void setToRed() {
			this.isRed = true;
		}

		// Methods

		public static boolean isRed(RBNode node) {
			if (node == null) {
				return false;
			} else {
				return node.isRed;
			}
		}

		public boolean hasChildren() {
			if (this.left != null || this.right != null) {
				return true;
			}
			return false;
		}

		public boolean hasBothChildren() {
			if (this.left != null && this.right != null) {
				return true;
			}
			return false;
		}

		public String toString() {
			if (isRed) {
				return "[" + this.key + ", R]";
			} else {
				return "[" + this.key + ", B]";
			}
		}
	}

	// Fields for RBTree:
	private RBNode root;
	private RBNode[] extremes;
	private int numOfElements;

	private static final boolean FLAG = false;
	private static final int NOT_FOUND = -1;

	// Constructors
	public RBTree() {
		this.root = new RBNode(Integer.MAX_VALUE, "", null);
		extremes = new RBNode[] { null, null };
		this.numOfElements = 0;
	}

	public RBTree(RBNode root) {
		this();
		this.root.setLeft(root);
		this.root.setParent(this.root);
		extremes = new RBNode[] { root, root };
		this.numOfElements = 1;
	}

	// Getters Setters

	/**
	 * public RBNode getRoot() returns the root of the red black tree
	 */
	public RBNode getRoot() {
		return this.root.getLeft();
	}

	public int getNumOfElements() {
		return this.numOfElements;
	}

	// Methods

	/**
	 * public boolean empty() returns true if and only if the tree is empty
	 */
	public boolean empty() {
		return (this.root.getLeft() == null);
	}

	// Sets left child of node x as node y
	private void leftChild(RBNode x, RBNode y) {
		x.setLeft(y);
		if (y != null) {
			y.setParent(x);
		}
	}

	// Sets right child for node x as node y
	private void rightChild(RBNode x, RBNode y) {
		x.setRight(y);
		if (y != null) {
			y.setParent(x);
		}
	}

	// Moves subtree y in the place of x
	private void transplantNodes(RBNode x, RBNode y) {
		if (x == this.getRoot()) {
			leftChild(this.root, y);
		} else if (x == x.getParent().getLeft()) {
			leftChild(x.getParent(), y);
		} else {
			rightChild(x.getParent(), y);
		}
	}

	// Replaces the values of node x by node y
	private void replaceNodes(RBNode x, RBNode y) {
		x.setKey(y.getKey());
		x.setValue(y.getValue());
	}

	// rotate to the left for node x
	private void leftRotation(RBNode x) {
		RBNode y = x.getRight();
		transplantNodes(x, y);
		rightChild(x, y.getLeft());
		leftChild(y, x);
	}

	// rotate to the right for node y
	private void rightRotation(RBNode x) {
		RBNode y = x.getLeft();
		transplantNodes(x, y);
		leftChild(x, y.getRight());
		rightChild(y, x);
	}

	// finds the uncle for the node x
	private RBNode findUncleForNode(RBNode x) {
		if (isLeftChild(x.getParent().getParent(), x.getParent())) {
			return x.getParent().getParent().getRight();
		}
		return x.getParent().getParent().getLeft();
	}

	/**
	 * public String search(int k) returns the value of an item with key k if it
	 * exists in the tree otherwise, returns null
	 */
	public String search(int k) {
		RBNode node = searchForNode(k);
		if (node == null) {
			return null;
		}
		return node.getValue();
	}

	public RBNode searchForNode(int k) {
		RBNode node = this.getRoot();
		while (node != null) {
			if (node.getKey() == k) {
				return node;
			} else {
				if (node.getKey() > k) {
					node = node.getLeft();
				} else {
					node = node.getRight();
				}
			}
		}
		return null;
	}

	/**
	 * public int insert(int k, String v) inserts an item with key k and value v
	 * to the red black tree. the tree must remain valid (keep its invariants).
	 * returns the number of color switches, or 0 if no color switches were
	 * necessary. returns -1 if an item with key k already exists in the tree.
	 */
	public int insert(int k, String v) {
		printIt("[INS] key " + k + ", value " + v);

		// If no root yet
		if (this.getRoot() == null) {
			this.root.setLeft(new RBNode(k, v, this.root));
			this.getRoot().setToBlack();
			printIt("> (root) [" + k + "," + v + "]");
			this.extremes[0] = this.getRoot();
			this.extremes[1] = this.getRoot();
			this.numOfElements = 1;
			return 1;
		}

		// Root exists, insert inside tree
		RBNode parent_node = findInsertionPlace(k);
		// Key already exists
		if (parent_node.getKey() == k) {
			printIt(" Key already exists");
			return NOT_FOUND;
		}
		// New key, inserting to place
		RBNode node = new RBNode(k, v, parent_node);
		if (parent_node.getKey() > k) {
			parent_node.setLeft(node);
		} else {
			parent_node.setRight(node);
		}

		this.numOfElements++;

		// Handle extremes array
		if (this.extremes[0].getKey() > node.getKey()) {
			this.extremes[0] = node;
		}
		if (this.extremes[1].getKey() < node.getKey()) {
			this.extremes[1] = node;
		}

		printIt("> (node) " + k + ", " + v + ", " + parent_node.toString());

		// Tree fixing process
		int num = fixTreeInsertion(node);

		// Handle successor and predecessor
		if (node != this.extremes[0]) {
			node.setPredecessor(findPredeccessor(node));
			node.getPredecessor().setSuccessor(node);
		}
		if (node != this.extremes[1]) {
			node.setSuccessor(findSuccessor(node));
			node.getSuccessor().setPredecessor(node);
		}

		// Return number of color switches
		return num;
	}

	/**
	 * Returns the node (parent node) that the insertion node needs to be
	 * inserted after
	 *
	 * @param k
	 *            The insertion node's KEY
	 * @return The node to insert after
	 */
	private RBNode findInsertionPlace(int k) {
		RBNode sub_node = this.getRoot();
		RBNode node;
		do {
			node = sub_node;
			if (sub_node.getKey() == k) {
				return node;
			} else if (sub_node.getKey() > k) {
				sub_node = sub_node.getLeft();
			} else {
				sub_node = sub_node.getRight();
			}
		} while (sub_node != null);

		return node;
	}

	/**
	 * fixes the tree after insertion based on the 4 different cases case1a,
	 * case1b, case 2 and case 3
	 *
	 * @param x
	 *            The node to check
	 * @return the number of color changes
	 */
	private int fixTreeInsertion(RBNode x) {
		// Root handled in insert() method

		printIt("> [FIX] (node) " + x.getKey() + ", " + x.getValue() + ", " + x.getParent());
		// Check if parent is black (nothing to do)
		if (!RBNode.isRed(x.getParent())) {
			return 0;
		} else {
			int num = 0;

			// Handle Case1a and Case1b (same handling)
			if (checkInsCase1a(x) || checkInsCase1b(x)) {
				printIt("> > [C1x] " + x.toString());
				x.getParent().setToBlack();
				findUncleForNode(x).setToBlack();
				x.getParent().getParent().setToRed();
				num += 3;
				x = x.getParent().getParent();
			} else {
				// Handle Case2 (normal)
				if (checkInsCase2N(x)) {
					printIt("> > [C2R] " + x.toString());
					leftRotation(x.getParent());
					x = x.getLeft();
				}

				// Handle Case2 (mirror)
				if (checkInsCase2M(x)) {
					printIt("> > [C2M] " + x.toString());
					rightRotation(x.getParent());
					x = x.getRight();
				}

				// Handle Case3 (normal)
				if (checkInsCase3N(x)) {
					printIt("> > [C3R] " + x.toString());
					rightRotation(x.getParent().getParent());
					x.getParent().setToBlack();
					x.getParent().getRight().setToRed();
					num += 2;
				}

				// Handle Case3 (mirror)
				if (checkInsCase3M(x)) {
					printIt("> > [C3M] " + x.toString());
					leftRotation(x.getParent().getParent());
					x.getParent().setToBlack();
					x.getParent().getLeft().setToRed();
					num += 2;
				}
			}

			// Handle red root element
			if (x == this.getRoot() && RBNode.isRed(x)) {
				x.setToBlack();
				num += 1;
				return num;
			}

			return (num + fixTreeInsertion(x));
		}
	}

	// case 1a
	private boolean checkInsCase1a(RBNode x) {
		boolean cond1 = (x.getParent().getRight() == x);
		boolean cond2 = (RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	// case 1b
	private boolean checkInsCase1b(RBNode x) {
		boolean cond1 = (x.getParent().getLeft() == x);
		boolean cond2 = (RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	// case 2 Normal
	private boolean checkInsCase2N(RBNode x) {
		boolean cond1 = (x.getParent().getRight() == x && x.getParent() == x.getParent().getParent().getLeft());
		boolean cond2 = (!RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	// case 3 Normal
	private boolean checkInsCase3N(RBNode x) {
		boolean cond1 = (x.getParent().getLeft() == x && x.getParent() == x.getParent().getParent().getLeft());
		boolean cond2 = (!RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	// case 2 Mirror
	private boolean checkInsCase2M(RBNode x) {
		boolean cond1 = (x.getParent().getLeft() == x && x.getParent() == x.getParent().getParent().getRight());
		boolean cond2 = (!RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	// case 3 Mirror
	private boolean checkInsCase3M(RBNode x) {
		boolean cond1 = (x.getParent().getRight() == x && x.getParent() == x.getParent().getParent().getRight());
		boolean cond2 = (!RBNode.isRed(findUncleForNode(x)));
		return (cond1 && cond2);
	}

	/**
	 * public int delete(int k)
	 *
	 * deletes an item with key k from the binary tree, if it is there; the tree
	 * must remain valid (keep its invariants). returns the number of color
	 * switches, or 0 if no color switches were needed. returns -1 if an item
	 * with key k was not found in the tree.
	 */
	public int delete(int k) {
		RBNode node = searchForNode(k); // the node to delete

		if (node == null) {
			return NOT_FOUND;
		}

		this.numOfElements--; // reduce the number of nodes since deleting one

		// Handling extremes
		if (node == extremes[0]) {
			printIt("> Deleting min " + node + " replacing with " + node.getSuccessor());
			extremes[0] = node.getSuccessor();
		}
		if (node == extremes[1]) {
			printIt("> Deleting max " + node + " replacing with " + node.getPredecessor());
			extremes[1] = node.getPredecessor();
		}

		int colorSwitches = delete(node);

		// Handle root is red after fixing
		if (RBNode.isRed(getRoot())) {
			getRoot().setToBlack();
			colorSwitches++;
		}

		printIt("Changed colors " + colorSwitches + " times");

		return colorSwitches;
	}

	private int delete(RBNode node) {
		printIt("[DEL] " + node);

		int colorSwitches = 0; // number of color changes
		RBNode repNode; // the node to replace the deleted one with

		if (node.hasBothChildren()) {
			// node has two children -> get successor
			repNode = node.getSuccessor();

			printIt("> Has both children, replacing and calling delete with " + repNode);

			replaceNodes(node, repNode); // replace details of node by repNode

			// Handling successor and predecessor
			node.setSuccessor(repNode.getSuccessor());
			if (repNode.getSuccessor() != null) {
				repNode.getSuccessor().setPredecessor(node);
			}

			// Check if node and repNode had different colors
			if (RBNode.isRed(node) != RBNode.isRed(repNode)) {
				colorSwitches++;
			}

			return colorSwitches + delete(repNode); // delete the replacement
													// node (after values
													// transfered)

		} else {
			// Handle predecessor and successor
			if (node.getPredecessor() != null) {
				node.getPredecessor().setSuccessor(node.getSuccessor());
			}
			if (node.getSuccessor() != null) {
				node.getSuccessor().setPredecessor(node.getPredecessor());
			}

			// Check if the node has one child (because doesn't have 2) or no
			// children
			if (node.hasChildren()) {
				// Set repNode to be the child of the node
				if (node.getLeft() != null) {
					printIt("> Has one child on the left " + node.getLeft());
					repNode = node.getLeft();
				} else {
					printIt("> Has one child on the right " + node.getRight());
					repNode = node.getRight();
				}
			} else {
				printIt("> Has no children");
				repNode = null; // because node has no children so delete it and
								// replace with null
			}

			// if node is red we replace it with repNode -> finish
			if (RBNode.isRed(node)) {
				printIt("> Node " + node + " is red, replace with black " + repNode);

				transplantNodes(node, repNode);
				return colorSwitches;
			}

			// if node is black and repNode is red, re-color repNode black and
			// replace -> finish
			if (!RBNode.isRed(node) && RBNode.isRed(repNode)) {
				printIt("> Node " + node + " is black, replace with red " + repNode);

				repNode.setToBlack();
				colorSwitches++;
				transplantNodes(node, repNode);
				return colorSwitches;
			}

			RBNode parent = node.getParent(); // gets the parent of node before
												// transplant (since can
												// transplant with null)

			// if it got to this point, node is black and repNode is black ->
			// tree needs to be fixed
			transplantNodes(node, repNode);

			// call fix tree with the repNode (after it was replaced with node,
			// the sibling and the parent
			return colorSwitches + fixTreeDeletion(repNode, findSiblingForNode(parent, repNode), parent,
					isLeftChild(parent, repNode));
		}

	}

	private RBNode findSiblingForNode(RBNode parent, RBNode node) {
		if (parent == this.root) {
			return null;
		}
		if (isLeftChild(parent, node)) {
			return parent.getRight();
		}
		return parent.getLeft();
	}

	private boolean isLeftChild(RBNode parent, RBNode node) {
		if (parent.getLeft() == node) {
			return true;
		}
		return false;
	}

	private int fixTreeDeletion(RBNode node, RBNode sibling, RBNode parent, boolean normal) {
		printIt("> [FIX] " + node + ", " + sibling + ", " + parent + ", normal: " + normal);

		int colorSwitches = 0;

		while (node != this.getRoot()) {

			// Check case A
			// Sibling is RED, parent is BLACK (must be since sibling is red) -> continue
			if (RBNode.isRed(sibling)) {
				printIt(">  > C-A: sibling " + sibling);
				parent.setToRed();
				sibling.setToBlack();
				colorSwitches += 2;
				if (normal) {
					printIt(">  >  > Normal case rotation");
					leftRotation(parent);
					sibling = parent.getRight();
				} else {
					printIt(">  >  > Mirror case rotation");
					rightRotation(parent);
					sibling = parent.getLeft();
				}
			}

			// Check case B
			// Sibling is BLACK, parent is RED, sibling's children BOTH BLACK ->
			// finish
			if (!RBNode.isRed(sibling) && RBNode.isRed(parent) && !RBNode.isRed(sibling.getLeft())
					&& !RBNode.isRed(sibling.getRight())) {
				printIt(">  > C-B: parent " + parent + ", sibling " + sibling + ", children " + sibling.getLeft() + ", "
						+ sibling.getRight());
				parent.setToBlack();
				sibling.setToRed();
				colorSwitches += 2;
				return colorSwitches;
			}

			// Checking before case C
			// Sibling is null -> continue with parent as double black node
			if (sibling == null) {
				node = parent;
				sibling = findSiblingForNode(parent.getParent(), parent);
				parent = parent.getParent();
				normal = isLeftChild(parent, node);
				continue;
			}

			// Check case C
			// Sibling is BLACK, parent is BLACK, sibling's children are BOTH
			// BLACK -> continue with parent as double black node
			if (!RBNode.isRed(sibling) && !RBNode.isRed(parent) && !RBNode.isRed(sibling.getLeft())
					&& !RBNode.isRed(sibling.getRight())) {
				printIt(">  > C-C: parent " + parent + ", sibling " + sibling + ", children " + sibling.getLeft() + ", "
						+ sibling.getRight());
				sibling.setToRed();

				// return colorSwitches + fixTreeDeletion(parent,
				// findSiblingForNode(parent.getParent(), parent),
				// parent.getParent());

				node = parent;
				sibling = findSiblingForNode(parent.getParent(), parent);
				parent = parent.getParent();
				normal = isLeftChild(parent, node);
				continue;
			}

			// Check case D
			// Sibling is BLACK, sibling has red son -> continue
			if (!RBNode.isRed(sibling) && (RBNode.isRed(sibling.getLeft()) || RBNode.isRed(sibling.getRight()))) {
				printIt(">  > C-Dx: sibling " + sibling + ", children " + sibling.getLeft() + ", "
						+ sibling.getRight());

				// if sibling is right child and has right black node
				if (!isLeftChild(parent, sibling) && !RBNode.isRed(sibling.getRight())) {
					printIt(">  > C-D1: ");
					sibling.setToRed();
					sibling.getLeft().setToBlack();
					colorSwitches += 2;
					rightRotation(sibling);

					// prepare for case D-3
					sibling = sibling.getParent();
				}

				// if sibling is left child and has left black node
				else if (isLeftChild(parent, sibling) && !RBNode.isRed(sibling.getLeft())) {
					printIt(">  > C-D2: ");
					sibling.setToRed();
					sibling.getRight().setToBlack();
					colorSwitches += 2;
					leftRotation(sibling);

					// prepare for case D-3
					sibling = sibling.getParent();
				}

				// non of the other Case D variations apply (or anyway needs to
				// do it after the above cases) -> finish
				printIt(">  > C-D3: ");

				// get sibling's red node and color it black (must be siblings'
				// right child in the normal case)
				if (normal) {
					printIt(">  >  > Normal case color change");
					sibling.getRight().setToBlack();
				} else {
					printIt(">  >  > Mirror case color change");
					sibling.getLeft().setToBlack();
				}
				colorSwitches++;

				// since sibling is black , color changes happen only if parent
				// is red
				if (RBNode.isRed(parent)) {
					sibling.setToRed();
					parent.setToBlack();
					colorSwitches += 2;
				}

				// anyway, rotate the tree towards the node
				if (normal) {
					printIt(">  >  > Normal case rotation");
					leftRotation(parent);
				} else {
					printIt(">  >  > Mirror case rotation");
					rightRotation(parent);
				}

				return colorSwitches;
			}
		}

		return colorSwitches;
	}

	private RBNode findSuccessor(RBNode x) {
		if (!x.hasChildren() && x == getRoot()) {
			return null;
		}
		if (x == extremes[1]) {
			return null;
		}
		if (x.getRight() != null) {
			return getMinInSubTree(x.getRight());
		}
		if (x == x.getParent().getLeft()) {
			return x.getParent();
		} else {
			RBNode hold = x;
			// we need to find the first left child, it's parent is the
			// successor
			while (hold != hold.getParent().getLeft()) {
				hold = hold.getParent();
			}
			if (hold.getParent() == this.root) {
				return null;
			}
			return hold.getParent();
		}
	}

	private RBNode findPredeccessor(RBNode x) {
		if (!x.hasChildren() && x == getRoot()) {
			return null;
		}
		if (x == extremes[0]) {
			return null;
		}
		if (x.getLeft() != null) {
			return getMaxInSubTree(x.getLeft());
		}
		if (x == x.getParent().getRight()) {
			return x.getParent();
		} else {
			RBNode hold = x;
			// we need to find the first right child, it's parent is the
			// predecessor.
			while (hold != hold.getParent().getRight()) {
				hold = hold.getParent();
			}
			return hold.getParent();
		}
	}

	/**
	 * public String min()
	 *
	 * Returns the value of the item with the smallest key in the tree, or null
	 * if the tree is empty
	 */
	public String min() {
		if (this.extremes[0] == null) {
			return null;
		}
		return this.extremes[0].getValue();
	}

	public RBNode checkMin() {
		RBNode elem = this.getRoot();
		return getMinInSubTree(elem);
	}

	public RBNode getMinInSubTree(RBNode node) {
		RBNode elem = node;
		while (node != null && elem.getLeft() != null) {
			elem = elem.getLeft();
		}

		return elem;
	}

	/**
	 * public String max()
	 *
	 * Returns the value of the item with the largest key in the tree, or null
	 * if the tree is empty
	 */
	public String max() {
		if (this.extremes[1] == null) {
			return null;
		}
		return this.extremes[1].getValue();
	}

	public RBNode checkMax() {
		RBNode elem = this.getRoot();
		return getMaxInSubTree(elem);
	}

	public RBNode getMaxInSubTree(RBNode node) {
		while (node != null && node.getRight() != null) {
			node = node.getRight();
		}

		return node;
	}

	/**
	 * public int[] keysToArray()
	 *
	 * Returns a sorted array which contains all keys in the tree, or an empty
	 * array if the tree is empty.
	 */
	public int[] keysToArray() {
		int[] arr = new int[this.numOfElements];
		int index = 0;
		for (RBNode node : getNodeArray()) {
			arr[index] = node.getKey();
			index++;
		}
		return arr;
	}

	/**
	 * public String[] valuesToArray()
	 *
	 * Returns an array which contains all values in the tree, sorted by their
	 * respective keys, or an empty array if the tree is empty.
	 */
	public String[] valuesToArray() {
		String[] arr = new String[this.numOfElements];
		int index = 0;
		for (RBNode node : getNodeArray()) {
			arr[index] = node.getValue();
			index++;
		}
		return arr;
	}

	private RBNode[] getNodeArray() {
		RBNode[] arr = new RBNode[this.numOfElements];
		RBNode node = this.extremes[0];
		for (int i = 0; i < arr.length; i++) {
			arr[i] = node;
			node = node.successor;
		}
		return arr;
	}

	/**
	 * public int size()
	 *
	 * Returns the number of nodes in the tree.
	 *
	 * precondition: none postcondition: none
	 */
	public int size() {
		return this.numOfElements;
	}

	// to check if parenting is correct
	public RBNode findParent(RBNode child) {
		RBNode elem = this.getRoot();
		if (child == elem) {
			return null;
		}
		while (elem.getLeft() != child && elem.getRight() != child) {
			if (child.getKey() > elem.getKey()) {
				elem = elem.getRight();
			} else {
				elem = elem.getLeft();
			}
		}
		return elem;
	}

	// print a tree
	public String toString() {
		String str = "";
		if (this.getRoot() != null) {
			RBNode node = this.getRoot();
			str += toString(node);
		} else {
			str = "Tree is empty.";
		}

		return str;
	}

	// print a subtree
	public String toString(RBNode node) {
		if (node == null) {
			return "";
		}
		String str = "";
		str = "{" + str + node.toString() + ": ";
		str += " (left) " + toString(node.getLeft());
		str += " (right) " + toString(node.getRight()) + "}";
		return str;
	}

	// printing aid using a boolean flag for debugging
	private void printIt(String str) {
		if (FLAG) {
			System.out.println(str);
		}
	}
}
