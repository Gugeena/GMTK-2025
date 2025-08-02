using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickupScript : MonoBehaviour
{
    private BoxCollider2D bc;

    public int weaponID;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        StartCoroutine(delay());    
    }

    private IEnumerator delay()
    {
        bc.enabled = false;
        yield return new WaitForSeconds(0.6f);
        bc.enabled = true;
    }
}
