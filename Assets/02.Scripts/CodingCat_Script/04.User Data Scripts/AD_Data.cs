/// <summary>
/// Editor-Related String Date class, used during runtime
/// </summary>
public static class AD_Data {
    /// <summary>
    /// Scene Name String Data
    /// </summary>
    //SCENE STRING
    public static readonly string SCENE_TITLE         = "ArrDef_Title";
    public static readonly string SCENE_MAIN          = "ArrDef_Lobby";
    public static readonly string SCENE_LOADING       = "ArrDef_Loading";

    //BATTLE SCENE STRING 
    public static readonly string SCENE_BATTLE_FOREST_EASY    = "Forest_Secluded_Easy";
    public static readonly string SCENE_BATTLE_FOREST_NORMAL  = "Forest_Secluded_Normal";
    public static readonly string SCENE_BATTLE_FOREST_HARD    = "Forest_Secluded_Hard";
    public static readonly string SCENE_BATTLE_DUNGEON_EASY   = "Dungeon_Entrance_Easy";
    public static readonly string SCENE_BATTLE_DUNGEON_NORMAL = "Dungeon_Entrance_Normal";
    public static readonly string SCENE_BATTLE_DUNGEON_HARD   = "Dungeon_Entrance_Hard";
    public static readonly string SCENE_BATTLE_DEV            = "_dev_stage";

    /// <summary>
    /// Stage Info Panel Battle Stage Name String Data
    /// </summary>
    public static readonly string STAGEINFO_DEV     = "DEV MODE STAGE";
    public static readonly string STAGEINFO_FOREST  = "FOREST STAGE";
    public static readonly string STAGEINFO_DESERT  = "DESERT STAGE";
    public static readonly string STAGEINFO_DUNGEON = "DUNGEON STAGE";

    /// <summary>
    /// CCPooler Tag String Data Player's Objects
    /// </summary>
    public static readonly string POOLTAG_MAINARROW       = "MainArrow";
    public static readonly string POOLTAG_MAINARROW_LESS  = "MainArrow_Less";
    public static readonly string POOLTAG_SUBARROW        = "SubArrow";
    public static readonly string POOLTAG_SUBARROW_LESS   = "SubArrow_Less";
    public static readonly string POOLTAG_SPECIAL_ARROW   = "Pref_SpecialArrow";
    public static readonly string POOLTAG_FLOATING_DAMAGE = "FloatingDamage";

    /// <summary>
    /// CCPooler Tag String Data with Monster's
    /// </summary>
    public static readonly string POOLTAG_MONSTER_NORMAL = "Monster_Normal";
    public static readonly string POOLTAG_MONSTER_FREQ   = "Monster_Freq";
    public static readonly string POOLTAG_MONSTER_ELITE  = "Monster_Elite";

    /// <summary>
    /// [MainArrowPoolTag] + [HitEffectPoolTag] + [Counts]
    /// </summary>
    public static readonly string POOLTAG_HITEFFECT = "_HitEffect_";
    public static readonly string POOLTAG_HITSKILL_EFFECT = "_HitSkillEff_";

    /// <summary>
    /// [ArrowPoolTag] + [ProjectilePoolTag] + [Counts]
    /// </summary>
    public static readonly string POOLTAG_PROJECTILE = "_projectile_";

    /// <summary>
    /// Unity Layer Mask string Data
    /// </summary>
    public static readonly string LAYER_DISABLELINE  = "DisableCollider";
    public static readonly string LAYER_MONSTER      = "ObjectMonster";
    public static readonly string LAYER_ARROW        = "ObjectArrow";
    public static readonly string LAYER_ARROW_LESS   = "ObjectArrowLess";
    public static readonly string LAYER_BOW          = "ObjectBow";

    /// <summary>
    /// Editor GameObject Tag String Data
    /// </summary>
    public static readonly string OBJECT_TAG_MONSTER = "Monster";
    public static readonly string OBJECT_TAG_BOW     = "ArrowDefence_Bow";

    /// <summary>
    /// Stage Info Dictionary Key
    /// </summary>
    public static readonly string STAGE_KEY_FOREST_SECLUDED  = "SECLUDED_FOREST";
    public static readonly string STAGE_KEY_DUNGEON_ENTRANCE = "ENTRANCE_DUNGEON";
    public static readonly string STAGE_KEY_FOREST_SECLUDED_EASY   = "forest_secluded_e";
    public static readonly string STAGE_KEY_FOREST_SECLUDED_NORMAL = "forest_secluded_n";
    public static readonly string STAGE_KEY_FOREST_SECLUDED_HARD   = "forest_secluded_h";
    public static readonly string STAGE_KEY_DUNGEON_ENTRANE_EASY   = "dungeon_entrance_e";
    public static readonly string STAGE_KEY_DUNGEON_ENTRANE_NORMAL = "dungeon_entrance_n";
    public static readonly string STAGE_KEY_DUNGEON_ENTRANE_HARD   = "dungeon_entrance_h";
    public static readonly string STAGE_KEY_DEV                    = "dev";
}
