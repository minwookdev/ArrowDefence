interface IDamageable
{
    void OnHitObject(float value);

    void OnHitObject(ref ActionCat.DamageStruct damage);

    void OnHitWithDirection(ref ActionCat.DamageStruct damage, UnityEngine.Vector3 angles);

    void OnHitWithAngle(ref ActionCat.DamageStruct damage, float angle);
}

interface IActiveSkill
{
    void OnSkillActive();
}

interface IPassiveSkill
{

}


