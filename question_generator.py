import json
import re
from typing import Any

from gemini_client import get_gemini_client


QUESTION_SCHEMA_RULES = """
Return ONLY valid JSON. No markdown fences. No extra text.

JSON schema:
{
  "topic": "string",
  "difficulty": "easy|medium|hard",
  "question": "string",
  "choices": ["string", "string", "string", "string"],
  "answer_index": 0,
  "explanation": "string"
}

Rules:
- choices must have exactly 4 items
- answer_index must be 0,1,2,or 3
- question must be one sentence
- explanation must be one short sentence
"""


def _extract_json(text: str) -> str:
    """
    Gemini sometimes returns extra whitespace.
    This tries to pull out the first JSON object from the text.
    """
    text = text.strip()
    match = re.search(r"\{.*\}", text, flags=re.DOTALL)
    if not match:
        raise ValueError(f"Could not find JSON object in model output:\n{text}")
    return match.group(0)


def generate_mcq(topic: str, difficulty: str) -> dict[str, Any]:
    client, model = get_gemini_client()

    prompt = f"""
You are a question generator for a quiz game.

Topic: {topic}
Difficulty: {difficulty}

{QUESTION_SCHEMA_RULES}
"""

    resp = client.models.generate_content(
        model=model,
        contents=prompt,
    )

    raw_text = (resp.text or "").strip()
    json_str = _extract_json(raw_text)
    data = json.loads(json_str)

    _validate_question(data)
    return data


def _validate_question(data: dict[str, Any]) -> None:
    required = ["topic", "difficulty", "question", "choices", "answer_index", "explanation"]
    for k in required:
        if k not in data:
            raise ValueError(f"Missing key: {k}")

    if not isinstance(data["choices"], list) or len(data["choices"]) != 4:
        raise ValueError("choices must be a list of exactly 4 strings")

    if not isinstance(data["answer_index"], int) or data["answer_index"] not in [0, 1, 2, 3]:
        raise ValueError("answer_index must be 0, 1, 2, or 3")