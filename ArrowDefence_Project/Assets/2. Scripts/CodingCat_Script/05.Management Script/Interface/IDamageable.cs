interface IDamageable
{
    void OnHitObject(float value);

    void OnHitObject(ref ActionCat.DamageStruct damage);

    void OnHitWithDirection(ref ActionCat.DamageStruct damage, UnityEngine.Vector3 contactPoint, UnityEngine.Vector3 direction);

    void OnHitWithQuaternion(ref ActionCat.DamageStruct damage, UnityEngine.Quaternion quaternion);

    void OnHitWithAngle(ref ActionCat.DamageStruct damage, float angle);
}

interface IActiveSkill
{
    void OnSkillActive();
}

interface IPassiveSkill
{

}


