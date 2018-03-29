from data_objects import *
from passlib.hash import pbkdf2_sha256
import urllib3
import pymysql
import json
import time
import random
import uuid


# GENERAL

def connect_to_db():
    # TODO: update correct db adds
    mydb = pymysql.connect(host='mysqlsrv.cs.tau.ac.il', port=3306, user='DbMysql16', passwd='DbMysql16',
                           db='DbMysql16',
                           charset='utf8', use_unicode=True)
    # mydb = pymysql.connect(host='localhost', port=3305, user='DbMysql16', passwd='DbMysql16', db='DbMysql16',
    #                        charset='utf8', use_unicode=True)
    cursor = mydb.cursor()

    return mydb, cursor


def get_hint_for_word(word, difficulty):
    res = word

    if difficulty == 2:
        if False:
            url = "http://api.datamuse.com/words?rel_rhy={0}&max=1".format(word)
            req = urllib3.PoolManager().request('GET', url)
            j_content = json.loads(req.data.decode('utf-8'))
            if len(j_content) > 0:
                res = "Rhymes with {0}".format(j_content[0]['word'])
            else:
                res = "Starts with {0} Ends with {1}".format(word[0].upper(), word[-1].upper())
        else:
            res = "Starts with {0}...".format(word[:int((len(word) + 1) / 2)])

    if difficulty == 3:
        res = "Starts with {0}...".format(word[0])

    return res


# Song Related
# ---------------

# SELECT

def get_words_for_song(song_id):
    """
    Gets a dictionary containing: key=word, value=frequency. The list has all the words that are in the given song.
    :param song_id: The requested song ID (String)
    :return: A dictionary of the form {'word': frequency}, where frequency is an integer
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT word_frequency.word, word_frequency.frequency FROM word_frequency JOIN word_in_song ON word_frequency.word = word_in_song.word WHERE word_in_song.song_id LIKE '{0}' ORDER BY word_frequency.frequency DESC".format(
        song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    mydb.close()
    word_frequency = {}
    for pair in result:
        word_frequency[pair[0]] = pair[1]
    return word_frequency


def get_tags_for_song(song_id):
    """
    Gets a list containing all the tags which belong to a song.
    :param song_id: The requested song's ID (String)
    :return: A list of all the song's tags
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT song_tags.tag FROM DbMysql16.song_tags WHERE song_tags.song_id LIKE '{0}'".format(song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    tags = []
    for tag in result:
        tags.append(tag[0])
    mydb.close()
    return tags


def get_song_details(song_id):
    """
    Gets the song details for the song with the given 'song_id'.
    :param song_id: The requested song's id
    :return: The details of a song
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT * FROM song WHERE song.id LIKE '{0}'".format(song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    song = Song(result[0], result[1], result[2], result[3], result[4], result[5], result[6], result[7], result[8],
                result[9])
    mydb.close()
    return song


def get_top_users():
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT user.name,AVG(user_song_history.score) FROM user_song_history JOIN user ON user.id LIKE user_song_history.user_id GROUP BY user_song_history.user_id ORDER BY AVG(user_song_history.score) DESC LIMIT 4"
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    players = []
    for r in result:
        players.append((r[0], "{:.2f}".format(float(r[1]))))
    return players


def get_top_tags():
    """
    Returns a dictionary containing the most common tags and the number of entries in descending order.
    :return: A dictionary of the form {'tag': occurrences}
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT * FROM DbMysql16.top_tags"
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    mydb.close()
    tag_occurence = []
    for pair in result:
        tag_occurence.append(pair[0])
    return tag_occurence


def get_top_popular():
    """
    Returns the 10 most popular songs in the database 
    :return: A list including the songs': artist, songs, image_url and video_url
    """
    mydb, cursor = connect_to_db()
    sql_stt = 'SELECT * FROM DbMysql16.top_popular GROUP BY artist LIMIT 8'
    songs = []
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    mydb.close()
    for res in result:
        songs.append(get_song_details(res[0]))
    return songs


def get_recently_played():
    mydb, cursor = connect_to_db()
    sql_stt = 'SELECT DISTINCT song_id FROM user_song_history ORDER BY user_song_history.play_date DESC LIMIT 4'
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    songs = []
    for item in result:
        song = get_song_details(item[0])
        songs.append(song)
    return songs


