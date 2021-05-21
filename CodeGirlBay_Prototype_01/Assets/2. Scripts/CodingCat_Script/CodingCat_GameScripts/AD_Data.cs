using JetBrains.Annotations;

public static class AD_Data
{
    /// <summary>
    /// Scene Tags
    /// </summary>
    public const string Scene_Title         = "ArrowDefence_Title";
    public const string Scene_Main          = "ArrowDefence_Main";
    public const string Scene_Loading       = "ArrowDefence_Loading";
    public const string Scene_Battle_Dev    = "ArrowDefence_Battle_01";
    public const string Scene_Battle_Forest = "ArrowDefence_Battle_Forest";
    public const string Scene_Battle_Desert = "ArrowDefence_Battle_Desert";

    /// <summary>
    /// Tags
    /// </summary>
    public const string Tag_Bow   = "ArrowDefence_Bow";
    public const string Tag_Arrow = "ArrowDefence_Arrow";

    /// <summary>
    /// Object Pool Type String Data
    /// </summary>
    public const string Arrow   = "ARROW";
    public const string Effect  = "EFFECT";
    public const string Monster = "MONSTER";

    public enum ArrowAttrubute
    {
        Arrow_Normal = 0,
        Arrow_Effect = 1,
        Arrow_Special = 2
    }
}
