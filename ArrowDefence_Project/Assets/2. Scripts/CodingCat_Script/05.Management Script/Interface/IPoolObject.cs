public interface IPoolObject
{   
    void DisableRequest(UnityEngine.GameObject target); //제거 대상

    void DisableRequest();
}

public interface IArrowObject
{
    void ShotToDirectly(UnityEngine.Vector2 force, ActionCat.DamageStruct damage);

    void ShotToDirectly(UnityEngine.Vector2 force);

    void ShotToTarget(UnityEngine.Vector3 target);

    void ForceToTarget(UnityEngine.Vector3 target);
}
