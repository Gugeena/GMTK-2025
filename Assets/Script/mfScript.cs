using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public class mfScript : MonoBehaviour
{
    [SerializeField]
    private ShakeSelfScript shakeSelfScript;
    private camShakerScript camShakerScript;
    private Rigidbody2D rb;

    private bool move, goBack;
    [SerializeField]
    private float speed, returnSpeed;

    private Vector2 startPos;

    private Transform playerTransform; 

    public int direction;
    private int teleportCount;

    public GameObject blowUpParticles;
    public GameObject pickup;

    [SerializeField]
    private ParticleSystem particles;
    void Awake()
    {
        
        move = false;
        playerTransform = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        camShakerScript = GetComponent<camShakerScript>();
        StartCoroutine(life());
        shakeSelfScript.Begin();
    }

    private void Update()
    {
        if (move)
        {
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);

        }
        else if (goBack) {
            backing();
        }
        if(teleportCount >= 2 && transform.position.x >= startPos.x)
        {
            StartCoroutine(back());
        }
    }

    private void backing()
    {
        Vector2 direction = playerTransform.position - this.transform.position;

        if (direction == Vector2.zero && playerTransform.gameObject.GetComponent<PlayerMovement>().currentWeapon == 0)
        {
            Instantiate(pickup, playerTransform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (direction == Vector2.zero && playerTransform.gameObject.GetComponent<PlayerMovement>().currentWeapon != 0)
        {
            Instantiate(blowUpParticles, this.gameObject.transform.position, Quaternion.identity);   
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, returnSpeed * Time.deltaTime);
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private IEnumerator life()
    {
        yield return new WaitForSeconds(0.8f);
        move = true;
        StartCoroutine(camShakerScript.shake());
        startPos = transform.position;
    }

    private IEnumerator back()
    {
        rb.velocity = Vector2.zero;
        move = false;
        camShakerScript.Stop();
        particles.Stop();
        yield return new WaitForSeconds(0.6f);
        rb.gravityScale = 1;
        goBack = true;
    }

    private IEnumerator teleport()
    {
        yield return new WaitForSeconds(0.2f);
        teleportCount++;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LLocation")
        {
            rb.MovePosition(new Vector2(SidePortalScript.RLocation.position.x, transform.position.y));
            StartCoroutine(teleport());
        }
        else if (collision.gameObject.name == "RLocation")
        {
            rb.MovePosition(new Vector2(SidePortalScript.LLocation.position.x + 0.25f, transform.position.y));
            StartCoroutine(teleport());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "LLocation")
        {
            rb.MovePosition(new Vector2(SidePortalScript.RLocation.position.x, transform.position.y));
            StartCoroutine(teleport());
        }
        else if (collision.gameObject.name == "RLocation")
        {
            rb.MovePosition(new Vector2(SidePortalScript.LLocation.position.x + 0.25f, transform.position.y));
            StartCoroutine(teleport());
        }
    }
}
