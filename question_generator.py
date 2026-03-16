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

    return match.group(0)


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


def generate_mcq(subject: str, topic: str, difficulty: str) -> dict[str, Any]:
    """
    Generate one primary school multiple-choice question for the given subject.
    """
    if subject not in {"Math & Logic", "Environmental Science", "English Grammar", "Global Citizenship"}:
        raise ValueError("subject must be 'Math & Logic', 'Environmental Science', 'English Grammar'or 'Global Citizenship'.")

    if difficulty not in {"easy", "medium", "hard"}:
        raise ValueError("difficulty must be 'easy', 'medium', or 'hard'.")

    client, model = get_gpt_client()

    if subject == "Math & Logic":
        subject_rules = """
Create exactly ONE primary school math multiple-choice question.

Difficulty guide:
- easy: simple addition/subtraction, small numbers, basic counting, number comparison
- medium: mixed arithmetic, multiplication/division, simple word problems, missing number questions
- hard: more challenging word problems, mixed operations, time or money problems, 2-step thinking
"""
    elif subject == "Environmental Science":
        subject_rules = """
Create exactly ONE primary school Environmental Science multiple-choice question.

Difficulty guide:
- easy: plants, animals, weather, recycling, water, air
- medium: habitats, pollution, saving energy, protecting nature, natural resources
- hard: environmental problems, cause and effect, conservation, simple sustainability ideas

Additional subject requirements:
- keep the environmental concepts simple and age-appropriate
- focus on daily-life examples that primary school students can understand
- avoid advanced scientific or technical terminology
- keep the wording clear, short, and natural
"""
    elif subject == "English Grammar":
        subject_rules = """
Create exactly ONE primary school English Grammar multiple-choice question.

Difficulty guide:
- easy: simple vocabulary, spelling, matching common words
- medium: sentence meaning, word choice, simple grammar, singular/plural
- hard: reading comprehension, sentence completion, grammar in context

Additional subject requirements:
- use age-appropriate English for primary school students
- avoid overly difficult or rare words
- keep the question clear and short
"""
    else:
        subject_rules = """
Create exactly ONE primary school Global Citizenship multiple-choice question.

Difficulty guide:
- easy: kindness, helping others, respecting differences, sharing, community rules
- medium: fairness, responsibility, teamwork, inclusion, understanding other cultures
- hard: global awareness, empathy, solving conflicts, cooperation, being a responsible citizen

Additional subject requirements:
- keep the ideas simple and suitable for primary school students
- focus on real-life school, family, and community situations
- avoid political, controversial, or highly abstract topics
- keep the wording clear, short, and natural
"""

    prompt = f"""
You are a question generator for a quiz game.

Subject: {subject}
Topic: {topic}
Difficulty: {difficulty}

{QUESTION_SCHEMA_RULES}

{subject_rules}

Additional requirements:
- make the question age-appropriate for primary school students
- keep the wording simple and natural
- ensure there is exactly one correct answer
- make sure the explanation matches the correct answer exactly
- set the "topic" field to "{subject}"
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
    subject: str,
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
            question_data = generate_mcq(subject=subject, topic=topic, difficulty=difficulty)
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
