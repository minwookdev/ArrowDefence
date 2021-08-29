public static class AD_Data
{
    /// <summary>
    /// Scene Tags
    /// </summary>
    public const string Scene_Title         = "ArrowDefence_Title";
    public const string Scene_Main          = "ArrowDefence_Main";
    public const string Scene_Loading       = "ArrowDefence_Loading";
    public const string Scene_Battle_Dev    = "ArrowDefence_Battle_00";
    public const string Scene_Battle_Forest = "ArrowDefence_Battle_01";
    public const string Scene_Battle_Desert = "ArrowDefence_Battle_02";

    /// <summary>
    /// Tags
    /// </summary>
    public const string Tag_Bow   = "ArrowDefence_Bow";
    public const string Tag_Arrow = "ArrowDefence_Arrow";
    public static readonly string Tag_BattleScene_MainCanvas = "BattleScene_MainCanvas";

    /// <summary>
    /// Object Pool Type String Data
    /// </summary>
    public const string Arrow   = "ARROW";
    public const string Effect  = "EFFECT";
    public const string Monster = "MONSTER";

    public static readonly string StageInfoDev     = "DEV MODE STAGE";
    public static readonly string StageInfoForest  = "FOREST STAGE";
    public static readonly string StageInfoDesert  = "DESERT STAGE";
    public static readonly string StageInfoDungeon = "DUNGEON STAGE";

    /// <summary>
    /// CCPooler Data 
    /// string Data is need to Spawn Object, 
    /// byte Data is need to Return to Pool Object.
    /// </summary>
    public static readonly byte ARROW_MAIN = 0;
    public static readonly byte ARROW_MAIN_LESS = 1;
    public static readonly string Arrow_Main_Tag      = "Main_Arrow";
    public static readonly string Arrow_Main_Less_Tag = "Main_Arrow_Less";

    //const (상수)형은 빌드 시 인게임 내에서 불러오는데 문제? 가 있을수 있다고 하니, 
    //static readonly로 바꿔주도록 한다

    public enum ArrowAttrubute
    {
        Arrow_Normal = 0,
        Arrow_Effect = 1,
        Arrow_Special = 2
    }
}
