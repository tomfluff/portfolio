import java.io.File;
import java.util.Arrays;
import java.util.Scanner;

public class SpellingCorrector {

	/**
	 * This method uses a given scanner to read words and compile a dictionary of the words.
	 * @param scanner The scanner used as a source
	 * @return An array representing a dictionary
	 */
	public static String[] scanVocabulary(Scanner scanner) {
		String[] words = new String[3000];
		int index = 0;
		String word = "";
		while (scanner.hasNext() && words[2999] == null) {
			word = scanner.next().toLowerCase();
			if (!word.equals("") && !checkWordInArray(words, word)) {
				words[index] = word;
				index++;
			}
		}
		String[] new_words = Arrays.copyOf(words, index);
		Arrays.sort(new_words);
		return new_words;
	}

	/**
	 * This method calculates the hamming distance weather two words.
	 * @param word1 The word to compare
	 * @param word2 The word to compare to
	 * @return An integer representing how close the words are to one another
	 */
	public static int calcHammingDistance(String word1, String word2) {
		int ham_dist = Math.abs(word1.length() - word2.length());
		char[] word1C = word1.toCharArray();
		char[] word2C = word2.toCharArray();
		for (int i = 0; i < Math.min(word1C.length, word2C.length); i++) {
			if (word1C[i] != word2C[i]) {
				ham_dist += 1;
			}
		}
		return ham_dist;
	}

	/**
	 * This method finds similar words based on the hamming distance
	 * @param word The word to find similar word to
	 * @param vocabulary The dictionary of words to search in
	 * @return An array of arrays, a[0] are the words with hamming distance of 0, a[1] are with 1 and a[2] are with 2
	 */
	public static String[][] findSimilarWords(String word, String[] vocabulary) {
		String[][] sim_array = new String[3][];
		sim_array[0] = new String[0];
		String[] temp_w1 = new String[vocabulary.length];
		int index_w1 = 0;
		String[] temp_w2 = new String[vocabulary.length];
		int index_w2 = 0;
		for (int i =0; i<vocabulary.length;i++) {
			switch (calcHammingDistance(word, vocabulary[i])) {
			case 0:
				sim_array[0] = new String[1];
				sim_array[0][0] = word;
				break;
			case 1:
				temp_w1[index_w1] = vocabulary[i];
				index_w1++;
				break;
			case 2:
				temp_w2[index_w2] = vocabulary[i];
				index_w2++;
				break;
			default:
				break;
			}
		}
		sim_array[1] = Arrays.copyOf(temp_w1, index_w1);
		sim_array[2] = Arrays.copyOf(temp_w2, index_w2);
		return sim_array;
	}

	/**
	 * Splits a sentence to words in lower case
	 * @param sentence The sentence to split
	 * @return An array or words
	 */
	public static String[] splitSentence(String sentence) {
		String[] words_v0 = sentence.split(" ");
		String[] words_v1 = new String[words_v0.length];
		int index_v1 = 0;
		for (int i = 0; i < words_v0.length; i++) {
			if (!words_v0[i].equals("")) {
				words_v1[index_v1] = words_v0[i].toLowerCase();
				index_v1++;
			}
		}
		String[] words_v2 = Arrays.copyOf(words_v1, index_v1);
		return words_v2;
	}

	/**
	 * This method builds a sentence based on an array of words.
	 * @param words The array of words to build the sentence by
	 * @return A string representing the sentence
	 */
	public static String buildSentence(String[] words) {
		String sentence = "";
		for (int i = 0; i < words.length; i++) {
			if (!words[i].equals("")) {
				sentence += words[i].trim();
				if (i < words.length - 1) {
					sentence += " ";
				}
			}
		}
		return sentence;
	}

	/**
	 * Checks if a given word exists in a vocabulary
	 * @param vocabulary The dictionary of words to check in
	 * @param word The word to check
	 * @return A boolean value weather the word exists or not respectively
	 */
	public static boolean isInVocabulary(String[] vocabulary, String word) {
		word = word.toLowerCase().trim();
		for (int i = 0; i < vocabulary.length; i++) {
			if (vocabulary[i] != null && vocabulary[i].equals(word)) {
				return true;
			}
		}
		return false;
	}

