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

    public QuestionUI questionUI;

    private Question currentQuestion;

    // key format: "subject|difficulty"
    private Dictionary<string, List<Question>> remainingQuestionPools = new Dictionary<string, List<Question>>();

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("questions");

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

    private string GetPoolKey(string subject, string difficulty)
    {
        return subject.Trim() + "|" + difficulty.Trim().ToLower();
    }

    private List<Question> GetSourceListByDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                return database.easy;
            case "medium":
                return database.medium;
            case "hard":
                return database.hard;
            default:
                Debug.LogError("Invalid difficulty: " + difficulty);
                return null;
        }
    }

    private List<Question> BuildFilteredQuestionList(string subject, string difficulty)
    {
        List<Question> sourceList = GetSourceListByDifficulty(difficulty);

        if (sourceList == null || sourceList.Count == 0)
        {
            return new List<Question>();
        }

        List<Question> filteredList = sourceList.FindAll(q =>
            q != null &&
            !string.IsNullOrEmpty(q.topic) &&
            q.topic.Trim().ToLower() == subject.Trim().ToLower()
        );

        return filteredList;
    }

    private void ResetPoolForSubjectAndDifficulty(string subject, string difficulty)
    {
        string key = GetPoolKey(subject, difficulty);
        remainingQuestionPools[key] = BuildFilteredQuestionList(subject, difficulty);
    }

    public Question GetRandomQuestion(string subject, string difficulty)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            Debug.LogError("Subject is null or empty.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(difficulty))
        {
            Debug.LogError("Difficulty is null or empty.");
            return null;
        }

        string key = GetPoolKey(subject, difficulty);

        if (!remainingQuestionPools.ContainsKey(key))
        {
            ResetPoolForSubjectAndDifficulty(subject, difficulty);
        }

        List<Question> list = remainingQuestionPools[key];

        if (list == null || list.Count == 0)
        {
            Debug.Log("All questions used for subject: " + subject + ", difficulty: " + difficulty + ". Resetting pool.");
            ResetPoolForSubjectAndDifficulty(subject, difficulty);
            list = remainingQuestionPools[key];
        }

        if (list == null || list.Count == 0)
        {
            Debug.LogError("No questions found for subject: " + subject + " and difficulty: " + difficulty);
            return null;
        }

        int index = Random.Range(0, list.Count);
        Question selectedQuestion = list[index];
        list.RemoveAt(index);

        return selectedQuestion;
    }

    public void AskQuestion(string difficulty)
    {
        if (SessionManager.Instance == null)
        {
            Debug.LogError("SessionManager.Instance is null.");
            waitingForAnswer = false;
            lastAnswerCorrect = false;
            return;
        }

        string subject = SessionManager.Instance.SelectedSubject;
        currentQuestion = GetRandomQuestion(subject, difficulty);

        if (currentQuestion == null)
        {
            lastAnswerCorrect = false;
            waitingForAnswer = false;
            return;
        }

        if (questionUI != null)
        {
            questionUI.ShowQuestion(currentQuestion);
            waitingForAnswer = true;
        }
        else
        {
            Debug.Log("QUESTION (" + subject + " / " + difficulty + "): " + currentQuestion.question);

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