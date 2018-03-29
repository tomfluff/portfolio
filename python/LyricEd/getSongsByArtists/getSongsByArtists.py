#!/usr/bin/env python2.7
# -*- coding: utf-8 -*-

import sys
import re
import csv
import codecs
import os
import py_genius
import lxml.html
import requests
import json
import urllib3
import math
from googleapiclient.discovery import build
from bs4 import BeautifulSoup

# global vocabulary (word : frequency) variable (e.g. {'world':120.345,...})
vocab = dict()
# global max song count for artist
ARTIST_SONG_LIMIT = 50


def load_credentials():
    lines = [line.rstrip('\n') for line in open('credentials.ini')]
    client_id = client_secret = client_access_token = ''
    for line in lines:
        if "client_id" in line:
            client_id = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
        if "client_secret" in line:
            client_secret = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
        # Currently only need access token to run, the other two perhaps for future implementation
        if "client_access_token" in line:
            client_access_token = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
    return client_id, client_secret, client_access_token


def setup(name_str):
    if not os.path.exists("output/"):
        os.makedirs("output/")
    outputfilename = "output/" + re.sub(r"[^A-Za-z0-9]+", '', name_str) + ".csv"
    with codecs.open(outputfilename, 'ab', encoding='utf8') as outputfile:
        outwriter = csv.writer(outputfile)
        if os.stat(outputfilename).st_size == 0:
            header = ['id', 'title', 'artist', 'release_year', 'difficulty', 'lyrics', 'popularity', 'video_url',
                      'length', 'image_url', 'lyrics_words', 'tags']
            outwriter.writerow(header)
            return outputfilename
        else:
            return outputfilename


def filter_song_title(song_title):
    # if the title has non conventional characters
    # or 'track list' substring, return True
    pattern = '[^a-zA-Z\s\'.,]|_|([tT]rack[\s]{0,1}[lL]ist)|([dD]iscography)|([cC]over)|([bB]onus[\s]{0,1}[tT]rack)'
    return re.search(pattern, song_title) is not None


def get_song_lyrics(path):
    lyrics = ''
    # gotta go regular html scraping... come on Genius
    page_url = "http://genius.com" + path
    page = requests.get(page_url)
    html = BeautifulSoup(page.text, "html.parser")
    # remove script tags that they put in the middle of the lyrics
    [h.extract() for h in html('script')]
    divs = html.findAll("div")
    for div in divs:
        try:
            if div["class"][0] == 'lyrics':
                lyrics = div.get_text()
                break
        except KeyError:
            pass
    lyrics = re.sub("[(\[].*?[)\]]\n", "", lyrics)
    lyrics = lyrics.replace('\n\n', '\n')

    return lyrics


def get_song_word_set_and_difficulty(lyrics):
    words = []
    freq = 0
    global vocab
    prev_size = len(vocab)
    for w in lyrics.split():
        if str.lower(w) in vocab:
            words.append(str.lower(w))
            freq += vocab[str.lower(w)]
        elif re.search('[^a-zA-Z]', w) is None and len(w) >= 3:
            url = "http://api.datamuse.com/words?sp={0}&md=fps&max=1".format(str.lower(w))
            req = urllib3.PoolManager().request('GET', url)
            j_content = json.loads(req.data.decode('utf-8'))
            if len(j_content) > 0 and j_content[0]['word'] == str.lower(w) and len(
                    j_content[0]['tags']) > 1 and 'u' not in j_content[0]['tags'] and 'prop' not in j_content[0]['tags']:
                w_freq = float(j_content[0]['tags'][-1][2:])
                vocab[str.lower(w)] = w_freq
                words.append(str.lower(w))
                freq += w_freq

    if len(words) == 0:
        return None, 0
    freq = freq / len(words)
    words = list(set(words))
    words = "['" + "','".join([str.lower(x) for x in words]) + "']"
    print("-> Vocabulary updated with {0} new words".format(len(vocab)-prev_size))
    return words, str(2 * abs(math.log10(freq/pow(10, 6))))


