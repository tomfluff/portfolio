package histogram;

public class IllegalKValue extends Exception {
	public IllegalKValue(int k){
		super("Illegal k value: " + k);
	}
}
