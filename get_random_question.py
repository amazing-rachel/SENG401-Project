# use to get random questions from the databse. just used for test, or example methond to get qustions. can delete after finish
import sqlite3
import random

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

difficulty = "easy"

cursor.execute("""
SELECT
    id,
    topic,
    difficulty,
    question_text,
    choice_a,
    choice_b,
    choice_c,
    choice_d,
    correct_answer
FROM question_bank
WHERE difficulty = ?
ORDER BY RANDOM()
LIMIT 1
""", (difficulty,))

row = cursor.fetchone()
conn.close()

if row is None:
    print("No question found.")
else:
    (
        q_id,
        topic,
        difficulty,
        question_text,
        choice_a,
        choice_b,
        choice_c,
        choice_d,
        correct_answer
    ) = row

    print(f"ID: {q_id}")
    print(f"Topic: {topic}")
    print(f"Difficulty: {difficulty}")
    print(question_text)
    print(f"A. {choice_a}")
    print(f"B. {choice_b}")
    print(f"C. {choice_c}")
    print(f"D. {choice_d}")
    print(f"Correct Answer: {correct_answer}")
