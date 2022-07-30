using TMPro;
using UnityEngine;

public class PanelCleanup : MonoBehaviour
{
    // Script only exists to prevent bug and put buttons back to same color and size before hovering
    // Bug is caused by disabling before hover is taken off

    private void OnDisable()
    {
        var buttons = GetComponentsInChildren<ButtonHandler>();
        foreach (var button in buttons)
        {
            if (button == null) { continue; }

            button.transform.localScale = button.GetSmallScale();
            button.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI buttonTMPRO);
            buttonTMPRO.color = button.GetDefaultColor();
        }
    }
}
