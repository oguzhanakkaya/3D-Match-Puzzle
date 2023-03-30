using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelArea : MonoBehaviour
{
    public static LevelArea Instance;

    private List<Item> items=new List<Item>();
    void Awake()
    {
        Instance = this;   
    }
    public void AddItemToList(Item item)
    {
        items.Add(item);
    }
    public void RemoveItemToList(Item item)
    {
        items.Remove(item);
    }
    public void CheckAllItemsFinished()
    {
        if (items.Count <= 0)
        {
            GameManager.instance.LevelComplete();
        }
    }
    public void ClearList()
    {
        items.Clear();
    }
}
