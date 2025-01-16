/* © 2020 TippyTap Corp.
 * You may use and modify the contents of this asset (Trajectory Aimer 2D) for personal and commercial use.
 * Distribution of any contents within this asset, whether modified or original, is not permitted.
 */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    Projectile script;
    bool editMode, editingInHierarchy;

    //Labels and Tooltips
    readonly GUIContent mainCamera_GUI = new GUIContent("Main Camera", "The Camera which is active in the scene."),
        rigidBody_GUI = new GUIContent("Rigidbody", "The Rigidbody 2D of the Projectile."),
        sensitivity_GUI = new GUIContent("Sensitivity", "This value determines the amount of velocity to be added to the Projectile while aiming.\n\nThe higher this value is, the less click-and-drag distance will be needed to produce an equally powerful shot."),
        maxShotSpeed_GUI = new GUIContent("Max Shot Speed", "The maximum limit to how much speed can be applied to a shot."),
        minShotSpeed_GUI = new GUIContent("Min Shot Speed", "The minimum limit to how much speed a shot is required to have to be applied.\n\nWhen aiming, if the aimed shot speed is below this amount, the Trajectory Line will hide.\nIf a shot is performed below this amount, the shot will be cancelled."),
        useTouchRadius_GUI = new GUIContent("Use Touch Radius", "When enabled, a click will need to be performed within the Touch Radius to start a shot.\n\nWhen disabled, a click can be located anywhere to start a shot."),
        touchRadius_GUI = new GUIContent("Touch Radius", "The radius of the circle around the Projectile which a click must be performed within to start a shot.\n\nIndicated by the yellow circle in the Scene and Game View (if Gizmos is enabled).\nIf a click is performed within multiple Projectiles' Touch Radius, the Projectile nearest to the click position will be selected."),
        freezeOnTouch_GUI = new GUIContent("Freeze On Touch", "When enabled, the Projectile will freeze in place when aiming.\n\nRigidbody 2D is set to Kinematic to achieve this.");


    //Called on selection
    private void OnEnable()
    {
        script = (Projectile)target;

        //Ignore selection in Play Mode
        if (Application.isPlaying)
            return;

        //Find Main Camera if not assigned
        if (script.mainCamera == null)
        {
            Camera mainCamera = Camera.main;
            script.mainCamera = mainCamera;

            //SetDirty to save changes to inspector values
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        //Begin checking if any variables changed
        EditorGUI.BeginChangeCheck();

        //Main Camera
        script.mainCamera = EditorGUILayout.ObjectField(mainCamera_GUI, script.mainCamera, typeof(Camera), true) as Camera;
        if (script.mainCamera == null)
            EditorGUILayout.HelpBox("The active Camera in the scene must be assigned.", MessageType.Warning);

        //Rigidbody 2D
        script.rigidBody = EditorGUILayout.ObjectField(rigidBody_GUI, script.rigidBody, typeof(Rigidbody2D), true) as Rigidbody2D;
        if (script.rigidBody == null)
            if (script.GetComponent<Rigidbody2D>() == null)
                EditorGUILayout.HelpBox("This GameObject is missing a Rigidbody 2D.", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("This GameObject's Rigidbody 2D must be assigned.", MessageType.Warning);


        //Sensitivity
        script.sensitivity = Mathf.Max(EditorGUILayout.FloatField(sensitivity_GUI, script.sensitivity), 0.01f);

        //Max Shot Speed
        script.maxShotSpeed = Mathf.Max(EditorGUILayout.FloatField(maxShotSpeed_GUI, script.maxShotSpeed), script.minShotSpeed);

        //Min Shot Speed
        script.minShotSpeed = Mathf.Clamp(EditorGUILayout.FloatField(minShotSpeed_GUI, script.minShotSpeed), 0, script.maxShotSpeed);

        //Use Touch Radius
        script.useTouchRadius = EditorGUILayout.Toggle(useTouchRadius_GUI, script.useTouchRadius);

        //Touch Radius
        //If Use Touch Radius enabled, show Touch Radius variable
        if (script.useTouchRadius)
        {
            EditorGUI.indentLevel++;
            script.touchRadius = Mathf.Max(EditorGUILayout.FloatField(touchRadius_GUI, script.touchRadius), 0f);
            EditorGUI.indentLevel--;
        }

        //Freeze On Touch
        script.freezeOnTouch = EditorGUILayout.Toggle(freezeOnTouch_GUI, script.freezeOnTouch);


        //If changes were detected
        if (EditorGUI.EndChangeCheck())
        {
            editMode = !Application.isPlaying;
            editingInHierarchy = !PrefabUtility.GetPrefabInstanceStatus(script.gameObject).Equals(PrefabInstanceStatus.NotAPrefab);

            //Udate prefab values only if they're made in the Project window or Prefab Mode in Edit mode
            if (!editingInHierarchy && editMode)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);

            //SetDirty to save changes to inspector values
            EditorUtility.SetDirty(target);
        }
    }
}