namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System.Globalization;
    using UnityEngine;

    public class CatPoolManager : Singleton<CatPoolManager>
    {
        //각 Object들 GameObject형이 아닌 Transform형으로 변수 수정
        [Header("Arrow Object Pool Route")]
        public Transform ArrowObjects;
        public Transform NormalArrowObjects;
        public Transform EffectArrowObjects;
        public Transform SpecialArrowObjects;

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
                ArrowObjects = this.transform.GetChild(0);

            if (NormalArrowObjects == null)
                NormalArrowObjects = ArrowObjects.transform.GetChild(0);

            if (EffectArrowObjects == null)
                EffectArrowObjects = ArrowObjects.transform.GetChild(1);

            if (SpecialArrowObjects == null)
                SpecialArrowObjects = ArrowObjects.transform.GetChild(2);
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

            //** 끝내 Disable된 화살 오브젝트를 찾지 못한 경우 for문 다시 돌리도록 기능수정
            //** 비활성화된 Arrow를 찾기위해 탐색하는 위치 Last Index부터 탐색하도록 수정

            return null;
        }

        /// <summary>
        /// PoolManager에서 비활성화 대상을 수집합니다
        /// </summary>
        /// <param name="objKind">오브젝트의 종류 Arrow, Effect, Monster</param>
        /// <param name="objType">종류별 ENUM, AD_GameScripts 참조</param>
        /// <param name="targetObj">회수될 대상, this.gameObject</param>
        public void CollectObject(string objKind, int objType, GameObject targetObj)
        {
            switch (objKind)
            {
                case AD_GameScripts.Arrow:
                    if (objType == (int)AD_GameScripts.ArrowAttrubute.Arrow_Normal)
                    {
                        targetObj.SetActive(false);
                        targetObj.transform.SetParent(this.NormalArrowObjects.transform);
                    }
                    else if (objType == (int)AD_GameScripts.ArrowAttrubute.Arrow_Effect)
                    {

                    }
                    else if (objType == (int)AD_GameScripts.ArrowAttrubute.Arrow_Special)
                    {

                    }
                    else
                    {
                        CatLog.Log("Arrow Object Type is Wrong, Can't Collect Object");
                    }
                    break;
                case AD_GameScripts.Effect:
                    break;
                case AD_GameScripts.Monster:
                    break;
                default:
                    break;
            }
        }
    }
}

