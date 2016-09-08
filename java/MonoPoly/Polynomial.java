public class Polynomial {

	private Monomial[] monoms;
	private int index;

	/**
	 * Creates a polynomial with (safe copies of) the given monomials
	 *
	 * @pre monomials != null
	 * @pre for all i, 0 <= i < monmials.length : monomials[i] != null
	 * @post for all i, 0 <= i < monmials.length : monomials[i].getCoefficient()
	 *       == getMonomial(i).getCoefficient()
	 * @post for all i,v, 0 <= i < monmials.length, 'a'<=v<='z' :
	 *       monomials[i].getDegree(v) == getMonomial(i).getDegree(v)
	 */
	public Polynomial(Monomial[] monomials) {
		this.index = monomials.length;
		this.monoms = new Monomial[this.index];
		for (int i = 0; i < this.index; i++) {
			this.monoms[i] = monomials[i].getCopy();
		}
	}

	public Polynomial(Monomial[] monomials, int index) {
		this.index = index;
		this.monoms = new Monomial[this.index];
		for (int i = 0; i < this.index; i++) {
			this.monoms[i] = monomials[i].getCopy();
		}
	}

	/**
	 * @return the number of monomials in this polynomial
	 */
	public int getMonomialCount() {
		return this.index;
	}

	/**
	 * @pre 0<=index < getMonomialCount()
	 * @return a safe copy of the monomial at the given index
	 */
	public Monomial getMonomial(int index) {
		return this.monoms[index].getCopy();
	}

	/**
	 * @pre other != null
	 * @post Creates a new Polynomial which is the sum of this polynomial and
	 *       other. E.g., the sum of 13b^2x^3z+15 and -4b^2x^3z is
	 *       9b^2x^3z+15
	 */
	public Polynomial add(Polynomial other) {
		Monomial[] res_mons = new Monomial[this.getMonomialCount() + other.getMonomialCount()];
		int index = 0;

		for (int i = 0; i < this.getMonomialCount(); i++) {
			boolean summed = false;
			if (!hasDegreeInArray(res_mons, this.getMonomial(i), index)) {
				for (int j = 0; j < other.getMonomialCount(); j++) {
					if (this.getMonomial(i).hasSameDegrees(other.getMonomial(j))) {
						summed = true;
						res_mons[index] = this.getMonomial(i).add(other.getMonomial(j));
						index++;
					}
				}
				if (!summed) {
					res_mons[index] = this.getMonomial(i);
					index++;
				}
			} else {
				res_mons[index] = this.getMonomial(i);
				index++;
			}
		}

		for (int i = 0; i < other.getMonomialCount(); i++) {
			if (!hasDegreeInArray(res_mons, other.getMonomial(i), index)) {
				res_mons[index] = other.getMonomial(i);
				index++;
			}
		}

		return new Polynomial(res_mons, index).fixPolynomial();
	}

	private Polynomial fixPolynomial() {
		Monomial[] r = new Monomial[this.getMonomialCount()];
		int index = 0;

		for (int i = 0; i < this.getMonomialCount(); i++) {
			if (!hasDegreeInArray(r, this.getMonomial(i), index)) {
				r[index] = this.getMonomial(i);
				for (int j = i+1; j < this.getMonomialCount(); j++) {
					if (this.getMonomial(i).hasSameDegrees(this.getMonomial(j))) {
						r[index] = r[index].add(this.getMonomial(j));
					}
				}
				index++;
			}
		}

		return new Polynomial(r, index);
	}

	private boolean hasDegreeInArray(Monomial[] res_mons, Monomial monomial, int index) {
		for (int i = 0; i < index; i++) {
			if (res_mons[i].hasSameDegrees(monomial)) {
				return true;
			}
		}
		return false;
	}

	/**
	 * @pre other != null
	 * @post Creates a new Polynomial which is the product of this polynomial
	 *       and other. E.g., the product of 13b^2x^3z+15 and -4b^2x^3z is
	 *       -52b^4x^6z^2-60b^2x^3z
	 */
	public Polynomial multiply(Polynomial other) {
		Monomial[] res_mon = new Monomial[this.getMonomialCount() * other.getMonomialCount()];
		int index = 0;
		for (int i = 0; i < this.getMonomialCount(); i++) {
			for (int j = 0; j < other.getMonomialCount(); j++) {
				res_mon[index] = other.monoms[j].multiply(this.monoms[i]);
				index++;
			}
		}
		return new Polynomial(res_mon, index).fixPolynomial();
	}

	/**
	 * @pre assignment != null
	 * @pre assignment.length == 26
	 * @return the result of assigning assignment[0] to a, assignment[1] to b
	 *         etc., and computing the value of this Polynomial
	 */
	public int evaluate(int[] assignment) {
		int res = 0;
		for (int i = 0; i < this.getMonomialCount(); i++) {
			res += this.monoms[i].evaluate(assignment);
		}
		return res;
	}


	/**
	 * Returns a string representation of this polynomial by the mathematical
	 * convention, but without performing normalization (summing of monomials).
	 * I.e., each monomial is printed according to Monomial.toString(), for
	 * example 13b^2x^3z+15-4b^2x^3z
	 */
	public String toString() {
		if (this.getMonomialCount() == 0) {
			return "0";
		}
		String str = "";
		for (int i = 0; i < this.getMonomialCount(); i++) {
			if (this.monoms[i].toString() != "0") {
				if (this.monoms[i].getSign() == 1 && i > 0) {
					str += "+";
				}
				str += this.monoms[i];
			}
		}
		if (str.isEmpty()) {
			return "0";
		}
		return str;
	}
}
