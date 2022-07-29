using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [Range(1f, 3f)] 
    [SerializeField]
    float sizeMultiplier = 1.2f;

    [Range(0.01f, 0.3f)]
    [SerializeField]
    float occurOverTime = 0.1f;

    Transform button = null;

    Vector3 smallScale;
    Vector3 largeScale;

    private void Start()
    {
        button = transform;

        smallScale = button.gameObject.transform.localScale;
        largeScale = smallScale * sizeMultiplier;
    }

    public void EnlargeButton()
    {
        button.localScale = largeScale;

        button.GetComponent<TextMeshProUGUI>().color = Color.yellow;

        StartCoroutine(ScaleOverTime(smallScale, largeScale, occurOverTime));
    }

    public void ShrinkButton()
    {
        button.localScale = smallScale;

        button.GetComponent<TextMeshProUGUI>().color = Color.grey;

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
