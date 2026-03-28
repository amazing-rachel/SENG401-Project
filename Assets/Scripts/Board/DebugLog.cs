using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebugOverlay : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public float messageLifetime = 5f;

    private class LogEntry {
        public string message;
        public float timeRemaining;
    }

    private List<LogEntry> logs = new List<LogEntry>();

    void OnEnable(){
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        // normal Debug.Log messages
        if (type != LogType.Log)
            return;

        if (logString.Contains("Request body")) return;

        logs.Add(new LogEntry
        {
            message = logString,
            timeRemaining = messageLifetime
        });
    }

    void Update(){
        // timers for removing expired logs
        for (int i = logs.Count - 1; i >= 0; i--)
        {
            logs[i].timeRemaining -= Time.deltaTime;

            if (logs[i].timeRemaining <= 0)
                logs.RemoveAt(i);
        }

        debugText.text = GetLatestMessage();
    }

    string GetLatestMessage() {
        if (logs.Count == 0)
            return "";

        int maxLines = 3;
        int startIndex = Mathf.Max(0, logs.Count - maxLines);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = startIndex; i < logs.Count; i++)
        {
            sb.AppendLine(logs[i].message);
        }

        return sb.ToString().TrimEnd('\n');  // prevents empty bottom line

    }
}