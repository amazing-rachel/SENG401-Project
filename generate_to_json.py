import json
from question_generator import generate_unique_questions


def main() -> None:
    all_questions = []


    # Math & Logic

    math_easy_topics = [
        "addition within 20",
        "subtraction within 20",
        "number comparison",
        "counting objects"
    ]

    math_medium_topics = [
        "multiplication basics",
        "division basics",
        "number patterns"
    ]

    math_hard_topics = [
        "logic puzzles",
        "multi-step word problems",
        "pattern reasoning"
    ]

    all_questions += generate_unique_questions(
        subject="Math & Logic",
        topic_list=math_easy_topics,
        difficulty="easy",
        target_count=30
    )

    all_questions += generate_unique_questions(
        subject="Math & Logic",
        topic_list=math_medium_topics,
        difficulty="medium",
        target_count=40
    )

    all_questions += generate_unique_questions(
        subject="Math & Logic",
        topic_list=math_hard_topics,
        difficulty="hard",
        target_count=30
    )

  
    # English Grammar

    english_easy_topics = [
        "basic vocabulary",
        "simple spelling",
        "matching words"
    ]

    english_medium_topics = [
        "sentence meaning",
        "word choice",
        "singular and plural nouns"
    ]

    english_hard_topics = [
        "sentence completion",
        "reading comprehension",
        "grammar in context"
    ]

    all_questions += generate_unique_questions(
        subject="English Grammar",
        topic_list=english_easy_topics,
        difficulty="easy",
        target_count=30
    )

    all_questions += generate_unique_questions(
        subject="English Grammar",
        topic_list=english_medium_topics,
        difficulty="medium",
        target_count=40
    )

    all_questions += generate_unique_questions(
        subject="English Grammar",
        topic_list=english_hard_topics,
        difficulty="hard",
        target_count=30
    )


    # Environmental Science

    env_easy_topics = [
        "plants and animals",
        "weather",
        "recycling"
    ]

    env_medium_topics = [
        "habitats",
        "pollution",
        "saving energy"
    ]

    env_hard_topics = [
        "conservation",
        "environmental protection",
        "cause and effect in nature"
    ]

    all_questions += generate_unique_questions(
        subject="Environmental Science",
        topic_list=env_easy_topics,
        difficulty="easy",
        target_count=30
    )

    all_questions += generate_unique_questions(
        subject="Environmental Science",
        topic_list=env_medium_topics,
        difficulty="medium",
        target_count=40
    )

    all_questions += generate_unique_questions(
        subject="Environmental Science",
        topic_list=env_hard_topics,
        difficulty="hard",
        target_count=30
    )

  
    # Global Citizenship
   
    global_easy_topics = [
        "kindness",
        "sharing",
        "respecting others"
    ]

    global_medium_topics = [
        "teamwork",
        "responsibility",
        "fairness"
    ]

    global_hard_topics = [
        "solving conflicts",
        "empathy",
        "community responsibility"
    ]

    all_questions += generate_unique_questions(
        subject="Global Citizenship",
        topic_list=global_easy_topics,
        difficulty="easy",
        target_count=30
    )

    all_questions += generate_unique_questions(
        subject="Global Citizenship",
        topic_list=global_medium_topics,
        difficulty="medium",
        target_count=40
    )

    all_questions += generate_unique_questions(
        subject="Global Citizenship",
        topic_list=global_hard_topics,
        difficulty="hard",
        target_count=30
    )


    # Save JSON

    with open("questions_seed.json", "w", encoding="utf-8") as f:
        json.dump(all_questions, f, indent=2, ensure_ascii=False)

    print(f"Saved {len(all_questions)} questions to questions_seed.json")


if __name__ == "__main__":
    main()
