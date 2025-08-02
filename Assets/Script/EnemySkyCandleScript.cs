using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkyCandleScript : MonoBehaviour
{
    public float movespeed = 3f;
    public Transform player;
    public float stopDistance = 3f;
    public GameObject attackHitbox;
    public bool canAttack = true;
    public float detectiondistance = 8f;
    public Animator animator;
    public bool canMove = true;
    Rigidbody2D rb;
    float hp = 2f;
    public GameObject particles;
    public GameObject headPivot;

    public Transform location;
    public GameObject orb;

    public Animator animatorofparent;

    public GameObject glow;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        print(canMove + "canMOOOVE");

        if (canAttack) handleFlip();

        if (hp <= 0)
        {
            StartCoroutine(death());
        }

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (detectiondistance > distance)
        {
            print("distance: " + distance + "-- stopdistance: " + stopDistance);
            if (distance > stopDistance)
            {
                if (canMove)
                {
                    print("Shevedi");
                    transform.rotation = Quaternion.Euler(0f, 0f, 0);

                    animator.SetBool("shouldAttack", false);

                    Vector2 targetposition = new Vector2(player.position.x, transform.position.y);

                    transform.position = Vector2.MoveTowards(transform.position, targetposition, movespeed * Time.deltaTime);

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
                if (canAttack)
                {
                    canMove = false;
                    StartCoroutine(attack());
                    canAttack = false;
                }
            }
        }
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
    
    public IEnumerator attack()
    {
        Instantiate(orb, location.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        canAttack = true;
        canMove = true;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "meleehitbox")
        {
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

    public IEnumerator death()
    {
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
        PlayerMovement.hp++;
        Instantiate(particles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        yield break;
    }
}