def get_related_words(theme_word):
    """
    Gets a dictionary of words from the database containing: key=word, value=frequency. All the returned words are related to the theme_word. By a relation such as "result_word IS A theme_word".
    :param theme_word: The theme word to relate the results to.
    :return: A dictionary of words of the form {'word': frequency}
    """
    url = "http://api.datamuse.com/words?ml=" + theme_word
    req = urllib3.PoolManager().request('GET', url)
    j_content = json.loads(req.data.decode('utf-8'))
    word_list = []
    for lst in j_content:
        word_list.append(lst['word'])
    mydb, cursor = connect_to_db()
    word_frequency = {}
    for word in word_list:
        sql_stt = "SELECT word_frequency.frequency FROM word_frequency WHERE word_frequency.word LIKE '{0}'".format(
            word)
        if (cursor.execute(sql_stt) > 0):
            word_frequency[word] = cursor.fetchone()[0]
    mydb.close()
    return word_frequency


def get_songs_by_filters(difficulty, tags=None, search=None, year=None, length=None, popular=False):
    """
    Gets a list of song IDs based on a search by parameters. The search depands on what arguments are passed to the function. The list will be limited to 30 results.
    :param length: The song length top limit
    :param year: The song release year
    :param difficulty: The required difficulty level
    :param tags: A list of tags the songs need to have (one or more per song)
    :param search: The search term to search by
    :param popular: 'True' if songs needs to be sorted by popularity, 'False' means the songs will be sorted by release year (newest first)
    :return: A list of songs ID for the search terms
    """
    mydb, cursor = connect_to_db()
    songs = []

    # set correct difficulty
    if not difficulty or difficulty == -1:
        difficulty_part = ""
    else:
        difficulty_part = "AND song.difficulty >= {0} AND song.difficulty <= {1} ".format(difficulty + 4,
                                                                                          difficulty + 5)

    # set correct length
    if not length or length == -1:
        new_length = 60 * 60
    else:
        new_length = int(length) * 60

    # search always uses difficulty
    # ---
    if not tags or len(tags) <= 0:
        tag_part = ""
    else:
        tag_part = "AND song.id IN (SELECT " \
                   "song_tags.song_id " \
                   "FROM " \
                   "song_tags " \
                   "WHERE " \
                   "song_tags.tag LIKE '{0}'".format(tags[0])
        for tag in tags[1:]:
            tag_part += " OR song_tags.tag LIKE '{0}'".format(tag)
        tag_part += ") "
    if not year or year == -1:
        year_part = ""
    else:
        year_part = "AND song.release_year = {0} ".format(year)

    sql_stt = "SELECT DISTINCT " \
              "song.id " \
              "FROM " \
              "song " \
              "WHERE " \
              "(song.title LIKE '%{0}%' " \
              "OR song.artist LIKE '%{0}%' " \
              "OR song.id IN (SELECT " \
              "song_tags.song_id " \
              "FROM " \
              "song_tags " \
              "WHERE " \
              "song_tags.tag LIKE '%{0}%')) " \
              "AND song.length <= {1} " \
              "{2}" \
              "{3}" \
              "{4}" \
              "ORDER BY song.popularity DESC " \
              "LIMIT 200".format(search, new_length, year_part, tag_part, difficulty_part)

    # print(sql_stt)

    cursor.execute(sql_stt)
    result = cursor.fetchall()
    # print(result)
    mydb.close()

    for r in result:
        songs.append(get_song_details(r[0]))

    return songs


def get_songs_by_words(words):
    """
    Gets a list of song IDs that has one or more of the words in the passed word list. Song difficulty filter may be used.
    :param words: A list of words to search for
    :param difficulty: The requested difficulty level (None := not important)
    :return: A list of songs ID based on the parameters
    """
    mydb, cursor = connect_to_db()
    song_ids = []
    for word in words:
        sql_stt = "SELECT word_in_song.song_id FROM word_in_song WHERE word_in_song.word LIKE '{0}'".format(word)
        cursor.execute(sql_stt)
        result = cursor.fetchall()
        for song_id in result:
            song_ids.append(song_id[0])
    song_ids = set(list(song_ids))
    mydb.close()
    return song_ids


