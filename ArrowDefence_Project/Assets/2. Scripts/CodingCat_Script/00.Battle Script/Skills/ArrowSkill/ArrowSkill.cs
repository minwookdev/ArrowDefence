namespace ActionCat
{
    using UnityEngine;

    public abstract class ArrowSkill
    {
        protected Transform arrowTr;
        protected Rigidbody2D rBody;

        public virtual void Init(Transform tr, Rigidbody2D rigid)
        {
            arrowTr = tr;
            rBody   = rigid;
        }

        public abstract void OnAir();
        public abstract void OnHit(GameObject target, IPoolObject arrow);
        public abstract void Clear();
    }

    public class ReboundArrow : ArrowSkill
    {
        GameObject alreadyHitTarget;
        int currentChainCount = 0;
        int maxChainCount     = 1;

        public override void OnAir()
        {
            CatLog.Log("");
        }

        public override void OnHit(GameObject target, IPoolObject arrow)
        {
            //if(ReferenceEquals(alreadyHitTarget, target) == false)
            //{
            //
            //}

            //if (alreadyHitTarget != target)
            //{
            //    //Active Skill
            //
            //
            //    //Save Target
            //    alreadyHitTarget = target;
            //}
            //else
            //{
            //    //Ignore Target
            //}
            if (currentChainCount >= maxChainCount) //return
            {
                arrow.DisableObject_Req(arrowTr.gameObject);
                return;
            }

            if(alreadyHitTarget != target)
            {
                currentChainCount++;
                alreadyHitTarget = target;
            }

            //arrow.DisableObject_Req(arrowTr.gameObject);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(arrowTr.position, 5f);
            if (hitColliders.Length == 0) //return
            {
                arrow.DisableObject_Req(arrowTr.gameObject);
                return;
            }
                

            Transform bestTargetTr = null;
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Vector2 directionToTarget = hitColliders[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if(distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    bestTargetTr   = hitColliders[i].transform;
                }
            }

            arrowTr.rotation = Quaternion.Euler(0f, 0f,
                               Quaternion.FromToRotation(Vector3.up, bestTargetTr.position - arrowTr.position).eulerAngles.z);
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }

    public class GuidanceArrow : ArrowSkill
    {
        public override void OnAir()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHit(GameObject target, IPoolObject arrow)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SplitArrow : ArrowSkill
    {
        public override void OnAir()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHit(GameObject target, IPoolObject arrow)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}
