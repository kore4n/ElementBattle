using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [Range(1f, 3f)] 
    [SerializeField]
    private float sizeMultiplier = 1.2f;

    [Range(0.01f, 0.3f)]
    [SerializeField]
    private float occurOverTime = 0.1f;

    private Transform button = null;

    private Vector3 smallScale;
    private Vector3 largeScale;

    private Color defaultColor = Color.grey;
    private Color onHoverColor = Color.yellow;

    public Vector3 GetSmallScale()
    {
        return smallScale;
    }

    public Color GetDefaultColor()
    {
        return defaultColor;
    }

    private void Start()
    {
        button = transform;

        smallScale = button.gameObject.transform.localScale;
        largeScale = smallScale * sizeMultiplier;
    }

    public void EnlargeButton()
    {
        button.localScale = largeScale;

        button.GetComponent<TextMeshProUGUI>().color = onHoverColor;

        StartCoroutine(ScaleOverTime(smallScale, largeScale, occurOverTime));
    }

    public void ShrinkButton()
    {
        button.localScale = smallScale;

        button.GetComponent<TextMeshProUGUI>().color = defaultColor;

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

}
