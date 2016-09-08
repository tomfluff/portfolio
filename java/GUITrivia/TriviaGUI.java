
import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Random;
import java.util.Set;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.graphics.Font;
import org.eclipse.swt.graphics.FontData;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.DirectoryDialog;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Text;

public class TriviaGUI {

	private static final int MAX_ERRORS = 3;
	private static final String n = System.getProperty("line.separator");
	private List<String> questions;
	private int qIndex;
	private int errorNum;
	private boolean gameOver = false;
	private Shell shell;
	private Label scoreLabel;
	private Composite questionPanel;
	private Font boldFont;
	private String lastAnswer = "";

	public void open() {
		createShell();
		runApplication();
	}

	/**
	 * Creates the widgets of the application main window
	 */
	private void createShell() {
		Display display = Display.getDefault();
		shell = new Shell(display);
		shell.setText("Trivia");

		// window style
		Rectangle monitor_bounds = shell.getMonitor().getBounds();
		// shell.setMinimumSize(645, 335);
		shell.setSize(new Point(monitor_bounds.width / 3, monitor_bounds.height / 4));
		shell.setLayout(new GridLayout());

		FontData fontData = new FontData();
		fontData.setStyle(SWT.BOLD);
		boldFont = new Font(shell.getDisplay(), fontData);

		// create window panels
		createFileLoadingPanel();
		createScorePanel();
		createQuestionPanel();
	}

	/**
	 * Creates the widgets of the form for trivia file selection
	 */
	private void createFileLoadingPanel() {
		final Composite fileSelection = new Composite(shell, SWT.NULL);
		fileSelection.setLayoutData(GUIUtils.createFillGridData(1));
		fileSelection.setLayout(new GridLayout(4, false));

		final Label label = new Label(fileSelection, SWT.NONE);
		label.setText("Enter trivia file path: ");

		// text field to enter the file path
		final Text filePathField = new Text(fileSelection, SWT.SINGLE | SWT.BORDER);
		filePathField.setLayoutData(GUIUtils.createFillGridData(1));

		// "Browse" button
		final Button browseButton = new Button(fileSelection, SWT.PUSH);
		browseButton.setText("Browse");
		browseButton.addSelectionListener(new SelectionAdapter() {
			@Override
			public void widgetSelected(SelectionEvent e) {
				filePathField.setText(GUIUtils.getFilePathFromFileDialog(shell));
			}
		});

		// "Play!" button
		final Button playButton = new Button(fileSelection, SWT.PUSH);
		playButton.setText("Play!");
		playButton.addSelectionListener(new SelectionAdapter() {
			@Override
			public void widgetSelected(SelectionEvent e) {
				qIndex = 0;
				errorNum = 0;
				gameOver = false;
				questions = new ArrayList<>();
				String filePath = filePathField.getText();
				File qf = new File(filePath);
				try {
					FileReader fr = new FileReader(qf);
					BufferedReader br = new BufferedReader(fr);
					Set<String> tempQuestions = new HashSet<>();
					String line = br.readLine();
					while (line != null) {
						tempQuestions.add(line);
						line = br.readLine();
					}
					br.close();
					fr.close();
					questions.addAll(tempQuestions);
					Collections.shuffle(questions);
					String[] q1 = questions.get(0).split("\t");
					qIndex = 1;
					updateQuestionPanel(q1[0], Arrays.asList(q1[1], q1[2], q1[3], q1[4]));
					scoreLabel.setText("0");
				} catch (FileNotFoundException e1) {
					if (filePath.replaceAll("\\s", "").trim().toCharArray().length == 0) {
						GUIUtils.showErrorDialog(shell, "Brows/Enter a question file.");
						filePathField.setText("");
					} else {
						GUIUtils.showErrorDialog(shell, "File:" + n + filePath + n + "Was not found.");
						filePathField.setText("");
					}
				} catch (Exception e1) {
					StringWriter sw = new StringWriter();
					PrintWriter pw = new PrintWriter(sw);
					e1.printStackTrace(pw);
					GUIUtils.showErrorDialog(shell, "[ERROR]" + n + sw.toString());
				}
			}
		});
	}

