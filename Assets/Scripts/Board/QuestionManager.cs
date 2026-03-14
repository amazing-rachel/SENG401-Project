using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public string topic;
    public string difficulty;
    public string question;
    public List<string> choices;
    public int answer_index;
    public string explanation;
    public string hint;
    public string learnMoreUrl;
}

[System.Serializable]
public class QuestionDatabase
{
    public List<Question> easy;
    public List<Question> medium;
    public List<Question> hard;
}

// Wrapper class for JsonUtility
[System.Serializable]
public class QuestionCollection
{
    public QuestionDatabase database;
}

public class QuestionManager : MonoBehaviour
{
    public QuestionDatabase database;

    [HideInInspector] public bool waitingForAnswer = false;
    [HideInInspector] public bool lastAnswerCorrect = false;

    public QuestionUI questionUI; // assign in Inspector

    private Question currentQuestion;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("questions"); // accesses questions.json in Assets/Resources
        if (jsonFile != null)
        {
            QuestionCollection qc = JsonUtility.FromJson<QuestionCollection>(jsonFile.text);
            database = qc.database;

            Debug.Log("Questions loaded: " + database.easy.Count + " easy, " + database.medium.Count + " medium, " + database.hard.Count + " hard");
        }
        else
        {
            Debug.LogError("questions.json not found in Resources!");
        }
    }

    public Question GetRandomQuestion(string difficulty)
    {
        List<Question> list = null;

        switch (difficulty)
        {
            case "easy":
                list = database.easy;
                break;
            case "medium":
                list = database.medium;
                break;
            case "hard":
                list = database.hard;
                break;
        }

        if (list == null || list.Count == 0)
        {
            Debug.LogError("No questions found for difficulty: " + difficulty);
            return null;
        }

        int index = Random.Range(0, list.Count);
        return list[index];
    }

    public void AskQuestion(string difficulty)
    {
        currentQuestion = GetRandomQuestion(difficulty);

        if (currentQuestion == null)
        {
            lastAnswerCorrect = false;
            waitingForAnswer = false;
            return;
        }

        if (questionUI != null)
        {
            questionUI.ShowQuestion(currentQuestion);
        }
        else
        {
            Debug.Log("QUESTION (" + difficulty + "): " + currentQuestion.question);
            for (int i = 0; i < currentQuestion.choices.Count; i++)
            {
                Debug.Log(i + ": " + currentQuestion.choices[i]);
            }

            waitingForAnswer = true;
            StartCoroutine(SimulateAnswer(currentQuestion));
        }
    }

    private IEnumerator SimulateAnswer(Question q)
    {
        yield return new WaitForSeconds(2f);

        lastAnswerCorrect = Random.value > 0.5f;

        Debug.Log("Answer correct: " + lastAnswerCorrect);

        waitingForAnswer = false;
    }
}