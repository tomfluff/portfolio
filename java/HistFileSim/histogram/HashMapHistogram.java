package histogram;

import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Set;

public class HashMapHistogram<T> implements IHistogram<T>{
	
	private HashMap<T, Integer> itemsMap; 

	public HashMapHistogram() {
		this.itemsMap = new HashMap<>();
	}
	
	@Override
	public Iterator<T> iterator() {
		return new HashMapHistogramIterator<>(this);
	}

	@Override
	public void addItem(T item) {
		try {
			addItemKTimes(item, 1);
		} catch (IllegalKValue e) {
			// k = 1, just silence the compiler
		}
	}

	@Override
	public void addItemKTimes(T item, int k) throws IllegalKValue {
		if (k <= 0) {
			throw new IllegalKValue(k);
		}
		itemsMap.put(item, getCountForItem(item) + k);
		
	}

	@Override
	public void addAll(Collection<T> items) {
		for (T item : items) {
			addItem(item);
		}
	}

	@Override
	public int getCountForItem(T item) {
		Integer v = itemsMap.get(item);
		if (v != null)
			return v;
		return 0;
	}

	@Override
	public void clear() {
		itemsMap = new HashMap<T, Integer>();
	}

	@Override
	public Set<T> getItemsSet() {
		return itemsMap.keySet();
	}

}
