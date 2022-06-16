namespace ActionCat.Interface
{
    public interface IToString {
        string ToString();
    }

    public interface IPoolObject {
        void DisableRequest();
    }

    public interface IArrowObject {
        void ShotByBow(UnityEngine.Vector2 direction, UnityEngine.Transform parent, ActionCat.DamageStruct damage);

        void ShotToDirection(UnityEngine.Vector2 direction, ActionCat.DamageStruct damage);

        void ForceToDirectly();

        void ForceToTarget(UnityEngine.Vector3 targetPosition);

        string GetMainTag();

        void PlayEffect(UnityEngine.Vector3 position);

        void PlayOneShot(UnityEngine.AudioClip audioClip);

        void PlayDefaultClip();
    }

    public interface IDamageable {
        void OnHit(ref ActionCat.DamageStruct damage, UnityEngine.Vector3 contactPoint, UnityEngine.Vector3 direction);
        bool TryOnHit(ref DamageStruct damage, UnityEngine.Vector3 point, UnityEngine.Vector2 direction);
        void OnHitElemental(ref DamageStruct damage, UnityEngine.Vector3 point, UnityEngine.Vector2 direction);
        bool IsAlive();
    }

    interface IPoolUser {
        System.Collections.IEnumerator Start();
    }

    public interface IMainMenu {
        bool IsTweenPlaying();
        void MenuOpen();
        void MenuClose();
    }

}
