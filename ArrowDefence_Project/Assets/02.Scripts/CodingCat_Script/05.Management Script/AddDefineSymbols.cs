using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
/// <summary>
/// Adds the given define symbols to PlayerSettings define symbols.
/// Just add your own define symbols to the Symbols property at the below.
/// </summary>
[InitializeOnLoad]
public class AddDefineSymbols : Editor
{
    private static readonly string Mobile_IOS      = "Mobile_Apple";
    private static readonly string Mobile_AOS      = "Mobile_Android";
    private static readonly string PC_WinOS        = "PC_Windows";
    private static readonly string PC_MacOS        = "PC_Mac";
    private static readonly string Enable_Log      = "ENABLE_LOG";
    /// <summary>
    /// Symbols that will be added to the editor
    /// </summary>
    public static readonly string[] Symbols = new string[] {
Mobile_AOS,
Mobile_IOS,
PC_WinOS,
PC_MacOS,
Enable_Log
};

    /// <summary>
    /// Add define symbols as soon as Unity gets done compiling.
    /// </summary>
    static AddDefineSymbols()
    {
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = definesString.Split(';').ToList();
        allDefines.AddRange(Symbols.Except(allDefines));

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
        EditorUserBuildSettings.selectedBuildTargetGroup,
        string.Join(";", allDefines.ToArray()));

    }
}
#endif