using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class BotonGotico : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public GameObject iconoReliquia; 

    void Start()
    {
        if (iconoReliquia != null)
        {
            iconoReliquia.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MostrarIcono();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OcultarIcono();
    }

    public void OnSelect(BaseEventData eventData)
    {
        MostrarIcono();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OcultarIcono();
    }

    private void MostrarIcono()
    {
        if (iconoReliquia != null)
        {
            iconoReliquia.SetActive(true);
        }
    }

    private void OcultarIcono()
    {
        if (iconoReliquia != null)
        {
            iconoReliquia.SetActive(false);
        }
    }
}