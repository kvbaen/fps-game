using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform[] weapons;

    [Header("Settings")]
    [SerializeField] private float switchTime;
    [SerializeField] private int selectedWeapon;

    private float timeSinceLastSwitch;
    private bool ShouldSwitchToFirstGun => Input.GetKey(playerController.switchToFirstGunKey) && timeSinceLastSwitch >= switchTime;
    private bool ShouldSwitchToSecondGun => Input.GetKey(playerController.switchToSecondGunKey) && timeSinceLastSwitch >= switchTime;
    private bool ShouldSwitchToKnife => Input.GetKey(playerController.switchToKnifeKey) && timeSinceLastSwitch >= switchTime;
    private bool ShouldSwitchToGranade => Input.GetKey(playerController.switchToGranadeKey) && timeSinceLastSwitch >= switchTime;
    private bool ShouldSwitchToNextWeapon => Input.mouseScrollDelta.y < 0;
    private bool ShouldSwitchToPreviousWeapon => Input.mouseScrollDelta.y > 0;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    private void Start()
    {
        SetWeapon();
        Select(selectedWeapon);
        timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        if (ShouldSwitchToFirstGun)
        {
            selectedWeapon = 0;
        }

        if (ShouldSwitchToSecondGun)
        {
            selectedWeapon = 1;
        }

        if (ShouldSwitchToKnife)
        {
            selectedWeapon = 2;
        }

        if (ShouldSwitchToGranade)
        {
            selectedWeapon = 3;
        }
        if (ShouldSwitchToNextWeapon)
        {
            if(selectedWeapon + 1 > 3)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        else if(ShouldSwitchToPreviousWeapon) 
        {
            if (selectedWeapon - 1 < 0)
            {
                selectedWeapon = 3;
            }
            else
            {
                selectedWeapon--; ;
            }
        }
        if (previousSelectedWeapon != selectedWeapon)
        {
            Select(selectedWeapon);
        }


        timeSinceLastSwitch += Time.deltaTime;
    }

    private void SetWeapon()
    {
        weapons = new Transform[transform.childCount];
        
        for(int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }
    }

    private void Select(int weaponIndex)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }
        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected()
    {
        Debug.Log("Switched weapon to: " + weapons[selectedWeapon].name);
    }

}
