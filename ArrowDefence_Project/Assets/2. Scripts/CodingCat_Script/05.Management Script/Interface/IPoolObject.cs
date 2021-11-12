public interface IPoolObject
{
    void DisableRequest(UnityEngine.GameObject target);
}

public interface IArrowObject
{
    void ShotArrow(UnityEngine.Vector2 force);

    void ShotArrow(UnityEngine.Vector3 target);

    void ForceArrow(UnityEngine.Vector3 target);
}
