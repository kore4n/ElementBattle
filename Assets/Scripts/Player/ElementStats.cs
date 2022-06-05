using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element Stats", menuName = "ScriptableObjects/Element Stats")]
public class ElementStats : ScriptableObject
{
    public float dmgMultiplier;
    public float kbMultiplier;
    public float moveMultiplier;
}
