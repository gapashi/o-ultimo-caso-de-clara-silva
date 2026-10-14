using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInvestigavel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Identificação Única")]
    public string idPista;

    [Header("Dados da Pista")]
    public Sprite assetGrande;
    [TextArea(3, 5)]
    public string falaGabriel;

    [Header("Reação Gabriel")]
    public Sprite expressaoGabriel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        InspectorManager.Instance.DefinirCursorMao();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InspectorManager.Instance.DefinirCursorSeta();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InspectorManager.Instance.RegistrarPistaColetada(idPista);
        InspectorManager.Instance.AbrirInspecao(assetGrande, falaGabriel, expressaoGabriel);
    }
}
