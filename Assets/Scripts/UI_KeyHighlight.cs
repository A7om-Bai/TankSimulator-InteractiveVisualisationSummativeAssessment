using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_KeyHighlight : MonoBehaviour
{
    public Image image;

    [Header("Fade Settings")]
    public float fadeInTime = 0.08f;
    public float fadeOutTime = 0.25f;
    public float maxAlpha = 0.35f;

    Coroutine fadeRoutine;
    Color baseColor;

    void Awake()
    {
        if (!image)
            image = GetComponent<Image>();

        baseColor = image.color;

        // ✅ 开局确保完全透明
        SetAlpha(0f);
    }

    // ✅ KeyBar 调用的唯一入口
    public void PlayOnce()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOnce());
    }

    IEnumerator FadeOnce()
    {
        // ---- Fade In ----
        yield return Fade(0f, maxAlpha, fadeInTime);

        // ---- Fade Out ----
        yield return Fade(maxAlpha, 0f, fadeOutTime);

        fadeRoutine = null;
    }

    IEnumerator Fade(float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / time));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        Color c = baseColor;
        c.a = a;
        image.color = c;
    }

    // ✅ KeyBar Clear / 取消用（如果你有）
    public void ResetHighlight()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = null;
        SetAlpha(0f);
    }
}
