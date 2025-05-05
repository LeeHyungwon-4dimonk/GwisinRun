using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Item UI Image Components")]
    [SerializeField] private Image[] m_itemImagesUI = new Image[6];
    [SerializeField] private Image m_selectedItemUI;
    [SerializeField] private GridLayoutGroup m_InventoryGridContainer;
    private Vector3 m_startPosition;
    private RectTransform m_selectedItemUIRectTransform;
    private float m_imageSpacing;

    private void OnEnable()
    {
        foreach (var itemImage in m_itemImagesUI)
        {
            itemImage.type = Image.Type.Filled;
            itemImage.preserveAspect = true;
            itemImage.gameObject.SetActive(false);
        }

        m_selectedItemUIRectTransform = m_selectedItemUI.rectTransform;
        m_startPosition = m_selectedItemUIRectTransform.localPosition;

        m_imageSpacing = m_InventoryGridContainer.spacing.x + m_itemImagesUI[0].rectTransform.rect.width + 10;
        
        GameManager.Instance.Inventory.OnSelectedItemChanged.AddListener(DisplaySelectedItemImage);
        GameManager.Instance.Inventory.OnAddItem.AddListener(AddItem);
        GameManager.Instance.Inventory.OnDropOrUseItem.AddListener(RemoveItem);
    }

    private void OnDisable()
    {
        GameManager.Instance.Inventory.OnSelectedItemChanged.RemoveListener(DisplaySelectedItemImage);
    }

    private void DisplaySelectedItemImage(int index)
    {
        m_selectedItemUIRectTransform.localPosition = new Vector3(m_startPosition.x + index * m_imageSpacing, m_startPosition.y, m_startPosition.z);
    }

    private void AddItem(int index, Item item)
    {
        m_itemImagesUI[index].sprite = item.ItemSprite;
        m_itemImagesUI[index].gameObject.SetActive(true);
    }

    private void RemoveItem(int index)
    {
        m_itemImagesUI[index].sprite = null;
        m_itemImagesUI[index].gameObject.SetActive(false);
    }
}
