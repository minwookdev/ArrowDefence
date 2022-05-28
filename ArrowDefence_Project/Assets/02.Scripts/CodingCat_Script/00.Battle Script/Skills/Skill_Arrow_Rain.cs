namespace ActionCat {
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    public class Skill_Arrow_Rain : AD_BowSkill {
        // [ Saved-Variables ]
        private byte arrowCount;
        private float shotDelay;

        public override string GetNameByTerms() {
            throw new System.NotImplementedException();
        }

        public override string GetDescByTerms() {
            return string.Format(DescByTerms, arrowCount.ToString().GetColor(StringColor.YELLOW));
        }


        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="entity"></param>
        public Skill_Arrow_Rain(SkillDataArrowRain entity) : base(entity) {
            this.arrowCount = entity.ArrowShotCount;
            this.shotDelay  = entity.ShotInterval;
        }
        #region ES3
        public Skill_Arrow_Rain() : base() { }
        #endregion

        public override void Init() {
            
        }

        public override void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type) {
            //string tag = (type == ARROWTYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;
            //controller.StartCoroutine(RainArrow(tag, bowTr, damage));

            if(TryGetTag(type, out string tag)) {
                controller.StartCoroutine(RainArrow(tag, bowTr, damage));
            }
        }

        IEnumerator RainArrow(string poolTag, Transform bowTr, DamageStruct damage) {
            for (int i = 0; i < arrowCount; i++) {
                yield return new WaitForSeconds(shotDelay);

                var randomArrowPos = new Vector3(Random.Range(-4f, 4f), Random.Range(-7.5f, -8.75f), 0f);
                var randomdest     = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(6f, 4f), 0f);

                //randomArrow Pos in Global World Position -> Debugging Used
                /* left-top     : -4, -7.5
                   right-top    :  4, -7.5
                   right-bottom :  4, -8.75
                   left-bottom  : -4, -8.75 */

                var arrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, bowTr.parent.root, GameGlobal.ArrowScale, randomArrowPos, Quaternion.identity);
                if (arrow) {
                    arrow.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, CalculateAngle(arrow.transform.position, randomdest));
                    arrow.ShotToDirection(arrow.transform.up, damage); // * force.magnitude;
                }
            }
        }

        public float CalculateAngle(Vector3 from, Vector3 to) {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }

        public float GetAngle(Vector3 from, Vector3 to) {
            Vector3 v = to - from;
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        public override string ToString() {
            return "Arrow_Rain";
        }
    }
}
