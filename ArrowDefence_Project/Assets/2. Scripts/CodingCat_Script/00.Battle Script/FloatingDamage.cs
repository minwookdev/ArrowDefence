namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class FloatingDamage : MonoBehaviour, IPoolObject {
        [Header("COMPONENT")] //Caching This Properties.
        [SerializeField] Transform floatingTr     = null;
        [SerializeField] TextMeshProUGUI tmpCount = null;
        [SerializeField] Image imageCrit          = null;
        [SerializeField] CanvasGroup canvasGroup  = null;

        [Header("PROPERTY")]
        public float MoveDuration = 1f;
        public float MoveDistance = 1f;
        public float FadeDuration = 0.5f;

        [Header("OPTIONS")]
        [SerializeField] bool isScale = false;
        
        //private variables
        bool isCritical = false;

        public void OnFloating(string countstring, Vector2 direction) {
            StartCoroutine(FloatingProgress(countstring, direction, isReverse:false));
        }

        System.Collections.IEnumerator FloatingProgress(string countstring, Vector2 direction, bool isReverse) {
            //Phase 0. Init Values
            float progress = 0f;
            float speed = 1 / MoveDuration;
            Vector2 targetPosition = (Vector2)floatingTr.position + (direction.normalized * MoveDistance);
            ///is Reverse rotation
            ///Vector2 targetPos = (isCritical) ? (Vector2)tr.position - (direction.normalized * MoveDistance) : (Vector2)tr.position + (direction.normalized * MoveDistance);
            canvasGroup.alpha = 1f;
            tmpCount.text = countstring;

            //Phase 1. Move to Target Position.
            while(progress < 1) {
                progress += Time.deltaTime * speed;
                floatingTr.position = Vector3.Lerp(floatingTr.position, targetPosition, progress);
                yield return null;
            }
            //Check. Position
            if ((Vector2)floatingTr.position != targetPosition)
                floatingTr.position = targetPosition;

            //Phase 2. Disable alpha.
            progress = 0f;
            speed = 1 / FadeDuration;
            while (progress < 1) {
                progress += Time.deltaTime * speed;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
                yield return null;
            }
            //Check. Alpha
            if (canvasGroup.alpha != 0f)
                canvasGroup.alpha = 0f;
            tmpCount.text = "";

            //Disable GameObejct (this action is stop the coroutine)
            DisableRequest();
        }

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target);

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject);
    }
}
