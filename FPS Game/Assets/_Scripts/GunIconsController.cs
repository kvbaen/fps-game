using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GunIconsController : MonoBehaviour
{
    [SerializeField] private WeaponSwitching weaponSwitching;
    [SerializeField] private string iconsLocation = "WeaponIcons";
    public Transform[] gunIconPanels;

    private Dictionary<string, Sprite> weaponIcons = new Dictionary<string, Sprite>();

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
            TMP_Text weaponNameTextUI = gunIconPanels[i].GetChild(1).GetComponent<TMP_Text>();
            Image weaponImage = gunIconPanels[i].GetChild(2).GetComponent<Image>();

            Transform weapon = weaponSwitching.weapons[i].GetChild(0);

            if (weapon != null && weaponNameTextUI.text != weapon.name)
            {
                string weaponName = weapon.name;
                weaponNameTextUI.text = weaponName;
                string location = $"{iconsLocation}/{weaponName}_Icon";
                if (weaponIcons.ContainsKey(weaponName))
                {
                    weaponImage.sprite = weaponIcons[weaponName];
                }
                else
                {
                    ResourceRequest loadingIcon = Resources.LoadAsync<Sprite>(location);
                    Color weaponColor = i == weaponSwitching.selectedWeapon ? Color.white : new Color(1f, 1f, 1f, 0.6f);
                    loadingIcon.completed += (asyncOperation) =>
                    {
                        Sprite loadedIcon = (Sprite)loadingIcon.asset;
                        weaponIcons[weaponName] = loadedIcon;
                        weaponImage.sprite = loadedIcon;
                        weaponImage.color = weaponColor;
                        weaponNameTextUI.color = weaponColor;
                    };
                }
            }
            else
            {
                weaponNameTextUI.text = "";
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