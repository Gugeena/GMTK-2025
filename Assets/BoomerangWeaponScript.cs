using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;

public class BoomerangWeaponScript : MonoBehaviour
{
    bool shouldReturn = false;
    public GameObject player;
    Vector2 targetPoint;
    public GameObject blowUpParticles;
    public GameObject pickup;
    public Rigidbody2D rb;

    public Transform RLocation;
    public Transform LLocation;

    private void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.Rotate(0, 0, 1440 * Time.deltaTime);

        if (shouldReturn)
        {
            targetPoint = player.transform.position;
            if (Vector2.Distance(targetPoint, this.transform.position) < 0.5f)
            {
                if (player.GetComponent<PlayerMovement>().currentWeapon == 0)
                {
                    Instantiate(pickup, player.transform.position, Quaternion.identity);
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPoint, 15f * Time.deltaTime);
                //float angle = Mathf.Atan2(targetPoint.x, targetPoint.y) * Mathf.Rad2Deg;
                //this.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LLocation")
        {
            RLocation = GameObject.Find("RLOCATIONLOCATION").transform;
            this.transform.position = new Vector3(RLocation.position.x, this.transform.position.y, 0);
            shouldReturn = true;
            return;
        }
        else if (collision.gameObject.name == "RLocation")
        {
            LLocation = GameObject.Find("LLOCATIONLOCATION").transform;
            this.transform.position = new Vector3(LLocation.position.x, this.transform.position.y, 0);
            shouldReturn = true;
            return;
        }

        if (collision.gameObject.tag == "enemyorb" ||
            collision.gameObject.layer == 8 ||
            collision.gameObject.name == "Hand_L" ||
            collision.gameObject.name == "Hand_R" ||
            collision.gameObject.name == "Leg_L" ||
            collision.gameObject.name == "Leg_R" ||
            collision.gameObject.name == "headPivot" ||
            collision.gameObject.tag == "weaponPickup" ||
            collision.gameObject.name == "square" ||
            collision.gameObject.name == "Square" ||
            collision.gameObject.name == "Torso")
        {
            return;
        }

        print("collided with: " + collision.gameObject.tag + ", name of: " + collision.gameObject.name);
        rb.velocity = Vector2.zero;
        shouldReturn = true;
    }
}

