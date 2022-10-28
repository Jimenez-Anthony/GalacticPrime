using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum ITEMTYPE {Weapon, CharacterUpgrade, Usable, Consumable};

    public new string name = "New Item";
    [TextArea(15, 20)]
    public string description;
    public ITEMTYPE itemType;
    public Sprite icon = null;
    public int durability = 0;
    public int stackSize = 0;
    public GameObject prefab;
   
}
