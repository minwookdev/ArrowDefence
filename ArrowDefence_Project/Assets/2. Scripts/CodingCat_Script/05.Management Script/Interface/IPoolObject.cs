public interface IPoolObject
{
    void DisableObject_Req(UnityEngine.GameObject target);
}

public interface IArrowObject
{
    void ShotArrow(UnityEngine.Vector2 force);

    void ShotArrow(UnityEngine.Vector3 target, UnityEngine.Vector2 force);
}
