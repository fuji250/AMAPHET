using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    //public Slider hpSlider;
    public Slider staminaSlider;

    //�����I���p�l��
    public GameObject endPanel;

    public void Init(PlayerManager playerManager)
    {
        //hpSlider.maxValue = playerManager.maxHp;
        //hpSlider.value = playerManager.maxHp;
        staminaSlider.maxValue = playerManager.maxStamina;
        staminaSlider.value = playerManager.maxStamina;
    }
    public void UpdateHP(int hp)
    {
        //hpSlider.DOValue(hp, 0.5f);
    }
    public void UpdateStamina(float stamina)
    {
        staminaSlider.DOValue(stamina,0.3f);
    }

    //�����I���p�l���\��
    public void OpenEndPanel()
    {
        endPanel.SetActive(true);
    }
}