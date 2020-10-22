using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mapa : MonoBehaviour
{
    [SerializeField]
    private GameObject telaMapa;

    [Header("Dimensions Value")]
    [SerializeField]
    Vector2 terrainDimesions;
    [SerializeField]
    Vector2 maxMapSize = Vector2.one * 2000;
    [SerializeField]
    Vector2 minMapSize = Vector2.one * 1000;

    [Header("Marcador Values")]
    [SerializeField]
    private Sprite marcador;
    [SerializeField]
    private GameObject marcadorCenaPrefab;
    [SerializeField]
    private GameObject painelConfirmacaoMarcador;
    [SerializeField]
    private float zoomPercentage = 110;

    RectTransform rectTransform;
    GameObject marcadorAtual = null;

    GameObject marcadorCenaAtual = null;
    Vector3 mousePosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (gameObject.activeInHierarchy) {
            if (Input.GetMouseButtonDown(1))
            {
                MarkerOption();
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                ScrollMap(Input.mouseScrollDelta.y);
            }
        }
    }

    void ScrollMap(float scrollValue)
    {
        bool isZoomIn = (scrollValue > 0) ? true : false;

        if (isZoomIn)
        {
            if (rectTransform.rect.height < maxMapSize.y && rectTransform.rect.width < maxMapSize.x)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width * zoomPercentage / 100);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height * zoomPercentage / 100);
            }
            else return;
        }

        else
        {
            if (rectTransform.rect.height > minMapSize.y && rectTransform.rect.width > minMapSize.x)
            {

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width * 100 / zoomPercentage);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height * 100 / zoomPercentage);
            }
            else return;
        }

        ScrollMapChildren(isZoomIn);
    }

    void ScrollMapChildren(bool isZoomIn)
    {
        for(int i = 1; i<transform.childCount; i++)
        {
            RectTransform childRect = transform.GetChild(i).GetComponent<RectTransform>();
            childRect.anchorMax = childRect.anchorMin = Vector2.one / 2;

            float translationPercentage = (isZoomIn) ? zoomPercentage / 100 : 100 / zoomPercentage;

            childRect.anchoredPosition *= translationPercentage;
            //new Vector2(childRect.anchoredPosition.x * translationPercentage, childRect.anchoredPosition.y * translationPercentage);

            childRect.anchorMax = childRect.anchorMin = Vector2.zero;

        }
    }

    void MarkerOption()
    {
        mousePosition = Input.mousePosition;
        if (marcadorAtual != null)
            painelConfirmacaoMarcador.SetActive(true);

        else           
            PutMarker();

    }

    public void RemoveMarker()
    {
        if(marcadorAtual != null)
        {
            Destroy(marcadorAtual);
            marcadorAtual = null;

            Destroy(marcadorCenaAtual);
            marcadorCenaAtual = null;
        }
    }

    public void PutMarker()
    {
        Vector3 pos = mousePosition;
        GameObject marcador = new GameObject("Marcador", typeof(RectTransform), typeof(Image));

        RectTransform rectMarcador = marcador.GetComponent<RectTransform>();
        Image imageMarcador = marcador.GetComponent<Image>();

        rectMarcador.pivot = Vector2.zero;
        rectMarcador.anchorMax = Vector2.zero;
        rectMarcador.anchorMin = Vector2.zero;

        rectMarcador.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 75);
        rectMarcador.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 75);

        imageMarcador.sprite = this.marcador;
        imageMarcador.preserveAspect = true;

        rectMarcador.position = pos;

        marcador.transform.SetParent(gameObject.transform, true);

        rectMarcador.localScale = Vector3.one;

        if (rectMarcador.anchoredPosition.x < rectTransform.rect.width && rectMarcador.anchoredPosition.x > 0 && rectMarcador.anchoredPosition.y < rectTransform.rect.height && rectMarcador.anchoredPosition.y > 0)
        {
            if (marcadorAtual != null)
            {
                Destroy(marcadorAtual);
                marcadorAtual = null;
            }
            marcadorAtual = marcador;

            Vector3 posCena = new Vector3(rectMarcador.anchoredPosition.x * (terrainDimesions.x / rectTransform.rect.width), 30f,
                rectMarcador.anchoredPosition.y * (terrainDimesions.y / rectTransform.rect.height));

            Debug.Log("Posição Mapa: " + rectMarcador.anchoredPosition.x + " - " + rectMarcador.anchoredPosition.y);
            Debug.Log("Posição Cena: " + posCena.x + " - " + posCena.z);

            if (marcadorCenaAtual != null)
                marcadorCenaAtual.transform.position = posCena;
            else
                marcadorCenaAtual = Instantiate(marcadorCenaPrefab, posCena, marcadorCenaPrefab.transform.rotation);
        }
        else
        {
            Destroy(marcador);
        }
    }

    public void FastTravel(RectTransform buttonTransform)
    {
        Vector2 correctPosition = new Vector2(buttonTransform.anchoredPosition.x + buttonTransform.rect.width/2, buttonTransform.anchoredPosition.y + buttonTransform.rect.height/2);

        Vector3 pos = new Vector3(correctPosition.x * (terrainDimesions.x / rectTransform.rect.width), 100,
            correctPosition.y * (terrainDimesions.y / rectTransform.rect.height));

        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit hit;       

        Physics.Raycast(ray, out hit, 100,LayerMask.GetMask("Ground"));

        Player.player.transform.position = hit.point;
        telaMapa.SetActive(false);
    }

}
