from question_generator import generate_mcq

def main() -> None:
    q = generate_mcq(topic="primary school math question", difficulty="hard")
    print(f"\nTopic: {q['topic']}")
    print(f"Difficulty: {q['difficulty']}\n")

    print(f"QUESTION: {q['question']}\n")

    letters = ["A", "B", "C", "D"]
    for i, choice in enumerate(q["choices"]):
        print(f"{letters[i]}) {choice}")

    correct_letter = letters[q["answer_index"]]

    print(f"\nANSWER: {correct_letter}")
    print(f"EXPLANATION: {q['explanation']}\n")


if __name__ == "__main__":
    main()