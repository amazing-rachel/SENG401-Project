# generate 100 primary math questions in JSON
import json
from question_generator import generate_unique_questions


def main() -> None:
    easy_topics = [
        "addition within 20",
        "subtraction within 20",
        "counting",
        "number comparison"
    ]

    medium_topics = [
        "addition and subtraction word problems",
        "basic multiplication",
        "basic division",
        "missing number equations"
    ]

    hard_topics = [
        "mixed operations",
        "time word problems",
        "money word problems",
        "two-step arithmetic problems"
    ]

    easy_questions = generate_unique_questions(
        topic_list=easy_topics,
        difficulty="easy",
        target_count=20
    )

    medium_questions = generate_unique_questions(
        topic_list=medium_topics,
        difficulty="medium",
        target_count=40
    )

    hard_questions = generate_unique_questions(
        topic_list=hard_topics,
        difficulty="hard",
        target_count=40
    )

    all_questions = easy_questions + medium_questions + hard_questions

    with open("questions_seed.json", "w", encoding="utf-8") as f:
        json.dump(all_questions, f, indent=2, ensure_ascii=False)

    print(f"Saved {len(all_questions)} questions to questions_seed.json")


if __name__ == "__main__":
    main()
