# check the questions context input in database
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

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
    correct_answer,
    explanation
FROM question_bank
ORDER BY id
LIMIT 20
""")

rows = cursor.fetchall()

for row in rows:
    print(row)
    print("-" * 80)

conn.close()
