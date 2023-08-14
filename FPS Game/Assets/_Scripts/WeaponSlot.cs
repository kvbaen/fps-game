using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public bool SlotFull = false;
    [SerializeField]
    public int maxCountItems = 1;
    public int SlotCount;
    private void Update()
    {
        SlotCount = transform.childCount;
        SlotFull = SlotCount >= maxCountItems;
    }


}
