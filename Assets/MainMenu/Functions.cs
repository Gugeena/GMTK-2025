using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Functions : MonoBehaviour
{
    public GameObject FadeOut;

    public void Play()
    {
        StartCoroutine(StartGame());
    }

    public void Quit()
    {
       Application.Quit();
    }

    private IEnumerator StartGame()
    {
        FadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }
}
