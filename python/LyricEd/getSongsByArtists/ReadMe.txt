Requirements:
	Remember to install all of those for Python 3,
	preferably with virtual environment.
	1) Python 3
	2) bs4 library (install using pip)
	3) textblob library (install using pip)
	4) google-api-python-client (install using pip)

Before use:
	1) Register to Genius API at: http://genius.com/api-clients
	2) Fill your Genius API's client in credentials.ini.
	3) Register to LastFM API at: https://www.last.fm/api
	4) Fill your LastFM API's key in credentials.ini.
	5) Register to YouTube API at: https://developers.google.com/youtube/
	6) Create a text file, and write artists/bands names in it (one name per row)
	7) Keep the 'vocab.txt' file in the same folder

How to use:
	python getSongsByArtists.py <input file path> <output file name>
	[ e.g.   python getSongsByArtists.py ./names.txt out01 ]
	The script will use the vocabulary file 'vocab.txt' to calculate difficulty.
	On top of that it will update the file with new words whenever it encounters them.
	 The file is a dictionary of a word and it's frequency in the English language.
