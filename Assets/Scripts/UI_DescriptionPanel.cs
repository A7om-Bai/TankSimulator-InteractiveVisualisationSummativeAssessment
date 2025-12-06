using UnityEngine;
using TMPro;

public class UI_DescriptionPanel : MonoBehaviour
{
    public static UI_DescriptionPanel Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI descriptionText; // 文本
    public GameObject panelRoot;            // 整个面板

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        if (panelRoot != null)
            panelRoot.SetActive(false);  // 启动时隐藏
    }

    public static void Show(string text)
    {
        if (Instance == null) return;

        if (Instance.panelRoot != null)
            Instance.panelRoot.SetActive(true);

        if (Instance.descriptionText != null)
            Instance.descriptionText.text = text;
    }

    public static void Hide()
    {
        if (Instance == null) return;

        if (Instance.panelRoot != null)
            Instance.panelRoot.SetActive(false);
    }
}
