using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public Text playerHPValue;
    public Image playerFrontHPBar;


    private void Update()
    {
        SetPlayerHPValue();
        SetPlayerFrontHPBar();
    }

    public void SetPlayerHPValue()
    {
        if (null != GameManager.Instance.GetPlayerData())
        {
            playerHPValue.text = GameManager.Instance.GetPlayerData().GetPlayerHP().ToString();
        }
    }

    public void SetPlayerFrontHPBar()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();

        playerFrontHPBar.fillAmount = playerData.GetPlayerHP() / (float)playerData.GetPlayerMaxHP();
    }
}
