/* © 2020 TippyTap Corp.
 * You may use and modify the contents of this asset (Trajectory Aimer 2D) for personal and commercial use.
 * Distribution of any contents within this asset, whether modified or original, is not permitted.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrajectoryAim))]
public class TrajectoryAimEditor : Editor
{
    TrajectoryAim script;
    bool editMode, editingInHierarchy;
    Vector2 editorVelocity;

    //Labels and Tooltips
    readonly GUIContent projectile_GUI = new GUIContent("Projectile", "The GameObject which will be getting launched.\n\nCan be accessed by script through 'TrajectoryAim.instance.Projectile'. \n\nIf 'Collisions' is True, only colliding layers in the Projectile's 'Layer Collision Matrix' will be scanned for collisions.\n\nChange the layers scanned for collisions through 'TrajectoryAim.instance.layerMask'.\n\n(Layer Collision Matrix can be found in Edit > Project Settings > Physics 2D)"),
        dotsParent_GUI = new GUIContent("Dots Parent", "The Transform of the GameObject which contains all the dots as its children.\n\nIt should only contain dots as its children and should always have at least 1 child."),
        straightLineMode_GUI = new GUIContent("Straight Line Mode", "Disables gravity calculations. Dots will be arranged into a straight line based on the starting velocity of a shot."),
        dots_GUI = new GUIContent("Dots", "The number of dots in the trajectory line.\n\nCan be accessed by script through 'TrajectoryAim.instance.Dots'.\n\nIn Edit Mode, dots will automatically be added/deleted when changing this value.\nIn Play Mode, dots will be set inactive/active and instantiated only if not enough dots already exist under the 'Dots Parent' GameObject (dots won't be deleted in Play Mode by decreasing this value)."),
        dotSpread_GUI = new GUIContent("Dot Spread", "The amount of space between dots."),
        dotSize_GUI = new GUIContent("Dot Size", "The scale of the dots.\n\nCan be accessed by script through 'TrajectoryAim.instance.DotSize'.\nAll dots' scales will change to this value when it is changed."),
        gradient_GUI = new GUIContent("Gradient", "The gradient of the dots.\n\nSetting the gradient's Alpha and Color to be the same across the entire gradient (making it a single color) will skip the cost of updating the gradient across all the dots."),
        collisions_GUI = new GUIContent("Collisions", "Enables scanning the trajectory path for collisions.\n\nOnly colliding layers in the Projectile's 'Layer Collision Matrix' will be scanned for collisions. \n\nChange the layers scanned for collisions through 'TrajectoryAim.instance.layerMask'.\n\n(Layer Collision Matrix can be found in Edit > Project Settings > Physics 2D)"),
        collisionIndicator_GUI = new GUIContent("Collision Indicator", "The Transform of the GameObject that will be placed in the position of a detected collision."),
        projectileScale_GUI = new GUIContent("Projectile Scale", "The scale of the projectile being launched.\n\nIndicated as a green circle in the Scene and Game view (if Gizmos is on).\n\nIf the collider of the projectile is a Circle Collider, this value should be equal to the scale of the collider. Otherwise, it is recommended that it should be as large as possible WHILE still being completely inside of the projectile's collider.\n\nThis value is essentially the thickness of the trajectory line and if any part of the line overlaps with a collidable object, a collision will be detected."),
        collisionAccuracy_GUI = new GUIContent("Collision Accuracy", "The number of scans used to accurately find the position of an already detected collision.\n\nSet this value low for better efficiency.\n1 = No additional scans."),
        editModeVelocity_GUI = new GUIContent("Edit Mode Example Velocity", "The velocity used to draw the example Trajectory Line in Edit Mode.");

    //Called on selection
    private void OnEnable()
    {
        script = (TrajectoryAim)target;
        editMode = script.editMode;
        editingInHierarchy = !PrefabUtility.GetPrefabInstanceStatus(script.gameObject).Equals(PrefabInstanceStatus.NotAPrefab);

        /*
        OBSOLETE
        editingInHierarchy = (PrefabUtility.GetPrefabType(script.gameObject) == PrefabType.PrefabInstance);
        updated code is above.      
        */

        //Ignore selection in Project window and Prefab Mode
        //Ignore selection in Play Mode
        if (!editingInHierarchy || !editMode)
            return;

        if (script.Projectile == null)
        {
            Projectile projectile = FindObjectOfType<Projectile>();
            if (projectile != null)
            {
                script.Projectile = projectile.gameObject;

                //SetDirty to save inspector value changes
                EditorUtility.SetDirty(target);
            }
        }


        //If Projectile isn't missing
        if (script.Projectile != null)
        {
            UpdateDotAmount();
            //Apply the Rigidbody 2D's gravity scale
            script.rbGravityScale = script.Projectile.GetComponent<Rigidbody2D>().gravityScale;
            //Show an example trajectory
            ApplyChanges();
        }
    }


    public override void OnInspectorGUI()
    {
        editMode = !Application.isPlaying;
        editingInHierarchy = !PrefabUtility.GetPrefabInstanceStatus(script.gameObject).Equals(PrefabInstanceStatus.NotAPrefab);


        //Begin checking if any variables changed
        EditorGUI.BeginChangeCheck();


        //Projectile GameObject
        EditorGUI.BeginChangeCheck();
        script.Projectile = EditorGUILayout.ObjectField(projectile_GUI, script.Projectile, typeof(GameObject), true) as GameObject;
        if (EditorGUI.EndChangeCheck())
            UpdateDotAmount();

        //Display warning if Projectile GameObject is missing
        if (script.Projectile == null)
            EditorGUILayout.HelpBox("A Projectile GameObject needs to be assigned.", MessageType.Warning);


        //Dots Parent Transform
        script.dotsParent = EditorGUILayout.ObjectField(dotsParent_GUI, script.dotsParent, typeof(Transform), true) as Transform;
        if (script.dotsParent == null)
            //Display warning if Dots Parent Transform is missing
            EditorGUILayout.HelpBox("The 'Dots Parent' GameObject needs to be assigned.", MessageType.Warning);
        else if (script.dotsParent.childCount.Equals(0))
            //Display warning if Dots Parent doesn't have any children
            EditorGUILayout.HelpBox("The 'Dots Parent' GameObject doesn't have any children. There needs to be at least 1.", MessageType.Warning);


        //Straight Line Mode toggle
        script.straightLineMode = EditorGUILayout.Toggle(straightLineMode_GUI, script.straightLineMode);


        //Number of Dots
        EditorGUI.BeginChangeCheck();
        script.Dots = Mathf.Max(EditorGUILayout.IntField(dots_GUI, script.Dots), 1);
        //If changed, update number of dots
        if (EditorGUI.EndChangeCheck())
            UpdateDotAmount();


        //Dot Spread
        script.DotSpread = Mathf.Max(EditorGUILayout.FloatField(dotSpread_GUI, script.DotSpread), 0.001f);


        //Dot Size
        EditorGUI.BeginChangeCheck();
        script.DotSize = Mathf.Max(EditorGUILayout.FloatField(dotSize_GUI, script.DotSize), 0.001f);
        //If changed and in Edit Mode, update all dots' sizes.
        if (EditorGUI.EndChangeCheck() && editMode)
            script.UpdateDotsSize();


        //Dots' Gradient
        EditorGUI.BeginChangeCheck();
        script.Gradient = EditorGUILayout.GradientField(gradient_GUI, script.Gradient);
        //Only update in Play Mode, Edit Mode is handled automatically
        if (EditorGUI.EndChangeCheck() && !editMode)
            script.UpdateGradient();


        //Collisions
        script.Collisions = EditorGUILayout.Toggle(collisions_GUI, script.Collisions);


        //If Collisions enabled, show collision related variables
        if (script.Collisions)
        {
            EditorGUI.indentLevel++;

            //Collision Indicator Transform
            script.collisionIndicator = EditorGUILayout.ObjectField(collisionIndicator_GUI, script.collisionIndicator, typeof(Transform), true) as Transform;
            //Display warning if Collision Indicator Transform is missing
            if (script.collisionIndicator == null)
            {
                EditorGUILayout.HelpBox("A 'Collision Indicator' GameObject needs to be assigned so that it can be positioned where a collision is detected.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);
            }


            //Projectile Scale
            script.projectileScale = Mathf.Max(EditorGUILayout.FloatField(projectileScale_GUI, script.projectileScale), 0.001f);


            //Collision Accuracy slider
            if (!script.straightLineMode)
                //Only show if Straight Line Mode is enabled
                script.collisionAccuracy = EditorGUILayout.IntSlider(collisionAccuracy_GUI, script.collisionAccuracy, 1, 5);
            else
                //Collision Accuracy won't have any impact if Straight Line Mode is enabled
                //Hide and set Collision Accuracy to 1 for efficiency
                script.collisionAccuracy = 1;


            EditorGUI.indentLevel--;
        }


        if (editMode)
        {
            EditorGUILayout.Space();
            //Edit Mode Example Trajectory Velocity
            script.editModeVelocity = EditorGUILayout.Vector2Field(editModeVelocity_GUI, script.editModeVelocity);
        }


        //If changes were detected
        if (EditorGUI.EndChangeCheck())
        {
            //Update prefab values only if they're made in the Project window or Prefab Mode during Edit mode
            if (!editingInHierarchy && editMode)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);


            //Ignore updating changes when in the Project window or Prefab Mode
            //Ignore changes in Play Mode, TrajectoryAim script will handle any changes
            if (!editingInHierarchy || !editMode)
                return;


            //Continue if in Edit Mode
            //SetDirty to save changes to inspector values
            EditorUtility.SetDirty(target);

            ApplyChanges();
        }
    }


    //Applies any changes to the values in the inspector
    void ApplyChanges()
    {
        //Update the list containing all the dots under the Dot Parent Transform
        script.UpdateDotList();


        //If Collision Indicator isn't missing
        if (script.collisionIndicator != null)
            //Set it active/inactive if Collisions are enabled/disabled, respectively
            script.collisionIndicator.gameObject.SetActive(script.Collisions);

        //Update all dots' colors based on the Gradient
        script.UpdateGradient();

        //If Projectile isn't missing
        if (script.Projectile != null)
        {
            //Show an example trajectory
            script.VisualizeParabolaInEditMode(script.editModeVelocity, script.Projectile.transform.position);
        }
    }


    //Update the number of dots
    void UpdateDotAmount()
    {
        //Avoid instantiating dots when in the Project window or Prefab Mode
        //Ignore in Play Mode, TrajectoryAim script will handle any changes
        editingInHierarchy = !PrefabUtility.GetPrefabInstanceStatus(script.gameObject).Equals(PrefabInstanceStatus.NotAPrefab);

        if (!editingInHierarchy || !editMode)
            return;


        //Get number of dots in hierarchy
        int dotsInHierarchy = script.dotsParent.childCount;

        //Update the number of dots in the hierarchy
        //if the number of dots and the Dots integer aren't the same
        if (script.Dots != dotsInHierarchy)
        {
            Transform dotsParent = script.dotsParent;

            //Instantiate more dots if there isn't enough in the hierarchy
            if (script.Dots > dotsInHierarchy)
            {
                //Duplicate the first dot under the parent Dot Parent
                GameObject dot = script.dotsParent.GetChild(0).gameObject;

                while (script.Dots > dotsInHierarchy)
                {
                    Instantiate(dot, dotsParent);
                    dotsInHierarchy++;
                }
            }
            else
            {
                //Delete dots if there's too many in the hierarchy
                List<Transform> dotChildren = new List<Transform>(dotsParent.GetComponentsInChildren<Transform>());
                dotChildren.Remove(dotsParent);

                while (script.Dots < dotsInHierarchy)
                {
                    dotsInHierarchy--;
                    DestroyImmediate(dotChildren[dotsInHierarchy].gameObject);
                    dotChildren.TrimExcess();
                }
            }
        }
    }
}