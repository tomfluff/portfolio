
public class AverageCalculator {

	public static void main(String[] args) {
		
		int num;
		int sum = 0;
		double avg;
		
		for (String str : args) {
			num = Integer.parseInt(str);
			sum += num;
		}
		avg = ((double) sum) / args.length;
		
		System.out.println("The sum is: "+ sum + ".");
		System.out.println("The average is: "+ avg + ".");

	}

}
