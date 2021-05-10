using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int trapDamage;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ("Player" == collision.tag)
        {
            GameManager.Instance.HitPlayer(trapDamage);
            Debug.Log("Player Trap Damage !!!");

            PlayerMove playMV = collision.GetComponent<PlayerMove>();

            Vector3 DirectionValue;
            DirectionValue = collision.transform.position - this.transform.position;

            playMV.PlayerKnockBack(DirectionValue);
        }
    }
}
