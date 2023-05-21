using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    public Transform leftSlingShotOrigin, rightSlingShotOrigin;
    public LineRenderer slingShotLeftRubber, slingShotRightRubber;

    public Transform animalRestPosition;
    public GameObject animal;
    public float throwSpeed;
    public SlingShotState slingShotState;

    private Vector3 slingShotMiddleVector;

    public LineRenderer trajectory;


    void Start()
    {
        slingShotState = SlingShotState.Idle;
        
        slingShotLeftRubber.SetPosition(0, leftSlingShotOrigin.position);
        slingShotRightRubber.SetPosition(0, rightSlingShotOrigin.position);

        slingShotMiddleVector = new Vector3((leftSlingShotOrigin.position.x + rightSlingShotOrigin.position.x) / 2, (leftSlingShotOrigin.position.y + rightSlingShotOrigin.position.y) / 2, 0);
    }

    void SetTrajectoryActive(bool active)
    {
        trajectory.enabled = true;
    }


    void SetSlingshotRubbersActive(bool active)
    {
        slingShotLeftRubber.enabled = active;
        slingShotRightRubber.enabled = active;
    }
    void DisplaySlingshotRubbers()
    {
        slingShotLeftRubber.SetPosition(1, animal.transform.position);
        slingShotRightRubber.SetPosition(1, animal.transform.position);

    }
    void InitializeThrow()
    {
        animal.transform.position = new Vector3(animalRestPosition.position.x, animalRestPosition.position.y,0);
        slingShotState = SlingShotState.Idle;
        SetSlingshotRubbersActive(true);
    }

    void Update()
    {
        switch(slingShotState)
        {
            case SlingShotState.Idle:
                InitializeThrow();
                DisplaySlingshotRubbers();
                if(Input.GetMouseButtonDown(0))
                {
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if(animal.GetComponent<PolygonCollider2D>() == Physics2D.OverlapPoint(location))
                    {
                        slingShotState = SlingShotState.Pulling;
                    }
                }
                break;


            case SlingShotState.Pulling:
                DisplaySlingshotRubbers();
                if (Input.GetMouseButton(0))
                {
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    location.z = 0;
                    if(Vector3.Distance(location,slingShotMiddleVector) > 2.5f)
                    {
                        var maxPosition = (location - slingShotMiddleVector).normalized * 2.5f + slingShotMiddleVector;
                        animal.transform.position = maxPosition;
                        
                    }
                    else
                    {
                        animal.transform.position = location;
                    }

                    float pullDistance = Vector3.Distance(slingShotMiddleVector, animal.transform.position);
                    ShowTrajectory(pullDistance);
                }
                else
                {
                    SetTrajectoryActive(false);
                     
                    float distance = Vector3.Distance(slingShotMiddleVector, animal.transform.position);
                    if(distance > 0.5)
                    {
                        SetSlingshotRubbersActive(false);
                        slingShotState = SlingShotState.Flying;
                        ThrowAnimal(distance);
                    }
                }
                break;          
                
        }
    }

    void ShowTrajectory(float distance)
    {
        SetTrajectoryActive(true);
        Vector3 diff = slingShotMiddleVector - animal.transform.position;
        int segmentCount = 25;
        Vector2[] segments = new Vector2[segmentCount];
        segments[0] = animal.transform.position;

        Vector2 segVelocity = new Vector2(diff.x, diff.y) * throwSpeed * distance;
        for (int i = 1; i < segmentCount; i++)
        {
            float timeCurve = (i * Time.fixedDeltaTime * 5);
            segments[i] = segments[0] + segVelocity * timeCurve + 0.5f * Physics2D.gravity * Mathf.Pow(timeCurve, 2);
        }
        trajectory.positionCount = segmentCount;

        for (int j = 0; j < segmentCount; j++)
        {
            trajectory.SetPosition(j, segments[j]);
        }

    }
    void ThrowAnimal(float distance) 
    { 
        Vector3 velocity =slingShotMiddleVector - animal.transform.position;
        animal.GetComponent<Animal>().Onthrow();
        animal.GetComponent < Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * throwSpeed * distance;
    }

   
}
