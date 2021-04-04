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
    float randomnessFactor = 4f;
    int numOfGroundRays = 17;
    float angle = 180;

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
            AvoidObstacles();
            CheckDistanceToWp();
            isGrounded = GroundCheck();
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("verticalSpeed", rb.velocity.y);
            if (isGrounded)
            {
                CalculateVelocityChange(CalculateMovement() + avoidVector.normalized * 3);
            }
            TurnCharacter(CalculateMovement() + avoidVector.normalized * 3);
            FallFromMap();
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

    //private void CheckIfStuck()
    //{
    //    if (rb.velocity == Vector3.zero)
    //    {
    //        waypointNo--;
    //    }
    //}

    private void AvoidObstacles()
    {
        avoidVector = Vector3.zero;

        for (int i = 0; i < numOfGroundRays; i++)
        {
            var rot = transform.rotation;
            var rotVariaton = Quaternion.AngleAxis((i / (float)numOfGroundRays) * angle -90, transform.up);
            var direction = rot * rotVariaton * (transform.forward * 6f - transform.up);

            Ray ray = new Ray(transform.position + transform.up, direction);
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, 15f))
            {
                avoidVector -= 1/numOfGroundRays * direction;
            }
            else
            {
                Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
                RaycastHit hit;
                if (Physics.SphereCast(origin, 4f, transform.forward, out hit, 3f))
                {
                    if (hit.transform.GetComponent<Obstacles>() != null)
                    {
                        var obs = hit.transform.GetComponent<Obstacles>();
                        if (obs.obsType != Obstacles.ObstacleType.FinishLine && obs.obsType != Obstacles.ObstacleType.RotatingPlatform)
                        {
                            avoidVector = (new Vector3(hit.point.x, 0f, hit.point.z) - transform.position);
                            if (Vector3.Dot(transform.forward, avoidVector) > 0)
                            {
                                avoidVector *= -1f;
                            }
                            if (obs.obsType == Obstacles.ObstacleType.RotatingStick && Vector3.Distance(hit.point, transform.position) <= 3f)
                            {
                                canJump = true;
                            }
                        }
                        else
                        {
                            avoidVector = Vector3.Lerp(avoidVector, Vector3.zero, Time.deltaTime);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < numOfGroundRays; i++)
        {
            var rot = transform.rotation;
            var rotVariaton = Quaternion.AngleAxis((i / (float)numOfGroundRays) * angle - 90, transform.up);
            var direction = rot * rotVariaton * (transform.forward * 6f - transform.up);
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
        if(Waypoints.GetChild(wpNo).childCount > 1)
        {
            int i = Random.Range(0, Waypoints.GetChild(wpNo).childCount);
            waypointPos = Waypoints.GetChild(wpNo).GetChild(i).position + randomness;
        }
        else if (wpNo == Waypoints.childCount-1)
        {
            int randX = Random.Range(-21, 21);
            waypointPos = Waypoints.GetChild(wpNo).position + new Vector3(randX, 0f, 0f);
        }
        else if(wpNo == 3 || wpNo == 4)
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
        if(Vector3.Distance(transform.position,target) < randomnessFactor)
        {
            waypointNo++;
            target = GetWaypointPosition(waypointNo);
        }
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = Vector3.zero;
        return movement = (target - transform.position).normalized;
    }

    public override void CalculateVelocityChange(Vector3 movementVector)
    {
        base.CalculateVelocityChange(movementVector);
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
        base.ApplyJumpForce();
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
        StartCoroutine(StunCo());
        StartCoroutine(RespawnCo());
        waypointNo = 0;
        GetWaypointPosition(waypointNo);
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
