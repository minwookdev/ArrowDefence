namespace CodingCat_Tests
{
    using CodingCat_Games;
    using CodingCat_Scripts;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MultipleShot : MonoBehaviour
    {
        public GameObject thisArrow;
        [SerializeField]
        private int numOfArrows;
        [SerializeField]
        private float arrowSpread;

        public delegate void MultiShotDelegate(MultipleShot multi);
        public MultiShotDelegate multiShot;

        //public event MultiShotDelegate multiShotEvent;

        //private void MultiShot(int arrowcounts, float dist)
        //{
        //    for(int i =0;i<arrowcounts;i++)
        //    {
        //        Quaternion target = Quaternion.AngleAxis((dist * (i - (arrowcounts / 2))), transform.up);
        //        var firingArrow = GameObject.Instantiate(arrow, transform.position, target * transform.rotation);
        //    }
        //}
        //
        private void MultiShotMethod()
        {
            byte numShot = 3;
            float spreadAngle = 25f;
        
            Quaternion qAngle = Quaternion.AngleAxis(-numShot / 2 * spreadAngle, transform.up) * transform.rotation;
            Quaternion qDelta = Quaternion.AngleAxis(spreadAngle, transform.up);
        
            for(int i =0; i < numShot;i++)
            {
                var fArrow = GameObject.Instantiate(thisArrow, transform.position, qAngle);
                fArrow.GetComponent<Rigidbody2D>().AddForce(fArrow.transform.up * 100f);
                qAngle = qDelta * qAngle;
            }
        }

        private void Update()
        {
            //if(Input.GetKeyDown(KeyCode.O))
            //{
            //    //this.MultiShot(4, 180f);
            //    MultiShotMethod();
            //}

            if(Input.GetKeyDown(KeyCode.O))
            {
                ShotMultipleArrow();
            }

            if(Input.GetMouseButtonDown(0))
            {
                this.ShotMultipleArrow2();
            }

            if(Input.GetKeyDown(KeyCode.Z))
            {
                this.MultiShotMethod();
            }
        }

        private void ShotMultipleArrow()
        {
            float facingRotation = Mathf.Atan2(transform.position.y, transform.position.x) * Mathf.Rad2Deg;
            float startRotation = facingRotation + arrowSpread /2f;
            float angleIncrease = arrowSpread / (((float)numOfArrows - 1f));

            for(int i =0;i<numOfArrows;i++)
            {
                float tempRotation = startRotation - angleIncrease * i;
                var newArrow = Instantiate(thisArrow, transform.position, Quaternion.Euler(0f, 0f, tempRotation));
                var temp = newArrow.GetComponent<Bullets>();
                if(temp)
                {
                    temp.Setup(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad),
                                          Mathf.Sin(tempRotation * Mathf.Deg2Rad)));
                }
            }
        }

        private void ShotMultipleArrow2()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float facingRotation = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            //CatLog.Log("FacingRotation : " + facingRotation.ToString());
            float startRotation = facingRotation + arrowSpread / 2f;
            float angleIncrease = arrowSpread / (((float)numOfArrows - 1f));

            for (int i = 0; i < numOfArrows; i++)
            {
                if (i == (numOfArrows * 0.5f) - 0.5f) continue;

                float tempRotation = startRotation - angleIncrease * i;
                //var newArrow = Instantiate(thisArrow, transform.position, Quaternion.Euler(0f, 0f, tempRotation));
                //var newBullet = CatObjectPooler.SpawnFromPool
                //        ("Object_Bullet_Test", transform.position, Quaternion.Euler(0f, 0f, tempRotation));
                //var temp = newBullet.GetComponent<Bullets>();

                var temp = CatObjectPooler.SpawnFromPool<Bullets>("Object_Bullet_Test", transform, transform.position,
                    Quaternion.Euler(0f, 0f, tempRotation));

                if (temp)
                {
                    temp.Setup(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad),
                                          Mathf.Sin(tempRotation * Mathf.Deg2Rad)));
                }
            }
        }
    }
}
