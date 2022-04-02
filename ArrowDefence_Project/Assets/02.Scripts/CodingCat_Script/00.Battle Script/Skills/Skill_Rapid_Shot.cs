namespace ActionCat {
    using System.Collections;
    using UnityEngine;

    public class Skill_Rapid_Shot : AD_BowSkill {
        private byte arrowCount;
        private float shotDelay;
        ACEffector2D muzzleEffect = null;

        //Not Saved
        string effectPoolTag = "";
        WaitForSeconds rapidShotWait = new WaitForSeconds(0.2f);

        public override string GetNameByTerms() {
            I2.Loc.LocalizedString loc = termsName;
            return loc;
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = termsDesc;
            return string.Format(loc, arrowCount);
        }

        public override string ToString() => "Rapid_Shot";

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="entity"></param>
        public Skill_Rapid_Shot(SkillDataRapidShot entity) : base(entity) {
            this.arrowCount   = entity.ArrowShotCount;
            this.shotDelay    = entity.ShotInterval;
            this.muzzleEffect = entity.muzzleEffect;
        }
        #region ES3
        public Skill_Rapid_Shot() : base() { }
        #endregion

        public override void Init() {
            effectPoolTag = "Eff_Rapid_Muzzle_01";
            CCPooler.AddPoolList(effectPoolTag, 5, muzzleEffect.gameObject, false);
            CatLog.Log("RAPID SHOT INITIALIZE COMPLETE.");
        }

        public override void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type) {
            ///Get the GameObject's MonoBehavior and run a Coroutine with it.
            ///R. Skill Class has no life cycle.
            //string poolTag = (type == ARROWTYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;
            //controller.StartCoroutine(RapidShot(bowTr.parent.root, bowTr.eulerAngles, damage, initPos, poolTag, controller.effectTr.position));

            if(TryGetTag(type, out string tag)) {
                controller.StartCoroutine(RapidShot(bowTr.parent.root, bowTr.eulerAngles, damage, initPos, tag, controller.effectTr.position));
            }
        }

        private IEnumerator RapidShot(Transform parentTr, Vector3 eulerAngles, DamageStruct damage, Vector3 arrowInitPos, string poolTag, Vector2 effectPos) {
            yield return rapidShotWait;

            byte arrowcount = 0;

            while(arrowcount < arrowCount) {
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

                var randomRotation = Quaternion.Euler(0f, 0f, (eulerAngles.z - 90f) + randomAngle);
                var ccArrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, parentTr, GameGlobal.ArrowScale, arrowInitPos, randomRotation); //90f is offset

                if (ccArrow) {
                    ccArrow.ShotToDirection(ccArrow.transform.up, damage); // * force.magnitude;
                    arrowcount++;
                }

                // RapidShot Muzzle Effect Active
                CCPooler.SpawnFromPool<ACEffector2D>(effectPoolTag, effectPos, Quaternion.identity).PlayOnce(randomRotation.eulerAngles.z);
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
    }
}
