using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private float originalYPosition;
    public float returnToYSpeed = 2f;
    public float verticalSlowdownFactor = 0.25f;
    public float randomizedx;
    public float randomizedy;
    bool shouldrandomize = false;
    bool hasRandomized = true;

    public GameObject[] weapons;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();

        // rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.bodyType = RigidbodyType2D.Kinematic;
        originalYPosition = transform.position.y;

        randomizedx = UnityEngine.Random.RandomRange(1, 5);
        randomizedy = UnityEngine.Random.RandomRange(3, 6);

        canMove = false;
        StartCoroutine(waiter());
    }

    public IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }


    // Update is called once per frame
    void Update()
    {
        if(shouldrandomize && !hasRandomized)
        {
            //randomizedx = UnityEngine.Random.RandomRange(1, 5);
            //randomizedy = UnityEngine.Random.RandomRange(3, 6);
            //hasRandomized = true;
            //StartCoroutine(randomizertimer());
        }

        print(canMove + "canMOOOVE");

        if (canAttack) handleFlip();

        if (hp <= 0)
        {
            StartCoroutine(death());
        }

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        print("distance: " + distance + "-- stopdistance: " + stopDistance);

        if (distance > stopDistance)
        {
            if (canMove)
            {
                print("Shevedi");
                transform.rotation = Quaternion.Euler(0f, 0f, 0);

                animator.SetBool("shouldAttack", false);

                Vector2 targetposition = new Vector2(player.position.x, originalYPosition);
                float currentY = transform.position.y;
                float TargetY = Mathf.MoveTowards(currentY, originalYPosition, returnToYSpeed * Time.deltaTime);
                Vector2 newposition = Vector2.MoveTowards(new Vector2(transform.position.x, currentY), new Vector2(targetposition.x, TargetY), movespeed * Time.deltaTime);

                transform.position = newposition;

                if (player.transform.position.x > this.transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            shouldrandomize = true;
        }
        else
        {
            float minusrandomizedx = UnityEngine.Random.RandomRange(0, 1);
            Vector2 targetposition;
            if (minusrandomizedx < 0)
            {
                targetposition = new Vector2(player.position.x + randomizedx, player.position.y + randomizedy);
            }
            else
            {
                targetposition = new Vector2(player.position.x - randomizedx, player.position.y + randomizedy);
            }
            transform.position = Vector2.MoveTowards(transform.position, targetposition, (movespeed * verticalSlowdownFactor) * Time.deltaTime);

            if (canAttack)
            {
                StartCoroutine(attack());
                canAttack = false;
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
    
    public IEnumerator randomizertimer()
    {
        yield return new WaitForSeconds(10f);
        hasRandomized = false;
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
        gameObject.SetActive(false);
        yield break;
    }
}
