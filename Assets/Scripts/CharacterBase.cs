using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField]
    [Header("Movement Parameters")]
    protected float speed = 5f;
    [SerializeField]
    float maxVelocityChange = 3f;
    [SerializeField]
    float turnSpeed = 8f;
    protected Vector3 targetVelocity;
    protected Vector3 velocityChange;

    protected Animator anim;
    protected Rigidbody rb;

    [Header("Jump Parameters")]
    [SerializeField]
    protected float jumpForce = 400f;
    protected bool canJump = false;

    float groundDistance = 0.2f;
    [SerializeField]
    protected LayerMask mask;

    [Header("Stun Parameters")]
    protected float spawnTime = 2f;
    protected bool isStunned;
    protected bool respawning;
    Vector3 startPos;
    Quaternion startRot;

    [SerializeField]
    protected bool racing = false;

    public virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        GameManager.gmInstance.OnRaceStart += EnableControls;
    }

    public bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, groundDistance, mask, QueryTriggerInteraction.Ignore);
    }

    public void EnableControls(bool start)
    {
        racing = start;
    }

    public virtual void CalculateVelocityChange()
    {
        targetVelocity = transform.forward * speed;
        Vector3 currentVelocity = rb.velocity;
        //if (targetVelocity.magnitude < currentVelocity.magnitude)
        //{
        //    targetVelocity = currentVelocity;
        //    rb.velocity /= 1.1f;
        //}
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
        if(movementVector != Vector3.zero)
        {
            Quaternion direction = Quaternion.LookRotation(movementVector);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * turnSpeed);
        }
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
        if (transform.position.y < -2f)
        {
            StartCoroutine(StunCo());
            StartCoroutine(RespawnCo());
        }
    }

    public virtual IEnumerator RespawnCo()
    {
        if (!respawning)
        {
            respawning = true;
            yield return new WaitForSeconds(spawnTime);
            transform.position = startPos;
            transform.rotation = startRot;
            respawning = false;
        }

    }

    public virtual IEnumerator StunCo()
    {
        isStunned = true;
        anim.enabled = false;
        yield return new WaitForSeconds(spawnTime);
        anim.SetFloat("speed", 0f);
        isStunned = false;
        anim.enabled = true;
    }
}