def get_length_from_youtube(video_url):
    """
    Gets the song length based on the YouTube API
    :param video_url: the video url in youtube (string), (e.g. http://www.youtube.com/watch?v=JGwWNGJdvx8)
    :return: a string of an integer of length in seconds
    """
    youtube_api = ''
    print("-> Fetching video length from YouTube, {0}".format(video_url))
    lines = [line.rstrip('\n') for line in open('credentials.ini')]
    for line in lines:
        if "youtube_api" in line:
            youtube_api = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
    video_id = video_url[video_url.index('?v=')+3:]
    searchUrl = "https://www.googleapis.com/youtube/v3/videos?id=" + video_id + "&key=" + youtube_api + "&part=contentDetails"
    response = urllib3.PoolManager().request('GET', searchUrl)
    data = json.loads(response.data.decode('utf-8'))
    all_data = data['items']
    if not all_data:
        return None
    contentDetails = all_data[0]['contentDetails']
    duration = contentDetails['duration']
    k = re.split('(\d{1,2}H)?(\d{1,2}M)?(\d{1,3}S)?', duration[2:])
    print("--> YouTube details for length, {0}, {1}".format(duration, k))
    h = m = s = '00'
    for _k in k:
        if not _k:
            continue
        if _k[-1] == 'H':
            h = _k[:-1]
        if _k[-1] == 'M':
            m = _k[:-1]
        if _k[-1] == 'S':
            s = _k[:-1]
    int_length = int(h) * 3600 + int(m) * 60 + int(s)
    length = str(int_length)
    print("--> The video is {0} sec long".format(length))
    return length


def get_video_from_youtube(title, artist):
    """
    Gets url to the video of a song from Youtube.
    :param title: the song title (string)
    :param artist: the song artist (string)
    :return: a string of the video url (e.g. http://www.youtube.com/watch?v=JGwWNGJdvx8)
    """
    youtube_api = ''
    print("-> Fetching YouTube video for {0} - {1}".format(title, artist))
    lines = [line.rstrip('\n') for line in open('credentials.ini')]
    for line in lines:
        if "youtube_api" in line:
            youtube_api = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
    youtube = build('youtube', 'v3', developerKey=youtube_api)
    trm = artist + ' ' + title
    search_response = youtube.search().list(q=trm, part='id', maxResults=1, type="").execute()
    idd = ''
    for search_result in search_response.get('items', []):
        if search_result['id']['kind'] == 'youtube#video':
            idd = search_result['id']['videoId']
    if not idd:
        return None
    video_url = "http://www.youtube.com/watch?v=" + idd
    print("--> Video fetched, {0}".format(video_url))
    return video_url


def get_song_tags(title, artist):
    """
    Gets the tags of a song based on the Last.FM API
    :param title: the song title (string)
    :param artist: the artist name (string)
    :return: a string representing the set of tags, all lower-case (e.g. '['rock','summer',...]')
    """
    last_api = ''
    lines = [line.rstrip('\n') for line in open('credentials.ini')]
    for line in lines:
        if "last_api" in line:
            last_api = re.findall(r'[\"\']([^\"\']*)[\"\']', line)[0]
    url = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=" + last_api + "&artist=" + artist + "&track=" + title
    page = requests.get(url)
    doc = lxml.html.fromstring(page.content)
    tags = []
    for tag in doc.xpath("//tag/name/text()"):
        tags.append(tag.lower())
    return str(tags)


