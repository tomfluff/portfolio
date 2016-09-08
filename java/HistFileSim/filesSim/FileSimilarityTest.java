package filesSim;

import java.io.File;


public class FileSimilarityTest {
	private static final String FOLDER = "../sample_files";

	public static void main(String[] args) {
		FileIndex fileIndex = new FileIndex();
		fileIndex.index(FOLDER);
		System.out.println("Indexed " + fileIndex.getNumIndexedFiles() + " files.");

		File firstFile = new File(FOLDER + File.separator + "file1.txt");
		for (int i = 2; i <= 3; i++) {
			File otherFile = new File(FOLDER + File.separator + "file" + i + ".txt");

			System.out.printf("%s: cosine similarity: %.3f, number of common words: %d%n", otherFile.getAbsolutePath(),
					fileIndex.getCosineSimilarity(firstFile, otherFile), fileIndex.getCommonTokensNum(firstFile, otherFile));
		}

	}
}