	/**
	 * Creates the panel that displays the current score
	 */
	private void createScorePanel() {
		Composite scorePanel = new Composite(shell, SWT.BORDER);
		scorePanel.setLayoutData(GUIUtils.createFillGridData(1));
		scorePanel.setLayout(new GridLayout(2, false));

		final Label label = new Label(scorePanel, SWT.NONE);
		label.setText("Total score: ");

		// The label which displays the score; initially empty
		scoreLabel = new Label(scorePanel, SWT.NONE);
		scoreLabel.setLayoutData(GUIUtils.createFillGridData(1));
	}

	/**
	 * Creates the panel that displays the questions, as soon as the game
	 * starts. See the updateQuestionPanel for creating the question and answer
	 * buttons
	 */
	private void createQuestionPanel() {
		questionPanel = new Composite(shell, SWT.BORDER);
		questionPanel.setLayoutData(new GridData(GridData.FILL, GridData.FILL, true, true));
		questionPanel.setLayout(new GridLayout(2, true));

		// Initially, only displays a message
		Label label = new Label(questionPanel, SWT.NONE);
		label.setText("No question to display, yet.");
		label.setLayoutData(GUIUtils.createFillGridData(2));
	}

	/**
	 * Serves to display the question and answer buttons
	 */
	private void updateQuestionPanel(String question, List<String> answers) {

		List<String> randAns = new ArrayList<>(answers);
		Collections.shuffle(randAns);

		// clear the question panel
		Control[] children = questionPanel.getChildren();
		for (Control control : children) {
			control.dispose();
		}

		// create the instruction label
		Label instructionLabel = new Label(questionPanel, SWT.CENTER | SWT.WRAP);
		instructionLabel.setText(lastAnswer + "Answer the following question:");
		instructionLabel.setLayoutData(GUIUtils.createFillGridData(2));

		// create the question label
		Label questionLabel = new Label(questionPanel, SWT.CENTER | SWT.WRAP);
		questionLabel.setText(question);
		questionLabel.setFont(boldFont);
		questionLabel.setLayoutData(GUIUtils.createFillGridData(2));

		// create the answer buttons
		for (int i = 0; i < 4; i++) {
			Button answerButton = new Button(questionPanel, SWT.PUSH | SWT.WRAP);
			answerButton.setText(randAns.get(i));
			GridData answerLayoutData = GUIUtils.createFillGridData(1);
			answerLayoutData.verticalAlignment = SWT.FILL;
			answerButton.setLayoutData(answerLayoutData);
			if (randAns.get(i) == answers.get(0)) {
				answerButton.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						if (!gameOver) {
							scoreLabel.setText((Integer.parseInt(scoreLabel.getText()) + 3) + "");
							handleNextQuestion();
						}
					}
				});
			} else {
				answerButton.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						if (!gameOver) {
							scoreLabel.setText((Integer.parseInt(scoreLabel.getText()) - 1) + "");
							errorNum++;
							if (errorNum >= 3) {
								gameOverStatement();
							} else {
								handleNextQuestion();
							}
						}
					}
				});
			}
		}

		// create the "Pass" button to skip a question
		Button passButton = new Button(questionPanel, SWT.PUSH);
		passButton.setText("Pass");
		GridData data = new GridData(GridData.CENTER, GridData.CENTER, true, false);
		data.horizontalSpan = 2;
		passButton.setLayoutData(data);
		passButton.addSelectionListener(new SelectionAdapter() {
			@Override
			public void widgetSelected(SelectionEvent e) {
				if (!gameOver) {
					handleNextQuestion();
				}
			}
		});

		// two operations to make the new widgets display properly
		questionPanel.pack();
		questionPanel.getParent().layout();
	}

	private void handleNextQuestion() {
		if (qIndex < questions.size()) {
			String[] nextq = questions.get(qIndex).split("\t");
			qIndex++;
			updateQuestionPanel(nextq[0], Arrays.asList(nextq[1], nextq[2], nextq[3], nextq[4]));
		} else {
			// finished questions printing result
			gameOverStatement();
		}
	}

	private void gameOverStatement() {
		GUIUtils.showInfoDialog(shell, "GAME OVER",
				String.format("Your final score is %s after %d questions.", scoreLabel.getText(), qIndex));
		gameOver = true;
	}

	/**
	 * Opens the main window and executes the event loop of the application
	 */
	private void runApplication() {
		shell.open();
		Display display = shell.getDisplay();
		while (!shell.isDisposed()) {
			if (!display.readAndDispatch())
				display.sleep();
		}
		display.dispose();
		boldFont.dispose();
	}
}
