namespace CodingCat_Tests
{
    using CodingCat_Games;
    using System;
    using UnityEngine;

    public class Bullets : MonoBehaviour
    {
        public Rigidbody2D RBody;
        public float Speed = 10f;
        //private float destTime = 1f;

        private MultipleShot multipleShot;

        private void Start()
        {
            RBody = GetComponent<Rigidbody2D>();
            //Destroy(this.gameObject, destTime);

            multipleShot = GameObject.FindObjectOfType<MultipleShot>();
            multipleShot.multiShot += TestMethod;
            //multipleShot.multiShot += TestMethod; 중복으로 함수를 넣는것도 가능
            multipleShot.multiShot += TestMethod2;
            //이렇게 다른곳에서도 호출이 가능.
            multipleShot.multiShot(multipleShot);

            //multipleShot.multiShotEvent += TestMethod;
            //multipleShot.multiShotEvent += TestMethod;  //Event도 중복으로 삽입 메서드 삽입 가능..?
            //multipleShot.multiShotEvent(multipleShot);  보닌이 아닌 다른 곳에서는 사용 불가.
            //Invoke("DisableBullet", 1f);
        }

        private void OnEnable() => Invoke("DisableBullet", 1f);

        public void Setup(Vector2 moveDirection)
        {
            RBody.velocity = moveDirection.normalized * Speed;
        }

        private void TestMethod(MultipleShot multi)
        {

        }

        private void TestMethod2(MultipleShot multi)
        {

        }

        private void DisableBullet() => gameObject.SetActive(false);

        private void OnDisable()
        {
            CatObjectPooler.ReturnToPool(gameObject);
            CancelInvoke();
        }
    }
}
