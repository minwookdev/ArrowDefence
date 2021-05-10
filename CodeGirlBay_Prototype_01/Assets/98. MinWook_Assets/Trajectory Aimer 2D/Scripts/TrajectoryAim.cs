/* © 2020 TippyTap Corp.
 * You may use and modify the contents of this asset (Trajectory Aimer 2D) for personal and commercial use.
 * Distribution of any contents within this asset, whether modified or original, is not permitted.
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrajectoryAim : MonoBehaviour
{
    public static TrajectoryAim instance;

    [SerializeField]
    private GameObject projectile;

    public bool straightLineMode;

    [SerializeField]
    private int dots = 8;
    int last_dots;

    [SerializeField]
    private float dotSpread = 1;

    public Transform dotsParent;

    [SerializeField]
    private Gradient gradient;
    bool singleColorGradient;

    [SerializeField]
    bool checkForCollisions;

    public Transform collisionIndicator;

    int dotsVisible;

    public float projectileScale = 1;

    [Range(1, 5)]
    public int collisionAccuracy = 1;


    [HideInInspector]
    public LayerMask layerMask;

    [HideInInspector]
    public bool editMode = true;

    [SerializeField]
    float dotSize = 0.5f;

    [HideInInspector]
    public List<Dot> dotList = new List<Dot>();

    [Space]
    public Vector2 editModeVelocity = new Vector2(25, 40);

    [HideInInspector]
    public bool autoUpdateLayerMask = true;

    Rigidbody2D projectileRB;
    [HideInInspector]
    public float rbGravityScale = 1;

    public class Dot
    {
        public GameObject gameObject;
        public Transform transform;
        public SpriteRenderer spriteRenderer;

        public Dot(GameObject dotGameObject)
        {
            gameObject = dotGameObject;
            transform = gameObject.transform;
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
    }

    void Awake()
    {
        instance = this;
        editMode = false;
    }

    private void Start()
    {
        //Initialization
        DotsVisible = dots;
        last_dots = dots;

        UpdateProjectile();
        UpdateDotList();
        UpdateDotAmount();
        UpdateGradient();

        //Disable Collisions if the Collision Indicator is missing
        if (collisionIndicator == null)
            checkForCollisions = false;
        else
            HideCollisionIndicator();

        //Initialize listeners
        //Changes in these variables will automatically update the trajectory line accordingly
        OnDotsChange += DotsChangeHandler;
        OnProjectileChange += ProjectileChangeHandler;
        OnDotsVisibleChange += DotsVisibleChangeHandler;
        OnCollisionsChange += CollisionsChangeHandler;
        OnDotSizeChange += DotSizeChangeHandler;
        OnGradientChange += GradientChangeHandler;

        Hide();
    }

    //Returns a position along a trajectory path based on time
    public Vector2 GetParabolaPosition(Vector2 initialPosition, Vector2 direction, float time)
    {
        float gravity = Physics2D.gravity.y * rbGravityScale;

        float xPos = initialPosition.x + (direction.x * Time.fixedDeltaTime * (dotSpread * time));
        float yPos = initialPosition.y + (direction.y * Time.fixedDeltaTime * (dotSpread * time));

        //Gravity is accounted for if Straight Line Mode is off
        if (!straightLineMode)
            yPos += gravity / 2f * Mathf.Pow(Time.fixedDeltaTime, 2) * Mathf.Pow(dotSpread * time, 2);

        return new Vector2(xPos, yPos);
    }

    //Updates trajectory line
    public void UpdateAim(Vector2 shotVelocity, Vector2 projectilePosition)
    {
        //Update Projectile Rigidbody's gravity scale
        rbGravityScale = projectileRB.gravityScale;
        Vector2 lastDotPos = projectilePosition, currentDotPos = projectilePosition;

        for (int i = 1; i < dots + 1; i++)
        {
            currentDotPos = GetParabolaPosition(projectilePosition, shotVelocity, i);

            if (checkForCollisions)
            {
                //If a collision is detected, the remaining dots are are hidden
                if (CollisionCheck(i, lastDotPos, currentDotPos, projectilePosition, shotVelocity))
                {
                    DotsVisible = i - 1;
                    return;
                }
            }

            //Set dot position
            dotList[i - 1].transform.position = currentDotPos;
            lastDotPos = currentDotPos;
        }

        //If a collision wasn't detected, show all dots
        if (checkForCollisions)
            DotsVisible = Dots;
    }

    //Checks for a collision between consequent points on a trajectory path
    private bool CollisionCheck(int dotNumber, Vector2 lastDotPos, Vector2 currentDotPos, Vector2 projectilePosition, Vector2 shotDirection)
    {
        //Cast a Circle Cast between the current 2 neighboring points on the trajectory
        Vector2 currentDirection = currentDotPos - lastDotPos;
        RaycastHit2D circleCast;
        Debug.DrawLine(lastDotPos, currentDotPos, Color.yellow);

        //First cast, level 1 accuracy
        if (circleCast = Physics2D.CircleCast(lastDotPos, projectileScale / 2f, currentDirection, currentDirection.magnitude, layerMask))
        {
            //Collision detected
            Vector2 collisionPos = circleCast.centroid;

            //Check if collided with self.
            //Set the Projectile GameObject to its own unique layer and disable collisions with
            //the same layer in the Collision Layer Matrix to skip this step for efficiency
            if (circleCast.collider.gameObject.Equals(Projectile))
            {
                //Collided with self
                RaycastHit2D[] hitResults = new RaycastHit2D[2];
                hitResults = Physics2D.CircleCastAll(lastDotPos, projectileScale / 2f, currentDirection, currentDirection.magnitude, layerMask);

                //See if cast has detected a collision with anything else
                if (hitResults.Length > 1)
                {
                    //Collision detected. Cast collided with another collider too
                    circleCast = hitResults[1];
                    collisionPos = circleCast.centroid;
                }
                else
                    //False alarm, no collision detected. Only collided with self
                    return false;
            }


            //Find collision position accurately
            //collisionAccuracy = 1 means skipping this
            if (collisionAccuracy > 1)
            {
                RaycastHit2D accurateCircleCast;
                float lastScanTime = dotNumber - 1, currentScanTime = dotNumber, halfwayScanTime = dotNumber - 0.5f;
                Vector2 lastScanPos = lastDotPos, currentScanPos = currentDotPos, halfwayScanPos;

                //Find which half of the Circle Cast the collision occured
                //Repeat this as many times as collisionAccuracy is set to minus 1
                //Each loop finds which half of the last loop the collision occured
                for (int i = 1; i < collisionAccuracy; i++)
                {
                    halfwayScanTime = (currentScanTime + lastScanTime) / 2f;
                    halfwayScanPos = GetParabolaPosition(projectilePosition, shotDirection, halfwayScanTime);
                    Vector2 scanDirection = halfwayScanPos - lastScanPos;

                    //L = lastScanPos
                    //C = currentScanPos
                    //H = halfwayScanPos
                    //Identify if collision is in first or second
                    //Circle Cast between lastScanPos to halwaysScanPos
                    if (accurateCircleCast = Physics2D.CircleCast(lastScanPos, projectileScale / 2f, scanDirection, scanDirection.magnitude, layerMask))
                    {
                        //Collision detected
                        //Collision is in FIRST half
                        // L----H----C
                        // L----C
                        currentScanTime = halfwayScanTime;
                        currentScanPos = GetParabolaPosition(projectilePosition, shotDirection, currentScanTime);
                        collisionPos = accurateCircleCast.centroid;
                    }
                    else
                    {
                        //Collision wasn't detected
                        //Collision is in SECOND half
                        // L----H----C
                        //      L----C
                        lastScanTime = halfwayScanTime;
                        lastScanPos = GetParabolaPosition(projectilePosition, shotDirection, lastScanTime);


                        //Find the accurate collision position if it's the last loop
                        if (i == collisionAccuracy)
                        {
                            currentScanPos = GetParabolaPosition(projectilePosition, shotDirection, currentScanTime);
                            scanDirection = currentScanPos - halfwayScanPos;

                            //Circle Cast for accurate collision position
                            if (accurateCircleCast = Physics2D.CircleCast(lastScanPos, projectileScale / 2f, scanDirection, scanDirection.magnitude, layerMask))
                                collisionPos = accurateCircleCast.centroid;
                            else
                                //Accurate cast didn't hit anything
                                //False alarm. No collision after all
                                return false;
                        }
                    }
                }
            }

            //COLLISION CONFIRMED
            collisionIndicator.position = collisionPos;
            return true;
        }

        //Original Circle Cast hit nothing
        return false;
    }

    //Make the trajectory line active in hierarchy
    public void Show()
    {
        gameObject.SetActive(true);
    }

    //Make the trajectory line inactive in hierarchy
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //Hides all the dots beyond the last visible one
    void HideDotsBeyond(int lastVisibleDot)
    {
        int totalDots = dotList.Count;
        lastVisibleDot = Mathf.Clamp(lastVisibleDot, 0, totalDots);

        for (int i = lastVisibleDot; i < totalDots; i++)
        {
            if (dotList[i].gameObject.activeSelf)
                dotList[i].gameObject.SetActive(false);
            else
                //If reached already inactive dots, no need to continue
                return;
        }
    }

    //Sets Dots active in hierarchy up until the lastDot
    void ShowDotsUpTo(int lastDot)
    {
        lastDot = Mathf.Clamp(lastDot, 0, Dots);

        for (int i = 0; i < lastDot; i++)
        {
            if (!dotList[i].gameObject.activeSelf)
                dotList[i].gameObject.SetActive(true);
        }
    }

    //Set all Dots active
    void ShowAllDots()
    {
        ShowDotsUpTo(Dots);
    }

    //Hides the Collision Indicator GameObject
    void HideCollisionIndicator()
    {
        if (collisionIndicator.gameObject.activeSelf)
            collisionIndicator.gameObject.SetActive(false);
    }

    //Sets the Collision Indicator GameObject active
    void ShowCollisionIndicator()
    {
        if (!collisionIndicator.gameObject.activeSelf)
            collisionIndicator.gameObject.SetActive(true);
    }

    //Updates the color of all visible dots
    public void UpdateGradient()
    {
        //Check if the Gradient is a single color to make process more efficient
        //Gradient will be a single color of all alphaKeys and colorKeys are equal
        singleColorGradient = true;

        //Check if all alphaKeys are equal
        for (int i = 1; i < gradient.alphaKeys.Length; i++)
        {
            if (!gradient.alphaKeys[0].alpha.Equals(gradient.alphaKeys[i].alpha))
            {
                singleColorGradient = false;
                break;
            }
        }

        //If alphaKeys were equal, continue check to see if Gradient is single color
        if (singleColorGradient)
        {
            //Check if all colorKeys are equal
            for (int i = 1; i < gradient.colorKeys.Length; i++)
            {
                if (!gradient.colorKeys[0].color.Equals(gradient.colorKeys[i].color))
                {
                    singleColorGradient = false;
                    break;
                }
            }
        }


        if (singleColorGradient)
            //All keys are equal. Assign 1 color to all dots
            for (int i = 0; i < dots; i++)
                dotList[i].spriteRenderer.color = gradient.Evaluate(0);
        else
            //Keys aren't equal, find each Dot's color
            for (int i = 0; i < dots; i++)
            {
                float time = (float)i / dots;
                dotList[i].spriteRenderer.color = gradient.Evaluate(time);
            }

    }

    //Arranges dots into a trajectory line in Edit Mode
    public void VisualizeParabolaInEditMode(Vector2 shotVelocity, Vector2 projectilePosition)
    {
        //Need a reference to the dotsParent to find all dots
        if (dotsParent == null)
            return;

        //Find the dots
        UpdateDotList();
        Vector2 currentDotPos = projectilePosition;

        //Put each Dot into position
        for (int i = 1; i < dots + 1; i++)
        {
            currentDotPos = GetParabolaPosition(projectilePosition, shotVelocity, i);
            dotList[i - 1].transform.position = currentDotPos;
        }

        //Place the collisionIndicator at the end of the trajectory if appropriate
        if (checkForCollisions && collisionIndicator != null)
            collisionIndicator.position = GetParabolaPosition(projectilePosition, shotVelocity, dots + 1);
    }

    //Number of dots
    //When changed, the number of dots in the trajectory line will be updated
    public int Dots
    {
        get { return dots; }
        set
        {
            if (dots.Equals(value)) return;
            int newDotsValue = Mathf.Clamp(value, 1, 1000);

            dots = newDotsValue;
            if (OnDotsChange != null)
                OnDotsChange(dots);
        }
    }
    public delegate void OnDotsChangeDelegate(int newVal);
    public event OnDotsChangeDelegate OnDotsChange;
    private void DotsChangeHandler(int newVal)
    {
        //Number of dots has changed
        //Update the number of dots being shown
        UpdateDotAmount();
        //Update the color of those dots
        if (!singleColorGradient)
            UpdateGradient();
    }

    //Updates the number of dots being shown
    void UpdateDotAmount()
    {
        //There must be at least 1 child under the Dot Parent
        //because can't hide dots if none exist and
        //need at least 1 dot to duplicate more if needed
        if (dotList.Count > 0)
        {
            if (Dots > last_dots)
            {
                //MORE dots than before
                if (Dots > dotList.Count)
                    //Instantiate more Trajectory Dots if not enough exist
                    while (Dots > dotList.Count)
                    {
                        dotList.Add(new Dot(Instantiate(dotList[0].gameObject, dotsParent)));
                    }

                ShowAllDots();
            }
            else
            {
                //LESS dots than before
                //Hide extra dots
                HideDotsBeyond(Dots);
            }
        }
        else
            Debug.LogError("Trajectory Aim Error: the 'Dot Parent' doesn't have any children. There must be at least 1 child under the 'Dot Parent' at all times.");

        DotsVisible = Dots;
    }


    //Number of dots ACTUALLY visible
    //This value changes if Collisions are enabled and
    //a detected collision changes the number of dots visible
    private int DotsVisible
    {
        get { return dotsVisible; }
        set
        {
            if (dotsVisible.Equals(value)) return;
            int oldValue = dotsVisible;
            dotsVisible = value;
            if (OnDotsVisibleChange != null)
                OnDotsVisibleChange(oldValue, dotsVisible);
        }
    }
    public delegate void OnDotsVisibleChangeDelegate(int oldVal, int newVal);
    public event OnDotsVisibleChangeDelegate OnDotsVisibleChange;
    private void DotsVisibleChangeHandler(int oldVal, int newVal)
    {
        //Number of dots visible changed
        //If Collisions enabled, hide/show collisionIndicator accordingly
        if (checkForCollisions)
        {
            if (newVal.Equals(Dots))
                HideCollisionIndicator();
            else
                ShowCollisionIndicator();
        }

        if (newVal > oldVal)
            //MORE dots visible now
            ShowDotsUpTo(dotsVisible);
        else
            //LESS dots visible now
            HideDotsBeyond(dotsVisible);

        //Update dots' colors
        if (!singleColorGradient)
            UpdateGradient();
    }


    //The object which will be getting launched
    public GameObject Projectile
    {
        get { return projectile; }
        set
        {
            if (projectile != null && projectile.Equals(value)) return;
            projectile = value;

            if (OnProjectileChange != null)
                OnProjectileChange(projectile);
        }
    }
    public delegate void OnProjectileChangeDelegate(GameObject newVal);
    public event OnProjectileChangeDelegate OnProjectileChange;
    private void ProjectileChangeHandler(GameObject newVal)
    {
        //Projectile has changed
        UpdateProjectile();
    }

    //Get projectile's layermask from the Layer Collision Matrix
    void UpdateProjectile()
    {
        if (projectile == null)
            return;

        projectileRB = projectile.GetComponent<Rigidbody2D>();

        if (!autoUpdateLayerMask)
            return;

        layerMask = Physics2D.GetLayerCollisionMask(projectile.layer);
    }


    //Whether or not to check for collisions
    public bool Collisions
    {
        get { return checkForCollisions; }
        set
        {
            if (checkForCollisions.Equals(value)) return;
            checkForCollisions = value;

            if (OnCollisionsChange != null)
                OnCollisionsChange(checkForCollisions);
        }
    }
    public delegate void OnCollisionsChangeDelegate(bool newVal);
    public event OnCollisionsChangeDelegate OnCollisionsChange;
    private void CollisionsChangeHandler(bool newVal)
    {
        //Collisions changed
        if (newVal.Equals(false))
        {
            HideCollisionIndicator();
            ShowAllDots();
        }
    }


    //The scale of the dots in the trajectory line
    public float DotSize
    {
        get { return dotSize; }
        set
        {
            if (Mathf.Approximately(dotSize, value)) return;
            dotSize = value;

            if (OnDotSizeChange != null)
                OnDotSizeChange(dotSize);
        }
    }
    public delegate void OnDotSizeChangeDelegate(float newVal);
    public event OnDotSizeChangeDelegate OnDotSizeChange;
    private void DotSizeChangeHandler(float newVal)
    {
        //Dot size changed
        UpdateDotsSize();
    }

    //Update each Dot's size
    public void UpdateDotsSize()
    {
        foreach (Dot dot in dotList)
            dot.transform.localScale = Vector2.one * dotSize;
    }


    //Put all dots into dotList
    public void UpdateDotList()
    {
        dotList.Clear();

        //Get all children from the dotsParent Transform
        List<Transform> dotTransforms = new List<Transform>(dotsParent.GetComponentsInChildren<Transform>());
        dotTransforms.Remove(dotsParent);

        //Add all the child Transforms to dotList
        foreach (Transform dotTransform in dotTransforms)
        {
            dotList.Add(new Dot(dotTransform.gameObject));
        }
    }


    //The gradient of the dots in the trajectory line
    public Gradient Gradient
    {
        get { return gradient; }
        set
        {
            if (gradient.Equals(value)) { return; }
            gradient = value;

            if (OnGradientChange != null)
                OnGradientChange(gradient);
        }
    }
    public delegate void OnGradientChangeDelegate(Gradient newVal);
    public event OnGradientChangeDelegate OnGradientChange;
    private void GradientChangeHandler(Gradient newVal)
    {
        //Gradient changed. Update it
        UpdateGradient();
    }


    //The space between dots in the trajectory line
    public float DotSpread
    {
        get { return dotSpread; }
        set
        {
            value = Mathf.Max(value, 0.001f);
            if (Mathf.Approximately(dotSpread, value)) return;
            dotSpread = value;
        }
    }

#if UNITY_EDITOR
    //Draw a green circle showing the projectileScale
    private void OnDrawGizmosSelected()
    {
        if (projectile != null && checkForCollisions)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(projectile.transform.position, Vector3.back, projectileScale / 2f);
        }
    }
#endif

}