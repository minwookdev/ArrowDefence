namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System.Collections.Generic;
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

        [Header("Arrows List")]
        [SerializeField] private List<GameObject> nArrowList = new List<GameObject>();
        [SerializeField] private List<GameObject> eArrowList = new List<GameObject>();
        [SerializeField] private List<GameObject> sArrowList = new List<GameObject>();

        private void Start()
        {
            this.PoolObjectRouteInitialize();
            this.SetNormalArrowGameObject();
        }

        private void SetNormalArrowGameObject()
        {
            this.normalArrow = Resources.Load("ArrowDefence_Arrows/Object_Arrow") as GameObject;
            
            //List Managing Object Pool
            for(int i = 0;i<normalArrowCounts;i++)
            {
                nArrowList.Add(Instantiate(normalArrow));
                nArrowList[i].SetActive(false);
                nArrowList[i].transform.SetParent(NormalArrowObjects, false);
            }
            
            //GameObject[] nmArrowObj = new GameObject[normalArrowCounts];
            //
            //for(int i =0;i<normalArrowCounts;i++)
            //{
            //    nmArrowObj[i] = Instantiate(normalArrow);
            //    nmArrowObj[i].SetActive(false);
            //    nmArrowObj[i].transform.SetParent(NormalArrowObjects.transform, false);
            //}
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
            //for(int i =0;i<normalArrowCounts;i++)
            //{
            //    if(NormalArrowObjects.transform.GetChild(i).gameObject.activeSelf == false)
            //    {
            //        return NormalArrowObjects.transform.GetChild(i).gameObject;
            //    }
            //}

            //** 끝내 Disable된 화살 오브젝트를 찾지 못한 경우 for문 다시 돌리도록 기능수정
            //* Last Index 부터 탐색하도록 수정 완료.
            //* List로 관리되도록 수정 완료.

            for(int i = normalArrowCounts - 1; i >= 0; i--)
            {
                if(nArrowList[i].activeSelf == false)
                {
                    return nArrowList[i];
                }
            }

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
                case AD_Data.Arrow:
                    if (objType == (int)AD_Data.ArrowAttrubute.Arrow_Normal)
                    {
                        targetObj.SetActive(false);
                        targetObj.transform.SetParent(this.NormalArrowObjects.transform, false);
                    }
                    else if (objType == (int)AD_Data.ArrowAttrubute.Arrow_Effect)
                    {

                    }
                    else if (objType == (int)AD_Data.ArrowAttrubute.Arrow_Special)
                    {

                    }
                    else
                    {
                        CatLog.Log("Arrow Object Type is Wrong, Can't Collect Object");
                    }
                    break;
                case AD_Data.Effect:
                    break;
                case AD_Data.Monster:
                    break;
                default:
                    break;
            }
        }

        //Load, Collect Method 오브젝트 종류별로 나눠놓는게 관리에 좋을것 같음
        //추후 Pool로 관리될 오브젝트가 늘어날것을 감안
    }
}

