public static class AD_Data
{
    /// <summary>
    /// Scene Tags
    /// </summary>
    public static readonly string Scene_Title         = "ArrowDefence_Title";
    public static readonly string Scene_Main          = "ArrowDefence_Main";
    public static readonly string Scene_Loading       = "ArrowDefence_Loading";
    public static readonly string Scene_Battle_Dev    = "ArrowDefence_Battle_00";
    public static readonly string Scene_Battle_Forest = "ArrowDefence_Battle_01";
    public static readonly string Scene_Battle_Desert = "ArrowDefence_Battle_02";

    public static readonly string StageInfoDev     = "DEV MODE STAGE";
    public static readonly string StageInfoForest  = "FOREST STAGE";
    public static readonly string StageInfoDesert  = "DESERT STAGE";
    public static readonly string StageInfoDungeon = "DUNGEON STAGE";

    /// <summary>
    /// CCPooler Tag Data Arrow's
    /// </summary>
    public static readonly string TAG_MAINARROW      = "MainArrow";
    public static readonly string TAG_MAINARROW_LESS = "MainArrow_Less";
    public static readonly string TAG_SUBARROW       = "SubArrow";
    public static readonly string TAG_SUBARROW_LESS  = "SubArrow_Less";

    /// <summary>
    /// CCPooler Tag Data with Monster's
    /// </summary>
    public static readonly string TAG_MONSTER_NORMAL = "Monster_Normal";

    /// <summary>
    /// Unity Layer Mask string Data
    /// </summary>
    public static readonly string LAYER_DISABLELINE  = "DisableCollider";
    public static readonly string LAYER_MONSTER      = "ObjectMonster";
    public static readonly string LAYER_ARROW        = "ObjectArrow";
    public static readonly string LAYER_BOW          = "ObjectBow";

    public static readonly string OBJECT_TAG_MONSTER = "Monster";
    public static readonly string OBJECT_TAG_BOW     = "ArrowDefence_Bow";

    //const (상수)형은 빌드 시 인게임 내에서 불러오는데 문제? 가 있을수 있다고 하니, 
    //static readonly로 바꿔주도록 한다
}
