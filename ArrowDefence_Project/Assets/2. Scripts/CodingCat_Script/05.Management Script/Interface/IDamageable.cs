interface IDamageable
{
    void OnHitObject(float value);

    void OnHitObject(ref ActionCat.DamageStruct damage);
}

interface IActiveSkill
{
    void OnSkillActive();
}

interface IPassiveSkill
{

}


