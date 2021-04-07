using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

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
                CalculateVelocityChange();
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
                        AudioManager.instance.PlayAudio("jump");
                    }
                }
                inputActive = false;
                jumpTimerActive = false;
                anim.SetFloat("speed", 0f);
            }
        }
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

    public void Respawn()
    {
        AudioManager.instance.PlayAudio("respawn");
        StartCoroutine(StunCo());
        StartCoroutine(RespawnCo());
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void GetStunned(float force, Vector3 Vector,ForceMode forceMode)
    {
        AudioManager.instance.PlayAudio("stun");
        rb.AddForce(force * Vector, forceMode);
        StartCoroutine(StunCo());
    }

    public void Finish()
    {
        StartCoroutine(FinishCo());
    }

    public IEnumerator FinishCo()
    {
        AudioManager.instance.PlayAudio("clap");
        anim.SetBool("finished", true);
        isStunned = false;
        Camera mainCam = Camera.main;
        mainCam.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        racing = false;
        GameObject wall = GameObject.Find("WallToPaint");
        float t = 0;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        anim.SetBool("isGrounded", isGrounded);
        var curPos = transform.position;
        anim.SetFloat("speed", 0f);
        yield return new WaitForSeconds(2f);
        anim.SetBool("finished", false);
        anim.SetFloat("speed", 1f);
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(curPos, new Vector3(wall.transform.position.x, transform.position.y, wall.transform.position.z - 4f),t);
            transform.LookAt(new Vector3(wall.transform.position.x, transform.position.y, wall.transform.position.z));            
            yield return new WaitForEndOfFrame();
        }
        anim.SetFloat("speed", 0f);
        
        yield return new WaitForSeconds(0.5f);
        mainCam.GetComponent<Es.InkPainter.Sample.MousePainter>().enabled = true;
        TextureCheck texCheck = mainCam.GetComponent<TextureCheck>();
        texCheck.enabled = true;
        texCheck.isPainting = true;
        texCheck.percentText.gameObject.SetActive(true);
        mainCam.GetComponent<CameraFollow>().target = wall.transform;
        Transform vCams = GameObject.Find("VirtualCams").transform;
        vCams.GetChild(0).gameObject.SetActive(false);
        vCams.GetChild(1).gameObject.SetActive(true);
        GameManager.gmInstance.FinishRace();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CharacterBase>() != null)
        {
            AudioManager.instance.PlayAudio("hit");
        }
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
