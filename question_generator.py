import json
import re
from typing import Any

from chatgpt_client import get_gpt_client


QUESTION_SCHEMA_RULES = """
Return valid JSON only.

Required JSON format:
{
  "topic": "string",
  "difficulty": "easy | medium | hard",
  "question": "string",
  "choices": ["string", "string", "string", "string"],
  "answer_index": 0,
  "explanation": "string"
}

Rules:
- exactly 4 choices
- exactly 1 correct answer
- answer_index must be 0, 1, 2, or 3
- the explanation must match the correct answer
- use language suitable for primary school students
- no negative numbers
- no decimals for easy questions
- no fractions unless difficulty is hard
- no trick questions
- the question must be clear and short
- choices should be distinct
- only output JSON, no markdown, no code block, no extra text
"""


def _extract_json(text: str) -> str:
    """
    Extract the first JSON object from the model output.
    """
    text = text.strip()

    if text.startswith("{") and text.endswith("}"):
        return text

    match = re.search(r"\{.*\}", text, re.DOTALL)
    if not match:
        raise ValueError("No JSON object found in model output.")

    json_str = match.group(0).strip()
    json_str = json_str.rstrip(",")
    return json_str


def _validate_question(data: dict[str, Any]) -> None:
    """
    Validate the generated question structure and values.
    """
    required_keys = {
        "topic",
        "difficulty",
        "question",
        "choices",
        "answer_index",
        "explanation",
    }

    missing = required_keys - data.keys()
    if missing:
        raise ValueError(f"Missing required fields: {missing}")

    if not isinstance(data["topic"], str) or not data["topic"].strip():
        raise ValueError("Field 'topic' must be a non-empty string.")

    if data["difficulty"] not in {"easy", "medium", "hard"}:
        raise ValueError("Field 'difficulty' must be 'easy', 'medium', or 'hard'.")

    if not isinstance(data["question"], str) or not data["question"].strip():
        raise ValueError("Field 'question' must be a non-empty string.")

    if not isinstance(data["choices"], list):
        raise ValueError("Field 'choices' must be a list.")

    if len(data["choices"]) != 4:
        raise ValueError("Field 'choices' must contain exactly 4 items.")

    if not all(isinstance(choice, str) and choice.strip() for choice in data["choices"]):
        raise ValueError("All choices must be non-empty strings.")

    if len(set(choice.strip() for choice in data["choices"])) != 4:
        raise ValueError("Choices must be distinct.")

    if not isinstance(data["answer_index"], int):
        raise ValueError("Field 'answer_index' must be an integer.")

    if data["answer_index"] < 0 or data["answer_index"] > 3:
        raise ValueError("Field 'answer_index' must be between 0 and 3.")

    if not isinstance(data["explanation"], str) or not data["explanation"].strip():
        raise ValueError("Field 'explanation' must be a non-empty string.")
    
    correct = data["choices"][data["answer_index"]]
    if correct not in data["explanation"]:
        raise ValueError("Explanation does not reference the correct answer.")


def generate_mcq(topic: str, difficulty: str) -> dict[str, Any]:
    """
    Generate one primary school math multiple-choice question.
    """
    if difficulty not in {"easy", "medium", "hard"}:
        raise ValueError("difficulty must be 'easy', 'medium', or 'hard'.")

    client, model = get_gpt_client()

    prompt = f"""
You are a question generator for a quiz game.

Create exactly ONE primary school math multiple-choice question.

Topic: {topic}
Difficulty: {difficulty}

{QUESTION_SCHEMA_RULES}

Difficulty guide:
- easy: simple addition/subtraction, small numbers, basic counting, number comparison
- medium: mixed arithmetic, multiplication/division, simple word problems, missing number questions
- hard: more challenging word problems, mixed operations, time or money problems, 2-step thinking

Additional requirements:
- make the question age-appropriate for primary school students
- keep the wording simple and natural
- ensure there is exactly one correct answer
- make sure the explanation matches the correct answer exactly
"""

    resp = client.responses.create(
        model=model,
        input=prompt
    )

    raw_text = (resp.output_text or "").strip()
    json_str = _extract_json(raw_text)
    data = json.loads(json_str)

    _validate_question(data)
    return data


def generate_unique_questions(
    topic_list: list[str],
    difficulty: str,
    target_count: int
) -> list[dict[str, Any]]:
    """
    Generate multiple unique questions for the given difficulty.
    Uniqueness is checked by question text within the current run.
    """
    if target_count <= 0:
        return []

    results: list[dict[str, Any]] = []
    seen_questions: set[str] = set()

    topic_index = 0
    attempts = 0
    max_attempts = target_count * 10

    while len(results) < target_count and attempts < max_attempts:
        topic = topic_list[topic_index % len(topic_list)]
        topic_index += 1
        attempts += 1

        try:
            question_data = generate_mcq(topic=topic, difficulty=difficulty)
            question_text = question_data["question"].strip()

            if question_text in seen_questions:
                print(f"Duplicate skipped: {question_text}")
                continue

            seen_questions.add(question_text)
            results.append(question_data)
            print(f"Generated {len(results)}/{target_count}: {question_text}")

        except Exception as e:
            print(f"Generation failed on attempt {attempts}: {e}")

    if len(results) < target_count:
        print(
            f"Warning: only generated {len(results)} unique questions "
            f"out of requested {target_count}."
        )

    return results
