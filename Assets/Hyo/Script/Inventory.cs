using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 6;
    public Item[] items;
    private int m_itemCount;
    
    
    public UnityEvent<string, int> OnUseItem;
    public UnityEvent<int> OnDropOrUseItem;
    public UnityEvent<int, Item> OnAddItem;
    public UnityEvent<int> OnSelectedItemChanged;
    
    //# ???? ?¬í??(20250503) -- ????
    private int m_selectedItemIndex;
    public int SelectedItemIndex => m_selectedItemIndex;
    
    // 250503 ì¶?ê°? :: S
    public int GetSelectedIndex() => m_selectedItemIndex;
    public Item GetItemAt(int index) => items[index];
    public void ClearItemAt(int index)
    {
        items[index] = null;
        m_itemCount--;
        OnDropOrUseItem?.Invoke(index);
    }
    // 250503 ì¶?ê°? :: E

    private void Awake()
    {
        items = new Item[maxSlots];
    }

    void Update()
    {
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            return;
        
        if (GameManager.Instance.Input.ItemsActionPressed)
        {
            m_selectedItemIndex = GameManager.Instance.Input.LastPressedKey;
            // Debug.Log($"{m_selectedItemIndex+ 1} ë²?ì§? ?¬ë¡¯ ????");
            OnSelectedItemChanged?.Invoke(m_selectedItemIndex);
        }
    }
    //# ???? ?¬í??(20250503) -- ??

    public bool AddItem(Item item)
    {
        if (m_itemCount >= maxSlots)
        {
            Debug.Log("?¸ë²¤??ë¦¬ê? ê°??? ì°¼ì?µë????.");
            return false;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                continue;
            
            items[i] = item;
            item.gameObject.SetActive(false);
            OnAddItem?.Invoke(i, items[i]);
            m_itemCount++;
            break;
        }
        
        return true;
    }

    //# ???? ?¬í??(20250503) -- ????
    public void UseItem()
    {
        if (m_selectedItemIndex < 0 || m_selectedItemIndex >= items.Length)
            return;

        if (items[m_selectedItemIndex] is IUsable)
        {
            UsableItem item = items[m_selectedItemIndex] as UsableItem;
            if (item == null)
                return;
            
            OnUseItem?.Invoke(item.ItemName, item.Value); 
            OnDropOrUseItem?.Invoke(m_selectedItemIndex);
            
            items[m_selectedItemIndex] = null;
            item.Use();
            m_itemCount--;
        }
    }

    public bool RemoveKey()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;
            
            if (items[i].ItemName == "Key")
            {
                OnDropOrUseItem?.Invoke(i);
                Destroy(items[i].gameObject);
                items[i] = null;
                m_itemCount--;
                return true;
            }
        }
        return false;
    }
    public void DropItem(Vector3 startPosition, Vector3 direction, float force)
    {
        if (items[m_selectedItemIndex] == null)
            return;

        Item item = items[m_selectedItemIndex];
        if (item == null)
            return;
        
        item.gameObject.SetActive(true);
        item.ThrowItem(startPosition, direction, force);
        OnDropOrUseItem?.Invoke(m_selectedItemIndex);
        
        items[m_selectedItemIndex] = null;
        
        m_itemCount--;
        Debug.Log($"{m_selectedItemIndex + 1} ½½·Ô¿¡ ¾ÆÀÌÅÛÀ» ¹ö¸³´Ï´Ù.");
    }
    //# ¼öÁ¤ »çÇ×(20250503) -- ³¡
}
