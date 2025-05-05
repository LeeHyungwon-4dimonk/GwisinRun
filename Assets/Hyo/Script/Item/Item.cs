using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] protected ItemType m_type;
    [SerializeField] protected string m_itemName;
    [SerializeField] protected Sprite m_itemSprite;
    public Sprite ItemSprite => m_itemSprite;
    public string ItemName => m_itemName;

    private Rigidbody m_rigidbody;

    public Item(string name, ItemType type)
    {
        m_itemName = name;
        m_type = type;
    }
    
    void Start()
    {
        m_itemName = m_type.ToString();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    
    public override string ToString()
    {
        return m_itemName;
    }
    
    public void Interact()
    {
        bool added = GameManager.Instance.Inventory.AddItem(this); // 인벤토리에 추가
        
        if(!added)
        {
            //todo 추후 UI에 추가할지 고민
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }

    public void ThrowItem(Vector3 position, Vector3 direction, float force)
    {
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.LookRotation(direction);
        m_rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnDisable()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
    }
}
