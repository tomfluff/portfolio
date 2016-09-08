package histogram;

import java.util.Comparator;

public class HashMapHistogramComparator<T> implements Comparator<T>{
	
	private IHistogram<T> map;
	

	public HashMapHistogramComparator(IHistogram<T> map) {
		this.map = map;
	}

	@Override
	public int compare(T arg0, T arg1) {
		return (-1) * Integer.compare(map.getCountForItem(arg0), map.getCountForItem(arg1));
	}
	
}
