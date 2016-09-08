/**
 * BinomialHeap
 *
 * An implementation of binomial heap over non-negative integers.
 * @author Yotam Sechayk
 *
 *
 */
public class BinomialHeap {

	private static final boolean flag = false;

	/**
	 * public class HeapNode
	 * Our Binomial Heap is compromised of Binomial Trees of HeapNode Objects.
	 *
	 */
	public class HeapNode {

		//Fields of HeapNode Class
		private int key;
		private int degree; // number of children root has
		private HeapNode next;
		private HeapNode prev;
		private HeapNode parent;
		private HeapNode leftChild;

		//Constructor of HeapNode Class
		public HeapNode(int value) {
			this.key = value;
			this.degree = 0;
			this.next = null;
			this.prev = null;
			this.parent = null;
			this.leftChild = null;
		}

		//Getters and Setters of HeapNode Class
		public int getKey() {
			return key;
		}

		public void setKey(int key) {
			this.key = key;
		}

		public int getDegree() {
			return degree;
		}

		public void setDegree(int degree) {
			this.degree = degree;
		}

		public HeapNode getNext() {
			return next;
		}

		public void setNext(HeapNode next) {
			this.next = next;
		}

		public HeapNode getPrev() {
			return prev;
		}

		public void setPrev(HeapNode prev) {
			this.prev = prev;
		}

		public HeapNode getParent() {
			return parent;
		}

		public void setParent(HeapNode parent) {
			this.parent = parent;
		}

		public HeapNode getLeftChild() {
			return leftChild;
		}

		public void setLeftChild(HeapNode leftChild) {
			this.leftChild = leftChild;
		}

		@Override
		public boolean equals(Object obj) {
			if (this == obj)
				return true;
			if (obj == null)
				return false;
			if (getClass() != obj.getClass())
				return false;
			HeapNode other = (HeapNode) obj;
			if (!getOuterType().equals(other.getOuterType()))
				return false;
			if (degree != other.degree)
				return false;
			if (key != other.key)
				return false;
			if (leftChild == null) {
				if (other.leftChild != null)
					return false;
			} else if (!leftChild.equals(other.leftChild))
				return false;
			if (next == null) {
				if (other.next != null)
					return false;
			} else if (!next.equals(other.next))
				return false;
			if (parent == null) {
				if (other.parent != null)
					return false;
			} else if (!parent.equals(other.parent))
				return false;
			if (prev == null) {
				if (other.prev != null)
					return false;
			} else if (!prev.equals(other.prev))
				return false;
			return true;
		}

		@Override
		public String toString() {
			return "[key=" + key + ", degree=" + degree + "]";
		}

		public String toLongString() {
			return "[key=" + key + ", degree=" + degree + ", parent=" + parent + ", prev=" + prev + ", next=" + next
					+ ", leftChild=" + leftChild + "]";
		}

		public String printSubTreeOfNode() {
			String str = "[key=" + key + ", degree=" + degree + ", parent=" + parent + ", prev=" + prev + ", next="
					+ next + ", leftChild=" + leftChild + "]";
			HeapNode node = this.getLeftChild();

			while (node != null) {
				str += "\n\t" + node.printSubTreeOfNode();
				node = node.getNext();
			}
			return str;
		}

		private BinomialHeap getOuterType() {
			return BinomialHeap.this;
		}

		public String printNodeListFromStartNode(boolean longString) {
			HeapNode node = this;
			String str = "";
			while (node != null) {
				if (longString) {
					str += node.toLongString();
					str += "->\n";
				} else {
					str += node.toString();
					str += "->";
				}
				node = node.next;
			}
			return str;
		}

	}

	//Fields of BinomialHeap Class
	private HeapNode startNode;
	private HeapNode minNode;
	private int size;

	//Constructors of BinomialHeap Class
	public BinomialHeap() {
		this.startNode = null;
		this.minNode = null;
		this.size = 0;
	}

	public BinomialHeap(HeapNode startNode) {
		this();
		if (startNode != null) {
			this.startNode = startNode;
			this.minNode = findMinForHeap();
			this.size = getSizeForHeap();
		}
	}

