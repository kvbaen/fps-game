using FpsGame.ProjectileGun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunIconsController : MonoBehaviour
{
    [SerializeField]
    private WeaponSwitching weaponSwitching;
    private string iconsLocation = "WeaponIcons";
    public Transform[] gunIconPanels;

    void Start()
    {
        gunIconPanels = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            gunIconPanels[i] = transform.GetChild(i);
        }
        if (weaponSwitching != null)
        {
            RefreshIcons();
        }
    }


    public void RefreshIcons()
    {
        for (int i = 0; i < gunIconPanels.Length; i++)
        {
            TMP_Text weaponNameText = gunIconPanels[i].GetChild(1).GetComponent<TMP_Text>();
            Image weaponImage = gunIconPanels[i].GetChild(2).GetComponent<Image>();
            Transform weapon = weaponSwitching.weapons[i].GetChild(0);
            if (weapon != null && weaponNameText.text != weapon.name)
            {
                string weaponName = weapon.name;
                weaponNameText.text = weaponName;
                string location = iconsLocation + "/" + weaponName + "_Icon";
                ResourceRequest loadingIcon = Resources.LoadAsync<Sprite>(location);
                Color weaponColor = i == weaponSwitching.selectedWeapon ? new Color(255, 255, 255, 1) : new Color(255, 255, 255, 0.6f);
                loadingIcon.completed += (asyncOperation) =>
                {
                    weaponImage.sprite = (Sprite)loadingIcon.asset;
                    weaponImage.color = weaponColor;
                    weaponNameText.color = weaponColor;
                };
            }
            else
            {
                weaponNameText.text = "";
                weaponImage.sprite = null;
            }
        }
    }

    public void SwitchActiveWeapon()
    {
        for (int i = 0; i < gunIconPanels.Length; i++)
        {
            TMP_Text weaponNameText = gunIconPanels[i].GetChild(1).GetComponent<TMP_Text>();
            Image weaponImage = gunIconPanels[i].GetChild(2).GetComponent<Image>();
            Color weaponColor = i == weaponSwitching.selectedWeapon ? new Color(255, 255, 255, 1) : new Color(255, 255, 255, 0.6f);
            weaponImage.color = weaponColor;
            weaponNameText.color = weaponColor;
        }
    }
}
