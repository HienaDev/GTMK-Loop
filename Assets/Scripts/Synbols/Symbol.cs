using System;
using UnityEngine;

public enum EnemyType
{
    None,
    Goblin,
    Troll,
    Dragon
}

[CreateAssetMenu(fileName = "Symbol", menuName = "Scriptable Objects/Symbol")]
public class Symbol : ScriptableObject
{
    public string name = "Circle";
    [ColorUsage(true, true)]
    public Color color;

    public EnemyType killsThese = EnemyType.None;
}
