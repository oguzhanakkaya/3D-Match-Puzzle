using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public ItemTypes itemType;
    public bool isSelected;
    private Rigidbody rb;
    private Collider collider;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }
    public void ItemSelected()
    {
        isSelected = true;
        collider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
