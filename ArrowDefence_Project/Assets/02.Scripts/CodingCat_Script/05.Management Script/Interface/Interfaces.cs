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

        string GetEffectKey();

        void PlayEffect(UnityEngine.Vector3 position);
    }

    public interface IDamageable {
        void OnHitObject(float value);

        void OnHitObject(ref ActionCat.DamageStruct damage);

        void OnHitWithDirection(ref ActionCat.DamageStruct damage, UnityEngine.Vector3 contactPoint, UnityEngine.Vector3 direction);

        void OnHitWithQuaternion(ref ActionCat.DamageStruct damage, UnityEngine.Quaternion quaternion);

        void OnHitWithAngle(ref ActionCat.DamageStruct damage, float angle);

        bool OnHitWithResult(ref DamageStruct damage, UnityEngine.Vector3 point, UnityEngine.Vector2 direction);

        bool IsAlive();
    }

    interface IPoolUser {
        System.Collections.IEnumerator Start();
    }

}
