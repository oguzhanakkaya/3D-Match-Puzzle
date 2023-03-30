using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class Basket : MonoBehaviour
{
    public static Basket Instance;

    private List<Item> itemsInBasket=new List<Item>();
    private List<GameObject> basketList= new List<GameObject>();

    private const int numberOfMaxItemInBasket = 7;

    private LevelArea levelArea;
    private LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AddBasketsToList();

        levelArea = LevelArea.Instance;
        levelManager = LevelManager.instance;
    }

    public void AddItemToBasket(Item item)
    {
        levelArea.RemoveItemToList(item);

        if (itemsInBasket.Count==0) // No Item In Basket
        {
            itemsInBasket.Add(item);
            ItemMoveToBasket(0, item);
        }
        else if (itemsInBasket.Count+1>numberOfMaxItemInBasket)  // Basket Full And No Match
        {
            GameManager.instance.LevelFail();
        }
        else  // Other
        {
            for ( int i = itemsInBasket.Count-1; i>=0; i--)
            {
                if (itemsInBasket[i].itemType == item.itemType)
                {
                    itemsInBasket.Insert(i + 1, item);
                    ItemMoveToBasket(i+1,item);

                    return;
                }
            }

            itemsInBasket.Add(item);
            ItemMoveToBasket(itemsInBasket.Count-1,item);

        }
    }
    private void ItemMoveToBasket(int i,Item item)
    {
        CheckItemPositions();
        Invoke("CheckMatch",.2f);
    }
    private void CheckItemPositions()
    {
        for(int i = 0; i < itemsInBasket.Count; i++)
        {
            if (itemsInBasket[i].transform.parent != basketList[i])
            {
                itemsInBasket[i].transform.DOMove(basketList[i].transform.position, .2f);
            }
        }
    }
    private void CheckMatch()
    {
        if (itemsInBasket.Count>=3)
        {
            for (int i = 0; i < itemsInBasket.Count; i++)
            {
                var isMatched = i + 2 < itemsInBasket.Count
                    && itemsInBasket[i].itemType == itemsInBasket[i + 1].itemType
                    && itemsInBasket[i].itemType == itemsInBasket[i + 2].itemType;
                
                if (isMatched)
                {
                    MatchMove(itemsInBasket[i], itemsInBasket[i+1], itemsInBasket[i+2]);
                }

            }
        }
    }
    private void MatchMove(Item leftItem,Item midItem,Item rightItem)
    {
        DG.Tweening.Sequence mySequence = DOTween.Sequence();
        mySequence.
            Join(leftItem.transform.DOMove(midItem.gameObject.transform.position, .2f))
            .Join(rightItem.transform.DOMove(midItem.gameObject.transform.position, .2f))
            .OnComplete(() =>
            {
                ItemMatched(leftItem);
                ItemMatched(midItem);
                ItemMatched(rightItem);
            
                CheckItemPositions();
                levelArea.CheckAllItemsFinished();
            });
        
       
    }
    private void ItemMatched(Item item)
    {
        itemsInBasket.Remove(item);
        levelManager.RecycleItem(item.gameObject);
    }
    private void AddBasketsToList()
    {
        foreach (Transform child in transform)
        {
            basketList.Add(child.gameObject);
        }
    }
    public void ClearBasketList()
    {
        itemsInBasket.Clear();
    }
}
