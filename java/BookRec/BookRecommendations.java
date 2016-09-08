import java.io.File;
import java.io.FileWriter;
import java.util.Scanner;

public class BookRecommendations {

	private static final int NO_RATING = -1;
	private static final int AGE_GROUP_MARGINE_SIZE = 3;


	Book[] books;
	User[] users;
	int[][] ratings;

	/**
	 *
	 * @param books
	 * @param users
	 * @param ratings
	 * @pre ratings.length == users.length
	 * @pre ratings[0].length == books.length
	 */
	public BookRecommendations(Book[] books, User[] users, int[][] ratings){
		this.books = books;
		this.users = users;
		this.ratings = ratings;
	}

	/**
	 *
	 * @param fileName
	 * @param usersArray
	 * @param booksArray
	 * @return
	 * @throws Exception
	 * @pre usersArray.length != 0
	 * @pre booksArray.length != 0
	 * @pre fileName is a legal fileName, the format of the file is as expected
	 * @post $ret.length = usersArray.length
	 * @post $ret[0].length = booksArray.length
	 * @post $res[i][j] == the rating of usersArray[i] to the booksArray[j]
	 */
	public static int[][] loadRatingsData(String fileName, User[] usersArray, Book[] booksArray) throws Exception{
		if (usersArray.length == 0) {
			throw new Exception("[ERROR] No users found.");
		} if (booksArray.length == 0) {
			throw new Exception("[ERROR] No books found.");
		}
		int[][] ratings = initializeRatingsArray(usersArray, booksArray);
		File file = new File(fileName);
		Scanner scanner = new Scanner(file);
		scanner.useDelimiter(System.getProperty("line.separator"));
		scanner.next();
		while (scanner.hasNext()) {
			String[] details = scanner.next().split(";");
			details[0] = details[0].replace("\"", "");
			details[1] = details[1].replace("\"", "");
			details[2] = details[2].replace("\"", "");
			int userIndex = findUserById(usersArray, Integer.parseInt(details[0]));
			int bookIndex = findBookByISBN(booksArray, details[1]);
			try {
				ratings[userIndex][bookIndex] = Integer.parseInt(details[2]);
			} catch (Exception e) {
				throw new Exception("[ERROR] The book with ISBN: "+ details[1] + " and/or the user with ID: "+ details[0] + " is missing.");
			}
		}
		scanner.close();
		return ratings;
	}

	/**
	 *
	 * @param userIndex
	 * @return
	 * @pre userIndex >0 0 && userIndex < this.users.length
	 * @post $ret = average rating of all the books this.users[userIndex] rated
	 */
	public double getAverageRatingForUser(int userIndex){
		double avg = 0;
		int amount = 0;
		for (int i =0; i < this.books.length; i++) {
			if (this.ratings[userIndex][i] != NO_RATING) {
				avg += this.ratings[userIndex][i];
				amount++;
			}
		}
		if (amount != 0) {
			avg = avg / amount;
			return avg;
		} else {
			return NO_RATING;
		}
	}

	/**
	 *
	 * @param bookIndex
	 * @return
	 * @pre bookIndex >= 0 && bookIndex < this.books.length
	 * @post $ret = NO_RATING if no user had rated this.books[bookIndex]
	 * @post otherwise, $ret = average rating of this.books[bookIndex] among all the users who rated it
	 */
	public double getAverageRatingForBook(int bookIndex){
		double avg = 0;
		int amount = 0;
		for (int i =0; i < this.users.length; i++) {
			if (this.ratings[i][bookIndex] != NO_RATING) {
				avg += this.ratings[i][bookIndex];
				amount++;
			}
		}
		if (amount != 0) {
			avg = avg / amount;
			return avg;
		} else {
			return NO_RATING;
		}
	}

	/**
	 *
	 * @param user
	 * @return
	 * @pre there exist i s.t. (such that) this.users[i] == user && user.age != NO_AGE
	 * @post $ret.lenght = this.users.lenght
	 * @post $ret[i] == true <=> this.users[i] in the user group of "user" ( user.age - AGE_GROUP_MARGINE_SIZE  <= this.users.age <= user.age + AGE_GROUP_MARGINE_SIZE
	 */
	public boolean[] getUsersInAgeGroup(User user){
		boolean[] boolArray = new boolean[this.users.length];
		for (int i =0; i < this.users.length; i++) {
			if (Math.abs(user.getAge() - this.users[i].getAge()) <= AGE_GROUP_MARGINE_SIZE) {
				boolArray[i] = true;
			} else {
				boolArray[i] = false;
			}
		}
		return boolArray;
	}

