using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple =false, Inherited = true)]
public sealed class LabelExAttribute : PropertyAttribute {
    public readonly string label = "";

    public LabelExAttribute(string label = "") {
        this.label = label;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LabelExAttribute))]
internal sealed class LabelExDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var labelAttribute = (LabelExAttribute)base.attribute;
        label.text = labelAttribute.label;
        EditorGUI.PropertyField(position, property, label, true);
    }
}

#endif