	private static boolean checkWordInArray(String[] array, String word) {
		for (int i = 0; i < array.length; i++) {
			if (array[i] == null) {
				break;
			} if (array[i].equals(word)) {
				return true;
			}
		}
		return false;
	}

	private static boolean isChoiceCorrect(int maxChoice, String choice) {
		int num = 0;
		try {
			num = Integer.parseInt(choice);
		} catch (Exception exception) {
			printInvalidChoice();
			return false;
		}
		if (num < 1 || num > maxChoice) {
			printInvalidChoice();
			return false;
		}
		return true;
	}

	/****  use these method to print all your output messages ****/
	private static void printWordIncorrect(String word){
		System.out.println("the word " + word + " is incorrect");
	}

	private static void printReadVocabulary(String vocabularyFileName, int numOfWords){
		System.out.println("Read " + numOfWords + " words from " + vocabularyFileName);
	}

	private static void printNumOfCorrections(int num, int distance){
		System.out.println(num+" corrections of distance " + distance);
	}

	private static void printEnterYourSentence(){
		System.out.println("Enter your sentence:");
	}

	private static void printEnterYourChoice(){
		System.out.println("Enter your choice:");
	}

	private static void printCorrectionOption(int i, String correction){
		System.out.println(i + ". " + correction);
	}

	private static void printInvalidChoice(){
		System.out.println("[WARNING] Invalid choice, try again.");
	}

	private static void printCorrectSentence(String sentence){
		System.out.println("The correct sentence is: " + sentence);
	}

	private static void printNumOfCorrectedWords(int numOfWords){
		System.out.println("Corrected "+ numOfWords + " words.");
	}

	/**
	 * Based on a file path given in the arguments operates a program to spell-correct sentences
	 * @param args The arguments of the function, should have the file path in position 0
	 * @throws Exception Based on handling the file
	 */
	public static void main(String[] args) throws Exception {
		//Initial validation
		if (args[0] == null) {
			throw new Exception("[ERROR] A FILE PATH missing from arguments.");
		}
		// Getting vocabulary
		String file_path = args[0];
		String[] vocab;
		Scanner scanner_01;
		try {
			File file = new File(file_path);
			scanner_01 = new Scanner(file);
			vocab = scanVocabulary(scanner_01);
			scanner_01.close();
		} catch (Exception e) {
			throw new Exception("[ERROR] A problem with opening the file.");
		}
		printReadVocabulary(file_path, vocab.length);
		// Getting sentences and correcting
		Scanner scanner_02 = new Scanner(System.in);
		printEnterYourSentence();
		String sentence = scanner_02.nextLine();
		while (!sentence.equals("quite")) {
			int cnt_correct = 0;
			String[] words = splitSentence(sentence);
			for (int i = 0; i < words.length; i++) {
				if (!isInVocabulary(vocab, words[i])) {
					String[][] sim_words = findSimilarWords(words[i], vocab);
					printWordIncorrect(words[i]);
					printNumOfCorrections(sim_words[1].length, 1);
					printNumOfCorrections(sim_words[2].length, 2);
					int count = 0;
					for (int j = 0; j < sim_words[1].length && count < 8; j++) {
						printCorrectionOption(count+1, sim_words[1][j]);
						count++;
					}
					for (int j = 0; j < sim_words[2].length && count < 8; j++) {
						printCorrectionOption(count+1, sim_words[2][j]);
						count++;
					}
					String fix_word = "";
					if (sim_words[0].length == 0 && sim_words[1].length == 0 && sim_words[2].length == 0) {
						System.out.println("[WARNING] No fixes found, uses original word.");
					} else {
						printEnterYourChoice();
						String choice_str = "";
						do {
							choice_str = scanner_02.nextLine();
							if (choice_str.equals("quite")) {
								System.exit(0);
							}
						} while (!isChoiceCorrect(count, choice_str));
						int choice = Integer.parseInt(choice_str);
						if (choice > sim_words[1].length) {
							fix_word = sim_words[2][choice - sim_words[1].length - 1];
						} else {
							fix_word = sim_words[1][choice - 1];
						}
						words[i] = fix_word;
						cnt_correct++;
					}
				}
			}
			String fixed_sentence = buildSentence(words);
			printCorrectSentence(fixed_sentence);
			printNumOfCorrectedWords(cnt_correct);
			printEnterYourSentence();
			sentence = scanner_02.nextLine();
		}
		scanner_02.close();
	}
}
