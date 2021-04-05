using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCharacter : CharacterBase,IEffectable
{
    [SerializeField]
    Transform Waypoints;

    Vector3 target;
    Vector3 avoidVector;

    int waypointNo = 0;
    float randomnessFactor = 5f;
    int numOfGroundRays = 17;
    float angle = 360;

    bool isGrounded = true;

    public override void Awake()
    {
        base.Awake();
        Waypoints = GameObject.Find("Waypoints").transform;
    }

    public override void Start()
    {
        base.Start();
        target = GetWaypointPosition(waypointNo);
    }

    private void Update()
    {
        if (!isStunned)
        {
            isGrounded = GroundCheck();
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("verticalSpeed", rb.velocity.y);
            if (racing)
            {
                AvoidObstacles();
                CheckDistanceToWp();
                if (isGrounded)
                {
                    CalculateVelocityChange();
                }
                TurnCharacter(CalculateMovement() + avoidVector);
                FallFromMap();
            }
        }

        //if (ragController.activateRagdoll)
        //{
        //    transform.position = ragController.hips.position - new Vector3(0f, currentY, 0f);
        //}
    }

    private void FixedUpdate()
    {
        if (!isStunned && racing)
        {
            if (isGrounded)
            {
                MoveCharacter(targetVelocity, velocityChange);
                anim.SetFloat("speed", rb.velocity.magnitude);
                ApplyJumpForce();
            }            
        }
    }

    private void AvoidObstacles()
    {
        avoidVector = Vector3.zero;
        if (isGrounded)
        {
            for (int i = 0; i < numOfGroundRays; i++)
            {
                var rot = transform.rotation;
                var rotVariaton = Quaternion.AngleAxis((i / (float)numOfGroundRays) * angle - 90, transform.up);
                var direction = rot * rotVariaton * (transform.forward * 3f - transform.up);

                Ray ray = new Ray(transform.position + transform.up, direction);
                RaycastHit hitInfo;
                if (!Physics.Raycast(ray, out hitInfo, 3.5f))
                {
                    avoidVector -= new Vector3(direction.x, 0f, direction.z);
                }
                else if (hitInfo.transform.GetComponent<Obstacles>() != null)
                {
                    if (hitInfo.transform.GetComponent<Obstacles>().obsType != Obstacles.ObstacleType.FinishLine && hitInfo.transform.GetComponent<Obstacles>().obsType != Obstacles.ObstacleType.RotatingPlatform)
                    {
                        avoidVector -= new Vector3(direction.x, 0f, direction.z);
                    }
                    else if (hitInfo.transform.GetComponent<Obstacles>().obsType == Obstacles.ObstacleType.RotatingPlatform || hitInfo.transform.GetComponent<Obstacles>().obsType == Obstacles.ObstacleType.RotatingStick)
                    {
                        avoidVector = Vector3.zero;
                    }
                }
            }
        }


        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        RaycastHit hit;
        if (Physics.SphereCast(origin, 4f, transform.forward, out hit, 2f))
        {
            if (hit.transform.GetComponent<Obstacles>() != null)
            {
                var obs = hit.transform.GetComponent<Obstacles>();
                if (obs.obsType != Obstacles.ObstacleType.FinishLine && obs.obsType != Obstacles.ObstacleType.RotatingPlatform)
                {
                    avoidVector = (new Vector3(hit.transform.position.x, 0f, hit.transform.position.z) - transform.position);

                    if (obs.obsType == Obstacles.ObstacleType.RotatingStick)
                    {
                        if(hit.distance <= 2.5f)
                        {
                            canJump = true;
                        }
                        avoidVector = Vector3.zero;;
                    }
                }
                else if(obs.obsType == Obstacles.ObstacleType.RotatingPlatform)
                {
                    avoidVector = Vector3.zero;
                }
            }
            if (hit.transform.GetComponent<CharacterBase>() != null)
            {
                avoidVector -= (new Vector3(hit.transform.position.x, 0f, hit.transform.position.z) - transform.position);
            }
            if (Vector3.Dot(transform.forward, avoidVector) > 0)
            {
                avoidVector *= -1f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < numOfGroundRays+1; i++)
        {
            var rot = transform.rotation;
            var rotVariaton = Quaternion.AngleAxis((i / (float)numOfGroundRays) * angle - 90, transform.up);
            var direction = rot * rotVariaton * (transform.forward * 3f - transform.up);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + transform.up, direction);
        }
    }

    private Vector3 GetWaypointPosition(int wpNo)
    {
        float randomX = Random.Range(-randomnessFactor, randomnessFactor);
        float randomZ = Random.Range(-randomnessFactor, randomnessFactor);
        Vector3 randomness = new Vector3(randomX, 0f, randomZ);
        Vector3 waypointPos = new Vector3();
        if(Waypoints.GetChild(wpNo).childCount != 0)
        {
            int i = Random.Range(0, Waypoints.GetChild(wpNo).childCount);
            waypointPos = Waypoints.GetChild(wpNo).GetChild(i).position + randomness;
        }
        else if (wpNo == Waypoints.childCount-1)
        {
            int randX = Random.Range(-21, 21);
            waypointPos = Waypoints.GetChild(wpNo).position + new Vector3(randX, 0f, 0f);
        }
        else if(wpNo == 1 || wpNo == 2)
        {
            waypointPos = Waypoints.GetChild(wpNo).position;
        }
        else
        {
            waypointPos = Waypoints.GetChild(wpNo).position + randomness;
        }
        return waypointPos;
    }

    private void CheckDistanceToWp()
    {
        if(Vector3.Distance(transform.position,target) < 6f)
        {
            if (waypointNo < Waypoints.childCount - 1)
            {
                waypointNo++;
            }
            target = GetWaypointPosition(waypointNo);
        }
    }

    private Vector3 CalculateMovement()
    {
        return (target - transform.position).normalized;
    }

    public override void CalculateVelocityChange()
    {
        base.CalculateVelocityChange();
    }

    public override void MoveCharacter(Vector3 targetVelocity, Vector3 velocitychange)
    {
        base.MoveCharacter(targetVelocity, velocitychange);
    }

    public override void TurnCharacter(Vector3 movementVector)
    {
        base.TurnCharacter(movementVector);
    }

    public override void Jump()
    {
        base.Jump();
    }

    public override void ApplyJumpForce()
    {
        if (canJump)
        {
            rb.AddForce((Vector3.up) * jumpForce);
            canJump = false;
        }
    }

    public override void FallFromMap()
    {
        base.FallFromMap();
    }

    public override IEnumerator RespawnCo()
    {
        yield return StartCoroutine(base.RespawnCo());
    }

    public override IEnumerator StunCo()
    {
        yield return StartCoroutine(base.StunCo());
    }

    public void Respawn()
    {
        waypointNo = 0;
        StartCoroutine(StunCo());
        StartCoroutine(RespawnCo());
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void GetStunned(float force, Vector3 Vector, ForceMode forceMode)
    {
        rb.AddForce(force * Vector, forceMode);
        StartCoroutine(StunCo());
    }

    public void Finish()
    {
        enabled = false;
    }
}
