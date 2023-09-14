using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectBtnController : MonoBehaviour
{
    public GameObject enterMapPanelDialog;
    private Button button;
    private MenuController menuController;
    private GameObject menuCanvas;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenDialogEnterMap);
        menuController = FindObjectOfType<MenuController>();
        menuCanvas = transform.parent.transform.parent.gameObject;
        enterMapPanelDialog = menuCanvas.transform.Find("EnterMapPanel_Dialog").gameObject;
    }

    private void OpenDialogEnterMap()
    {
        menuController.selectedMap = button.name[..^3];
        if (enterMapPanelDialog != null)
        {
            TextMeshProUGUI textEnterMapPanelDialog = enterMapPanelDialog.GetComponentInChildren<TextMeshProUGUI>();
            textEnterMapPanelDialog.text = "Do you want to play " + button.name[..^3] + "?";
            enterMapPanelDialog.SetActive(true);
        }
    }
}
