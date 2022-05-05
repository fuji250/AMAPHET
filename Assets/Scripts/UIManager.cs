using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    //public Slider hpSlider;
    public Slider staminaSlider;

    //試合終了パネル
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

    //試合終了パネル表示
    public void OpenEndPanel()
    {
        endPanel.SetActive(true);
    }
}