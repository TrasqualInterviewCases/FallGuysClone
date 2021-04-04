using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCharacter : CharacterBase,IEffectable
{
    [SerializeField]
    Transform Waypoints;

    Vector3 target;

    int waypointNo = 0;

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
                CheckDistanceToWp();
                isGrounded = GroundCheck();
                anim.SetBool("isGrounded", isGrounded);
                anim.SetFloat("verticalSpeed", rb.velocity.y);
                if (isGrounded)
                {
                    CalculateVelocityChange(CalculateMovement());
                }
                TurnCharacter(CalculateMovement());
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
            }
            ApplyJumpForce();
        }
    }

    private void DetectObstacle()
    {

    }

    private void AvoidObstacle()
    {

    }

    private Vector3 GetWaypointPosition(int wpNo)
    {
        Vector3 waypointPos = new Vector3();
        if(Waypoints.GetChild(wpNo).childCount > 1)
        {
            int i = Random.Range(0, Waypoints.GetChild(wpNo).childCount - 1);
            waypointPos = Waypoints.GetChild(wpNo).GetChild(i).position;
        }
        else if (wpNo == Waypoints.childCount-1)
        {
            int randX = Random.Range(-21, 21);
            waypointPos = Waypoints.GetChild(wpNo).position + new Vector3(randX, 0f, 0f);
        }
        else
        {
            waypointPos = Waypoints.GetChild(wpNo).position;
        }
        return waypointPos;
    }

    private void CheckDistanceToWp()
    {
        if(Vector3.Distance(transform.position,target) < 1f)
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
