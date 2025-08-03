using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class weaponPickupScript : MonoBehaviour
{
    private BoxCollider2D bc;

    public int weaponID;

    public string scenename;
    private void Awake()
    {
        scenename = SceneManager.GetActiveScene().name;
        bc = GetComponent<BoxCollider2D>();
        if (scenename == "Tutorial") StartCoroutine(delay()); 
        StartCoroutine(deleter());
        if (scenename != "Tutorial") bc.enabled = true;
    }

    private IEnumerator delay()
    {
        bc.enabled = false;
        yield return new WaitForSeconds(0.6f);
        bc.enabled = true;
    }

    private IEnumerator deleter()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
}