	@Override
	public String toString() {
		String str = "START -> ";
		HeapNode node = this.startNode;
		while (node != null) {
			str += node.toString() + " -> ";
			node = node.getNext();
		}
		str += "END";
		return str;
	}

	public String toLongString() {
		String str = "START -> ";
		HeapNode node = this.startNode;
		while (node != null) {
			str += "\n\t" + node.toLongString() + " -> ";
			node = node.getNext();
		}
		str += "\nEND";
		return str;
	}

	private int getSizeForHeap() {
		int size = 0;
		HeapNode node = this.startNode;
		while (node != null) {
			size += Math.pow(2, node.getDegree());
			node = node.getNext();
		}
		return size;
	}

	private HeapNode findMinForHeap() {
		HeapNode node = this.startNode;
		HeapNode min = node;
		if (node == null)
			return null;
		while (node.getNext() != null) {
			node = node.getNext();
			if (node.getKey() <= min.getKey()) {
				min = node;
			}
		}
		return min;
	}

	/**
	 * public boolean empty()
	 *
	 * precondition: none
	 *
	 * The method returns true if and only if the heap is empty.
	 *
	 */
	public boolean empty() {
		if (this.startNode == null) {
			return true;
		}
		return false;
	}

	/**
	 * public void insert(int value)
	 *
	 * Insert value into the heap
	 *
	 */
	public void insert(int value) {
		printIt("\n[INS] Inserting key: " + value);
		HeapNode node = new HeapNode(value);
		BinomialHeap heap = new BinomialHeap(node);
		this.meld(heap);
		printIt("[INS] Finished\n\t" + this);
	}

	/**
	 * public void deleteMin()
	 *
	 * Delete the minimum value
	 *
	 */
	public void deleteMin() {
		printIt("[DELMIN] Deleting minimun: " + this.minNode);
		printIt("\tCURRENT HEAP: " + this.toString());
		HeapNode min = this.minNode;
		if (min.getNext() != null)
			min.getNext().setPrev(min.getPrev());
		if (min.getPrev() != null)
			min.getPrev().setNext(min.getNext());
		if (min == this.startNode) {
			printIt("\t-> Min is startNode, new startNode is: " + min.getNext());
			this.startNode = min.getNext();
		}
		this.minNode = this.findMinForHeap();

		printIt("\tULTERED HEAP: " + this.toString());

		// handle min has degree 0 (must update the size)
		if (min.getLeftChild() == null) {
			printIt("\t-> Minimum has degree 0");
			--this.size;
			return;
		}

		// find last item and keep "first item"
		HeapNode last = min.getLeftChild();
		HeapNode tempNext = min.getLeftChild();
		HeapNode tempPrev = min.getLeftChild();
		HeapNode firstItem = min.getLeftChild();

		while (last != null) {
			if (last.getNext() == null)
				firstItem = last;
			last.setParent(null);
			tempNext = last.getNext();
			tempPrev = last.getPrev();
			last.setNext(tempPrev);
			last.setPrev(tempNext);
			last = tempNext;
		}

		printIt("\t-> Head of new heap: " + firstItem);
		BinomialHeap heap2 = new BinomialHeap(firstItem);
		this.meld(heap2);
		this.minNode = findMinForHeap();
		printIt("\tFINAL HEAP: " + this.toString());
	}

	/**
	 * public int findMin()
	 *
	 * Return the minimum value
	 *
	 */
	public int findMin() {
		return this.minNode.getKey();
	}

