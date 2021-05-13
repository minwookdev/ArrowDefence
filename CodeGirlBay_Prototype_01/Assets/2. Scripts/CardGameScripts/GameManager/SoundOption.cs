using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    public List<Sprite> soundHandleImageList;
    public Image soundHandleImageObj;
    public Scrollbar soundScrollBarObj;
    public List<AudioSource> soundObj;


    private void Awake()
    {
        // Base Sound Value 0.05f
        soundScrollBarObj.value = 0.05f;
    }

    private void Update()
    {
        if (0 < soundObj.Count)
        {
            SetSoundVolume();
        }
    }

    private void SetSoundVolume()
    {
        float scrollBarValue = soundScrollBarObj.value * 0.1f;

        for (int i = 0; i < soundObj.Count; ++i)
        {
            if(soundObj[i].volume == scrollBarValue)
            {
                continue;
            }

            else
            {
                soundObj[i].volume = scrollBarValue;

                if(66 <= scrollBarValue * 1000)
                {
                    soundHandleImageObj.sprite = soundHandleImageList[2];
                }
                else if(33 <= scrollBarValue * 1000)
                {
                    soundHandleImageObj.sprite = soundHandleImageList[1];
                }
                else
                {
                    soundHandleImageObj.sprite = soundHandleImageList[0];
                }

                GameManager.Instance.SetBgmSoundValue(scrollBarValue);
            }
        }
    }

}
