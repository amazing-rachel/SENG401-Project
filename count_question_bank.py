# count if the total number of questions inport to database is correct
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

cursor.execute("SELECT COUNT(*) FROM question_bank")
count = cursor.fetchone()[0]

print(f"Total questions in question_bank: {count}")

cursor.execute("""
SELECT difficulty, COUNT(*)
FROM question_bank
GROUP BY difficulty
ORDER BY difficulty
""")

rows = cursor.fetchall()

print("Count by difficulty:")
for row in rows:
    print(row)

conn.close()

cursor.execute("""
SELECT topic, COUNT(*)
FROM question_bank
GROUP BY topic
ORDER BY topic
""")

rows = cursor.fetchall()

print("Count by topic:")
for row in rows:
    print(row)
