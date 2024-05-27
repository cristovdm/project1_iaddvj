using UnityEngine;
using TMPro;

public class TextboxAnimator : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float duration = 2f;
    public float moveDistance = 50f;

    private Color originalColor = Color.yellow; 
    private Vector3 originalPosition;
    private float elapsedTime = 0f;
    private bool isAnimating = false;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
        originalPosition = textComponent.rectTransform.localPosition;
        textComponent.color = originalColor;
        textComponent.enabled = false;
    }

    void Update()
    {
        if (isAnimating)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            textComponent.rectTransform.localPosition = originalPosition + Vector3.up * Mathf.Lerp(0, moveDistance, t);

            if (elapsedTime >= duration)
            {
                isAnimating = false;
                textComponent.enabled = false;
            }
        }
    }

    public void ShowTextbox(string message)
    {
        textComponent.text = message;
        textComponent.enabled = true;
        textComponent.color = originalColor;
        textComponent.rectTransform.localPosition = originalPosition;
        elapsedTime = 0f;
        isAnimating = true;
    }
}
