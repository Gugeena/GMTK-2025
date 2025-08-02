using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject Scroll;

    bool Paused = false;

    public GameObject Fadeout;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Paused)
            {
                Scroll.SetActive(true);
                Time.timeScale = 0f;
                Paused = true;
            }
            else
            {
                Scroll.SetActive(false);
                Time.timeScale = 1f;
                Paused = false;
            }
        }
    }

    private IEnumerator MainMenu()
    {
        Time.timeScale = 1f;
        Fadeout.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Scroll.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void YesMainMenu()
    {
        StartCoroutine(MainMenu());
    }

    public void No()
    {
        Scroll.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
