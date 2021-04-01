using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float maxVelocityChange = 10f;
    [SerializeField]
    float turnSpeed = 8f;

    Animator anim;
    Rigidbody rb;


    Vector3 mousePos;
    bool inputActive = false;
    float hor = 0f;
    float ver = 0f;
    Vector3 movement;

    [Header("Jump Parameters")]
    [SerializeField]
    float jumpForce = 400f;
    bool canJump = false;
    bool jumpTimerActive = false;
    float jumpTimer = 0f;
    [SerializeField]
    float maxJumpTimer = 0.2f;
    [SerializeField]
    float minMouseMovementToJump = 100f;

    bool isGrounded = true;
    float groundDistance = 0.2f;
    [SerializeField]
    LayerMask mask;


    [SerializeField]
    float spawnTime = 2f;

    Vector3 startPos;
    Quaternion startRot;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Update()
    {
        isGrounded = GroundCheck();
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalSpeed", rb.velocity.y);
        GetInput();
        TurnCharacter(movement);
        FallFromMap();
    }

    private void FixedUpdate()
    {
        if (inputActive && isGrounded && movement != Vector3.zero)
        {
            MoveCharacter(movement);
        }
        ApplyJumpForce();
    }


    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            inputActive = true;
            jumpTimerActive = true;
            jumpTimer = 0f;
        }
        if (jumpTimerActive)
        {
            jumpTimer += Time.deltaTime;
        }
        else
        {
            jumpTimer = 0;
        }
        if (inputActive)
        {
            Vector3 currentMousePos = Input.mousePosition;
            hor = currentMousePos.x - mousePos.x;
            ver = currentMousePos.y - mousePos.y;
            movement = new Vector3(hor, 0f, ver).normalized;
            if (isGrounded)
            {
                anim.SetFloat("speed", rb.velocity.magnitude);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(jumpTimer <= maxJumpTimer && isGrounded)
            {
                if(Mathf.Abs(hor) >= minMouseMovementToJump || Mathf.Abs(ver) >= minMouseMovementToJump)
                {
                    Jump();
                }
            }
            inputActive = false;
            jumpTimerActive = false;
            anim.SetFloat("speed", 0f);
        }

    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, groundDistance, mask, QueryTriggerInteraction.Ignore);
    }

    private void MoveCharacter(Vector3 movementVector)
    {
        Vector3 targetVelocity = transform.forward * movementVector.magnitude * speed;
        Vector3 currentVelocity = rb.velocity;
        if (targetVelocity.magnitude < currentVelocity.magnitude)
        {
            targetVelocity = currentVelocity;
            rb.velocity /= 1.1f;
        }
        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0f;
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        if (Mathf.Abs(velocityChange.magnitude) < targetVelocity.magnitude)
        {
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    private void TurnCharacter(Vector3 movementVector)
    {
        Quaternion direction = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * turnSpeed);
    }

    private void Jump()
    {
        canJump = true;        
    }

    private void ApplyJumpForce()
    {
        if (canJump)
        {
            rb.AddForce((Vector3.up + transform.forward) * jumpForce);
            canJump = false;
        }
    }

    private void FallFromMap()
    {
        if (rb.velocity.y <= -12f)
        {
            StartCoroutine(Respawna());
        }
    }

    public IEnumerator Respawna()
    {
        yield return new WaitForSeconds(spawnTime);
        transform.position = startPos;
        transform.rotation = startRot;

    }

    public void Respawn()
    {

    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void GetStunned(float force, Vector3 position, float radius)
    {
        rb.AddExplosionForce(force, position, radius);
    }

    public IEnumerator StunCo()
    {
        throw new System.NotImplementedException();
    }

    public void Finish()
    {
        Camera mainCam = Camera.main;
        mainCam.GetComponent<Es.InkPainter.Sample.MousePainter>().enabled = true;
        mainCam.GetComponent<CameraFollow>().target = GameObject.Find("WallToPaint").transform;
        enabled = false;
    }
}
