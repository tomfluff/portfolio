import java.io.File;
import java.util.Arrays;
import java.util.Scanner;

public class Book {

	private static final int MAX_BOOK_IN_FILE = 20000;


	String ISBN;
	String bookName;
	String bookAuthor;
	String yearOfPublication;
	String publisher;

	public Book(String ISBN, String bookName, String bookAuthor, String yearOfPUblication, String publisher){
		this.ISBN = ISBN;
		this.bookName = bookName;
		this.bookAuthor = bookAuthor;
		this.yearOfPublication = yearOfPUblication;
		this.publisher = publisher;
	}

	public String getISBN() {
		return ISBN;
	}

	public String getBookName() {
		return bookName;
	}

	public String getBookAuthor() {
		return bookAuthor;
	}

	public String getYearOfPublication() {
		return yearOfPublication;
	}

	public String getPublisher() {
		return publisher;
	}

	public String toString(){
		StringBuffer sB = new StringBuffer();
		char sep = ',';
		sB.append(this.ISBN).append(sep).append(this.bookName).append(sep).append(this.bookAuthor);
		return sB.toString();
	}


	/**
	 *
	 * @param fileName
	 * @return
	 * @throws Exception
	 * @pre fileName is a legal fileName, the format of the file is as expected
	 * @post $ret is an Arrays of Book objects read from the file fileName
	 */
	public static Book[] loadBooksData(String fileName) throws Exception {
		File file = new File(fileName);
		Scanner scanner = new Scanner(file);
		scanner.useDelimiter(System.getProperty("line.separator"));
		scanner.next();
		Book[] tempBooks = new Book[MAX_BOOK_IN_FILE];
		int index = 0;
		while (scanner.hasNext()) {
			String[] details = scanner.next().split(";");
			details[0] = details[0].replace("\"", "");
			details[1] = details[1].replace("\"", "");
			details[2] = details[2].replace("\"", "");
			details[3] = details[3].replace("\"", "");
			details[4] = details[4].replace("\"", "");
			tempBooks[index] = new Book(details[0], details[1], details[2], details[3], details[4]);
			index++;
		}
		scanner.close();
		return Arrays.copyOf(tempBooks, index);
	}

}
