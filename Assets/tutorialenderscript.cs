using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialenderscript : MonoBehaviour
{
    public GameObject fadeout;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator gadasvla()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(0.95f);
        SceneManager.LoadScene(2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(gadasvla());
        }
    }
}
