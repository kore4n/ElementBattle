using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element Tooltip", menuName = "ScriptableObjects/ElementTooltip")]
public class ElementTooltip : ScriptableObject
{
    [TextArea(15, 20)]
    public string text;
}
