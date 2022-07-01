using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element Controller", menuName = "ScriptableObjects/Element Controller")]
public class ElementController : ScriptableObject
{
    public ArmController armController;
    public ElementStats elementStats;
    public ElementPrefabs elementPrefabs;
}