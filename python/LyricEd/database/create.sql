# Create schemas

# Create tables
CREATE TABLE IF NOT EXISTS song
(
    id VARCHAR(63) NOT NULL,
    title VARCHAR(63) NOT NULL,
    artist VARCHAR(63),
    release_year INT,
    difficulty FLOAT(0),
    lyrics VARCHAR(16383) NOT NULL,
    popularity INT,
    video_url VARCHAR(127) NOT NULL,
    length INT,
    image_url VARCHAR(127),
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS word_in_song
(
    song_id VARCHAR(63) NOT NULL,
    word VARCHAR(63) NOT NULL
);

CREATE TABLE IF NOT EXISTS song_tags
(
    song_id VARCHAR(63) NOT NULL,
    tag VARCHAR(63) NOT NULL
);

CREATE TABLE IF NOT EXISTS user_song_history
(
    user_id VARCHAR(63) NOT NULL,
    song_id VARCHAR(63) NOT NULL,
    score FLOAT(0),
    play_date DATETIME,
    favorite TINYINT(1) NOT NULL,
    number_of_mistakes INT
);

CREATE TABLE IF NOT EXISTS user
(
    id VARCHAR(63) NOT NULL,
    email VARCHAR(63) NOT NULL,
    name VARCHAR(63),
    password VARCHAR(255),
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS song_playlist
(
    playlist_id VARCHAR(63) NOT NULL,
    song_id VARCHAR(63) NOT NULL
);

CREATE TABLE IF NOT EXISTS user_song_keep
(
    user_id VARCHAR(63) NOT NULL,
    song_id VARCHAR(63) NOT NULL,
    add_date DATETIME
);

CREATE TABLE IF NOT EXISTS playlist
(
    id VARCHAR(63) NOT NULL,
    title VARCHAR(127) NOT NULL,
    description VARCHAR(511),
    create_date DATETIME,
    image_url VARCHAR(127),
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS word_frequency
(
    word VARCHAR(63) NOT NULL,
    frequency FLOAT(0) NOT NULL,
    PRIMARY KEY(word)
);


# Create FKs
ALTER TABLE song_tags
    ADD    FOREIGN KEY (song_id)
    REFERENCES song(id)
    ON DELETE CASCADE
;

ALTER TABLE user_song_history
    ADD    FOREIGN KEY (user_id)
    REFERENCES user(id)
    ON DELETE CASCADE
;

ALTER TABLE user_song_history
    ADD    FOREIGN KEY (song_id)
    REFERENCES song(id)
    ON DELETE CASCADE
;

ALTER TABLE user_song_keep
    ADD    FOREIGN KEY (user_id)
    REFERENCES user(id)
    ON DELETE CASCADE
;

ALTER TABLE user_song_keep
    ADD    FOREIGN KEY (song_id)
    REFERENCES song(id)
    ON DELETE CASCADE
;

ALTER TABLE song_playlist
    ADD    FOREIGN KEY (playlist_id)
    REFERENCES playlist(id)
    ON DELETE CASCADE
;

ALTER TABLE song_playlist
    ADD    FOREIGN KEY (song_id)
    REFERENCES song(id)
    ON DELETE CASCADE
;

ALTER TABLE word_in_song
    ADD    FOREIGN KEY (word)
    REFERENCES word_frequency(word)
    ON DELETE CASCADE
;

ALTER TABLE word_in_song
    ADD    FOREIGN KEY (song_id)
    REFERENCES song(id)
    ON DELETE CASCADE
;


# Create Indexes
CREATE INDEX word_in_song_index ON word_in_song (word);
CREATE INDEX tag_of_song_index ON song_tags (tag);
CREATE INDEX song_title_index ON song (title);
CREATE INDEX song_artist_index ON song (artist);
CREATE INDEX word_index ON word_frequency (word);
CREATE INDEX user_email_index ON user (email);


# Create Views
CREATE VIEW most_played_songs AS
SELECT song.*, COUNT(*) AS play_count
FROM song JOIN user_song_history ON song.id = user_song_history.song_id
GROUP BY song.id
ORDER BY play_count DESC
LIMIT 10;

CREATE VIEW top_tags AS
SELECT song_tags.tag AS tag, COUNT(song_tags.song_id) AS count
FROM song_tags
GROUP BY song_tags.tag
ORDER BY count DESC
LIMIT 30;

CREATE VIEW artists_list AS
    SELECT
        song.artist AS artist,
        COUNT(song.id) AS song_count
    FROM
        song
    GROUP BY song.artist
    ORDER BY COUNT(song.id) DESC;
    
