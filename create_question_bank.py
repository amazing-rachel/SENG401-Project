# create question bank to store question in database
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

cursor.execute("""
CREATE TABLE IF NOT EXISTS question_bank (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    topic TEXT,
    difficulty TEXT,
    question_text TEXT UNIQUE,
    choice_a TEXT,
    choice_b TEXT,
    choice_c TEXT,
    choice_d TEXT,
    correct_answer TEXT,
    explanation TEXT
)
""")

conn.commit()
conn.close()

print("question_bank table created.")
