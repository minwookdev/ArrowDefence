using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    GameObject playerGO;
    Transform playerTR;

    private void Awake()
    {
        playerGO = this.gameObject;
        playerTR = playerGO.transform;
    }

    private void Update()
    {
        PushMoveKey();
    }

    private void PushMoveKey()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        {
            playerTR.position += Vector3.left * 0.1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            playerTR.position += Vector3.right * 0.1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            playerTR.position += Vector3.down * 0.1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            playerTR.position += Vector3.up * 0.1f;
        }
    }
}
