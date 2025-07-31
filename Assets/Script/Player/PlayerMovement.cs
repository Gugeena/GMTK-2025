using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed, dashForce;
    [SerializeField]
    private Transform camFollowTransform, headPivotTransform;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isRunning, isDashing, isJumping, isGrounded;

    private int direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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


        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            StartCoroutine(dash());
        }
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
        isDashing = true;
        rb.AddForce(transform.right * direction * dashForce);
        anim.Play("player_dash");
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
    }

    private IEnumerator jump()
    {
        yield return new WaitForSeconds(1f);
    }
}
