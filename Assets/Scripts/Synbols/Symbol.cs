using System;
using UnityEngine;

public enum EnemyType
{
    None,
    Goblin,
    Troll,
    Dragon,
    Ogre,
    Smile,
}

[CreateAssetMenu(fileName = "Symbol", menuName = "Scriptable Objects/Symbol")]
public class Symbol : ScriptableObject
{
    public string name = "Circle";

    public Sprite sprite;

    public Material material;

    [ColorUsage(true, true)]
    public Color color;

    public EnemyType killsThese = EnemyType.None;
}
