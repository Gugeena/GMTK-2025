using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class camShakerScript : MonoBehaviour
{
    [SerializeField] private GameObject shakeCam;


    public float amplitude, frequency, time;

    private float oldAmp, oldFreq;

    public bool onCollide;

    private void Awake()
    {
        if (shakeCam == null) shakeCam = GameObject.Find("ShakeCam");
    }
    public IEnumerator shake()
    {
        CinemachineVirtualCamera cam = shakeCam.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin cbmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        oldAmp = cbmcp.m_AmplitudeGain;
        oldFreq = cbmcp.m_FrequencyGain;
        cbmcp.m_AmplitudeGain = amplitude;
        cbmcp.m_FrequencyGain = frequency;


        cam.enabled = true;

        if (time > 0)
        {
            yield return new WaitForSeconds(time);
            cam.enabled = false;
            cbmcp.m_AmplitudeGain = oldAmp;
            cbmcp.m_FrequencyGain = oldFreq;
        }
    }

    public void Stop()
    {
        StopAllCoroutines();

        CinemachineVirtualCamera cam = shakeCam.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin cbmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmcp.m_AmplitudeGain = oldAmp;
        cbmcp.m_FrequencyGain = oldFreq;
        cam.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCollide && collision.gameObject.layer != 9) 
        {
            StartCoroutine(shake());
        }
    }

}
