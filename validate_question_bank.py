# validate the content imported in database, like blank and invalid value
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

cursor.execute("""
SELECT id, question_text
FROM question_bank
WHERE question_text IS NULL OR TRIM(question_text) = ''
""")
bad_questions = cursor.fetchall()

cursor.execute("""
SELECT id, question_text
FROM question_bank
WHERE correct_answer IS NULL OR TRIM(correct_answer) = ''
""")
bad_answers = cursor.fetchall()

cursor.execute("""
SELECT id, question_text
FROM question_bank
WHERE choice_a IS NULL OR choice_b IS NULL OR choice_c IS NULL OR choice_d IS NULL
""")
bad_choices = cursor.fetchall()

print("Empty question_text:", len(bad_questions))
print("Empty correct_answer:", len(bad_answers))
print("Missing choices:", len(bad_choices))

if bad_questions:
    print("Bad question_text rows:", bad_questions[:5])

if bad_answers:
    print("Bad correct_answer rows:", bad_answers[:5])

if bad_choices:
    print("Bad choice rows:", bad_choices[:5])

conn.close()
