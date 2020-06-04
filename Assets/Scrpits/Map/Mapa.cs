using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mapa : MonoBehaviour
{
    [SerializeField]
    Vector2 terrainDimesions;
    [SerializeField]
    private Sprite marcador;
    [SerializeField]
    private GameObject marcadorCenaPrefab;

    RectTransform rectTransform;
    GameObject marcadorAtual = null;

    GameObject marcadorCenaAtual = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && gameObject.activeInHierarchy)
        {
            Vector3 pos = Input.mousePosition;
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

            Debug.Log("Pos on 2D: "+rectMarcador.anchoredPosition);

            if (rectMarcador.anchoredPosition.x < rectTransform.rect.width && rectMarcador.anchoredPosition.y < rectTransform.rect.height)
            {
                if (marcadorAtual != null)
                {
                    Destroy(marcadorAtual);
                    marcadorAtual = null;
                }
                marcadorAtual = marcador;

                Vector3 posCena = new Vector3(rectMarcador.anchoredPosition.x * (terrainDimesions.x / rectTransform.rect.width), 30f, 
                    rectMarcador.anchoredPosition.y * (terrainDimesions.y / rectTransform.rect.height));

                Debug.Log("Pos on 3D" + posCena);

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
    }

}
