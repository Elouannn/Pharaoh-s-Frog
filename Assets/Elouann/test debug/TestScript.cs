using UnityEngine;
using UnityEngine.EventSystems;

public class TestPointerDown : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down d�tect� sur : " + gameObject.name);
    }
}