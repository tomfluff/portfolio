package filesSim;

import java.io.File;
import java.io.IOException;
import java.util.List;

import java.util.Map;
import java.util.HashMap;

import histogram.IHistogram;

public class FileIndex {
	private static final String ERROR = "[ERROR] ";
	private Map<String, IHistogram<String>> index = new HashMap<String, IHistogram<String>>();

	/**
	 * Given a path to a folder, reads all the files in it and indexes them
	 */
	public void index(String folderPath) {
		// first, clear the previous contents of the index
		clearPreviousIndex();
		File folder = new File(folderPath);
		File[] listFiles = folder.listFiles();
		for (File file : listFiles) {
			// for every file in the folder
			if (file.isFile()) {
				String path = file.getAbsolutePath();
				System.out.println("Indexing " + path);
				try {
					// add to the index if read is successful
					addFileToIndex(file);
				} catch (IOException e) {
					System.out.println(ERROR + "failed to read from "
							+ path);
				}
			}
		}
	}

	/**
	 * Adds the input file to the index
	 * @throws IOException
	 */
	public void addFileToIndex(File file) throws IOException {
		List<String> tokens = FileUtils.readAllTokens(file);
		String path = file.getAbsolutePath();
		if (tokens.isEmpty()) {
			System.out.println(ERROR + "ignoring empty file " + path);
			return;
		}
		index.put(path, HistogramsFactory.getHistogram());
		index.get(path).addAll(tokens);
	}

	/**
	 * Called at the beginning of index() in order to clear the fields from
	 * previously indexed files. After calling it the index contains no files.
	 */
	public void clearPreviousIndex() {
		index.clear();
	}

	/**
	 * Given indexed input files, compute their cosine similarity based on their
	 * indexed tokens
	 */
	public double getCosineSimilarity(File file1, File file2) {
		if (!verifyFile(file1) || !verifyFile(file2)) {
			return Double.NaN;
		}

		double ret = calcSimilarTokensMult(file1.getAbsolutePath(), file2.getAbsolutePath());

		ret = ret / (calcAllTokensRoot(file1.getAbsolutePath()) * calcAllTokensRoot(file2.getAbsolutePath()));

		return ret;
	}

	private double calcSimilarTokensMult(String path1, String path2) {
		double ret = 0.0;
		for (String word : index.get(path1).getItemsSet()) {
			if (index.get(path2).getCountForItem(word) != 0) {
				ret += index.get(path1).getCountForItem(word) * index.get(path2).getCountForItem(word);
			}
		}
		return ret;
	}

	private double calcAllTokensRoot(String path) {
		double ret = 0.0;

		for (String word : index.get(path).getItemsSet()) {
			ret += Math.pow(index.get(path).getCountForItem(word), 2);
		}

		return Math.sqrt(ret);
	}

	/**
	 * Given indexed input files, return the number of the common token in both files
	 */
	public int getCommonTokensNum(File file1, File file2){
		if (!verifyFile(file1) || !verifyFile(file2)) {
			return 0;
		}
		int count = 0;
		for (String word : index.get(file1.getAbsolutePath()).getItemsSet()) {
			if (index.get(file2.getAbsolutePath()).getCountForItem(word) != 0) {
				count++;
			}
		}
		return count;
	}


	/**
	 * returns true iff the input file is currently indexed. Otherwise, prints
	 * an error message.
	 */
	public boolean verifyFile(File file) {
		if (index.containsKey(file.getAbsolutePath())) {
			return true;
		}
		return false;
	}

	/**
	 * @return the number of files currently indexed.
	 */
	public int getNumIndexedFiles() {
		return index.size();
	}

}
