using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectClickObject();
    }
    private void DetectClickObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {

                        var clickedItem = raycastHit.transform.gameObject.TryGetComponent<Item>(out Item item);

                        if (clickedItem && !item.isSelected)
                        {
                            item.ItemSelected();
                            Basket.Instance.AddItemToBasket(item);
                        }
                }
            }
        }
    }
}
