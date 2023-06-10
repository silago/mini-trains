using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemsSettings", fileName = "ItemsSettings", order = 0)]
[System.Serializable]
public class ItemsSettings : ScriptableObject
{
    public List<ItemSettings> items;
}