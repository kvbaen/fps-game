using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform[] weapons;

    [Header("Settings")]
    [SerializeField] private float switchTime;
    [SerializeField] public int selectedWeapon;

    private float timeSinceLastSwitch;
    private bool ShouldSwitchToFirstGun => Input.GetKey(playerController.switchToFirstGunKey) && timeSinceLastSwitch >= switchTime && !menuController._isGamePaused;
    private bool ShouldSwitchToSecondGun => Input.GetKey(playerController.switchToSecondGunKey) && timeSinceLastSwitch >= switchTime && !menuController._isGamePaused;
    private bool ShouldSwitchToKnife => Input.GetKey(playerController.switchToKnifeKey) && timeSinceLastSwitch >= switchTime && !menuController._isGamePaused;
    private bool ShouldSwitchToGranade => Input.GetKey(playerController.switchToGranadeKey) && timeSinceLastSwitch >= switchTime && !menuController._isGamePaused;
    private bool ShouldSwitchToNextWeapon => Input.mouseScrollDelta.y < 0 && !menuController._isGamePaused;
    private bool ShouldSwitchToPreviousWeapon => Input.mouseScrollDelta.y > 0 && !menuController._isGamePaused;
    private PlayerController playerController;
    private MenuController menuController;
    [SerializeField]
    private GunIconsController gunIconsController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    private void Start()
    {
        SetWeapon();
        Select(selectedWeapon);
        timeSinceLastSwitch = 0f;
        menuController = FindObjectOfType<MenuController>();
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
            if (selectedWeapon + 1 > 3)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        else if (ShouldSwitchToPreviousWeapon)
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
            gunIconsController.SwitchActiveWeapon();
        }


        timeSinceLastSwitch += Time.deltaTime;
    }

    private void SetWeapon()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
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
