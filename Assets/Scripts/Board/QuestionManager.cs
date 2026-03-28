using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class Question
{
    public string topic;
    public string difficulty;
    public string question;
    public List<string> choices;
    public int answer_index;
    public string explanation;
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
public class SubjectInfo { public string subject; public string infoText; }
[System.Serializable]
public class SubjectInfoCollection { public List<SubjectInfo> subjects; }

[System.Serializable]
public class QuestionCollection
{
    public QuestionDatabase database;
}

public class QuestionManager : MonoBehaviour
{
    public QuestionDatabase database;
    public Dictionary<string, string> subjectInfoData = new Dictionary<string, string>();
    [HideInInspector] public bool waitingForAnswer = false;
    [HideInInspector] public bool lastAnswerCorrect = false;

    public QuestionUI questionUI;

    private Question currentQuestion;

    // key format: "subject|difficulty"
    private Dictionary<string, List<Question>> remainingQuestionPools = new Dictionary<string, List<Question>>();
    // Per pool: normalized question texts already shown this round (until pool resets)
    private Dictionary<string, HashSet<string>> usedQuestionTextsPerPool = new Dictionary<string, HashSet<string>>();

    void Start()
    {
        if (SessionManager.Instance != null)
            SessionManager.Instance.UsedQuestionKeysThisRun.Clear();

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

        // Load JSON file (subject_info)
        TextAsset infoFile = Resources.Load<TextAsset>("subject_info");
        if (infoFile != null)
        {
            SubjectInfoCollection sic = JsonUtility.FromJson<SubjectInfoCollection>(infoFile.text);
            foreach (SubjectInfo info in sic.subjects)
                subjectInfoData[info.subject.Trim().ToLower()] = info.infoText;
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

    /// <summary>Used for dedup keys (session + pool).</summary>
    public static string NormalizeQuestionText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
        string s = text.Trim().ToLowerInvariant();
        // Merge near-identical stems ("apples, and" vs "apples and") so JSON duplicates don't slip through as different keys.
        s = Regex.Replace(s, @"[,;:.!?""'()\[\]]", "");
        s = Regex.Replace(s, @"\s+", " ");
        return s.Trim();
    }

    /// <summary>Stable key: subject + normalized question (cross-difficulty dedup for one run).</summary>
    public static string SessionQuestionKey(string subject, string questionText)
    {
        return subject.Trim().ToLowerInvariant() + "|" + NormalizeQuestionText(questionText);
    }

    /// <summary>
    /// Same topic + difficulty: drop duplicate question strings.
    /// order is not predictable.
    /// </summary>
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

        var seen = new HashSet<string>();
        var unique = new List<Question>();
        foreach (Question q in filteredList)
        {
            string fp = NormalizeQuestionText(q.question);
            if (string.IsNullOrEmpty(fp) || seen.Contains(fp))
                continue;
            seen.Add(fp);
            unique.Add(q);
        }

        for (int i = unique.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Question tmp = unique[i];
            unique[i] = unique[j];
            unique[j] = tmp;
        }

        return unique;
    }

    /// <summary>
    /// Same as BuildFilteredQuestionList but drops questions already shown this run.
    /// Used when the draw pool still has items but every remaining one is blocked by session dedup.
    /// </summary>
    private List<Question> BuildFilteredQuestionListExcludingSession(string subject, string difficulty)
    {
        List<Question> all = BuildFilteredQuestionList(subject, difficulty);
        if (SessionManager.Instance == null)
            return all;

        var fresh = new List<Question>();
        foreach (Question q in all)
        {
            if (!SessionManager.Instance.UsedQuestionKeysThisRun.Contains(SessionQuestionKey(subject, q.question)))
                fresh.Add(q);
        }

        for (int i = fresh.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Question tmp = fresh[i];
            fresh[i] = fresh[j];
            fresh[j] = tmp;
        }

        return fresh;
    }

    private void ResetPoolForSubjectAndDifficulty(string subject, string difficulty)
    {
        string key = GetPoolKey(subject, difficulty);
        if (usedQuestionTextsPerPool.ContainsKey(key))
            usedQuestionTextsPerPool[key].Clear();
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

        if (!usedQuestionTextsPerPool.ContainsKey(key))
            usedQuestionTextsPerPool[key] = new HashSet<string>();
        HashSet<string> usedInPool = usedQuestionTextsPerPool[key];

        List<Question> candidates = BuildCandidates(list, usedInPool, subject);

        // Pool was refilled but every entry was already shown this run — rebuild from unused only.
        // Do NOT clear UsedQuestionKeysForSubject here; that caused the same question to appear twice in one round.
        if (candidates.Count == 0 && list.Count > 0 && SessionManager.Instance != null)
        {
            list = BuildFilteredQuestionListExcludingSession(subject, difficulty);
            remainingQuestionPools[key] = list;
            usedInPool.Clear();
            candidates = BuildCandidates(list, usedInPool, subject);
        }

        if (candidates.Count == 0)
        {
            usedInPool.Clear();
            candidates = BuildCandidates(list, usedInPool, subject);
        }

        // Every question for this subject+difficulty was already used this run — reset pool and session for this subject so play can continue.
        if (candidates.Count == 0 && list != null && list.Count == 0 && SessionManager.Instance != null)
        {
            SessionManager.Instance.ClearQuestionKeysForSubject(subject);
            ResetPoolForSubjectAndDifficulty(subject, difficulty);
            list = remainingQuestionPools[key];
            candidates = BuildCandidates(list, usedInPool, subject);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("No drawable question for subject: " + subject + ", difficulty: " + difficulty);
            return null;
        }

        int pick = Random.Range(0, candidates.Count);
        Question selectedQuestion = candidates[pick];

        list.Remove(selectedQuestion);
        usedInPool.Add(NormalizeQuestionText(selectedQuestion.question));
        if (SessionManager.Instance != null)
            SessionManager.Instance.UsedQuestionKeysThisRun.Add(SessionQuestionKey(subject, selectedQuestion.question));

        return selectedQuestion;
    }

    private List<Question> BuildCandidates(List<Question> list, HashSet<string> usedInPool, string subject)
    {
        var candidates = new List<Question>();
        foreach (Question q in list)
        {
            string fp = NormalizeQuestionText(q.question);
            if (usedInPool.Contains(fp))
                continue;
            if (SessionManager.Instance != null &&
                SessionManager.Instance.UsedQuestionKeysThisRun.Contains(SessionQuestionKey(subject, q.question)))
                continue;
            candidates.Add(q);
        }
        return candidates;
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