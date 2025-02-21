using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StartStat", menuName = "ScriptableObjects/StartStatScriptableObject", order = 1)]
public class StartStatScriptableObject : ScriptableObject
{
    public float speed;
    public ushort capacity;
    public ushort amount;
}
