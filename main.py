from fastapi import FastAPI 
from pydantic import BaseModel 
from motor.motor_asyncio import AsyncIOMotorClient
from fastapi.middleware.cors import CORSMiddleware 
import os 
import bcrypt


app = FastAPI() 
 
MONGO_URL = os.getenv("MONGO_URL")

client = AsyncIOMotorClient(MONGO_URL)
db = client["game_db"]

users_collection = db["users"]
results_collection = db["game_results"]

# Allow Unity / WebGL to access the backend 

app.add_middleware( 
    CORSMiddleware, 
    allow_origins=["*"],  # for testing only; replace with site URL when doing Itch.io stuff
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



# Register endpoint 

@app.post("/register") 

async def register(user: User): 

    username = user.username.strip() 
    password = user.password.strip() 
 

    if not username or not password:
        return {"status": "empty_fields"}

    existing_user = await users_collection.find_one({"username": username})

    if existing_user:
        return {"status": "username_taken"}
    
    # Hash password before storing
    hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt())

    await users_collection.insert_one({
        "username": username,
        "password": hashed_password.decode('utf-8')
    })

    return {"status": "account_created"}



# Login endpoint 

@app.post("/login") 

async def login(user: User): 

    username = user.username.strip() 
    password = user.password.strip() 


    if not username or not password: 
        return {"status": "empty_fields"} 

    existing_user = await users_collection.find_one({"username": username})
    if existing_user:
        stored_password = existing_user["password"].encode('utf-8')
        if bcrypt.checkpw(password.encode('utf-8'), stored_password):
            return {"status": "success"}

    return {"status": "invalid"}


# Save Game Result endpoint

@app.post("/save_results")
async def save_results(data: GameResult):

    username = data.username.strip()
    result = data.result.strip().lower()

    if result not in ["win", "loss"]:
        return {"status": "invalid_result"}

    await results_collection.insert_one({
        "username": username,
        "result": result
    })

    return {"status": "saved"}



# Show Player Stats

@app.get("/stats/{username}")
async def get_stats(username: str):

    wins = await results_collection.count_documents({
        "username": username,
        "result": "win"
    })

    losses = await results_collection.count_documents({
        "username": username,
        "result": "loss"
    })

    return {
        "username": username,
        "wins": wins,
        "losses": losses
    }