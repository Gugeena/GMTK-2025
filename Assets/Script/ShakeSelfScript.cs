using UnityEngine;
using System.Collections;

public class ShakeSelfScript : MonoBehaviour
{
    [Header("Info")]
    private Vector3 _startPos;
    private Vector3 _randomPos;

    [Header("Settings")]
    [Range(0f, 2f)]
    public float _distance = 0.1f;
    [Range(0f, 0.1f)]
    public float _delayBetweenShakes = 0f;



    public void Begin()
    {
        StopAllCoroutines();
        _startPos = transform.localPosition;
        StartCoroutine(Shake());
    }

    public void stopShake()
    {
        StopAllCoroutines();
        transform.localPosition = _startPos;
    }

    private IEnumerator Shake()
    {
        while (true)
        {
            _randomPos = _startPos + (Random.insideUnitSphere * _distance);

            transform.localPosition = _randomPos;

            if (_delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(_delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }
    }
}
