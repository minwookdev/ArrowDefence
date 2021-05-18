using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerExam : MonoBehaviour
{
    public static ManagerExam Instance;

    public CheckCol[] checkIndexL = new CheckCol[4];
    public CheckCol[] checkIndexR = new CheckCol[4];

    public List<string> indexLArray = new List<string>();
    public List<string> indexRArray = new List<string>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void StartBtnDown()
    {
        for (int i = 0; i < checkIndexL.Length; i++)
        {
            checkIndexL[i].isCheck = true;
        }

        for (int i = 0; i < checkIndexR.Length; i++)
        {
            checkIndexR[i].isCheck = true;
        }

        StartCoroutine(this.GetObjectName());
    }

    private IEnumerator GetObjectName()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < checkIndexL.Length; i++)
        {
            indexLArray.Add(checkIndexL[i].colName);
        }

        for (int i = 0; i < checkIndexR.Length; i++)
        {
            indexRArray.Add(checkIndexR[i].colName);
        }
    }
}
