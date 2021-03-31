﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float maxVelocityChange = 3f;
    [SerializeField]
    float turnSpeed = 8f;
    public Vector3 targetVelocity;
    public Vector3 velocityChange;

    public Animator anim;
    public Rigidbody rb;

    [Header("Jump Parameters")]
    [SerializeField]
    float jumpForce = 400f;
    bool canJump = false;

    float groundDistance = 0.2f;
    [SerializeField]
    LayerMask mask;


    [SerializeField]
    float spawnTime = 2f;

    Vector3 startPos;
    Quaternion startRot;

    public virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, groundDistance, mask, QueryTriggerInteraction.Ignore);
    }

    public virtual void CalculateVelocityChange(Vector3 movementVector)
    {
        targetVelocity = transform.forward * movementVector.magnitude * speed;
        Vector3 currentVelocity = rb.velocity;
        if (targetVelocity.magnitude < currentVelocity.magnitude)
        {
            targetVelocity = currentVelocity;
            rb.velocity /= 1.1f;
        }
        velocityChange = targetVelocity - currentVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0f;
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
    }

    public virtual void MoveCharacter(Vector3 targetVelocity, Vector3 velocityChange)
    {
        if (Mathf.Abs(velocityChange.magnitude) < targetVelocity.magnitude)
        {
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    public virtual void TurnCharacter(Vector3 movementVector)
    {
        Quaternion direction = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * turnSpeed);
    }

    public virtual void Jump()
    {
        canJump = true;
    }

    public virtual void ApplyJumpForce()
    {
        if (canJump)
        {
            rb.AddForce((Vector3.up + transform.forward) * jumpForce);
            canJump = false;
        }
    }

    public virtual void FallFromMap()
    {
        if (rb.velocity.y <= -12f)
        {
            StartCoroutine(RespawnCo());
        }
    }

    public virtual IEnumerator RespawnCo()
    {
        yield return new WaitForSeconds(spawnTime);
        transform.position = startPos;
        transform.rotation = startRot;
    }
}
