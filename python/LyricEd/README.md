# LyricEd
#### Aseel Kayal, Yotam Sechayk

### API's Used

- Genius https://docs.genius.com/
- YouTube https://developers.google.com/youtube/
- Last.FM https://www.last.fm/api
- Words API https://www.datamuse.com/api/

## OVERVIEW

## What does our application do?

Our website teaches language through music. After creating an account, the user looks for
the songs they are interested in using various filters we offer, and the website displays the
chosen song’s video as well as its lyrics missing certain words. The user then has to fill the
lyrics as they listen to the song.

## Goals

1. Expand the user’s vocabulary and enhance their listening skills
2. Teach users new english words and their meaning.
3. Introduce the users to new songs and genres.
4. Showcase how songs and music could help in learning a new language.


## SCREENS

## Home Screen

The home screen introduces the user to the website and its goals, and displays the top hits
in our database, songs recently played by our users, and the top users with the highest
scores. Additionally, it leads to the rest of the screens and allows the user to log into their
account, view their account, and search for their favorite hits.

![](screenshots/home.png?raw=true)

## Login Screen

The login screen can be used to log into an existing account, or to create a new one. If the
user logs in with the wrong credentials, or tries to create an account with an existing e-mail
or username, an error message is shown.

![](screenshots/login.png?raw=true)

## Profile Screen

The profile screen can be accessed only after a user has successfully logged in. It shows
some of their statistics, including their average score, highest score, average mistakes per
song and number of songs played. Additionally, a list of the user’s saved songs are
displayed for their convenience, as well as a list of the songs they have recently practiced.
The user can choose to play the songs in both lists, or remove songs from the “play later”
list. This section can be accessed only after the user has logged in.

![](screenshots/profile.png?raw=true)

## Search Screen

The search screen offers a variety of filters, enabling the user to look for their favorite music.
The user can query our database using a simple text search to find a specific keyword, or
choose a more complex search that includes a difficulty level, top tags, or even the duration
and the release year of a song. The results which meet the criteria the user has set are
displayed at the bottom of the page. The user can then choose one song to practice
immediately after their search,and save other results for later. This section can be accessed
only after the user has logged in.

![](screenshots/search.png?raw=true)

## Play Screen

The play screen contains the chosen song, and missing words. The number of missing
words changes according to the difficulty level: the easier the level is, the less words are
hidden from the user. The page also displays relevant information about the song, such as
its duration, release year, and tags which the user can alter. When pressing submit, the
user’s score and the correct words are displayed. This page can only be accessed after the
user has successfully logged in.

![](screenshots/play.png?raw=true)

## DB Update

The database is updated in the following cases:

- When the user creates an account, a new entry is added to the database including the
    user’s personal information
- When the user adds a song to their “keep” list to practice it later, an entry containing the
    user’s and song’s IDs is added to the database.
- When the user practices a song and presses on “Submit” in the play section, an entry is
    created containing the user’s ID and the song’s information
- When the user adds or removes tags for a specific song, the tags in the database are
    updated accordingly


