public interface IPoolObject
{
    void DisableObject_Req(UnityEngine.GameObject target);
}

public interface IArrowObject
{
    void ShotArrow(UnityEngine.Vector2 force);

    void ShotArrow(UnityEngine.Quaternion rotation, UnityEngine.Vector2 force);
}
