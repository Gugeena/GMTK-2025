using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] banished;
    bool canspawn = true;
    public GameObject spawningparticles;

    public GameObject[] weapons;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canspawn)
        {
            StartCoroutine(spawner());
        }
    }

    public IEnumerator spawner()
    {
        canspawn = false;
        yield return new WaitForSeconds(2f);
        int randomchoice = UnityEngine.Random.Range(0, 3);
        float randomx = UnityEngine.Random.Range(15.379f, 35.6f);
        if (randomchoice == 1)
        {
            float randomplace = UnityEngine.Random.Range(0, 2);
            if (randomplace == 1)
            {
                float randomy = UnityEngine.Random.Range(-2.24f, 3.4f);
                Instantiate(spawningparticles, new Vector2(randomx, randomy - 0.75f), Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                Instantiate(banished[1], new Vector2(randomx, randomy), Quaternion.identity);
            }
            else
            {
                float randomplacehighup = UnityEngine.Random.Range(0, 7);
                if (randomplacehighup == 1)
                {
                    Instantiate(banished[1], new Vector2(19.372f, -1.318f), Quaternion.identity);
                }
                else if (randomplacehighup == 2)
                {
                    Instantiate(banished[1], new Vector2(15.87f, 0.13f), Quaternion.identity);
                }
                else if (randomplacehighup == 3)
                {
                    Instantiate(banished[1], new Vector2(19.117f, 1.553f), Quaternion.identity);
                }
                else if (randomplacehighup == 4)
                {
                    float randomplacehighup1 = UnityEngine.Random.Range(0, 2);
                    if (randomplacehighup1 == 1)
                    {
                        Instantiate(banished[1], new Vector2(23.23f, 2.27f), Quaternion.identity);
                    }
                    else if (randomplacehighup == 2)
                    {
                        Instantiate(banished[1], new Vector2(26.85f, 2.27f), Quaternion.identity);
                    }
                }
                else if (randomplacehighup == 5)
                {
                    Instantiate(banished[1], new Vector2(31.31f, 1.553f), Quaternion.identity);
                }
                else if (randomplacehighup == 6)
                {
                    Instantiate(banished[1], new Vector2(33.109f, -1.319f), Quaternion.identity);
                }
            }
        }
        else
        {
            Instantiate(spawningparticles, new Vector2(randomx, -3.189f - 0.75f), Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
            Instantiate(banished[0], new Vector2(randomx, -3.189f), Quaternion.identity);
        }
        yield return new WaitForSeconds(0.5f);
        canspawn = true;
    }
}
