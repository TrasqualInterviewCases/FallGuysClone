﻿using System;
using System.Collections;
using UnityEngine;

public class PlayerCharacter : CharacterBase, IEffectable
{
    Vector3 mousePos;
    bool inputActive = false;
    float hor = 0f;
    float ver = 0f;
    Vector3 movement;

    [Header("Player Specific Jump Parameters")]
    [SerializeField]
    float maxJumpTimer = 0.2f;
    [SerializeField]
    float minMouseMovementToJump = 100f;
    bool jumpTimerActive = false;
    float jumpTimer = 0f;

    bool isGrounded = true;
    bool onRotatingPlatform = false;

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
        if (!isStunned)
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
        //if (ragController.activateRagdoll)
        //{
        //    transform.position = ragController.hips.position - new Vector3(0f, currentY, 0f);
        //}
    }

    private void FixedUpdate()
    {
        if (!isStunned)
        {
            if (inputActive && isGrounded && movement != Vector3.zero)
            {
                MoveCharacter(targetVelocity, velocityChange);
            }
            ApplyJumpForce();
        }
    }


    private void GetInput()
    {
        if (racing)
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
                if (jumpTimer <= maxJumpTimer && isGrounded)
                {
                    if (Mathf.Abs(hor) >= minMouseMovementToJump || Mathf.Abs(ver) >= minMouseMovementToJump)
                    {
                        Jump();
                    }
                }
                inputActive = false;
                jumpTimerActive = false;
                anim.SetFloat("speed", 0f);
            }
        }
    }

    public override void CalculateVelocityChange(Vector3 movementVector)
    {
        base.CalculateVelocityChange(movementVector);
    }

    public override void MoveCharacter(Vector3 targetVelocity, Vector3 velocitychange)
    {
        if (!onRotatingPlatform)
        {
            base.MoveCharacter(targetVelocity, velocitychange);
        }
        else
        {
            rb.AddForce(transform.forward * speed, ForceMode.Force);
        }
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
        inputActive = false;
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

    public void GetStunned(float force, Vector3 Vector,ForceMode forceMode)
    {
        rb.AddForce(force * Vector, forceMode);
        StartCoroutine(StunCo());
    }

    public void Finish()
    {
        StartCoroutine(FinishCo());
    }

    public IEnumerator FinishCo()
    {
        racing = false;
        GameObject wall = GameObject.Find("WallToPaint");
        float t = 0;
        Vector3 curPos = transform.position;
        while (t < 1)
        {
            t += Time.deltaTime;
            rb.isKinematic = true;
            transform.position = Vector3.MoveTowards(curPos, (wall.transform.position - new Vector3(0f, 5f, 4f)),t*15f);
            transform.LookAt((wall.transform.position - new Vector3(0f, 6.2f, 3f)));
            anim.SetFloat("speed", 1f);
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector3.zero;
        anim.SetFloat("speed", 0f);
        Camera mainCam = Camera.main;
        yield return new WaitForSeconds(0.5f);
        mainCam.GetComponent<Es.InkPainter.Sample.MousePainter>().enabled = true;
        mainCam.GetComponent<TextureCheck>().isPainting = true;
        mainCam.GetComponent<TextureCheck>().percentText.gameObject.SetActive(true);
        mainCam.GetComponent<CameraFollow>().target = wall.transform;
        Transform vCams = GameObject.Find("VirtualCams").transform;
        vCams.GetChild(0).gameObject.SetActive(false);
        vCams.GetChild(1).gameObject.SetActive(true);
        GameManager.gmInstance.FinishRace();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Obstacles>() != null)
        {
            Obstacles obstacle = other.GetComponent<Obstacles>();
            if(obstacle.obsType == Obstacles.ObstacleType.RotatingPlatform)
            {
                onRotatingPlatform = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onRotatingPlatform = false;
    }
}
