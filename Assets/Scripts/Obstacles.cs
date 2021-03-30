using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    [SerializeField]
    float force = 2f;
    [SerializeField]
    float rotationSpeed = 0.1f;
    [SerializeField]
    float movementSpeed = 0.1f;
    [SerializeField]
    Transform target;

    Vector3 startPos;

    bool moving;
    bool movingForward = true;
    bool cycleStarted;

    float t;

    public enum ObstacleType
    {
        RotatingPlatform,
        RotatingStick,
        Wall,
        Moving,
        HalfDonut,
        FinishLine
    }

    public ObstacleType obsType;

    private void Start()
    {
        t = 0;
        startPos = transform.position;
    }

    void Update()
    {
        switch (obsType)
        {
            case ObstacleType.RotatingPlatform:
                transform.Rotate(transform.forward * rotationSpeed);
                break;
            case ObstacleType.RotatingStick:
                transform.Rotate(transform.up, rotationSpeed);
                break;
            case ObstacleType.Wall:
                //the wall is static
                break;
            case ObstacleType.Moving:
                    MovingObstacleBetweenPositions();
                break;
            case ObstacleType.HalfDonut:
                if (!cycleStarted)
                {
                    StartCoroutine(MoveDonutForward());
                    cycleStarted = true;
                }
                break;
            case ObstacleType.FinishLine:
                //finish line is static
                break;
        }
    }


    private IEnumerator MoveDonutForward()
    {
        int randomWaitTime = Random.Range(1, 5);
        yield return new WaitForSeconds(randomWaitTime);
        while (t <= 1 && movingForward)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, target.position, t * movementSpeed * 4f);
            yield return new WaitForEndOfFrame();
        }
        t = 1;
        movingForward = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(MoveDonutBackward());
    }

    private IEnumerator MoveDonutBackward()
    {

        while (t >= 0 && !movingForward)
        {
            t -= Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, target.position, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0;
        movingForward = true;

        StartCoroutine(MoveDonutForward());
    }

    private void MovingObstacleBetweenPositions()
    {
        t = Mathf.PingPong(Time.time * movementSpeed, 1f);
        transform.position = Vector3.Lerp(startPos, target.position, t);        
    }

    private void OnCollisionEnter(Collision collision)
    {
        IEffectable effectable = collision.gameObject.GetComponent<IEffectable>();
        if (effectable != null)
        {
            if (obsType == ObstacleType.Wall || obsType == ObstacleType.Moving)
            {
                StartCoroutine(effectable.Respawn());
            }
            else if (obsType == ObstacleType.RotatingStick || obsType == ObstacleType.HalfDonut)
            {
                effectable.GetStunned(force, collision.contacts[0].point, 1f);
            }

        }
    }


    private void OnTriggerStay(Collider other) //addforce to player and enemies that enter the trigger in the rotation direction
    {
         IEffectable effectable = other.GetComponent<IEffectable>();
         if (effectable != null)
         {
            if (obsType == ObstacleType.RotatingPlatform)
            {
                Vector3 forceToAdd = force * -Vector3.right * Mathf.Sign(rotationSpeed);
                effectable.ApplyForce(forceToAdd, ForceMode.Force);
            }
            else if (obsType == ObstacleType.FinishLine)
            {
                effectable.Finish();
            }
        }
    }



}
