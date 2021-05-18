using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCol : MonoBehaviour
{
    public bool isCheck = false;
    public string colName = null;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(isCheck)
        {
            if(collision.gameObject != null)
            {
                colName = collision.gameObject.name;
                isCheck = false;    
            }
            else
            {
                colName = "EMPTY";
                isCheck = false;
            }
        }
    }
}