	/**
	 *
	 * @param bookIndex
	 * @param ageGroup
	 * @return
	 * @pre ageGroup.length == this.users.length
	 * @pre bookIndex >= 0 && bookIndex < this.books.length
	 * @post $res = NO_RATING if there is no user in the age group that rated book[bookIndex]
	 * @post otherwise, $res = average ratings for all users this.users[i] s.t. ageGroup[i] == true
	 */
	public double getAverageRatingForBookInAgeGroup(int bookIndex, boolean[] ageGroup){
		double avg = 0;
		int amount = 0;
		for (int i = 0; i < this.users.length; i++) {
			if (ageGroup[i] == true && this.ratings[i][bookIndex] != NO_RATING) {
				avg += this.ratings[i][bookIndex];
				amount++;
			}
		}
		if (amount != 0) {
			return avg / amount;
		} else {
			return NO_RATING;
		}
	}

	/**
	 *
	 * @param user
	 * @return
	 * @pre there exist i s.t. this.users[i] == user  && user.age != NO_AGE
	 * @pos $res = NO_RATING if there is no user in the age group that rated book[bookIndex]
	 * @post $res = this.books[i] s.t. average for book[i] in the age group of user is maximum
	 */
	public Book getHighestRatedBookInAgeGroup(User user){
		double max = -1;
		Book maxBook = new Book(null, null, null, null, null);
		boolean[] ageGroup = getUsersInAgeGroup(user);
		for (int i = 0; i< this.books.length; i++) {
			double tempMax = getAverageRatingForBookInAgeGroup(i, ageGroup);
			if (tempMax >= max) {
				max = tempMax;
				maxBook = this.books[i];
			}
		}
		if (max != -1) {
			return maxBook;
		} else {
			return null;
		}
	}


	/**
	 *
	 * @param user
	 * @param fileName
	 * @throws Exception
	 * @pre fileName is a legal fileName, the format of the file is as expected
	 * @pre there exist i s.t. this.users[i] == user  && user.age != NO_AGE
	 */
	public void printRecommendationToFile(User user, String fileName) throws Exception{
		Book recBook = getHighestRatedBookInAgeGroup(user);
		File file = new File(fileName);
		FileWriter fileWrite = new FileWriter(file);
		String str1 = getRecommendedBookString(recBook);
		String str2 = getRecommendedBookAverageInUserGroup(getAverageRatingForBookInAgeGroup(findBookByISBN(this.books, recBook.getISBN()), getUsersInAgeGroup(user)));
		String str3 = getRecommendedBookAverageFoAllUsers(getAverageRatingForBook(findBookByISBN(this.books, recBook.getISBN())));
		fileWrite.write(str1);
		fileWrite.write(System.getProperty("line.separator"));
		fileWrite.write(str2);
		fileWrite.write(System.getProperty("line.separator"));
		fileWrite.write(str3);
		fileWrite.write(System.getProperty("line.separator"));
		fileWrite.close();
	}

	private String getRecommendedBookString(Book b){
		return "The recommended Book for you is: " + b.toString();
	}

	private String getRecommendedBookAverageInUserGroup(double average){
		return String.format("The book's average rating among its age group is: %.2f",average);
	}

	private String getRecommendedBookAverageFoAllUsers(double average){
		return String.format("The book's average rating among all the users is: %.2f",average);
	}


	/****              ****/
	/**** HELP METHODS ****/
	/****              ****/

	private static int findBookByISBN(Book[] booksArray, String isbn) {
		for (int i = 0; i < booksArray.length; i++) {
			if (booksArray[i].getISBN().equals(isbn)) {
				return i;
			}
		}
		return -1;
	}

	private static int findUserById(User[] usersArray, int userID) {
		for (int i = 0; i < usersArray.length; i++) {
			if (usersArray[i].getUserID() == userID) {
				return i;
			}
		}
		return -1;
	}

	private static int[][] initializeRatingsArray(User[] usersArray, Book[] booksArray) {
		int[][] ratings = new int[usersArray.length][];
		for (int i = 0; i < ratings.length; i++) {
			ratings[i] = new int[booksArray.length];
			for (int j = 0; j< ratings[i].length; j++) {
				ratings[i][j] = NO_RATING;
			}
		}
		return ratings;
	}
}
