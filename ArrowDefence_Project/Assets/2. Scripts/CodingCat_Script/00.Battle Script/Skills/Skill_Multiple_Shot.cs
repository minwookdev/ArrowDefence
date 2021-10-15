namespace CodingCat_Games
{
    using UnityEngine;
    using CodingCat_Scripts;
    using System;

    public class Skill_Multiple_Shot : AD_BowSkill
    {
        private byte arrowCount;
        private float spreadAngle;

        public override void BowSpecialSkill(float facingVec, float arrowSpreadAngle, byte numOfArrows, Transform arrowParent,
                                  AD_BowController adBow, Vector3 initScale, Vector3 initPos, Vector2 arrowForce, LOAD_ARROW_TYPE arrowType)
        {
            //CatLog.Log("Bow Special Effect Occured :: Multiple Shot");

            //float facingRotation = Mathf.Atan2(facingVec.y, facingVec.x) * Mathf.Rad2Deg;
            float startRotation = facingVec + arrowSpreadAngle / 2f;
            float angleIncrease = arrowSpreadAngle / ((float)numOfArrows - 1f);

            string poolTag = (arrowType == LOAD_ARROW_TYPE.ARROW_MAIN) ? AD_Data.POOLTAG_MAINARROW_LESS : AD_Data.POOLTAG_SUBARROW_LESS;

            for (int i = 0; i < numOfArrows; i++)
            {
                if (i == (numOfArrows * 0.5f) - 0.5f) continue;

                float tempRotation = startRotation - angleIncrease * i;

                #region OLD_CODE

                /*var newArrow = CatPoolManager.Instance.LoadEffectedArrow(adBow);
                //var arrowRBody = newArrow.GetComponent<Rigidbody2D>();
                //var arrowComponent = newArrow.GetComponent<AD_Arrow>();

                if (newArrow)
                {
                    //Arrow Set Position
                    newArrow.transform.SetParent(arrowParent, false);
                    newArrow.transform.localScale = initScale;
                    newArrow.transform.position = initPos;
                    newArrow.transform.rotation = Quaternion.Euler(0, 0, tempRotation - 90);

                    //Arrow Set Active & Shot Arrow
                    newArrow.SetActive(true);
                    newArrow.GetComponent<AD_Arrow_less>().ShotArrow(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad),
                                                                                 Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude);

                    //arrowRBody.isKinematic = false;
                    //arrowRBody.gravityScale = 0f;
                    //newArrow.GetComponent<AD_Arrow>().isLaunched = true;
                    //
                    //arrowRBody.AddForce(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude,
                    //                        ForceMode2D.Force);
                    //
                    //arrowComponent.arrowTrail.SetActive(true);
                    //arrowComponent.arrowTrail.GetComponent<TrailRenderer>().Clear();
                }*/

                #endregion

                var poolArrow = CCPooler.SpawnFromPool<AD_Arrow_less>(poolTag, arrowParent, initScale, initPos,
                                                       Quaternion.Euler(0f, 0f, tempRotation - 90f));

                if(poolArrow)
                {
                    poolArrow.ShotArrow(new Vector2(Mathf.Cos(tempRotation * Mathf.Deg2Rad), Mathf.Sin(tempRotation * Mathf.Deg2Rad)) * arrowForce.magnitude);
                }

            }
        }

        public override string ToString()
        {
            return "Spread_Shot";
        }
    }
}
