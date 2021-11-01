namespace ActionCat
{
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    public class Skill_Arrow_Rain : AD_BowSkill
    {
        private byte arrowCount;
        private float shotDelay;

        //Debug Options
        private bool isActiveDebugLine = false;
        private bool isDrawLine = false;
        private ArrowRain_LineRender debugLineRender;

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Skill_Arrow_Rain() : base() { }

        public Skill_Arrow_Rain(string id, string name, string desc, SKILL_LEVEL level, BOWSKILL_TYPE type, Sprite sprite, byte arrowcount, float delay) 
            : base(id, name, desc, level, type, sprite)
        {
            this.arrowCount = arrowcount;
            this.shotDelay  = delay;
        }

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Skill_Arrow_Rain(SkillDataArrowRain data)
            : base(data.SkillId, data.SkillName, data.SkillDesc, data.SkillLevel, data.SkillType, data.SkillIconSprite)
        {
            this.arrowCount = data.ArrowShotCount;
            this.shotDelay  = data.ShotInterval;
        }

        public override void BowSpecialSkill(float anglez, Transform arrowParent, MonoBehaviour mono, 
                                             Vector3 initscale, Vector3 initpos, Vector2 force, LOAD_ARROW_TYPE type)
        {
            #region LEGACY_CODE
            //base.BowSpecialSkill(facingVec, arrowSpreadAngle, numOfArrows, arrowParent, adBow, initScale, arrowInitPos, force, arrowType);

            //CatLog.Log("Bow Special Effect Occured :: Arrow Rain");

            //for (int i =0;i < numOfArrows;i++)
            //{
            //    //This Random initial Position is Arrow's Local Position
            //    //var randomArrowPos = new Vector3(Random.Range(-360, 360 + 1),
            //    //                                 Random.Range(-675, -790 - 1), 90f);
            //    //Setting Random Pos at Global Pos
            //    var randomArrowPos = new Vector3(Random.Range(-4f, 4f), 
            //                                     Random.Range(-7.5f, -8.75f), 90f);
            //    var randomdest     = new Vector3(Random.Range(-3.5f, 3.5f),
            //                                     Random.Range(6f, 4f), 90f);
            //
            //    //randomArrow Pos in Global World Position -> Debugging Used
            //    /* left-top     : -4, -7.5
            //       right-top    :  4, -7.5
            //       right-bottom :  4, -8.75
            //       left-bottom  : -4, -8.75 */
            //
            //    #region OLD_CODE
            //    //Arrow Set Position
            //    /*var newArrow = CatPoolManager.Instance.LoadEffectedArrow(adBow);
            //    if(newArrow)
            //    {
            //        newArrow.transform.SetParent(arrowParent);
            //        newArrow.transform.localScale = initScale;
            //        newArrow.transform.localPosition = randomArrowPos;
            //        newArrow.transform.rotation = Quaternion.Euler(0f, 0f, CalcutaleAngle(newArrow.transform.position, randomdest));
            //
            //        //Arrow Set Active & Shot
            //        newArrow.SetActive(true);
            //        newArrow.gameObject.GetComponent<AD_Arrow_less>().ShotArrow(newArrow.transform.up * force.magnitude);
            //    }*/
            //
            //    #endregion
            //
            //    var poolArrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, arrowParent, initScale, randomArrowPos, Quaternion.identity);
            //    if(poolArrow)
            //    {
            //        // LocalPosition이 아닌 Position 으로 좌표잡을수 있도록 변경 -> 변경 완료
            //        //poolArrow.transform.localPosition = new Vector3(randomArrowPos.x, randomArrowPos.y, 90f);
            //        poolArrow.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, CalculateAngle(poolArrow.transform.position, randomdest));
            //        poolArrow.ShotArrow(poolArrow.transform.up * force.magnitude);
            //    }
            //
            //    if (isActiveDebugLine)
            //    {
            //        if (isDrawLine)
            //        {
            //            var LineRenderObject = new GameObject("ArrowRain_LineRender_InitPos").
            //                                             AddComponent<ArrowRain_LineRender>();
            //            this.debugLineRender = LineRenderObject;
            //            isDrawLine = false;
            //        }
            //        debugLineRender.PointMaker(arrowParent, randomArrowPos, randomdest);
            //    }
            //}
            #endregion
            string poolTag = (type == LOAD_ARROW_TYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;
            mono.StartCoroutine(RainArrow(poolTag, arrowParent, initscale, force));
        }

        IEnumerator RainArrow(string poolTag, Transform arrowparent, Vector3 arrowscale, Vector2 force)
        {
            for (int i = 0; i < arrowCount; i++)
            {
                yield return new WaitForSeconds(shotDelay);

                var randomArrowPos = new Vector3(Random.Range(-4f, 4f),
                                                 Random.Range(-7.5f, -8.75f), 90f);
                var randomdest     = new Vector3(Random.Range(-3.5f, 3.5f),
                                                 Random.Range(6f, 4f), 90f);

                //randomArrow Pos in Global World Position -> Debugging Used
                /* left-top     : -4, -7.5
                   right-top    :  4, -7.5
                   right-bottom :  4, -8.75
                   left-bottom  : -4, -8.75 */

                var arrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, arrowparent, arrowscale, randomArrowPos, Quaternion.identity);
                if(arrow)
                {
                    arrow.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, CalculateAngle(arrow.transform.position, randomdest));
                    arrow.ShotArrow(arrow.transform.up * force);
                }

                if(isActiveDebugLine)
                {
                    if(isDrawLine)
                    {
                        var lineRenderObject = new GameObject("ArrowRain_LineRender_InitPos").AddComponent<ArrowRain_LineRender>();
                        debugLineRender = lineRenderObject;
                        isDrawLine = false;
                    }

                    debugLineRender.PointMaker(arrowparent, randomArrowPos, randomdest);
                }
            }
        }

        public float CalculateAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }

        public float GetAngle(Vector3 from, Vector3 to)
        {
            Vector3 v = to - from;
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        public override string ToString()
        {
            return "Arrow_Rain";
        }
    }

    public class ArrowRain_LineRender : MonoBehaviour
    {
        private string matPath         = "Assets/99. ArrowDefence_Resources/Materials/rope.mat";
        private string makerSpritePath = @"Assets\98. MinWook_Assets\Trajectory Aimer 2D\Sprites\Circle.png";

        Vector3[] arrowInitialLine = new Vector3[5] { new Vector3(-4f, -7.5f, 90f),
                                                      new Vector3(4f, -7.5f, 90f),
                                                      new Vector3(4f, -8.75f, 90f),
                                                      new Vector3(-4f, -8.75f, 90f),
                                                      new Vector3(-4f, -7.5f, 90f)};
        Vector3[] arrowDestLine = new Vector3[5] { new Vector3(-3.5f, 6f, 90f),
                                                   new Vector3(3.5f, 6f, 90f),
                                                   new Vector3(3.5f, 4f, 90f),
                                                   new Vector3(-3.5f, 4f, 90f),
                                                   new Vector3(-3.5f, 6f, 90f)};
        private float lineWidth = 0.1f;
        private byte posCount = 5;
        private Material lineMat;

        public void Start()
        {
            //LineMat = Resources.Load
#if UNITY_EDITOR
            var initLineRender = gameObject.AddComponent<LineRenderer>();
            initLineRender.startWidth = lineWidth;
            initLineRender.positionCount = posCount;
            //initLineRender.material = LineMat;
            initLineRender.SetPositions(arrowInitialLine);

            var destLineRender = new GameObject("ArrowRain_LineRender_DestPos").AddComponent<LineRenderer>();
            destLineRender.startWidth = lineWidth;
            destLineRender.positionCount = posCount;
            //destLineRender.material = LineMat;
            destLineRender.SetPositions(arrowDestLine);


            lineMat = (Material)Instantiate((Material)AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)));
            initLineRender.material = lineMat;
            destLineRender.material = lineMat;
            initLineRender.sortingLayerID = SortingLayer.NameToID("Bow");
            initLineRender.sortingOrder = 2;
            destLineRender.sortingLayerID = SortingLayer.NameToID("Bow");
            destLineRender.sortingOrder = 2;
#endif
        }

        public void PointMaker(Transform arrowParent, Vector2 initPos, Vector2 destPos)
        {
#if UNITY_EDITOR
            var posMarker = new GameObject("ArrowRain_dest_marker");
            posMarker.transform.SetParent(arrowParent, true);
            posMarker.transform.localScale = new Vector2(20f, 20f);
            posMarker.transform.position = destPos;
            //posMarker.transform.position.z = 90f;
            Vector3 swap = posMarker.transform.position;
            swap.z = 90f;
            posMarker.transform.position = swap;
            posMarker.AddComponent<SpriteRenderer>().sprite =
               (Sprite)Instantiate(AssetDatabase.LoadAssetAtPath<Sprite>(makerSpritePath));
            posMarker.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Bow");
            posMarker.GetComponent<SpriteRenderer>().sortingOrder = 2;
            Destroy(posMarker, 2f);
#endif
        }
    }
}
