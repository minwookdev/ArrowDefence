namespace ActionCat {

    using UnityEngine;
    using UnityEngine.UI;

    public class BowSprite : MonoBehaviour {
        [SerializeField] Image ImageBattleBow;
        [SerializeField] Sprite SpriteBow;

        public Sprite GetUISprite() {
            if(SpriteBow != null) {
                return SpriteBow;
            }
            else {
                return null;
            }
        }
    }
}
