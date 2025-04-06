using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Vector2 offset;
    private bool isDragging = false;
    private Vector2 targetPosition;
    public float snapSpeed = 5f; // Vitesse du retour au centre
    public float maxRadius = 80f; // Rayon maximum du déplacement
    public CardInfoMaj CardManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CardManager.lockUI == false)
        {
            isDragging = true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out offset
            );
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CardManager.lockUI == false)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
            {
                Vector2 newPosition = localPoint - offset;

                // Appliquer la contrainte circulaire
                if (newPosition.magnitude > maxRadius)
                {
                    newPosition = newPosition.normalized * maxRadius;
                }
                else
                {
                }

                //Si c'est à gauche
                if (newPosition.x < -45 & newPosition.y > -45 & newPosition.y < 45)
                {
                    CardManager.SelectSide(0);
                }
                //Si c'est à droite
                else if (newPosition.x > 45 & newPosition.y > -45 & newPosition.y < 45)
                {
                    CardManager.SelectSide(1);
                }
                //Si c'est en haut
                else if (newPosition.x > -45 & newPosition.x < 45 & newPosition.y > 45)
                {
                    CardManager.SelectSide(2);
                }
                //Si c'est en bas
                else if (newPosition.x > -45 & newPosition.x < 45 & newPosition.y < -45)
                {
                    CardManager.SelectSide(3);
                }
                // Si y'a rien on reset
                else
                {
                    CardManager.SelectSide(4);
                }
                rectTransform.localPosition = newPosition;
            }
        }
    }

    private void Update()
    {
        if (CardManager.lockUI == false)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                targetPosition = Vector2.zero; // Centre du Canvas
            }
            if (!isDragging)
            {
                if (CardManager.SelectedSide != 4 && CardManager.once == false)
                {
                    CardManager.LetGoDrag();
                }
                else
                {
                    CardManager.SelectSide(4);
                }
            }
        }
        if (!isDragging)
        {
            // Interpolation vers le centre
            rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, targetPosition, Time.deltaTime * snapSpeed);
        }
    }
}