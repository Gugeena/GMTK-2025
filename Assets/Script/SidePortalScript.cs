using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SidePortalScript : MonoBehaviour
{
    public static Transform RLocation;
    public static Transform LLocation;

    public Transform RRLocation;
    public Transform LLLocation;

    private void Start()
    {
        RLocation = RRLocation;
        LLocation = LLLocation;
    }
}
