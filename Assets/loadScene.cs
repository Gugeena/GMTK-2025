using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public static int SceneToLoad;
    void Start()
    {
        print(SceneToLoad);
        StartCoroutine(load());
    }

    private IEnumerator load()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneToLoad);
    }

}
