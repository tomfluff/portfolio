package filesSim;

import histogram.HashMapHistogram;
import histogram.IHistogram;

public class HistogramsFactory {
	public static IHistogram<String> getHistogram(){
		return new HashMapHistogram<>();
	}
}
