from flask import Flask, render_template, request, Markup, session, redirect, url_for
from flask_socketio import SocketIO
from db_communication import *
import re, datetime
from passlib.hash import pbkdf2_sha256

app = Flask(__name__)

app.config['SECRET_KEY'] = '^4pP$3cR3T!~!'
socketio = SocketIO(app)


@socketio.on('disconnect')
def disconnect_user():
    session.clear()
    # print(session)


@app.route("/login", methods=['POST', 'GET'])
def login():
    if request.method == 'GET':
        if 'logged_in' in session:
            if session['logged_in']:
                return redirect(url_for('profile'))
        else:
            return render_template('login.html')
    elif request.method == 'POST':
        if (request.form['btn'] == 'create'):
            user_id = create_user(request.form['name'], request.form['email'],
                                  pbkdf2_sha256.encrypt(request.form['pass'], rounds=200000, salt_size=16))
            if (user_id is not None):
                session['user_id'] = user_id
                session['logged_in'] = True
                session['user_name'] = request.form['name']
                return redirect(url_for('main'))
            else:
                error_message = Markup('<div class="alert alert-danger" role="alert"> \
                                        <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> \
                                        <span class="sr-only">Error:</span> \
                                        This e-mail address or username is already in use \
                                        </div>')
                return render_template('login.html', error=True, errorMessage=error_message)
        else:
            # print('needs to authenticate')
            user_id = authenticate_user(request.form['email'], request.form['pass'])
            if (user_id):
                # print('authenticated!')
                user = get_user_details(user_id)
                session['user_id'] = user.id
                session['user_name'] = user.name
                session['logged_in'] = True
            else:
                error_message = Markup('<div class="alert alert-danger" role="alert"> \
                                        <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> \
                                        <span class="sr-only">Error:</span> \
                                        Username or password are incorrect \
                                        </div>')
                session['logged_in'] = False
                return render_template('login.html', error=True, errorMessage=error_message)

        return redirect(url_for('.main'))


@app.route("/search")
def search():
    logged = False
    user_log = ''
    if (session):
        if 'logged_in' not in session:
            return redirect(url_for('login'))
        else:
            if session['logged_in']:
                logged = True
                user_log = Markup('<a class="nav-link" href="/profile"> Hello, {0} </a>'.format(session['user_name']))
            else:
                return redirect(url_for('login'))
    else:
        return redirect(url_for('login'))

    all_tags = get_top_tags()
    difficulty = request.args.get("diff")
    tags = request.args.get("tgs")
    year = request.args.get("yr")
    length = request.args.get("len")
    search = request.args.get("sch")
    songs = None
    keep_songs = None

    difficulty, length, search, tags, year = handle_search_variables(difficulty, length, search, tags, year)

    if len(request.args) > 0:
        songs = get_songs_by_filters(difficulty, tags, search, year, length)
        keep_songs, _  = get_user_keep(session['user_id'])

    # print(difficulty, tags, year, length, search, songs)

    return render_template('search.html', logged_in=logged, profile_login=user_log, user_id=session['user_id'],
                           this_year=datetime.datetime.now().timetuple()[0], search=search,
                           difficulty=difficulty, tag_list=tags, all_tags=all_tags, release_year=year, length=length,
                           songs=songs, keep_songs=keep_songs)


def handle_search_variables(difficulty, length, search, tags, year):
    # search term
    if not search:
        search = ''

    # tags
    if tags:
        tags = tags.split(',')

    # year
    if not year:
        year = -1
    else:
        year = int(year)

    # length
    if not length or length == '-1':
        length = -1
    else:
        length = int(length)

    # difficulty
    if not difficulty or difficulty == '-1':
        difficulty = -1
    else:
        # assume it's a search request so ned to fetch data
        difficulty = int(difficulty)
    return difficulty, length, search, tags, year


@app.route("/profile")
def profile():
    logged = False
    user_log = ''
    if (session):
        if 'logged_in' in session:
            if session['logged_in']:
                stats = get_user_stats(session['user_name'])
                _, songs = get_user_keep(session['user_id'])
                history_items = get_user_history(session['user_id'])
                return render_template('profile.html', user_name=session['user_name'], user_id=session['user_id'],
                                       avg_score=stats['avg_score'],
                                       max_score=stats['max_score'], avg_mistake=stats['avg_mistake'],
                                       song_cnt=stats['song_cnt'], songs=songs, history_items=history_items)
            else:
                return redirect(url_for('login'))
        else:
            return redirect(url_for('login'))
    return redirect(url_for('login'))


@app.route("/logout", methods=['POST'])
def logout():
    if (session):
        if 'logged_in' in session:
            if session['logged_in']:
                # print('disconnecting', session, url_for('main'))
                disconnect_user()
                return json.dumps({'status':'OK','url':url_for('main')})
    else:
        return json.dumps({'status': 'ERROR', 'url': url_for('main')})


