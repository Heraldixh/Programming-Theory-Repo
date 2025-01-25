using UnityEngine;

public enum DamageType
{
    Physical,
    Magical,
    True
}

public struct DamageInfo
{
    public int amount;
    public DamageType type;
    public Character source;
    public bool isCritical;
}