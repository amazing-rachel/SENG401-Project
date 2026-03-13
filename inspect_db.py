# use to check the database structure
import sqlite3

conn = sqlite3.connect("database.db")
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
tables = cursor.fetchall()

print("Tables:")
for table in tables:
    print(table[0])

print("\nSchema:")

for table in tables:
    table_name = table[0]
    print(f"\n--- {table_name} ---")
    cursor.execute(f"PRAGMA table_info({table_name});")
    columns = cursor.fetchall()
    for col in columns:
        print(col)

conn.close()
