package histogram;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;

public class HashMapHistogramIterator<T> implements Iterator<T>{

	private int index;
	private ArrayList<T> items;
	
	public HashMapHistogramIterator(IHistogram<T> items) {
		this.index = 0;
		this.items = new ArrayList<>();
		this.items.addAll(items.getItemsSet());
		Collections.sort(this.items, new HashMapHistogramComparator<>(items));
	}

	@Override
	public void remove() {
		throw new UnsupportedOperationException();
		
	}

	@Override
	public boolean hasNext() {
		if (this.index < this.items.size())
			return true;
		return false;
	}

	@Override
	public T next() {
		T item = this.items.get(index);
		this.index++;
		return item;
	}
}