def search(search_term, outputfilename, client_access_token):
    with codecs.open(outputfilename, 'ab', encoding='utf8') as outputfile:
        outwriter = csv.writer(outputfile)
        # initialize genius wrapper
        genius = py_genius.Genius(client_access_token)
        # get search results for artist name
        hits = genius.search(search_term)["response"]["hits"]

        if len(hits) == 0:
            print("No results for: " + search_term)
            return

        # get artist id for song search
        artist_id = hits[0]["result"]["primary_artist"]["id"]
        # initialize song list (preventing duplicates)
        songs_ids_set = set([])

        artist_song_count = 0
        # maximum pages is 50, meaning about 1000 records
        page = 1
        while True:
            if artist_song_count >= ARTIST_SONG_LIMIT:
                # limit number of songs for artist in the database.
                # Ranging from ARTIST_SONG_LIMIT to 2*ARTIST_SONG_LIMIT
                break

            json_obj = genius.get_artist_songs(artist_id, per_page=50, sort='popularity', page=page)
            songs = json_obj["response"]["songs"]

            print("Page {0}; Number of songs {1}".format(page, len(songs)))

            if len(songs) == 0:
                # no songs in the page, end while
                break

            for song in songs:
                song_id = song['id']

                # filter unwanted songs
                if song_id in songs_ids_set \
                        or song["primary_artist"]["id"] != artist_id \
                        or filter_song_title(song["title"]):
                    continue

                song_json = genius.get_song(song_id)
                song_json = song_json['response']['song']

                # song LYRICS
                lyrics = str.strip(get_song_lyrics(song['path']))
                if lyrics is None or not str.strip(lyrics):
                    # if no lyrics, skip song
                    continue

                # song TITLE
                title = song["title"]

                # song ARTIST NAME
                artist_name = song["primary_artist"]["name"]

                print('Processing Song: {0} by {1}'.format(title, artist_name))

                # song RELEASE YEAR
                if song_json['release_date'] is not None:
                    release_year = song_json['release_date'][:4]
                else:
                    release_year = ''

                # song POPULARITY
                popularity = ''
                if 'stats' in song_json and 'pageviews' in song_json['stats']:
                    popularity = song_json['stats']['pageviews']

                # song VIDEO URL
                video = ''
                for m in song_json['media']:
                    if m['provider'] == 'youtube':
                        video = m['url']
                        break
                if not video:
                    # if video has NO url from Genius
                    video = get_video_from_youtube(title, artist_name)
                if not video:
                    # if couldn't find a video, skips song
                    print("--> Skipping song")
                    continue

                # song LENGTH
                length = get_length_from_youtube(video)
                if length is None:
                    # if the video has an error, skip song
                    print("--> Skipping song")
                    continue

                # song IMAGE URL
                image_url = song['header_image_thumbnail_url']

                # song WORD SET, DIFFICULTY
                words, difficulty = get_song_word_set_and_difficulty(lyrics)
                if words is None:
                    # if the song lyrics has an error, skip song
                    print("--> Skipping song")
                    continue

                # song TAGS
                tags = get_song_tags(title, artist_name)

                print('-> Finished: {0} by {1} (difficulty: {2})'.format(title, artist_name, difficulty))

                artist_song_count += 1

                # add finished song to song_id_set
                songs_ids_set.add(song_id)

                # generate CSV row
                row = [song_id, title, artist_name, release_year, difficulty, lyrics, popularity, video, length,
                       image_url, words, tags]
                # write row to CSV
                outwriter.writerow(row)

            # write updated vocabulary
            with open('./vocab.txt', 'w') as file:
                file.write(str(vocab))

            # next result (songs) page
            page += 1


def main():
    global vocab
    arguments = sys.argv[1:]
    outputfilename = setup(arguments[1])
    client_id, client_secret, client_access_token = load_credentials()

    with open('./vocab.txt', 'r') as file:
        d_str = file.read().replace('\'', '\"')
        vocab = json.loads(d_str)

    print("Vocabulary has {0} words".format(len(vocab)))

    with open(arguments[0]) as input_file:
        artists = input_file.readlines()
        artists = [a.strip() for a in artists]

        for search_term in artists:
            print("Searching for \'{0}\'...".format(search_term))
            search(search_term, outputfilename, client_access_token)

    with open('./vocab.txt', 'w') as file:
        file.write(str(vocab))

    print("Updated vocabulary, now has {0} words.".format(len(vocab)))


if __name__ == '__main__':
    main()
