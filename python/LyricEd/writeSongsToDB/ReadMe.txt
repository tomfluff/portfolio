Requirements:
	Remember to install all of those for Python 3,
	preferably with virtual environment.
	1) Python 3
	2) pymysql

Before use:
	1) locate 'vocab.txt' file
	2) create a folder and place all of your '.csv' files inside

How to use:
	create an ssh tunnel as explained in the doc.
	(e.g. on linux with openssh-client: ssh <username>@nova.cs.tau.ac.il -L 3305:mysqlsrv.cs.tau.ac.il:3306)
	python writeSongsToDB.py <csv folder> <vocab file path>
	[ e.g.   python writeSongsToDB.py ./data ./vocab.txt
	The script will use the vocabulary file 'vocab.txt' to calculate difficulty.
	If a word is missing from the file it will be fetched from the API.
	Notice the script handles duplicates.
	If any error occures please copy the log and update the group so we could fix that.
