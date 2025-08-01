using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SidePortalScript : MonoBehaviour
{
    public Transform RLocation;
    public Transform LLocation;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player" && this.gameObject.name == "LLocation")
        {
            collision.gameObject.transform.position = RLocation.position;
        }
        else if (collision.gameObject.name == "Player" && this.gameObject.name == "RLocation")
        {
            collision.gameObject.transform.position = LLocation.position;
        }
    }
}
