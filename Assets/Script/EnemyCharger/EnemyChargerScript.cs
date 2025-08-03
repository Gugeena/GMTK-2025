using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChargerScript : MonoBehaviour
{
    public float movespeed = 3f;
    public Transform player;
    public float stopDistance = 4f;
    public GameObject attackHitbox;
    public bool canAttack = true;
    public float detectiondistance = 8f;
    public Animator animator;
    public bool canMove = true;
    Rigidbody2D rb;
    float hp = 2f;
    public GameObject particles;

    private bool isGrounded, isFalling;

    public GameObject[] weapons;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        isGrounded = false;

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        canAttack = false;
        canMove = false;
        StartCoroutine(waiter());
    }

    public IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack) handleFlip();

        if(hp <= 0)
        {
            StartCoroutine(death());
        }

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance > stopDistance)
        {

            if (canMove)
            {
                animator.SetBool("shouldRun", true);
                animator.SetBool("shouldFein", false);
                animator.SetBool("shouldAttack", false);

                Vector2 targetposition = new Vector2(player.position.x, transform.position.y);

                Vector2 moveDir = new Vector2(targetposition.x - transform.position.x, 0).normalized;
                rb.velocity = new Vector2(moveDir.x * movespeed, rb.velocity.y);

                if (player.transform.position.x > this.transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        else
        {
            if (canAttack && (int)player.transform.position.y == (int)this.transform.position.y)
            {
                // && (int) player.position.y == (int) transform.position.y
                canMove = false;
                animator.SetBool("shouldRun", false);
                animator.SetBool("shouldAttack", true);
                animator.SetBool("shouldFein", false);
                StartCoroutine(attack());
                canAttack = false;
            }
            else if ((int)player.transform.position.y != (int)this.transform.position.y)
            {
                animator.SetBool("shouldRun", false);
                animator.SetBool("shouldAttack", false);
                animator.SetBool("shouldFein", true);
            }

        }

        animator.SetBool("isGrounded", isGrounded);

    }

    public void handleFlip()
    {
        if (player.transform.position.x > this.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public IEnumerator death()
    {
        PauseScript.kill++;
        Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rbb in rbs)
        {
            rbb.bodyType = RigidbodyType2D.Dynamic;
            rbb.AddForce(transform.up * Random.Range(2f, 3f), ForceMode2D.Impulse);
            rbb.AddTorque(Random.Range(6f, 7f));
            rbb.gameObject.transform.parent = null;
            rbb.excludeLayers = rbb.excludeLayers ^ (1 << LayerMask.NameToLayer("Player"));
            BoxCollider2D bc = rbb.gameObject.GetComponent<BoxCollider2D>();
            if (bc != null) bc.isTrigger = false;
        }
        canMove = false;
        canAttack = false;
        Instantiate(weapons[UnityEngine.Random.Range(0, 4)], this.gameObject.transform.position, Quaternion.identity);
        PlayerMovement.hp++;
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield break;
    }

    public IEnumerator attack()
    {
        //animplay
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackHitbox.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("shouldAttack", false);
        canAttack = true;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "meleehitbox")
        {
            print("hit");
            hp--;
            float direction = Mathf.Sign(transform.localScale.x);
            float knockback = 4f;
            Vector2 force = new Vector2(-direction, 0);
            rb.AddForce(force * (knockback * 1000f), ForceMode2D.Impulse);
            print("damaged");
        }

        if (collision.gameObject.tag == "mfHitbox")
        {
            print("hit");
            StartCoroutine(death());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "meleehitbox")
        {
            print("hit");
            hp--;
            float direction = Mathf.Sign(transform.localScale.x);
            float knockback = 4f;
            Vector2 force = new Vector2(-direction, 0);
            rb.AddForce(force * (knockback * 1000f), ForceMode2D.Impulse);
            print("damaged");
        }
        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "mfHitbox")
        {
            print("hit");
            StartCoroutine(death());
        }
    }
}