def get_words_to_hide(song_id, difficulty):
    word_dict = get_words_for_song(song_id)
    num_of_words = len(word_dict)
    n = 0
    diff_range = (0, 0)

    # print(num_of_words, word_dict, n, diff_range, difficulty)

    if difficulty == 1:
        if num_of_words < 10:
            n = num_of_words
        else:
            n = max([num_of_words / 7, 10])
        diff_range = (0, n)
    if difficulty == 2:
        if num_of_words < 25:
            n = num_of_words
        else:
            n = max([2 * num_of_words / 7, 25])
        diff_range = (0, max([n, 2 * num_of_words / 3]))
    if difficulty == 3:
        if num_of_words < 35:
            n = num_of_words
        else:
            n = max([3 * num_of_words / 7, 35])
        diff_range = (max(0, num_of_words / 2 - n / 2), num_of_words)

    # print(diff_range, int(diff_range[0]), int(diff_range[1]), n)
    words_to_hide = random.sample(list(word_dict.keys())[int(diff_range[0]):int(diff_range[1])], int(n))

    return words_to_hide


def get_avg_score_for_song(song_id):
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT AVG(user_song_history.score) FROM user_song_history WHERE user_song_history.song_id LIKE '{0}'".format(
        song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    if not result[0]:
        return 0
    return result[0]


def get_next_keep(user_id, song_id):
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT user_song_keep.song_id FROM DbMysql16.user_song_keep WHERE user_song_keep.user_id LIKE '{0}' AND user_song_keep.song_id NOT LIKE '{1}' ORDER BY user_song_keep.add_date DESC".format(
        user_id, song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    return result[0] if result else ''


# INSERT/UPDATE

def add_tag_for_song(song_id, tag):
    """
    Add a tag to a song with 'song_id'.
    :param song_id: The song ID to add for
    :param tag: The tag to add
    :return: True/False if operation was successful
    """
    mydb, cursor = connect_to_db()
    sql_stt = "INSERT INTO song_tags (song_id, tag) VALUES(%s,%s)"
    cursor.execute(sql_stt, args=(song_id, tag))
    mydb.commit()
    mydb.close()
    return True


def remove_tag_for_song(song_id, tag):
    """
    Remove a tag from a song with 'song_id'.
    :param song_id: The song ID to remove for
    :param tag: The tag to remove
    :return: True/False if operation was successful
    """
    mydb, cursor = connect_to_db()
    sql_stt = "DELETE FROM song_tags WHERE song_tags.song_id LIKE '{0}' AND song_tags.tag LIKE'{1}'".format(song_id,
                                                                                                            tag)
    cursor.execute(sql_stt)
    mydb.commit()
    mydb.close()
    return True


# User Related
# ---------------

# SELECT


def get_user_details(user_id):
    """
    Gets the user's details.
    :param user_id: The requested user's ID
    :return: The user's details
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT * FROM user WHERE user.id LIKE '{0}'".format(user_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    user = User(result[0], result[1], result[2])
    return user


def get_user_keep(user_id):
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT user_song_keep.song_id FROM DbMysql16.user_song_keep WHERE user_song_keep.user_id LIKE '{0}' ORDER BY user_song_keep.add_date DESC".format(
        user_id)
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    mydb.close()
    song_ids = []
    songs = []

    for r in result:
        song_ids.append(r[0])
        songs.append(get_song_details(r[0]))

    return song_ids, songs


def get_user_history(user_id):
    """
    Gets a a list containing elements from the user's history log, sorted by date (newest first). The list will be limited to 30 entries.
    :param user_id: The user's ID to fetch for
    :return: A list of history elements.
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT * FROM user_song_history WHERE user_song_history.user_id LIKE '{0}' ORDER BY user_song_history.play_date DESC LIMIT 30".format(
        user_id)
    cursor.execute(sql_stt)
    result = cursor.fetchall()
    mydb.close()
    history_items = []
    for item in result:
        history = HistoryItem(get_song_details(item[1]), item[3], item[2], item[4])
        history_items.append(history)
    return history_items


