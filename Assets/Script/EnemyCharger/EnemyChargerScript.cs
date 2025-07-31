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
    public Animator animator;
    public bool canMove = true;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance > stopDistance)
        {
            if (canMove)
            {
                animator.SetBool("shouldRun", true);

                Vector2 targetposition = new Vector2(player.position.x, transform.position.y);

                transform.position = Vector2.MoveTowards(transform.position, targetposition, movespeed * Time.deltaTime);

                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (canAttack)
            {
                canMove = false;
                animator.SetBool("shouldRun", false);
                animator.SetBool("shouldAttack", true);
                StartCoroutine(attack());
                canAttack = false;
            }
        }

    }

    public IEnumerator attack()
    {
        //animplay
        yield return new WaitForSeconds(0.3f);
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        attackHitbox.SetActive(false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("shouldAttack", false);
        canAttack = true;
        canMove = true;
    }
}
