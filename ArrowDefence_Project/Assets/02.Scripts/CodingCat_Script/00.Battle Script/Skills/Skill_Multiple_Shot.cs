namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public class Skill_Multiple_Shot : AD_BowSkill, IToString {
        private byte arrowCount;
        private float spreadAngle;

        public override string GetNameByTerms() {
            I2.Loc.LocalizedString loc = termsName;
            return loc;
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = termsDesc;
            return string.Format(loc, arrowCount);
        }

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="entity"></param>
        public Skill_Multiple_Shot(SkillDataSpreadShot entity) : base(entity) {
            this.arrowCount  = entity.ArrowShotCount;
            this.spreadAngle = entity.SpreadAngle;
        }
        #region ES3
        public Skill_Multiple_Shot() : base() { }
        #endregion

        public override void Init() {
            
        }

        public override string ToString() => "Spread_Shot";

        public override void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type) {
            //SpreadShot(bowTr, type, initPos, ref damage);

            if(TryGetTag(type, out string tag)) {
                SpreadShot(bowTr, tag, initPos, ref damage);
            }
        }

        void SpreadShot(Transform bowTr, string tag, Vector3 initPos, ref DamageStruct damage) {
            float startRotation = bowTr.eulerAngles.z + spreadAngle / 2f;
            float angleIncrease = spreadAngle / ((float)arrowCount - 1f);

            for (int i = 0; i < arrowCount; i++) {
                if (i == (arrowCount * 0.5f) - 0.5f) continue; //Cancel Launch Middle Arrow

                float tempRotation = startRotation - angleIncrease * i;

                var arrow = CCPooler.SpawnFromPool<AD_Arrow_less>(tag, bowTr.parent.root, GameGlobal.ArrowScale, initPos,
                                                                  Quaternion.Euler(0f, 0f, tempRotation - 90f));
                if (arrow) {
                    arrow.ShotToDirection(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)), damage); 
                    // * force.magnitude;
                }
            }
        }

        #region LEGACY
        public void BowSpecialSkill(float anglez, Transform parent, MonoBehaviour mono, ref DamageStruct damage,
                                     Vector3 initScale, Vector3 initPos, Vector2 arrowForce, ARROWTYPE arrowType) {
            //float facingRotation = Mathf.Atan2(facingVec.y, facingVec.x) * Mathf.Rad2Deg; //this mean's transform.up..?

            #region LEGACY_CODE
            //float startRotation = facingVec + spreadAngle / 2f;
            //float angleIncrease = spreadAngle / ((float)arrowCount - 1f);
            //
            ////CatLog.Log("Bow Special Effect Occured :: Multiple Shot");
            //
            //string poolTag = (arrowType == LOAD_ARROW_TYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;
            //
            //for (int i = 0; i < arrowCount; i++)
            //{
            //    if (i == (arrowCount * 0.5f) - 0.5f) continue; //Cancel Launch Middle Arrow
            //
            //    float tempRotation = startRotation - angleIncrease * i;
            //
            //    #region OLD_CODE
            //
            //    //var newArrow = CatPoolManager.Instance.LoadEffectedArrow(adBow);
            //    //var arrowRBody = newArrow.GetComponent<Rigidbody2D>();
            //    //var arrowComponent = newArrow.GetComponent<AD_Arrow>();
            //
            //    //if (newArrow)
            //    //{
            //    //    //Arrow Set Position
            //    //    newArrow.transform.SetParent(arrowParent, false);
            //    //    newArrow.transform.localScale = initScale;
            //    //    newArrow.transform.position = initPos;
            //    //    newArrow.transform.rotation = Quaternion.Euler(0, 0, tempRotation - 90);
            //    //
            //    //    //Arrow Set Active & Shot Arrow
            //    //    newArrow.SetActive(true);
            //    //    newArrow.GetComponent<AD_Arrow_less>().ShotArrow(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad),
            //    //                                                                 Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude);
            //    //
            //    //    //arrowRBody.isKinematic = false;
            //    //    //arrowRBody.gravityScale = 0f;
            //    //    //newArrow.GetComponent<AD_Arrow>().isLaunched = true;
            //    //    //
            //    //    //arrowRBody.AddForce(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude,
            //    //    //                        ForceMode2D.Force);
            //    //    //
            //    //    //arrowComponent.arrowTrail.SetActive(true);
            //    //    //arrowComponent.arrowTrail.GetComponent<TrailRenderer>().Clear();
            //    //}
            //
            //    #endregion
            //
            //    var poolArrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, arrowParent, initScale, initPos,
            //                                           Quaternion.Euler(0f, 0f, tempRotation - 90f));
            //
            //    if (poolArrow)
            //    {
            //        poolArrow.ShotArrow(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude);
            //    }
            //}

            #endregion
            SpreadShot(anglez, arrowType, parent, ref damage, initScale, initPos, arrowForce);
        }

        void SpreadShot(float bowEulerAngleZ, ARROWTYPE arrowType, Transform parent, ref DamageStruct damage,
                     Vector3 arrowScale, Vector3 arrowPos, Vector2 force) {
            float startRotation = bowEulerAngleZ + spreadAngle / 2f;
            float angleIncrease = spreadAngle / ((float)arrowCount - 1f);

            string poolTag = (arrowType == ARROWTYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;

            for (int i = 0; i < arrowCount; i++)
            {
                if (i == (arrowCount * 0.5f) - 0.5f) continue; //Cancel Launch Middle Arrow

                float tempRotation = startRotation - angleIncrease * i;

                var arrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, parent, arrowScale, arrowPos,
                                                                  Quaternion.Euler(0f, 0f, tempRotation - 90f));
                if (arrow)
                    arrow.ShotToDirection(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)), damage); // * force.magnitude;
            }
        }
        #endregion
    }
}
