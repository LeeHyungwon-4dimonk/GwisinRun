using System.Collections;
using UnityEngine;

public class PlayerUseItem : MonoBehaviour
{

    private PlayerController m_playerController;
    private void Start()
    {
        GameManager.Instance.Inventory.OnUseItem.AddListener(UseItem);
    }

    void Update()
    {
        //# 수정 사항(20250502) -- 시작
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            return;
        //# 수정 사항(20250502) -- 끝
        
        //# 수정 사항(20250503) -- 시작
        if(GameManager.Instance.Input.UseItemKeyPressed)
        {
            GameManager.Instance.Inventory.UseItem();
        }
        //# 수정 사항(20250503) -- 끝
    }
    
    public void UseItem(string itemName, int value)
    {
        //# 수정 사항(20250502) -- 시작
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            return;
        //# 수정 사항(20250502) -- 끝
        
        switch (itemName)
        {
            case "StaminaPotion":
                PlayerHealth.CurrentStamina += value;

                if (PlayerHealth.CurrentStamina < 0)
                {
                    PlayerHealth.CurrentStamina = 0;
                }
                else if (PlayerHealth.CurrentStamina > PlayerHealth.MaxStamina)
                {
                    PlayerHealth.CurrentStamina = PlayerHealth.MaxStamina;
                }
                break;
            
            case "HeartPotion":
                PlayerHealth.CurrentHealth += value;

                if (PlayerHealth.CurrentHealth < 0)
                {
                    PlayerHealth.CurrentHealth = 0;
                    PlayerController.Die();
                }
                else if (PlayerHealth.CurrentHealth > PlayerHealth.MaxHealth)
                {
                    PlayerHealth.CurrentHealth = PlayerHealth.MaxHealth;
                }
                break;
            
            case "SpeedPotion":
                StartCoroutine(ItemSpeed(value));
                break;
            default: 
                
                break;
        }
    }

    IEnumerator ItemSpeed(float value)
    {
        PlayerMove.ItemSpeed = value;
        WaitForSeconds time = new WaitForSeconds(0.1f);

        if (value > 0)
        {
            while (PlayerMove.ItemSpeed > 0f)
            {
                yield return time;
                PlayerMove.ItemSpeed -= 0.1f;
            }
        }
        else if (value < 0)
        {
            while (PlayerMove.ItemSpeed < 0f)
            {
                yield return time;
                PlayerMove.ItemSpeed += 0.1f;
            }
        }
    }
}
