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

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }
    private void OnEnable() // Reset To Start State
    {
        if (originalScale!=Vector3.zero)
        {
            transform.localScale = originalScale;
        }

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        isSelected = false;
        collider.enabled = true;
        rb.constraints = RigidbodyConstraints.None;
    }
    public void ItemSelected()
    {
        isSelected = true;
        collider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
