using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : BaseUI
{
    List<Item> inventoryData;
    List<Image> inventoryImage;
    List<TMP_Text> inventoryCount;

    private int page = 0;
    protected override void Awake()
    {
        base.Awake();

        buttons["Previous"].onClick.AddListener(OnPreviousButton);
        buttons["Next"].onClick.AddListener(OnNextButton);

        inventoryData = GameManager.Data.PlayerStatusData.inventory;
        inventoryImage = new List<Image>();
        inventoryCount = new List<TMP_Text>();

        RectTransform[] children = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in children)
        {

            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null)
            {
                if (text.gameObject.name == "InventorySlotItemCount")
                    inventoryCount.Add(text);
            }                

            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                if (image.gameObject.name == "InventorySlotIcon")
                    inventoryImage.Add(image);
            }

        }

    }

    private void OnEnable()
    {
        page = 0;
        UpdateUI();
    }
    public void UpdateUI()
    {

        for(int i = 0; i < inventoryImage.Count; i++)
        {
            int inventoryIndex = i + (page * inventoryImage.Count);   
            if (inventoryData.Count > inventoryIndex)
            {
                inventoryImage[i].color = Color.white;
                inventoryCount[i].enabled = true;
                inventoryImage[i].sprite = inventoryData[inventoryIndex].Data.sprite;
                inventoryCount[i].text = inventoryData[inventoryIndex].Count.ToString();
                if (inventoryCount[i].text == "0")
                {
                    inventoryCount[i].enabled = false;
                }
                else
                {
                    inventoryCount[i].enabled = true;
                }
            }
            else
            {
                inventoryImage[i].color = Color.black;
                inventoryImage[i].sprite = null;
                inventoryCount[i].enabled = false;
            }           

        }

    }

    public void OnPreviousButton()
    {
        if (page > 0)
        {
            page--;
            UpdateUI();
        }
    }

    public void OnNextButton()
    {
        page++;
        UpdateUI();
    }

}
