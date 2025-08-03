using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject Scroll;

   public static bool Paused = false;

    public GameObject Fadeout;

    public Text kills, damage, time;
    public static float kill, dmg, dro;

    string scenename;

    public bool alreadyshown = false;

    private void Start()
    {
        scenename = SceneManager.GetActiveScene().name;

        if (scenename == "IrasScena")
        {
            kill = 0;
            dmg = 0;
            dro = 0;
        }

        StartCoroutine(Timer());
    }
    public IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            dro++;
            if (scenename != "IrasScena")
                break;
        }
    }

    private void Update()
    {
        if (scenename == "IrasScena")
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
        else if(scenename != "IrasScena" && !alreadyshown)
        {
            StartCoroutine(death());
        }
    }

    private IEnumerator MainMenu()
    {
        Time.timeScale = 1f;
        Fadeout.SetActive(true);
        yield return new WaitForSeconds(0.95f);
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

    public IEnumerator death()
    {
        alreadyshown = true;
        kills.text = "" + kill;
        damage.text = "" + dmg;
        time.text = "" + dro;
        yield return new WaitForSeconds(5f);
        Fadeout.SetActive(true);
        yield return new WaitForSeconds(0.95f);
        SceneManager.LoadScene(0);
    }
}
