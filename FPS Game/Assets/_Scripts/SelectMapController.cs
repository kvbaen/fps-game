using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectMapController : MonoBehaviour
{
    private MenuController menuController;
    private RectTransform rectTransform;
    [SerializeField] private Button mapBTNPrefab;
    void Start()
    {
        menuController = FindObjectOfType<MenuController>();
        rectTransform = GetComponent<RectTransform>();
        CreateMapBTNS();
    }

    private void CreateMapBTNS()
    {
        if (menuController.mapNames != null)
        {
            float widthBTN = (rectTransform.rect.width - 100) / 3;
            float heightBTN = (rectTransform.rect.height - 100) / 2;
            for (int i = 0; i < menuController.mapNames.Count; i++)
            {
                Vector3 positionBTN = new((i + 1) * 50 + (i % 3) * widthBTN, ((i + 1) / 3 + 1) * -50 + (i + 1) / 3 * -heightBTN, 0);
                Button mapBTN = Instantiate(mapBTNPrefab);
                mapBTN.name = menuController.mapNames[i] + "BTN";
                mapBTN.transform.SetParent(transform);
                RectTransform rectMapBTN = mapBTN.GetComponent<RectTransform>();
                TextMeshProUGUI mapName = mapBTN.GetComponentInChildren<TextMeshProUGUI>();
                mapName.text = menuController.mapNames[i];
                rectMapBTN.sizeDelta = new Vector2(widthBTN - 50, heightBTN - 50);
                rectMapBTN.anchoredPosition = positionBTN;
            }
        }
    }
}
