# Gemini Question Generator Module

This module integrates Google Gemini API to generate structured multiple-choice questions.

## Files

- `gemini_client.py` — Handles Gemini API connection
- `question_generator.py` — Builds and validates MCQ output
- `run_generate.py` — runner for testing

## Setup

1. Create a `.env` file in the project root:

GEMINI_API_KEY=your_api_key_here  
GEMINI_MODEL=gemini-2.5-flash  

2. Install dependencies:

pip install dotenv

3. Run:

python run_generate.py

## Output

Generates structured JSON containing:
- topic
- difficulty
- question
- choices
- answer_index
- explanation
  <img width="1800" height="444" alt="image" src="https://github.com/user-attachments/assets/609acad5-1687-4b78-8417-94253f819a52" />
