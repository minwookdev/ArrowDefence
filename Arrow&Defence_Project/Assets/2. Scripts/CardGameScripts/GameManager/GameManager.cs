using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    private PlayerData playerData;

    // Sound Value
    public float BgmSoundValue;
    //{
    //    get
    //    {
    //        return BgmSoundValue;
    //    }
    //
    //    set
    //    {
    //        BgmSoundValue = value;
    //    }
    //}

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
            Debug.Log("GameManager Create !!!");

            playerData = new PlayerData();
        }
        else
            Destroy(this.gameObject);
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
                return null;

            return instance;
        }
    }

    private void Update()
    {
        // 치트코드
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            playerData.Damage10PlayerHP();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            playerData.Heal10PlayerHP();
        }
    }

    public PlayerData GetPlayerData()
    {
        if (null == playerData)
            return null;
        else
            return playerData;
    }

    public void CreateNewPlayer()
    {
        playerData.CreateNewPlayer();
        Debug.Log("New Player Create !!!");
    }

    public void HitPlayer(int damage)
    {
        playerData.HitDamage(damage);
    }

    public float GetBgmSoundValue()
    {
        return BgmSoundValue;
    }

    public void SetBgmSoundValue(float value)
    {
        BgmSoundValue = value;
    }
}
