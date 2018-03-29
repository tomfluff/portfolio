class Song:
    def __init__(self, song_id, title, artist, release_year, difficulty, lyrics, popularity, video_url, length, image_url):
        self.id = song_id
        self.title = title
        self.artist = artist
        self.release_year = release_year if  release_year else '?'
        self.difficulty = difficulty
        self.lyrics = lyrics
        self.popularity = popularity
        self.video_url = video_url
        self.length = length
        self.image_url = image_url

    def get_str_len(self):
        return "%02d:%02d" % divmod(int(self.length), 60)


class User:
    def __init__(self, user_id, email, name):
        self.id = user_id
        self.name = name
        self.email = email


class HistoryItem:
    def __init__(self, song, date, score, mistakes):
        self.song = song
        self.date = date
        self.score = score
        self.mistakes = mistakes

