from fastapi import FastAPI 
from pydantic import BaseModel 
import sqlite3 
from fastapi.middleware.cors import CORSMiddleware 
import os 


app = FastAPI() 
 

# Allow Unity / WebGL to access the backend 

app.add_middleware( 
    CORSMiddleware, 
    allow_origins=["*"],  # for testing only; replace with your site URL in production 
    allow_methods=["*"], 
    allow_headers=["*"], 
) 

 
# Request model 

class User(BaseModel): 
    username: str 
    password: str 

 
class GameResult(BaseModel):
    username: str
    result: str   # "win" or "loss"

# Setup SQLite database 


DB_PATH = os.path.join(os.getcwd(), "users.db")  # Render will create this automatically 

conn = sqlite3.connect(DB_PATH, check_same_thread=False) 

cursor = conn.cursor() 

# Users Table
cursor.execute(""" 

CREATE TABLE IF NOT EXISTS users ( 
    username TEXT PRIMARY KEY, 
    password TEXT NOT NULL 
) 

""") 

# Game Results Table
cursor.execute("""
CREATE TABLE IF NOT EXISTS game_results (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT,
    result TEXT,
    played_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(username) REFERENCES users(username)
)
""")

conn.commit() 


# Register endpoint 


@app.post("/register") 

def register(user: User): 

    username = user.username.strip() 
    password = user.password.strip() 
 

    print(f"Register attempt: {repr(username)}, {repr(password)}") 


    if not username or not password: 
        return {"status": "empty_fields"} 

 

    cursor.execute("SELECT * FROM users WHERE username=?", (username,)) 

    if cursor.fetchone(): 
        print("Username already exists in DB") 
        return {"status": "username_taken"} 


    cursor.execute("INSERT INTO users (username, password) VALUES (?, ?)", (username, password)) 

    conn.commit() 

    print("Account successfully created") 

    return {"status": "account_created"} 



# Login endpoint 


@app.post("/login") 

def login(user: User): 

    username = user.username.strip() 
    password = user.password.strip() 


    if not username or not password: 
        return {"status": "empty_fields"} 

 

    cursor.execute("SELECT * FROM users WHERE username=? AND password=?", (username, password)) 

    if cursor.fetchone(): 
        print("Login successful") 
        return {"status": "success"} 

 

    print("Login failed") 

    return {"status": "invalid"} 



# Save Game Result endpoint


@app.post("/save_results")
def save_results(data: GameResult):

    username = data.username.strip()
    result = data.result.strip().lower()

    if result not in ["win", "loss"]:
        return {"status": "invalid_result"}

    cursor.execute(
        "INSERT INTO game_results (username, result) VALUES (?, ?)",
        (username, result)
    )

    conn.commit()

    print(f"Saved result: {username} -> {result}")

    return {"status": "saved"}



# Show Player Stats


@app.get("/stats/{username}")
def get_stats(username: str):
    cursor.execute(
        "SELECT COUNT(*) FROM game_results WHERE username=? AND result='win'",
        (username,)
    )
    wins = cursor.fetchone()[0]

    cursor.execute(
        "SELECT COUNT(*) FROM game_results WHERE username=? AND result='loss'",
        (username,)
    )
    losses = cursor.fetchone()[0]

    return {
        "username": username,
        "wins": wins,
        "losses": losses
    }