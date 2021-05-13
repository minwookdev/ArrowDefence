namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System.Globalization;
    using UnityEngine;

    public class CatPoolManager : Singleton<CatPoolManager>
    {
        //각 Object들 GameObject형이 아닌 Transform형으로 변수 수정
        [Header("Arrow Object Pool Route")]
        public GameObject ArrowObjects;
        public GameObject NormalArrowObjects;
        public GameObject EffectArrowObjects;
        public GameObject SpecialArrowObjects;

        [Header("Arrows")]                 public GameObject normalArrow;
        [Header("How Many ? (MAX : 255)")] public byte normalArrowCounts;

        private void Start()
        {
            this.PoolObjectRouteInitialize();
            this.SetNormalArrowGameObject();
        }

        private void SetNormalArrowGameObject()
        {
            this.normalArrow = Resources.Load("ArrowDefence_Arrows/Object_Arrow") as GameObject;

            GameObject[] nmArrowObj = new GameObject[normalArrowCounts];

            for(int i =0;i<normalArrowCounts;i++)
            {
                nmArrowObj[i] = Instantiate(normalArrow);
                nmArrowObj[i].SetActive(false);
                nmArrowObj[i].transform.SetParent(NormalArrowObjects.transform);
            }
        }

        private void PoolObjectRouteInitialize()
        {
            if (ArrowObjects == null)
                ArrowObjects = this.transform.GetChild(0).gameObject;

            if (NormalArrowObjects == null)
                NormalArrowObjects = ArrowObjects.transform.GetChild(0).gameObject;

            if (EffectArrowObjects == null)
                EffectArrowObjects = ArrowObjects.transform.GetChild(1).gameObject;

            if (SpecialArrowObjects == null)
                SpecialArrowObjects = ArrowObjects.transform.GetChild(2).gameObject;
        }

        public GameObject LoadNormalArrow(AD_BowController adBow)
        {
            //NormalArrows Object내의 Disable된 화살 오브젝트를 반환
            for(int i =0;i<normalArrowCounts;i++)
            {
                if(NormalArrowObjects.transform.GetChild(i).gameObject.activeSelf == false)
                {
                    return NormalArrowObjects.transform.GetChild(i).gameObject;
                }
            }

            //끝내 Disable된 화살 오브젝트를 찾지 못한 경우 for문 다시 돌리도록 기능수정

            return null;
        }
    }
}

