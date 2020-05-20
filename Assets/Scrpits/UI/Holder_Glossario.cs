using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Holder_Glossario : Button, ISelectHandler
{
    public GameObject texto;
    public GameObject titulo;


    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        UIController.uiController.AtualizarTextoGlossario(titulo, texto);

    }
       

}