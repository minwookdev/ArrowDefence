namespace ActionCat
{
    using System.Collections;
    using UnityEngine;

    public class Skill_Rapid_Shot : AD_BowSkill
    {
        private byte arrowCount;
        private float shotDelay;
        private WaitForSeconds rapidShotWait = new WaitForSeconds(0.2f);

        //WaitForSeconds waitForSec = new WaitForSeconds(0.2f);
        ///Default 
        ///byte  ArrowCount = 3;
        ///float ShotDelay  = 0.2f;
        
        public Skill_Rapid_Shot() : base() { }

        public Skill_Rapid_Shot(string id, string name, string desc, SKILL_LEVEL level, BOWSKILL_TYPE type, Sprite sprite, byte arrowcount, float delay) 
            : base(id, name, desc, level, type, sprite)
        {
            this.arrowCount = arrowcount;
            this.shotDelay  = delay;
        }

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Skill_Rapid_Shot(SkillDataRapidShot data)
            : base(data.SkillId, data.SkillName, data.SkillDesc, data.SkillLevel, data.SkillType, data.SkillIconSprite)
        {
            this.arrowCount = data.ArrowShotCount;
            this.shotDelay  = data.ShotInterval;
        }

        public override void BowSpecialSkill(float anglez, Transform parent, MonoBehaviour mono, 
                                             Vector3 initScale, Vector3 initPos, Vector2 arrowForce, LOAD_ARROW_TYPE type)
        {
            ///Get the GameObject's MonoBehavior and run a Coroutine with it.
            ///R. Skill Class has no life cycle.

            string poolTag = (type == LOAD_ARROW_TYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : 
                                                                         AD_Data.POOLTAG_SUBARROW_LESS;
            mono.StartCoroutine(RapidShot(parent, initScale, initPos, anglez, arrowForce, poolTag));
        }

        private IEnumerator RapidShot(Transform arrowParent, Vector3 arrowInitScale, Vector3 arrowInitPos, 
                                      float facingVec, Vector2 force, string poolTag)
        {
            yield return rapidShotWait;

            //CatLog.Log("Bow Special Effect Occured :: Rapid Shot");

            byte arrowcount = 0;

            while(arrowcount < arrowCount)
            {
                yield return new WaitForSeconds(shotDelay);

                // -3 ~ 3 Range of Random Angle
                short randomAngle = (short)Random.Range(-3, 3 + 1);

                #region OLD_CODE

                /*var newArrow  = CatPoolManager.Instance.LoadEffectedArrow(adBow);

                if (newArrow)
                {
                    //Arrow Set Position
                    newArrow.transform.SetParent(arrowParent);
                    newArrow.transform.localScale = arrowInitScale;
                    newArrow.transform.position = arrowInitPos;
                    //newArrow.transform.rotation = Quaternion.Euler(0f, 0f, facingVec - 90f); //기존 :: 고정 각도
                    newArrow.transform.rotation = Quaternion.Euler(0f, 0f, (facingVec - 90f) + randomAngle);

                    //Arrow Set Active & Shot
                    newArrow.SetActive(true);
                    newArrow.gameObject.GetComponent<AD_Arrow_less>().ShotArrow(newArrow.transform.up * force.magnitude);

                    //arrowRBdoy.isKinematic = false;
                    //arrowRBdoy.gravityScale = 0f;
                    //
                    //newArrow.GetComponent<AD_Arrow>().isLaunched = true;

                    //Force 자체를 집어넣을 경우 force Vector 자체에 방향 값이 있어서 방향 자체도 본체 화살 앵글에 고정되어 버리는 현상
                    //arrowRBdoy.AddForce(newArrow.transform.up * arrowForce, ForceMode2D.Force);

                    //newArrow.GetComponent<AD_Arrow>().arrowTrail.SetActive(true);
                    //newArrow.GetComponent<AD_Arrow>().arrowTrail.GetComponent<TrailRenderer>().Clear();

                    arrowCount++;
                }*/

                #endregion

                var ccArrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, arrowParent, arrowInitScale, arrowInitPos, 
                                                                    Quaternion.Euler(0f, 0f, (facingVec - 90f) + randomAngle));

                if (ccArrow)
                {
                    ccArrow.ShotArrow(ccArrow.transform.up); // * force.magnitude;
                    arrowcount++;
                }
            }

            #region LEGACY_CODE
            //var newArrow = CatPoolManager.Instance.LoadEffectedArrow(adBow);
            //var arrowRBdoy = newArrow.GetComponent<Rigidbody2D>();
            //
            //if (newArrow)
            //{
            //    newArrow.transform.SetParent(arrowParent);
            //    newArrow.transform.localScale = arrowInitScale;
            //    newArrow.transform.position = arrowInitPos;
            //    newArrow.transform.rotation = Quaternion.Euler(0f, 0f, facingVec - 90f);
            //
            //    yield return new WaitForSeconds(0.5f);
            //
            //    newArrow.SetActive(true);
            //
            //    arrowRBdoy.isKinematic = false;
            //    arrowRBdoy.gravityScale = 0f;
            //
            //    newArrow.GetComponent<AD_Arrow>().isLaunched = true;
            //
            //    arrowRBdoy.AddForce(force, ForceMode2D.Force);
            //
            //    newArrow.GetComponent<AD_Arrow>().arrowTrail.SetActive(true);
            //    newArrow.GetComponent<AD_Arrow>().arrowTrail.GetComponent<TrailRenderer>().Clear();
            //}
            #endregion
        }

        public override string ToString() => "Rapid_Shot";
    }
}
