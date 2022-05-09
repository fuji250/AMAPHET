using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    //public Slider hpSlider;
    public Slider staminaSlider;
    public Image staminaSliderColor;

    //試合終了パネル
    public GameObject endPanel;
    public Text endText;

    [NonSerialized]
    public Text winnerPlayer;

    

    public void Init(PlayerManager playerManager)
    {
        //hpSlider.maxValue = playerManager.maxHp;
        //hpSlider.value = playerManager.maxHp;
        staminaSlider.maxValue = playerManager.maxStamina;
        staminaSlider.value = playerManager.maxStamina;
    }

    private void Update()
    {
        if (staminaSlider.value < 30f)
        {
            staminaSliderColor.color = new Color32(69,120,153,255);
        }
        else
        {
            staminaSliderColor.color = new Color32(32,61,80,255);
        }
    }

    public void UpdateHP(int hp)
    {
        //hpSlider.DOValue(hp, 0.5f);
    }
    public void UpdateStamina(float stamina)
    {
        staminaSlider.DOValue(stamina,0.3f);
    }

    //試合終了パネル表示
    public void OpenEndPanel()
    {
        endText = winnerPlayer;
        endPanel.SetActive(true);
    }
}