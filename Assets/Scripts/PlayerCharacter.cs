using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase, IEffectable
{


    Vector3 mousePos;
    bool inputActive = false;
    float hor = 0f;
    float ver = 0f;
    Vector3 movement;

    [Header("Jump Parameters")]
    bool jumpTimerActive = false;
    float jumpTimer = 0f;
    [SerializeField]
    float maxJumpTimer = 0.2f;
    [SerializeField]
    float minMouseMovementToJump = 100f;

    bool isGrounded = true;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        isGrounded = GroundCheck();
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalSpeed", rb.velocity.y);
        GetInput();
        if (isGrounded)
        {
            CalculateVelocityChange(movement);
        }
        TurnCharacter(movement);
        FallFromMap();
    }

    private void FixedUpdate()
    {
        if (inputActive && isGrounded && movement != Vector3.zero)
        {
            MoveCharacter(targetVelocity,velocityChange);
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

    public void Respawn()
    {
        StartCoroutine(RespawnCo());
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