	/**
	 * public void meld (BinomialHeap heap2)
	 *
	 * Meld the heap with heap2
	 *
	 */
	public void meld(BinomialHeap heap2) {
		printIt("[MLD] Melding two binomial heaps");
		if (heap2.empty()) {
			printIt("\t-> The secondary heap is empty (nothing to add)");
			return;
		}
		if (this.empty()) {
			printIt("\t-> The primary heap is empty (set secondary as primary)");
			this.startNode = heap2.startNode;
			this.minNode = heap2.minNode;
			this.size = heap2.size;
			return;
		}

		BinomialHeap newHeap = new BinomialHeap(this.merge(heap2));

		// new heap has nothing in it -> finish
		if (newHeap.startNode != null) {
			if (this.minNode.getKey() <= heap2.minNode.getKey()) {
				newHeap.minNode = this.minNode;
			} else {
				newHeap.minNode = heap2.minNode;
			}
			printIt("\t-> The minimum for the melded heap will be: " + newHeap.minNode);

			HeapNode x = newHeap.startNode;
			printIt("\t-> Starting CASES check and handle");

			while (x.getNext() != null) {
				// Case 1 and Case 2
				if ((x.getDegree() != x.getNext().getDegree())
						|| (x.getNext().getNext() != null && x.getNext().getNext().getDegree() == x.getDegree())) {
					x = x.getNext();
				}
				// Case 3
				else {
					if (x.getKey() <= x.getNext().getKey()) {
						printIt("\t\t-> [C3] x: " + x + "->" + x.getNext());
						HeapNode next = x.getNext().getNext();
						HeapNode prev = x.getPrev();
						link(x.getNext(), x);
						x.setNext(next);
						x.setPrev(prev);
						if (x.getNext() != null)
							x.getNext().setPrev(x);
					}
					// Case 4 -  a kind of Case 3 Mirror
					else {
						if (x.getPrev() == null) {
							printIt("\t\t-> [C4] x: " + x + "->" + x.getNext());
							newHeap.startNode = x.getNext();
						}

						else {
							printIt("\t\t-> [C4] x.prev: " + x.getPrev() + "->" + x + "->" + x.getNext());
							x.getPrev().setNext(x.getNext());
						}
						link(x, x.getNext());
						x = x.getParent();
					}
				}
			}
		}

		this.startNode = newHeap.startNode;
		this.minNode = this.findMinForHeap();

		this.size = getSizeForHeap();
		printIt("\t-> Heap details, head: " + this.startNode + ", min: " + this.minNode + ", size: " + this.size);
	}

	private HeapNode merge(BinomialHeap heap2) {
		HeapNode a;
		HeapNode b;
		HeapNode c;
		HeapNode firstNode;

		printIt("this.start=" + this.startNode + ", heap2.start=" + heap2.startNode);
		if (this.startNode.getDegree() >= heap2.startNode.getDegree()) {
			a = heap2.startNode;
			b = this.startNode;
		} else {
			b = heap2.startNode;
			a = this.startNode;
		}
		firstNode = a;

		printIt("\t-> [MRG] Merging two heaps by node degrees");
		printIt("\t-> a:" + a + " b: " + b);
		while (a.getNext() != null && b.getNext() != null) {
			if (a.getNext().getDegree() <= b.getDegree()) {
				printIt("\t-> a.next.degree (" + a.getNext().getDegree() + ") <= b.degree (" + b.getDegree() + ")");
				a = a.getNext();
			} else {
				printIt("\t-> a.next.degree (" + a.getNext().getDegree() + ") > b.degree (" + b.getDegree() + ")");
				c = b.getNext();
				// insert b in between a and a.next
				a.getNext().setPrev(b);
				b.setNext(a.getNext());
				a.setNext(b);
				b.setPrev(a);
				c.setPrev(null);
				b = c;
			}
		}

		handleEndOfListForMerge(a, b);
		printIt("\t-> NODE LIST: " + firstNode.printNodeListFromStartNode(false));
		return firstNode;
	}

