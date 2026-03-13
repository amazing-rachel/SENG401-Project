# check if the correct_answer is in the four options
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

cursor.execute("""
SELECT
    id,
    question_text,
    choice_a,
    choice_b,
    choice_c,
    choice_d,
    correct_answer
FROM question_bank
""")

rows = cursor.fetchall()
bad_rows = []

for row in rows:
    q_id, question_text, a, b, c, d, correct = row
    choices = {a, b, c, d}
    if correct not in choices:
        bad_rows.append((q_id, question_text, correct))

print(f"Rows where correct_answer is not in choices: {len(bad_rows)}")

for row in bad_rows[:10]:
    print(row)

conn.close()
