using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arm Controller", menuName = "ScriptableObjects/Arm Controller")]
public class ArmController : ScriptableObject
{
    public RuntimeAnimatorController waterArms;
    public RuntimeAnimatorController earthArms;
    public RuntimeAnimatorController fireArms;
    public RuntimeAnimatorController airArms;
}