@app.route("/play")
def play():
    logged = False
    user_log = ''
    if (session):
        if 'logged_in' not in session:
            return redirect(url_for('login'))
        else:
            if session['logged_in']:
                logged = True
                user_log = Markup('<a class="nav-link" href="/profile"> Hello, {0} </a>'.format(session['user_name']))
            else:
                return redirect(url_for('login'))
    else:
        return redirect(url_for('login'))

    difficulty = request.args.get("diff")
    song_id = request.args.get("sg")
    if not song_id or not difficulty:
        # display error page
        pass
    song = get_song_details(song_id)
    word_list = get_words_to_hide(song_id, int(difficulty))
    song_tags = get_tags_for_song(song_id)
    lyrics = song.lyrics.replace('\n', '<br>')
    html_to_replace = Markup('<div class="form-group"> \
                                <div class="input-group"> \
                                    <input type="text" id="{0}" class="missing-word form-control" size="{1}" maxlength="{1}" data-word="{3}"> \
                                    <span class="glyphicon glyphicon-eye-open input-group-addon" data-toggle="tooltip" data-placement="bottom" aria-hidden="true" title="{2}"> \
                                          </span> \
                                </div> \
                      </div>')
    i = 0
    for word in word_list:
        hint = get_hint_for_word(word, int(difficulty))
        lyrics, k = re.subn(r'\s({0})\s'.format(word),
                            html_to_replace.format("word-{0}".format(i), len(word) + 1, hint, word),
                            lyrics, count=max([1, int(difficulty) - 1]))
        i += k

    m, s = divmod(int(song.length), 60)
    song_duration = "%02d:%02d" % (m, s)
    video_url = song.video_url[song.video_url.index('v=') + 2:]
    keep_ids, _ = get_user_keep(session['user_id'])
    keep_next = get_next_keep(session['user_id'], song_id)
    keep_count = get_keep_count_for_song(song_id)
    play_count = get_play_count_for_song(song_id)

    return render_template('play.html', logged_in=logged, profile_login=user_log, user_id=session['user_id'],
                           song_id=song_id, lyrics=lyrics,
                           song_tags=song_tags, song_artist=song.artist,
                           song_title=song.title, release_year=song.release_year, song_duration=song_duration,
                           video_url=video_url, num_of_words=i, difficulty=int(difficulty),
                           keep_ids=keep_ids, keep_next=keep_next, keep_count=keep_count, play_count=play_count)



@app.route("/")
def main():
    logged = False
    user_log = ''
    if (session):
        if 'logged_in' in session:
            if session['logged_in']:
                # display user name in banner
                logged = True
                user_log = Markup('<a class="nav-link" href="/profile"> Hello, {0} </a>'.format(session['user_name']))
    if not logged:
        user_log = Markup('<a class="nav-link" href="/login"> Login </a>')
    songs = get_top_popular()
    players = get_top_users()
    recent = get_recently_played()
    return render_template('home.html', logged_in=logged, profile_login=user_log, songs=songs, players=players, recent_songs=recent)


@app.route("/avg_score")
def avg_score():
    song_id = request.args.get("sg")
    avg = get_avg_score_for_song(song_id)
    return str(avg)


@app.route("/add_tag", methods=['POST'])
def add_tag():
    tag = request.form['tg']
    song_id = request.form['sg']

    if not tag:
        return json.dumps({'status': 'EMPTY'})
    tags = get_tags_for_song(song_id)
    if tag in tags:
        return json.dumps({'status': 'EXISTS'})

    add_tag_for_song(song_id, tag)
    return json.dumps({'status': 'OK'})


@app.route('/remove_tag', methods=['POST'])
def remove_tag():
    # print(request.form)
    tag = request.form['tg']
    song_id = request.form['sg']

    tags = get_tags_for_song(song_id)
    if tag in tags:
        remove_tag_for_song(song_id, tag)
        return json.dumps({'status': 'OK'})

    return json.dumps({'status': 'ERROR'})


@app.route("/add_keep", methods=['POST'])
def add_keep():
    user_id = request.form['us']
    song_id = request.form['sg']

    if not song_id or not user_id:
        return json.dumps({'status': 'EMPTY'})

    keeps_ids,_ = get_user_keep(user_id)
    if song_id not in keeps_ids:
        add_song_to_keep(user_id, song_id)

    return json.dumps({'status': 'OK'})


@app.route('/remove_keep', methods=['POST'])
def remove_keep():
    # print(request.form)
    user_id = request.form['us']
    song_id = request.form['sg']
    keep_ids, _ = get_user_keep(user_id)
    if song_id in keep_ids:
        remove_song_from_keep(user_id, song_id)
        return json.dumps({'status': 'OK'})

    return json.dumps({'status': 'ERROR'})


@app.route('/add_history', methods=['POST'])
def add_history():
    # print (request.form)
    user_id = request.form['us']
    song_id = request.form['sg']
    score = request.form['sc']
    mistakes = request.form['mk']

    if add_song_to_user_history(user_id,song_id,score, mistakes):
        remove_song_from_keep(user_id, song_id)
        return json.dumps({'status': 'OK'})
    else:
        return json.dumps({'status': 'ERROR'})



if __name__ == '__main__':
    # TODO: change for remote admission
    app.run(port=40444, host='0.0.0.0')
    # app.run()


def run_server():
    app.run(port=40444, host='0.0.0.0')