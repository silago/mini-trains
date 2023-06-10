using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    private List<BaseItem> items = new List<BaseItem>();

    public void AddItem(BaseItem baseItem)
    {
        items.Add(baseItem);
    }

    public BaseItem GetEmptyStorage()
    {
        //var storage = items.FirstOrDefault(x => x.ItemType.HasFlag(ItemType.Storage) && x.ProductionItem == null);
        //return storage;
        return null;
    }
}