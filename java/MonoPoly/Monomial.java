
/**
 * Represents a multiplication of variables in a-z with an integral coefficient
 */
public class Monomial {

	private static final int MAX_NUM_OF_VARIABLES = 26;

	private int coeff;
	private int[] vars; // represents the variables and their degree (plus 97 to all index for ASCII code) a is 0, z is 25


	/**
	 * @post this.getCoefficient() == coefficient
	 * @post for every v, 'a'<=v<='z', isVariable(v) == false
	 */
	public Monomial(int coefficient) {
		this.coeff = coefficient;
		this.vars = new int[MAX_NUM_OF_VARIABLES];
	}

	public Monomial(int coefficient, int[] variables) {
		this.coeff = coefficient;
		this.vars = variables;
	}

	/**
	 * @return the coefficient of this monomial
	 */
	public int getCoefficient() {
		return this.coeff;
	}

	/**
	 * @post getCoefficient() == coefficient
	 */
	public void setCoefficient(int coefficient) {
		this.coeff = coefficient;
	}

	/**
	 * @return true iff the input is a variable of this monomial (and appears in
	 *         toString).
	 */
	public boolean isVariable(char variable) {
		int index = ((int) Character.toLowerCase(variable)) - 97;
		if (index < 97 || index > 122) {
			return false;
		} else if (this.vars[index] != 0) {
				return true;
		} else {
			return false;
		}
	}

	/**
	 * @pre isVariable(variable)
	 * @return the degree of variable in this monomial
	 */
	public int getDegree(char variable) {
		int index = ((int) Character.toLowerCase(variable)) - 97;
		if (index < 97 || index > 122) {
			return 0;
		} else {
			return this.vars[index];
		}
	}

	/**
	 * @pre degree >= 0
	 * @pre 'a'<=variable<='z'
	 * @post getDegree(variable) = degree
	 */
	public void setDegree(char variable, int degree) {
		int index = ((int) Character.toLowerCase(variable)) - 97;
		if (index < 0 || index > 25) {
			// maybe throw exception
		} else {
			this.vars[index] = degree;
		}
	}

	/**
	 * @pre other!= null
	 * @return true iff the set of variables and the degree of each variable is
	 *         the same for this and other.
	 */
	public boolean hasSameDegrees(Monomial other) {
		for (int i = 0; i < MAX_NUM_OF_VARIABLES; i++) {
			if (this.vars[i] != other.vars[i]) {
				return false;
			}
		}
		return true;
	}

	/**
	 * @pre assignment != null
	 * @pre assignment.length == 26
	 * @return the result of assigning assignment[0] to a, assignment[1] to b
	 *         etc., and computing the value of this Monomial
	 */
	public int evaluate(int[] assignment) {
		int res = this.coeff;
		for (int i = 0; i < MAX_NUM_OF_VARIABLES; i++) {
			if (this.vars[i] != 0) {
				res *= Math.pow(assignment[i], this.vars[i]);
			}
		}
		return res;
	}

	/**
	 * Returns a string representation of this monomial by the mathematical
	 * convention. I.e., the coefficient is first (if not 1), then every
	 * variable in an alphabetic order followed by ^ and its degree (if > 1).
	 * For example, 13b^2x^3z
	 */
	public String toString() {
		if (this.coeff == 0) {
			return "0";
		}
		String str = "";
		if (this.coeff == -1) {
			str = "-";
		} else if (this.coeff == 1) {}
		else {
			str += this.coeff;
		}

		for (int i = 0; i < MAX_NUM_OF_VARIABLES; i++) {
			if (this.vars[i] != 0) {
				int var = i + 97;
				str += (char) var;
				if (this.vars[i] != 1) {
					str += ("^" + this.vars[i]);
				}
			}
		}
		return str;
	}

	/**
	 * Returns a "safe" copy of this monomial, i.e., if the copy is changed,
	 * this will not change and vice versa
	 */
	public Monomial getCopy() {
		return new Monomial(this.coeff, this.vars.clone());
	}

	/**
	 * Adds one Monomial to the other
	 * @pre this.hasSameDegrees(other) == true;
	 * @param other
	 * @return a new Monomial of the addition
	 */
	public Monomial add(Monomial other) {
		return new Monomial(this.coeff + other.coeff, this.vars.clone());
	}

	/**
	 * multiplies two Monomials together returning the result
	 * @param other
	 * @return
	 */
	public Monomial multiply(Monomial other) {
		int[] vars = new int[MAX_NUM_OF_VARIABLES];
		for (int i = 0; i < MAX_NUM_OF_VARIABLES; i++) {
			vars[i] = this.vars[i] + other.vars[i];
		}
		return new Monomial(this.coeff * other.coeff, vars);
	}

	public int getSign() {
		if (this.coeff < 0) {
			return -1;
		} else {
			return 1;
		}
	}
}
