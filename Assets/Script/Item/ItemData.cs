using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ConsumableType
{
   ATTACK,
   SPEED,
   HEALTH,
   STAMINA
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}
