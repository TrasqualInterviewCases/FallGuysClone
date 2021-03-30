using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IEffectable
{
    [Header("Character Controls")]
    [SerializeField]
    float runSpeed = 5f; //character's run speed
    [SerializeField]
    float turnSpeed = 10f; //character's turn speed depending on mouse axis input       
    [SerializeField]
    float maxJumpTimer = 0.2f; //max time between mouse down and up inputs to tell if the character should jump on mouse release
    [SerializeField]
    float groundDistance = 0.1f; //maxground distance to check if the character is grounded
    [SerializeField]
    float jumpForce = 300f;  //character's jump force
    [SerializeField]
    LayerMask mask; //ground layer mask for the ground check raycast
    [SerializeField]
    float stunDuration = 3f;


    Animator anim;
    Rigidbody rb;
    Collider coll;
    Rigidbody[] ragdollRB;
    Collider[] ragdollCOL;

    bool isGrounded = true; //ground check bool
    bool canMove = false; // Toggle's true on mouse down and false on mouse up to move the character
    bool canJump = false; // Starts the jumpTimer count
    bool jumpNow = false; // On input detection set true to appl the jumpforce
    bool isStunned = false;


    Vector3 mousePos;

    Vector3 movement;
    float jumpTimer;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        //ragdollRB = GetComponentsInChildren<Rigidbody>(true);
        //ragdollCOL = GetComponentsInChildren<Collider>(true);

        //foreach (var rigidbody in ragdollRB)
        //{
        //    rigidbody.isKinematic = true;
        //}
        //foreach (var colliders in ragdollCOL)
        //{
        //    colliders.enabled = false;
        //}

        //rb.isKinematic = false;
        //coll.enabled = true;
    }

    private void Update()
    {
        GetInput();
        Animate();
        TurnCharacter(movement);
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        ApplyJumpForce();
    }

    #region MovementControls
    private void GetInput()
    {
        //if (!isStunned)
        //{
            if (Input.GetMouseButtonDown(0))
            {
                canMove = true;
                canJump = true;
                mousePos = Input.mousePosition;
                jumpTimer = 0f;

            }
            if (canJump)
            {
                jumpTimer += Time.deltaTime;
            }
            if (canMove && jumpTimer >= maxJumpTimer)
            {
                Vector3 currentMousePos = Input.mousePosition;
                float hor = currentMousePos.x - mousePos.x;
                float ver = currentMousePos.y - mousePos.y;
                movement = new Vector3(hor, 0, ver).normalized;
            }
            if (Input.GetMouseButtonUp(0))
            {
                canMove = false;
                if (jumpTimer < maxJumpTimer && canJump && GroundCheck())
                {
                    Jump();
                }
            }
        //}
    }

    private void TurnCharacter(Vector3 movementVector)
    {
        Quaternion direction = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * turnSpeed);
    }

    private void MoveCharacter()
    {
        if (canMove && jumpTimer >= maxJumpTimer && GroundCheck() && movement != Vector3.zero)
            rb.MovePosition(transform.position + (transform.forward * runSpeed * Time.deltaTime));
    }
    #endregion

    private void Jump()
    {
        jumpNow = true;
        anim.SetTrigger("jump");
        anim.SetBool("run", false);
        canJump = false;
    }

    private void ApplyJumpForce()
    {
        if (jumpNow)
        {
            rb.AddForce((Vector3.up + transform.forward) * jumpForce);
            jumpNow = false;
            canMove = false;
        }
    }

    private void Animate()
    {
        if (!isStunned)
        {
            if (canMove && GroundCheck() && jumpTimer >= maxJumpTimer && movement != Vector3.zero)
            {
                anim.SetBool("run", true);
            }
            else
            {
                anim.SetBool("run", false);
            }
        }
    }

    private bool GroundCheck()
    {
       return isGrounded = Physics.CheckSphere(transform.position, groundDistance, mask,QueryTriggerInteraction.Ignore);
    }

    public void Respawn()
    {

    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, ForceMode.Force);
    }

    public void GetStunned(float force, Vector3 position, float radius)
    {
        rb.AddExplosionForce(force, position,radius);
        //StartCoroutine(StunCo());
    }

    public IEnumerator StunCo()
    {
        //if (!isStunned)
        //{
        //    isStunned = true;
        //    anim.enabled = false;
        //    rb.isKinematic = true;
        //    coll.enabled = false;
        //    foreach (var rigidbody in ragdollRB)
        //    {
        //        rigidbody.isKinematic = false;
        //    }
        //    foreach (var colliders in ragdollCOL)
        //    {
        //        colliders.enabled = true;
        //    }



            yield return new WaitForSeconds(1f);

        //    foreach (var rigidbody in ragdollRB)
        //    {
        //        rigidbody.isKinematic = true;
        //    }
        //    foreach (var colliders in ragdollCOL)
        //    {
        //        colliders.enabled = false;
        //    }

        //    isStunned = false;
        //    anim.enabled = true;
        //    rb.isKinematic = false;
        //    coll.enabled = true;
        //}
    }

    public void Finish()
    {
        Camera mainCam = Camera.main;
        mainCam.GetComponent<Es.InkPainter.Sample.MousePainter>().enabled = true;
        mainCam.GetComponent<CameraFollow>().target = GameObject.Find("WallToPaint").transform;
        enabled = false;
    }
}