def get_user_stats(user_name):
    """
    Gets a dictionary containing user statistical data, such as: average score, played songs count, average mistakes per song, etc.
    :param user_id: The user's ID to fetch for
    :return: A dictionary of the form {'statistic name': value}
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT AVG(score), MAX(score), AVG(number_of_mistakes), COUNT(user_song_history.song_id) FROM user_song_history WHERE user_song_history.user_id LIKE (SELECT user.id FROM user WHERE user.name LIKE '{0}')".format(
        user_name)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    statistics = dict()
    if (result[0] is None):
        statistics['avg_score'] = '-'
    else:
        statistics['avg_score'] = "{:.2f}".format(float(result[0]))
    if (result[1] is None):
        statistics['max_score'] = '-'
    else:
        statistics['max_score'] = "{:.2f}".format(float(result[1]))
    if (result[2] is None):
        statistics['avg_mistake'] = '-'
    else:
        statistics['avg_mistake'] = "{:.2f}".format(float(result[2]))
    if (result[3] == 0):
        statistics['song_cnt'] = '-'
    else:
        statistics['song_cnt'] = result[3]
    return statistics


# INSERT/UPDATE

def create_user(user_name, user_email, user_pass):
    """
    Creates a new user in the database
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT * FROM user WHERE user.email LIKE '{0}'".format(user_email)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    if result is None:
        user_id = str(uuid.uuid4())
        sql_stt = "INSERT INTO user(id,email,name,password) VALUES('{0}','{1}','{2}','{3}')".format(user_id, user_email,
                                                                                                    user_name,
                                                                                                    user_pass)
        cursor.execute(sql_stt)
        mydb.commit()
        mydb.close()
        return user_id
    else:
        mydb.close()
        return None


def authenticate_user(user_email, user_pass):
    """
    Checks if a user exists in the database
    """
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT user.password, user.id FROM user WHERE user.email LIKE '{0}'".format(user_email)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    if (result is None):
        # print('not such user')
        return None
    else:
        # print('user found, checking')
        if (pbkdf2_sha256.verify(user_pass, result[0])):
            return result[1]
    return None


# currently not used
def update_user_name(user_id, new_name):
    """
    Update the name of the user.
    :param user_id: The user's ID value
    :param new_name: The new name of the user
    :return: True/False if operation was successful
    """
    mydb, cursor = connect_to_db()
    sql_stt = "UPDATE user SET user.name = '" + new_name + "' WHERE user.user_id LIKE '" + user_id + "'"
    cursor.execute(sql_stt)
    mydb.commit()
    mydb.close()
    return True


def add_song_to_user_history(user_id, song_id, score, number_of_mistakes):
    """
    Adds a finished song to the user's history.
    :param user_id: The user's ID
    :param song_id: The song's ID
    :param score: The score of the user on the song
    :param number_of_mistakes: The number of mistakes made on this song
    :param favorite: True/False if the user added the song to the favorite
    :return: True/False if operation was successful
    """
    mydb, cursor = connect_to_db()
    sql_stt = "INSERT INTO user_song_history(user_id,song_id,score,play_date,number_of_mistakes) VALUES(%s,%s,%s,%s,%s)"
    cursor.execute(sql_stt, args=(user_id, song_id, score, time.strftime('%Y-%m-%d %H:%M:%S'), number_of_mistakes))
    mydb.commit()
    mydb.close()
    return True


def add_song_to_keep(user_id, song_id):
    """
    Add a song to the user's watch later list.
    :param user_id: The user's ID
    :param song_id: The song's ID
    :return: True/False if operation was successful
    """
    mydb, cursor = connect_to_db()
    sql_stt = "INSERT INTO user_song_keep(user_id,song_id,add_date) VALUES(%s,%s,%s)".format(
        user_id, song_id)
    cursor.execute(sql_stt, args=(user_id, song_id, time.strftime('%Y-%m-%d %H:%M:%S')))
    mydb.commit()
    mydb.close()
    return True


def remove_song_from_keep(user_id, song_id):
    mydb, cursor = connect_to_db()

    sql_stt = "DELETE FROM user_song_keep WHERE user_song_keep.song_id LIKE '{0}' AND user_song_keep.user_id LIKE'{1}'".format(
        song_id, user_id)
    cursor.execute(sql_stt)
    mydb.commit()
    mydb.close()
    return True


def get_play_count_for_song(song_id):
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT COUNT(*) FROM user_song_history WHERE user_song_history.song_id LIKE '{0}'".format(song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    if not result:
        return 0
    return result[0]

def get_keep_count_for_song(song_id):
    mydb, cursor = connect_to_db()
    sql_stt = "SELECT COUNT(*) FROM user_song_keep WHERE user_song_keep.song_id LIKE '{0}'".format(song_id)
    cursor.execute(sql_stt)
    result = cursor.fetchone()
    mydb.close()
    if not result:
        return 0
    return result[0]