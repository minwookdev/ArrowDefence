using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSoundValue : MonoBehaviour
{
    public List<AudioSource> SoundList; 

    private void Awake()
    {
        SetSoundVolume();
    }

    public void SetSoundVolume()
    {
        if (0 < SoundList.Count)
        {
            if (null != GameManager.Instance)
            {
                for (int i = 0; i < SoundList.Count; ++i)
                {
                    SoundList[i].volume = GameManager.Instance.BgmSoundValue;
                }
            }
            else
            {
                Debug.Log("GameManager Null Ref !!!");
            }
        }
        else
        {
            Debug.Log("SoundList Empty !!!");
        }
    }
}
