using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinPosition : MonoBehaviour
{
    // SerializeField
    public Vector3 OriginPosition;
    public Vector3 TargetPosition;
    public float MoveTime;
    public bool LoopOption = false;
    public float Duration;
    public bool stay = false;

    private bool direction = false;
    private bool endMove = false;
    private float timer = 0f;


    // Lerp
    private float startTime;
    private float journeyLength;
    private float distCovered;
    private float fracJourney;

    //Reverse
    private bool reverseToHide = false;

    //Stay
    private bool staying = false;

    private void Start()
    {
        SetStartTime();
        SetJourneyLength();
        staying = stay;
    }

    private void Update()
    {
        if (!endMove && !staying) MovePosition();
    }

    private void MovePosition()
    {
        if (!direction)
        {
            if (Vector3.Distance(this.transform.localPosition, TargetPosition) < 3.0f)
            {
                if (!LoopOption) endMove = true;

                if (timer >= Duration)
                {
                    timer = 0f;
                    direction = !direction;

                    SetStartTime();
                    SetJourneyLength();
                }
                else
                {
                    timer += Time.deltaTime * 60;
                    Debug.Log("Time.deltaTime Value : " + Time.deltaTime * 60);
                }

                if (reverseToHide)
                {
                    reverseToHide = false;
                    staying = stay;
                    endMove = false;
                }
            }

            else
            {
                distCovered = (Time.time - startTime) * MoveTime;
                fracJourney = distCovered / journeyLength;
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, TargetPosition, fracJourney);
                Debug.Log("Target Lerp : " + Vector3.Lerp(this.transform.localPosition, TargetPosition, fracJourney));
                Debug.Log("This Pos : " + this.transform.localPosition);
                Debug.Log("Distance : " + Vector3.Distance(this.transform.localPosition, TargetPosition));
            }
        }

        if (direction)
        {
            if (Vector3.Distance(this.transform.localPosition, OriginPosition) < 3.0f)
            {
                if (!LoopOption) endMove = true;

                if (timer >= Duration)
                {
                    timer = 0f;
                    direction = !direction;

                    SetStartTime();
                    SetJourneyLength();
                }
                else
                {
                    timer += Time.deltaTime * 60;
                    Debug.Log("Time.deltaTime Value : " + Time.deltaTime * 60);
                }


                if (reverseToHide)
                {
                    reverseToHide = false;
                    staying = stay;
                    endMove = false;
                }
            }

            else
            {
                distCovered = (Time.time - startTime) * MoveTime;
                fracJourney = distCovered / journeyLength;
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, OriginPosition, fracJourney);
                Debug.Log("Origin Lerp : " + Vector3.Lerp(this.transform.localPosition, OriginPosition, fracJourney));
                Debug.Log("This Pos : " + this.transform.localPosition);
            }
        }

    }

    private void SetStartTime()
    {
        startTime = Time.time;
    }

    private void SetJourneyLength()
    {
        if (!direction)
        {
            journeyLength = Vector3.Distance(OriginPosition, TargetPosition);
        }

        if (direction)
        {
            journeyLength = Vector3.Distance(TargetPosition, OriginPosition);
        }
    }

    public void ReverseToHide()
    {
        endMove = !endMove;
        reverseToHide = true;
    }

    public void StartTwinPosition()
    {
        staying = false;
    }

}
