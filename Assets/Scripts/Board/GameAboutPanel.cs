using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class GameAboutData
{
    public string gameDescription;
    public string sdgDescription;
}

/// <summary>
/// Login / menu "About" popup: game summary + SDG text, loaded from Resources/game_about.json.
/// In Unity: add Button + Panel + TMP, assign fields, play.
/// </summary>
public class GameAboutPanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject aboutPanel;
    public TMP_Text bodyText;
    public Button openButton;
    public Button closeButton;

    [Header("Fallback if JSON missing")]
    [TextArea(3, 8)]
    public string fallbackGameDescription = "";
    [TextArea(3, 8)]
    public string fallbackSdgDescription = "";

    [Header("Layout (applied at runtime)")]
    [Tooltip("Main text area: wide box, leaves bottom strip for Close button.")]
    public float bodyFontSize = 19f;

    void Awake()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);

        string game = fallbackGameDescription;
        string sdg = fallbackSdgDescription;

        TextAsset json = Resources.Load<TextAsset>("game_about");
        if (json != null)
        {
            GameAboutData data = JsonUtility.FromJson<GameAboutData>(json.text);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.gameDescription))
                    game = data.gameDescription;
                if (!string.IsNullOrEmpty(data.sdgDescription))
                    sdg = data.sdgDescription;
            }
        }

        if (bodyText != null)
        {
            // Clear sections + line breaks; TMP rich text for light structure
            string body = "<b>About the game</b>\n\n" + game.Trim()
                + "\n\n<b>SDG connection</b>\n\n" + sdg.Trim();
            bodyText.text = body;
            bodyText.enableWordWrapping = true;

            var tmp = bodyText as TextMeshProUGUI;
            if (tmp != null)
            {
                tmp.enableAutoSizing = false;
                tmp.fontSize = bodyFontSize;
                tmp.lineSpacing = 0f;
                tmp.paragraphSpacing = 10f;
                tmp.margin = new Vector4(16f, 12f, 16f, 12f);
                tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
                tmp.verticalAlignment = VerticalAlignmentOptions.Top;
                tmp.overflowMode = TextOverflowModes.Overflow;

                // Widen text box: narrow rects cause one-word-per-line. Reserve bottom for Close.
                RectTransform rt = tmp.rectTransform;
                rt.anchorMin = new Vector2(0.06f, 0.16f);
                rt.anchorMax = new Vector2(0.94f, 0.92f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
            }
        }

        LayoutCloseButton();

        if (openButton != null)
            openButton.onClick.AddListener(OpenAbout);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseAbout);
    }

    void OpenAbout()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(true);
        if (openButton != null)
            openButton.gameObject.SetActive(false);
    }

    void CloseAbout()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
        if (openButton != null)
            openButton.gameObject.SetActive(true);
    }

    void LayoutCloseButton()
    {
        if (closeButton == null)
            return;
        RectTransform rt = closeButton.GetComponent<RectTransform>();
        if (rt == null)
            return;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 36f);
        rt.sizeDelta = new Vector2(240f, 46f);
    }
}
