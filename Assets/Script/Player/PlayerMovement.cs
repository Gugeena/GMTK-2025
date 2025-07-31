using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed, dashForce, jumpForce;
    [SerializeField]
    private Transform camFollowTransform, headPivotTransform;

    [SerializeField]
    private AnimationClip[] punchCombos;

    [SerializeField]
    private Animator cameraAnim;

    private int comboIndex = 0;
    [SerializeField]
    private float comboTime;
    private float elapsed;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isRunning, isDashing, isGrounded, isFalling;
    private bool canPunch;

    private int direction;

    public Transform RLocation;
    public Transform LLocation;

    public bool canDash = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        direction = 1;
        canPunch = true;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleLooking();
        handleCombat();
    }

    void handleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if(!isDashing)rb.velocity = new Vector2 (x * speed, rb.velocity.y);

        isRunning = x != 0;
        anim.SetBool("isWalking", isRunning);

        if (isRunning) anim.SetLayerWeight(1, 0.5f);
        else anim.SetLayerWeight(1, 1f);
        if(isFalling)
        {
            rb.gravityScale = 3f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        if (!isDashing)
        {
            if (x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                direction = -1;
            }
            else if (x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                direction = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && canDash) 
        {
            canDash = false;
            StartCoroutine(dash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(jump());
        }

        isFalling = rb.velocity.y < -0.1f;
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isJumping", !isGrounded);
    }

    void handleCombat()
    {
        print(comboIndex);
        if (Input.GetMouseButtonDown(0) && canPunch)
        {
            StartCoroutine(punch());
        }

        if((elapsed >= comboTime) || (comboIndex >= punchCombos.Length) || isRunning)
        {
            comboIndex = 0;
        }
        if (comboIndex > 0) elapsed += Time.deltaTime;
    }
    void handleLooking()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 pos = Vector2.Lerp(mousePos, headPivotTransform.position, 0.99f);
        pos.z = 0;
        camFollowTransform.position = pos;
           

        //Vector3 aimDirection = (mousePos - transform.position).normalized;

        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        //angle = Mathf.Clamp(angle, -10, 20);

        //headTransform.eulerAngles = new Vector3(0, 0, angle);
    }


    private IEnumerator punch()
    {
        canPunch = false;
        anim.Play(punchCombos[comboIndex].name);
        comboIndex++;
        elapsed = 0;
        yield return new WaitForSeconds(0.4f);
        canPunch = true;
    }
    private IEnumerator dash()
    {
        print("dashing");
        isDashing = true;
        rb.gravityScale = 0f;
        if (!isGrounded)
        {
            rb.AddForce(transform.right * direction * (dashForce - 200));
        }
        else
        {
            rb.AddForce(transform.right * direction * dashForce);
        }
        anim.Play("player_dash");
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        rb.gravityScale = 1f;
        StartCoroutine(dashCooldown());
        yield break;
    }
    private IEnumerator jump()
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(transform.up * jumpForce);
        isGrounded = false;
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "LLocation")
        {
            this.transform.position = new Vector3(RLocation.position.x, this.transform.position.y, 0);
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }
        else if (collision.gameObject.name == "RLocation")
        {
            this.transform.position = new Vector3(LLocation.position.x, this.transform.position.y, 0);
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }

        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JumpPad"))
        {
            rb.AddForce(transform.up * 425 * rb.gravityScale);
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("camTrigger"))
        {
            cameraAnim.Play("camera_rise");
        }
        else if (collision.gameObject.CompareTag("camTriggerDown"))
        {
            cameraAnim.Play("camera_fall");
        }
    }

    public IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(0.8f);
        canDash = true;
    }
}
