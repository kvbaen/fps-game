using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public bool SlotFull = false;
    [SerializeField]
    public int maxCountItems = 1;
    private void Update()
    {
        SlotFull = transform.childCount >= maxCountItems;
    }


}
