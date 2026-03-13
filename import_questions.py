# import questions into database

import json
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

with open("questions_seed.json", "r") as f:
    questions = json.load(f)

for q in questions:

    choices = q["choices"]
    answer_index = q["answer_index"]
    correct_answer = choices[answer_index]

    cursor.execute("""
    INSERT OR IGNORE INTO question_bank (
        topic,
        difficulty,
        question_text,
        choice_a,
        choice_b,
        choice_c,
        choice_d,
        correct_answer,
        explanation
    )
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (
        q["topic"],
        q["difficulty"],
        q["question"],
        choices[0],
        choices[1],
        choices[2],
        choices[3],
        correct_answer,
        q["explanation"]
    ))

conn.commit()
conn.close()

print("Questions imported successfully.")
