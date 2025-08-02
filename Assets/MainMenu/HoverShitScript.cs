using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverShitScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Glow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Glow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Glow.SetActive(false);
    }
}
