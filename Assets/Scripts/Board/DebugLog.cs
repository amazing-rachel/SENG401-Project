using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebugOverlay : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public float messageLifetime = 3f;

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
        // Only allow normal Debug.Log messages
        if (type != LogType.Log)
            return;

        logs.Add(new LogEntry
        {
            message = logString,
            timeRemaining = messageLifetime
        });
    }

    void Update(){
        // Update timers and remove expired logs
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

        // Show only the most recent message
        return logs[logs.Count - 1].message;
    }
}