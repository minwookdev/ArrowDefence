/* © 2020 TippyTap Corp.
 * You may use and modify the contents of this asset (Trajectory Aimer 2D) for personal and commercial use.
 * Distribution of any contents within this asset, whether modified or original, is not permitted.
 */

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Camera mainCamera;

    [HideInInspector]
    public Vector2 initialTouchPos, currentTouchPos, shotVelocity;

    public Rigidbody2D rigidBody;

    public float sensitivity = 1, maxShotSpeed = 50, minShotSpeed = 0;

    public bool useTouchRadius;
    bool last_useTouchRadius;
    public float touchRadius = 1;
    float last_touchRadius;


    public bool freezeOnTouch;

    Vector2 lastVelocity;
    float lastAngularVelocity;

    [HideInInspector]
    public bool aiming, shotSpeedAboveMin;


    //Launches the Projectile
    public void Shoot()
    {
        if (freezeOnTouch)
            Unfreeze();

        aiming = false;
        rigidBody.velocity = shotVelocity;
        TrajectoryAim.instance.Hide();
        selectedProjectile = null;


        //Add code to run when shooting a shot here.

        /* Example:
         *       
         * Disable shooting a golf ball after a shot using:
         * SetSelectable(false);
         * 
         * Then enable shooting again after the ball has stopped moving using:
         * SetSelectable(true);
         */
    }


    //Begin aiming
    public void InitializeAim()
    {
        aiming = true;
        shotSpeedAboveMin = false;

        //Assign this Projectile to the Trajectory Aim script
        //If different than last Projectile,
        //trajectory Aim script will retrieve this Projectile's Layer Collision Matrix
        if (!TrajectoryAim.instance.Projectile.Equals(gameObject))
            TrajectoryAim.instance.Projectile = gameObject;

        if (freezeOnTouch)
            Freeze();


        //Add code to run when aiming starts here.

        /* Example:
         *       
         * (Using multiple Projectiles)
         * Change the color of the Collision Indicator to match the Projectile's color.
         * 
         * SpriteRenderer collisionIndicatorSR = TrajectoryAim.instance.collisionIndicator.GetComponent<SpriteRenderer>();
         * SpriteRenderer projectileSR = gameObject.GetComponent<SpriteRenderer>();
         * 
         * collisionIndicatorSR.color = projectileSR.color;       
         */
    }


    //Cancels a shot while in the aiming state
    //Called when shot speed is below the Min Shot Speed
    public void Cancel()
    {
        if (freezeOnTouch)
            Unfreeze();

        aiming = false;
        TrajectoryAim.instance.Hide();
        selectedProjectile = null;


        //Add code to run when a shot is cancelled here.
    }


    //Freezes Projectile in place and sets it's Rigidbody to Kinematic.
    public void Freeze()
    {
        //Store current momentum in case Deselect() is called
        lastVelocity = rigidBody.velocity;
        lastAngularVelocity = rigidBody.angularVelocity;

        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        rigidBody.isKinematic = true;


        //Add code to run when Projectile becomes FROZEN here.
    }

    public void Unfreeze()
    {
        rigidBody.isKinematic = false;

        //Add code to run when Projectile becomes UNFROZEN here.
    }


    //Makes the Projectile able/unable to be selected
    public void SetSelectable(bool value)
    {
        enabled = value;

        //Add code to run when Projectile becomes able or unable to be selected here.
    }



    void Update()
    {
        if (aiming)
        {
            //In an aiming state
            UpdateAim();

            if (Released())
            {
                //Only shoot if shot speed is above the minimum speed
                //otherwise cancel the shot
                if (shotVelocity.magnitude > minShotSpeed)
                    Shoot();
                else
                    Cancel();
            }
        }
        //NOT in an aiming state, initialize if started aiming
        else if (StartedAiming())
            InitializeAim();
    }

    //Check if projectile was selected
    bool StartedAiming()
    {
        //Check for a touch/left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            //Get the Initial Touch Position
            initialTouchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            //If Touch Radius isn't being used
            //projectile is selected
            if (!useTouchRadius)
                return true;

            //Touch Radius is being used
            float touchDistanceFromProjectile = (initialTouchPos - (Vector2)transform.position).magnitude;

            //If Initial Touch Position was within the Touch Radius AND
            //if projectile is closest to the Initial Touch Position out of other selected projectiles
            //projectile is selected
            if (touchDistanceFromProjectile <= touchRadius && BestSelection(touchDistanceFromProjectile))
                return true;
        }
        return false;
    }


    //Projectile currently selected
    static GameObject selectedProjectile = null;

    //Check if closer than last Selected Projectile to the Initial Touch Position
    bool BestSelection(float touchDistance)
    {
        //If there isn't a Selected Projectile
        if (selectedProjectile == null)
        {
            //Set as Selected Projectile
            selectedProjectile = gameObject;
            return true;
        }

        //There's another Selected Projectile
        //Check which one is closer
        GameObject otherProjectile = selectedProjectile;
        float otherProjectileTouchDistance = (initialTouchPos - (Vector2)otherProjectile.transform.position).magnitude;

        if (touchDistance <= otherProjectileTouchDistance)
        {
            //This projectile is closer to the touch position
            //Set this as the Selected Projectile
            otherProjectile.GetComponent<Projectile>().Deselect();
            selectedProjectile = gameObject;
            return true;
        }

        //This projectile wasn't closer to the Initial Touch Position than the other selected projectile
        return false;
    }


    //Used if using multiple projectiles
    //Used when an Initial Touch Position overlaps with more than 1 projectiles' Touch Radius and
    //a projectile closer to the Initial Touch Position overrides another projectile's selection
    public void Deselect()
    {
        //Another selection was closer to the touch position
        aiming = false;

        //If frozen in place, resume as if nothing happened
        if (freezeOnTouch)
        {
            Unfreeze();

            //Resume momentum prior to freezing
            rigidBody.velocity = lastVelocity;
            rigidBody.angularVelocity = lastAngularVelocity;
        }
    }

    //Checks if touch/left mouse button was released
    bool Released()
    {
        if (Input.GetMouseButtonUp(0))
            return true;
        else
            return false;
    }

    //Updates the velocity to apply to the Projectile
    void UpdateAim()
    {
        //Find the Shot Direction based on the current touch/mouse position
        currentTouchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        shotVelocity = (initialTouchPos - currentTouchPos) * sensitivity;

        ShotSpeedCheck();

        //Show the Trajectory Line if shot speed is above the set minimum
        if (shotSpeedAboveMin)
            TrajectoryAim.instance.UpdateAim(shotVelocity, transform.position);
    }


    //Updates the state of the shot depending on its current speed
    void ShotSpeedCheck()
    {
        bool current_ShotSpeedAboveMin = false;

        //Check if speed is above minimum
        if (minShotSpeed <= 0 || shotVelocity.magnitude >= minShotSpeed)
        {
            current_ShotSpeedAboveMin = true;

            //If shot power is above maximum, cap it
            if (shotVelocity.magnitude > maxShotSpeed)
                shotVelocity = shotVelocity.normalized * maxShotSpeed;
        }

        //Check if speed state just changed (did it JUST become more/less than minimum?)
        if (current_ShotSpeedAboveMin != shotSpeedAboveMin)
        {
            //Speed state has changed
            shotSpeedAboveMin = current_ShotSpeedAboveMin;

            if (shotSpeedAboveMin)
            {
                //Shot speed became MORE than minimum
                //SHOW trajectory
                TrajectoryAim.instance.Show();
            }
            else
            {
                //Shot speed became LESS than minimum
                //HIDE trajectory
                TrajectoryAim.instance.Hide();
            }
        }
    }

#if UNITY_EDITOR
    //Draws Touch Radius
    private void OnDrawGizmosSelected()
    {
        if (useTouchRadius)
        {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.back, touchRadius);
        }
    }
#endif

}

