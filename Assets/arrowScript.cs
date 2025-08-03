using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowScript : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool landed = false;
    private bool back = false;


    [SerializeField]
    private float returnSpeed = 20;
    [SerializeField]
    private GameObject returnHB;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (rb.velocity != Vector2.zero && !landed)
        {
            Vector2 direction = rb.velocity;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        else if (back)
        {
            backing();
        }
    }

    private void backing()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) player = GameObject.Find("Player_Tutorial");
        Transform playerTransform = player.transform;
         

        Vector2 direction = playerTransform.position - this.transform.position;

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, returnSpeed * Time.deltaTime);
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator goBack()
    {
        yield return new WaitForSeconds(0.5f);
        returnHB.SetActive(true);
        back = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            landed = true;
            rb.simulated = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
            StartCoroutine(goBack());
        }

        if (collision.gameObject.name == "LLocation")
        {
            Vector2 vel = rb.velocity;
            rb.MovePosition(new Vector2(SidePortalScript.RLocation.position.x, transform.position.y));
            rb.velocity = vel;
        }
        else if (collision.gameObject.name == "RLocation")
        {
            Vector2 vel = rb.velocity;
            rb.MovePosition(new Vector2(SidePortalScript.LLocation.position.x, transform.position.y));
            rb.velocity = vel;
        }
    }

}
