using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyOrbScript : MonoBehaviour
{
    public GameObject particle;
    public GameObject particle1;

    public Transform target;
    public float speed = 5f;

    private Vector2 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(particle, this.transform.position, Quaternion.identity);
        target = GameObject.Find("Player").transform;
        Vector2 targetPosition = target.position;

        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        moveDirection += new Vector2(0, -0.09f);
    }

    // Update is called once per frame
    void Update()
    {
       transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Instantiate(particle, this.transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision.gameObject.tag == "Enemy" || collision.gameObject.layer == 8))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "mfHitbox" || collision.gameObject.tag == "meleehitbox")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!(collision.gameObject.tag == "Enemy" || collision.gameObject.layer == 8))
        {
            Destroy(gameObject);
        }
    }
}