	private void handleEndOfListForMerge(HeapNode a, HeapNode b) {
		printIt("\t-> Handling ends of lists");
		printIt("\t\t-> a: " + a + "->" + a.getNext() + ", b: " + b + "->" + b.getNext());
		HeapNode c;
		if (a.getNext() == null) {
			printIt("\t\t-> a.next == null");
			while (b != null) {
				if (a.getDegree() > b.getDegree()) {
					c = b.getNext();
					if (a.getPrev() != null)
						a.getPrev().setNext(b);
					b.setPrev(a.getPrev());
					b.setNext(a);
					a.setPrev(b);
					printIt("\t\t-> Connected " + b + "->" + a);
					b = c;
				} else {
					break;
				}
			}
			a.setNext(b);
			if (b != null)
				b.setPrev(a);
		} else {
			printIt("\t\t-> a.next != null");
			c = a;
			while (a != null) {
				if (b.getDegree() >= a.getDegree()) {
					c = a;
					a = a.getNext();
				} else {
					break;
				}
			}
			if (c.getNext() != null)
				c.getNext().setPrev(b);
			b.setNext(c.getNext());
			b.setPrev(c);
			c.setNext(b);
			printIt("\t\t-> Connected " + c + "->" + b);
		}
	}

	private void link(HeapNode y, HeapNode z) {
		y.setParent(z);
		y.setNext(z.getLeftChild());
		z.setPrev(y.getPrev());
		if (z.getPrev() != null)
			z.getPrev().setNext(z);
		y.setPrev(null);
		if (z.getLeftChild() != null)
			z.getLeftChild().setPrev(y);
		z.setLeftChild(y);
		z.setDegree(z.getDegree() + 1);
	}

	/**
	 * public int size()
	 *
	 * Return the number of elements in the heap
	 *
	 */
	public int size() {
		return this.size;
	}

	/**
	 * public int minTreeRank()
	 *
	 * Return the minimum rank of a tree in the heap.
	 *
	 */
	public int minTreeRank() {
		return startNode.getDegree();
	}

	/**
	 * public boolean[] binaryRep()
	 *
	 * Return an array containing the binary representation of the heap.
	 *
	 */
	public boolean[] binaryRep() {
		boolean[] arr = new boolean[(int) Math.floor(Math.log(size) / Math.log(2)) + 1];
		HeapNode node = this.startNode;
		while (node != null) {
			arr[node.getDegree()] = true;
			node = node.getNext();
		}
		return arr;
	}

	/**
	 * public void arrayToHeap()
	 *
	 * Insert the array to the heap. Delete previous elemnts in the heap.
	 *
	 */
	public void arrayToHeap(int[] array) {
		BinomialHeap heap = new BinomialHeap(new HeapNode(array[0]));
		for (int i = 1; i < array.length; i++) {
			heap.insert(array[i]);
		}
		this.startNode = heap.startNode;
		this.minNode = heap.minNode;
		this.size = heap.size;
	}

	/**
	 * public boolean isValid()
	 *
	 * Returns true if and only if the heap is valid.
	 *
	 */
	public boolean isValid() {
		printIt("[VAL] Beginning check if heap is valid:");
		HeapNode node = this.startNode;
		printIt("\t-> heap.start=" + node + ", size=" + size);
		if (!this.empty()) {
			int[] degrees = new int[(int) Math.floor(Math.log(size) / Math.log(2)) + 1];
			while (node != null) {
				degrees[node.getDegree()] += 1;
				if (degrees[node.getDegree()] > 1)
					return false;
				if (!checkTreeForValid(node))
					return false;
				node = node.getNext();
			}
		} else {
			if (this.size != 0 || this.startNode != null)
				return false;
		}
		return true;
	}

	private boolean checkTreeForValid(HeapNode node) {
		if (node.getLeftChild() == null) {
			return true;
		} else {
			int index = node.getDegree() - 1;
			HeapNode child = node.getLeftChild();
			do {
				// this if statement checks the minheap property
				if (child.getParent().getKey() > child.getKey())
					return false;
				if (child.getDegree() != index)
					return false;
				checkTreeForValid(child);
				index--;
				child = child.getNext();
			} while (child != null);
		}
		return true;
	}

	private void printIt(String msg) {
		if (flag)
			System.out.println(msg);
	}

	public String printHeapDetailed() {
		String str = "HEAP: \n";
		HeapNode node = this.startNode;
		while (node != null) {
			str += "\t" + node.printSubTreeOfNode();
			str += "\t\n->\n";
			node = node.getNext();
		}
		str += "END";
		return str;
	}

}
