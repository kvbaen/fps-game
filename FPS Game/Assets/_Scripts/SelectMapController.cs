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
    private float paginationNumber = 1;
    private int currentPaginationIndex = 1;
    private List<Button> mapsButtons = new();
    [SerializeField]
    public Button previousBTN, nextBTN;
    void Start()
    {
        menuController = FindObjectOfType<MenuController>();
        rectTransform = GetComponent<RectTransform>();
        CreateMapBTNS();
        if (mapsButtons.Count > 6) nextBTN.gameObject.SetActive(true);
        for (int i = 0; i < mapsButtons.Count; i++)
        {
            mapsButtons[i].gameObject.SetActive(i >= (currentPaginationIndex - 1) * 6 && i < currentPaginationIndex * 6);
        }
    }

    private void CreateMapBTNS()
    {
        if (menuController.mapNames != null)
        {
            float widthBTN = (rectTransform.rect.width - 200) / 3;
            float heightBTN = (rectTransform.rect.height - 150) / 2;
            for (int i = 0; i < menuController.mapNames.Count; i++)
            {
                int j = i % 6;
                Vector3 positionBTN = new((j % 3 + 1) * 50 + (j % 3) * widthBTN, j / 3 > 0 ? -heightBTN * Mathf.Floor(j / 3) - 50 * (Mathf.Floor(j / 3) + 1) : -50, 0);
                Button mapBTN = Instantiate(mapBTNPrefab);
                mapBTN.name = menuController.mapNames[i] + "BTN";
                mapBTN.transform.SetParent(transform);
                RectTransform rectMapBTN = mapBTN.GetComponent<RectTransform>();
                TextMeshProUGUI mapName = mapBTN.GetComponentInChildren<TextMeshProUGUI>();
                mapName.text = menuController.mapNames[i];
                rectMapBTN.sizeDelta = new Vector2(widthBTN, heightBTN);
                rectMapBTN.localScale = Vector3.one;
                rectMapBTN.anchoredPosition = positionBTN;
                mapsButtons.Add(mapBTN);
            }
            paginationNumber = Mathf.Ceil(mapsButtons.Count / 6f);
        }
    }

    public void ChangePagination(bool next)
    {
        if (next && (currentPaginationIndex + 1) <= paginationNumber) currentPaginationIndex++;
        else if (!next && currentPaginationIndex - 1 > 0) currentPaginationIndex--;

        for (int i = 0; i < mapsButtons.Count; i++)
        {
            mapsButtons[i].gameObject.SetActive(i >= (currentPaginationIndex - 1) * 6 && i < currentPaginationIndex * 6);
        }
        nextBTN.gameObject.SetActive((currentPaginationIndex + 1) <= paginationNumber);
        previousBTN.gameObject.SetActive((currentPaginationIndex - 1) > 0);
    }
}
