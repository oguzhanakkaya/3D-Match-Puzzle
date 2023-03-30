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

    private const int numberOfMaxItemInBasket = 7;

    private List<Item> itemsInBasket=new List<Item>();
    private GameObject[] basketList= new GameObject[numberOfMaxItemInBasket];

    private LevelArea levelArea;
    private LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AddBasketsToArray();

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
        item.transform.localScale *= .75f;
        StartCoroutine(CheckItemPositions());
    }
    private IEnumerator CheckItemPositions()
    {
        for(int i = 0; i < itemsInBasket.Count; i++)
        {
            if (itemsInBasket[i].transform.parent != basketList[i])
            {
                itemsInBasket[i].transform.DOMove(basketList[i].transform.position, .2f);
            }
        }

        yield return new WaitForSeconds(.2f);
        CheckMatch();
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
        CheckBasketIsFull();
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
    private void CheckBasketIsFull()
    {
        if (itemsInBasket.Count >= numberOfMaxItemInBasket)
        {
            GameManager.instance.LevelFail();
        }
    }
    private void ItemMatched(Item item)
    {
        itemsInBasket.Remove(item);
        levelManager.RecycleItem(item.gameObject);
    }
    private void AddBasketsToArray()
    {
        for(int i = 0; i < numberOfMaxItemInBasket; i++)
        {
            basketList[i] = transform.GetChild(i).gameObject;
        }
    }
    public void ClearBasketList()
    {
        itemsInBasket.Clear();
    }
}
