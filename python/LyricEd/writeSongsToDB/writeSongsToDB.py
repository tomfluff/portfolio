import sys
import glob
import os
import ast
import csv
import pymysql
import urllib3
import json
import re
import codecs
from pymysql.constants import ER


# global vocabulary dictionary
vocab = dict()


def add_song_to_db(cursor, song_id, title, artist, release_year, difficulty, lyrics, popularity, video_url, length,
                   image_url):
    # print("-> Adding song: {0}".format(song_id))
    sql_stt = "INSERT INTO song(id,title,artist,release_year,difficulty,lyrics,popularity,video_url,length,image_url) VALUES(%s,%s,%s,%s,%s,%s,%s,%s,%s,%s)"
    release_year = int(release_year) if release_year else None
    difficulty = float(difficulty) if difficulty else None
    popularity = int(popularity) if popularity else None
    length = int(length) if length else None
    cursor.execute(sql_stt, args=(song_id, title, artist, release_year, difficulty, lyrics, popularity, video_url, length, image_url), )


def add_tags_for_song(cursor, song_id, tags):
    # print("-> Adding tags for song: {0}".format(song_id))
    sql_stt = "INSERT INTO song_tags(song_id,tag) VALUES(%s,%s)"
    for tag in tags:
        cursor.execute(sql_stt, args=(song_id, tag))


def add_words_frequency(cursor, words):
    global vocab

    # print("-> Adding words with frequency")
    sql_stt = "INSERT INTO word_frequency(word,frequency) VALUES(%s,%s)"
    for word in words:
        freq = -1
        try:
            if word in vocab:
                freq = vocab[word]
            else:
                url = "http://api.datamuse.com/words?sp={0}&md=f&max=1".format(word)
                req = urllib3.PoolManager().request('GET', url)
                j_content = json.loads(req.data.decode('utf-8'))
                if len(j_content) > 0:
                    freq = float(j_content[0]['tags'][-1][2:])
            if freq > -1:
                cursor.execute(sql_stt, args=(word, freq))
        except pymysql.IntegrityError as er:
            code = int(str(er)[1:5])
            # print("--> {0}.".format(er))
            if code == ER.DUP_ENTRY:
                pass
        except Exception:
            raise


def add_words_for_song(cursor, song_id, words):
    # print("-> Adding words for song: {0}".format(song_id))
    sql_stt = "INSERT INTO word_in_song(song_id,word) VALUES(%s,%s)"
    for word in words:
        cursor.execute(sql_stt, args=(song_id,word))


def ignore_song(difficulty, lyrics):
    if float(difficulty) > 8.5 or re.search('([lL]yrics will be)|(Instrumental)', lyrics) is not None:
        return True
    else:
        return False


def main():
    global vocab

    arguments = sys.argv[1:]

    # get vocabulary file
    os.chdir(arguments[0])
    for file in glob.glob('*.txt'):
        with open(file, 'r') as vocab_file:
            print(file)
        d_str = vocab_file.read().replace('\'','\"')
            vocab.update(json.loads(d_str))
            print(len(vocab))

    print("Creating DB connection...")
    mydb = pymysql.connect(
        host='localhost',
        port=3305,
        user='DbMysql16',
        passwd='DbMysql16',
        db='DbMysql16',
        charset='utf8', use_unicode=True)
    cursor = mydb.cursor()

    # name of csv which includes song details with 12 different attributes
    for file in glob.glob('*.csv'):
        print("Reading from file:", file)
        with codecs.open(file, 'r', encoding='utf-8') as f:
            csv_data = csv.reader(f)
            # skip names of attributes row
            next(csv_data)
            for row in csv_data:
                row_u = [r.encode('utf-8').decode('utf-8') for r in row]
                # [0]=id, [1]=title, [2]=artist, [3]=release_year,
                # [4]=difficulty, [5]=lyrics, [6]=popularity, [7]=video_url,
                # [8]=length, [9]=image_url, [10]=lyrics_words, [11]=tags
                add_all = True
                if not ignore_song(row_u[4], row_u[5]):
                    try:
                        add_song_to_db(cursor, row_u[0], row_u[1], row_u[2], row_u[3], row_u[4], row_u[5], row_u[6], row_u[7], row_u[8], row_u[9])
                    except pymysql.IntegrityError as er:
                        # if song already exists skip it
                        code = int(str(er)[1:5])
                        # print("--> {0}.".format(er))
                        if code == ER.DUP_ENTRY:
                            add_all = False
                    if add_all:
                        add_tags_for_song(cursor, row[0], [w for w in ast.literal_eval(row[11])])
                        add_words_frequency(cursor, ast.literal_eval(row[10]))
                        add_words_for_song(cursor, row[0], ast.literal_eval(row[10]))
                        mydb.commit()

    cursor.close()
    print("Finished")


if __name__ == '__main__':
    main()
