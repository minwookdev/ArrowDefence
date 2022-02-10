namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [RequireComponent(typeof(ParticleSystem))]
    public class ACEffector2D : MonoBehaviour, IPoolObject {
        [Header("COMPONENT")]
        [SerializeField] Transform tr = null;
        [SerializeField] ParticleSystem particleSys = null;
        [SerializeField] ParticleSystemRenderer particleRenderer = null;

        [Header("EFFECT PROPERTY")]
        [SerializeField] [ReadOnly] 
        EFFECTORTYPE effectorType = EFFECTORTYPE.NONE;
        [SerializeField] string sortingLayerName = "";
        [SerializeField] int sortingOrder = 0;

        //COROUTINE
        Coroutine playerCo  = null;
        WaitUntil waitUntil = null;

        void Awake() {
            waitUntil = new WaitUntil(() => particleSys.isStopped == true);
        }

        void Start() {
            if (tr == null)               throw new System.Exception("Component Not Cached.");
            if (particleSys == null)      throw new System.Exception("Component Not Cached.");
            if (particleRenderer == null) throw new System.Exception("Component Not Cached.");
        }

        public void DisableRequest() {
            CCPooler.ReturnToPool(gameObject);
        }

        public void PlayOnce(float degree) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.NEWEFFECT;
            }

            Vector3 rotation = tr.eulerAngles;
            rotation.z = degree;
            tr.eulerAngles = rotation;

            if(playerCo != null) {
                StopCoroutine(playerCo);
            } playerCo = StartCoroutine(RunEffect());
        }

        public void PlayOnce(bool isRandomRotation = false) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.NEWEFFECT;
            }

            if(isRandomRotation == true) {
                Vector3 eulerAngles = tr.eulerAngles;
                eulerAngles.z = GameGlobal.RandomAngleDeg();
                tr.eulerAngles = eulerAngles;
            }

            if(playerCo != null) {
                StopCoroutine(playerCo);
            } playerCo = StartCoroutine(RunEffect());
        }

        public void Play(bool isStartRandRotation = false) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.RESTARTER;
            }

            if(isStartRandRotation == true) {
                Vector3 eulerAngles = tr.eulerAngles;
                eulerAngles.z = GameGlobal.RandomAngleDeg();
                tr.eulerAngles = eulerAngles;
            }

            particleSys.Play();
        }

        IEnumerator RunEffect() {
            particleSys.Play();
            yield return waitUntil;
            DisableRequest();
        }

        public bool IsPlaying() {
            return particleSys.isPlaying;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ACEffector2D))]
        class ACEffector2DEditor : Editor {
            ACEffector2D effector;
            private void OnEnable() {
                effector = target as ACEffector2D;
            }

            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                GUILayout.Space(10f);
                if(GUILayout.Button("Apply Effect Property")) {
                    if(effector == null) {
                        CatLog.ELog("ACEffector2D is Not Assignment.");
                        return;
                    }

                    if(effector.TryGetComponent<ParticleSystem>(out ParticleSystem particle)) {
                        var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
                        particleRenderer.alignment = ParticleSystemRenderSpace.Local;
                        particleRenderer.sortingLayerName = effector.sortingLayerName;
                        particleRenderer.sortingOrder     = effector.sortingOrder;

                        var particleMain = particle.main;
                        particleMain.playOnAwake = false;
                        particleMain.scalingMode = ParticleSystemScalingMode.Hierarchy;

                        effector.transform.eulerAngles = Vector3.zero;
                        //effector.transform.rotation = Quaternion.Euler(Vector3.zero);

                        EditorUtility.SetDirty(effector);
                        CatLog.Log(StringColor.GREEN, "The Proeprties Changed Successfully.");
                    }
                    else {
                        CatLog.ELog("ParticleSystem is Not Assignment.");
                    }
                }
                if(GUILayout.Button("Only Change Layers")) {
                    if(effector = null) {
                        CatLog.ELog("ACEffector2D is Not Assignment.");
                        return;
                    }

                    if(effector.TryGetComponent<ParticleSystem>(out ParticleSystem particle)) {
                        var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
                        particleRenderer.sortingLayerName = effector.sortingLayerName;
                        particleRenderer.sortingOrder     = effector.sortingOrder;

                        EditorUtility.SetDirty(effector);
                        CatLog.Log(StringColor.GREEN, "The Proeprties Changed Successfully.");
                    }
                    else {
                        CatLog.ELog("ParticleSystem is Not Assignment.");
                    }
                }
                if(GUILayout.Button("Set to Default Property")) {
                    if(effector == null) {
                        CatLog.ELog("ACEffector2D is Not Assignment.");
                        return;
                    }

                    if(effector.TryGetComponent<ParticleSystem>(out ParticleSystem particle)) {
                        var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
                        particleRenderer.alignment = ParticleSystemRenderSpace.View;
                        particleRenderer.sortingLayerName = "Default";
                        particleRenderer.sortingOrder     = 0;

                        var particleMain = particle.main;
                        particleMain.playOnAwake = true;
                        particleMain.scalingMode = ParticleSystemScalingMode.Shape;

                        var eulerAngles = effector.transform.eulerAngles;
                        eulerAngles.x   = -90f;
                        effector.transform.eulerAngles = eulerAngles;


                        EditorUtility.SetDirty(effector);
                        CatLog.Log(StringColor.GREEN, "The Proeprties Changed Successfully.");
                    }
                    else {
                        CatLog.ELog("ParticleSystem is Not Assignment.");
                    }
                }
            }
        }
#endif
    }
}
