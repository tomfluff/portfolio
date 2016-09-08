import java.io.File;
import java.util.Arrays;
import java.util.Scanner;

public class User {
	int userID;
	String location;
	int age;

	private static final int NO_AGE = -1;
	private static final int MAX_USERS_IN_FILE = 20000;


	public User(int userID, String location, int age){
		this.userID = userID;
		this.location = location;
		this.age = age;
	}

	public User(int userID, String location){
		this.userID = userID;
		this.location = location;
		this.age = NO_AGE;
	}

	public int getUserID() {
		return userID;
	}

	public String getLocation() {
		return location;
	}

	public int getAge() {
		return age;
	}

	public String toString() {
		if (this.hasAge()) {
			return "User ID: " + this.userID + ", location: " + this.location + ", age: " + this.age;
		} else {
			return "User ID: " + this.userID + ", location: " + this.location;
		}
	}

	/**
	 *
	 * @return
	 * @post ($ret == true) <=> (this.age != NO_AGE)
	 */
	public boolean hasAge(){
		if (this.age != NO_AGE) {
			return true;
		}
		return false;

	}

	/**
	 *
	 * @param fileName
	 * @return
	 * @throws Exception
	 * @pre fileName is a legal fileName, the format of the file is as expected
	 * @post $ret is an Arrays of User objects read from the file fileName
	 */

	public static User[] loadUsersData(String fileName) throws Exception{
		File file = new File(fileName);
		Scanner scanner = new Scanner(file);
		scanner.useDelimiter(System.getProperty("line.separator"));
		scanner.next();
		User[] tempUsers = new User[MAX_USERS_IN_FILE];
		int index = 0;
		while (scanner.hasNext()) {
			String[] details = scanner.next().split(";");
			details[0] = details[0].replace("\"", "");
			details[1] = details[1].replace("\"", "");
			details[2] = details[2].replace("\"", "");
			if (details[2].equals("NULL")) {
				tempUsers[index] = new User(Integer.parseInt(details[0]), details[1]);
			} else {
				tempUsers[index] = new User(Integer.parseInt(details[0]), details[1], Integer.parseInt(details[2]));
			}
			index++;
		}
		scanner.close();
		return Arrays.copyOf(tempUsers, index);
	}


}
