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
    private AnimationClip[] attackCombos;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isRunning, isDashing, isGrounded, isFalling;

    private int direction;

    public Transform RLocation;
    public Transform LLocation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        direction = 1;
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            StartCoroutine(dash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(jump());
        }

        isFalling = rb.velocity.y < -2f;
        print(isFalling);
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isJumping", !isGrounded);
    }

    void handleCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.Play("player_punch");
        }
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

    private IEnumerator dash()
    {
        print("dashing");
        isDashing = true;
        rb.AddForce(transform.right * direction * dashForce);
        anim.Play("player_dash");
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
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
        if(collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }

        if (collision.gameObject.name == "LLocation")
        {
            this.transform.position = RLocation.position;
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }
        else if (collision.gameObject.name == "RLocation")
        {
            this.transform.position = LLocation.position;
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }
    }

}
