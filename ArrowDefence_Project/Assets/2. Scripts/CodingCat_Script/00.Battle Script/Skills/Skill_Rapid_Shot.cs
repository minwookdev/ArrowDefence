namespace ActionCat
{
    using CodingCat_Scripts;
    using System.Collections;
    using UnityEngine;

    public class Skill_Rapid_Shot : AD_BowSkill
    {
        private byte arrowCount;
        private float shotDelay;

        /// <summary>
        /// Constructor With no Parameters. (Used ES3. Don't Delete this) 
        /// </summary>
        public Skill_Rapid_Shot() : base() { }

        public Skill_Rapid_Shot(byte arrowcount, float delay, string name, string desc, SKILL_LEVEL level, BOWSKILL_TYPE type) 
            : base(name, desc, level, type)
        {
            this.arrowCount = arrowcount;
            this.shotDelay  = delay;
        }

        public override void BowSpecialSkill(float facingVec, float arrowSpreadAngle, byte numOfArrows, Transform arrowParent, 
                                             AD_BowController adBow, Vector3 initScale, Vector3 initPos, Vector2 arrowForce, LOAD_ARROW_TYPE arrowType)
        {
            //base.BowSpecialSkill(facingVec, arrowSpreadAngle, numOfArrows, arrowParent, adBow, arrowInitScale, arrowInitPos, arrowForce);

            string poolTag = (arrowType == LOAD_ARROW_TYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;
            adBow.StartCoroutine(RapidShot(adBow, arrowParent, initScale, initPos, facingVec, arrowForce, numOfArrows, poolTag));
        }

        private IEnumerator RapidShot(AD_BowController adBow, Transform arrowParent, Vector3 arrowInitScale, Vector3 arrowInitPos, 
                                      float facingVec, Vector2 force, byte numOfArrows, string poolTag)
        {
            yield return null;

            //CatLog.Log("Bow Special Effect Occured :: Rapid Shot");

            byte arrowCount = 0;

            while(arrowCount < numOfArrows)
            {
                yield return new WaitForSeconds(0.2f);

                // -3 ~ 3의 랜덤 각도
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
                    ccArrow.ShotArrow(ccArrow.transform.up * force.magnitude);
                    arrowCount++;
                }
            }

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
        }

        public override string ToString()
        {
            return "Rapid_Shot";
        }
    }
}
