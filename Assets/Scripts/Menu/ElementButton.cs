using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementButton : ButtonHandler
{
    [SerializeField] private Image elementImage = null;
    [SerializeField] private GameObject tooltipPanel = null;
    [SerializeField] private TextMeshProUGUI tooltipText = null;

    [SerializeField] private bool changeTransparency = false;
    [Range(0.1f, 1f)] [SerializeField] private float defaultTransparency = 1f;
    [Range(0.1f, 1f)] [SerializeField] private float onHoverTransparency = 0.7f;

    [SerializeField] ElementTooltip elementTooltip = null;

    public override void OnHover()
    {
        base.OnHover();

        if (!changeTransparency) { return; }

        elementImage.color = onHoverColor;
        elementImage.color = new Color(elementImage.color.r, elementImage.color.g, elementImage.color.b, defaultTransparency);
    }

    public override void OnStopHover()
    {
        base.OnStopHover();

        if (!changeTransparency) { return; }

        elementImage.color = defaultColor;
        elementImage.color = new Color(elementImage.color.r, elementImage.color.g, elementImage.color.b, onHoverTransparency);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        ShowTooltip();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        
        HideTooltip();
    }

    private void ShowTooltip()
    {
        tooltipPanel.gameObject.SetActive(true);
        tooltipText.text = elementTooltip.text;
    }

    private void HideTooltip()
    {
        tooltipPanel.gameObject.SetActive(false);
    }
}
