using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Range(1f, 3f)] 
    [SerializeField]
    private float sizeMultiplier = 1.2f;

    [Range(0.01f, 0.3f)]
    [SerializeField]
    private float occurOverTime = 0.1f;

    private Transform button = null;

    private TextMeshProUGUI textBox = null;
    

    private Vector3 smallScale;
    private Vector3 largeScale;

    [SerializeField] protected Color defaultColor = Color.grey;
    [SerializeField] protected Color onHoverColor = Color.yellow;

    public float fadeTime = 0.1f;

    private void FadeOut()
    {
        textBox.color = Color.Lerp(textBox.color, onHoverColor, fadeTime * Time.deltaTime);
    }

    public void RevertToDefault()
    {
        button.localScale = smallScale;

        textBox.color = defaultColor;


        // TODO: Reset transparency
    }

    private void Start()
    {
        button = transform;
        TryGetComponent<TextMeshProUGUI>(out textBox);

        smallScale = button.gameObject.transform.localScale;
        largeScale = smallScale * sizeMultiplier;
    }

    public virtual void OnHover()
    {
        button.localScale = largeScale;

        if (textBox != null)
        {
            textBox.color = onHoverColor;
        }

        StartCoroutine(ScaleOverTime(smallScale, largeScale, occurOverTime));

    }

    public virtual void OnStopHover()
    {
        button.localScale = smallScale;

        if (textBox != null)
        {
            textBox.color = defaultColor;
        }

        StartCoroutine(ScaleOverTime(largeScale, smallScale, occurOverTime));
    }

    IEnumerator ScaleOverTime(Vector3 originalScale, Vector3 finalScale, float time)
    {
        float currentTime = 0.0f;

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, finalScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // TODO: Play OnHover button sounds like Terraria

        OnHover();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnStopHover();
    }
}
