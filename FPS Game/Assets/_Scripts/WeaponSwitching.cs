using FpsGame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform[] weapons;

    [Header("Settings")]
    [SerializeField] private float switchTime;
    [SerializeField] private int selectedWeapon;

    private float timeSinceLastSwitch;
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = GetComponentInParent<InputManager>();
        SetWeapon();
        Select(selectedWeapon);
        timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        if (_inputManager.SwitchToRifle && timeSinceLastSwitch >= switchTime)
        {
            selectedWeapon = 0;
        }

        if (_inputManager.SwitchToPistol && timeSinceLastSwitch >= switchTime)
        {
            selectedWeapon = 1;
        }

        if (_inputManager.SwitchToKnife && timeSinceLastSwitch >= switchTime)
        {
            selectedWeapon = 2;
        }

        if (_inputManager.SwitchToGrenade && timeSinceLastSwitch >= switchTime)
        {
            selectedWeapon = 3;
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
