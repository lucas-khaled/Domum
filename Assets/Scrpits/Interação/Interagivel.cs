using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Interagivel : MonoBehaviour
{
    public GameObject worldCanvas;

    protected bool isPartOfDialogue = false;

    private List<Dialogo> dialogoCondicao = new List<Dialogo>();

    public void SetDialogoCondicao(Dialogo dialogo)
    {
        isPartOfDialogue = true;
        dialogoCondicao.Add(dialogo);
        EventsController.onDialogoTerminado += OnDialogoTerminado;
    }

    protected virtual void Start()
    {       
        worldCanvas.SetActive(false);
        worldCanvas.transform.localPosition = new Vector3(0, worldCanvas.transform.localPosition.y, 0);
        gameObject.layer = LayerMask.NameToLayer("Interagivel");
    }

    public void SwitchImagemInteracao(bool isActual)
    {
        if (GameController.gameController.QualOrigemInput() == OrigemInput.MOUSE)
        {
            worldCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameController.gameController.interagivelTeclado;
        }
        else
        {
            worldCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameController.gameController.interagivelJoystick;
        }
        worldCanvas.SetActive(isActual);
    }

    public virtual void Interact()
    {
        if (EventsController.onInteracao != null)
        {
            EventsController.onInteracao.Invoke(this);
        }
        if (isPartOfDialogue)
        {
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoCondicao[0]);
        }
    }

    void OnDialogoTerminado(Dialogo dialogo)
    {
        if(!isPartOfDialogue)
        {
            return;
        }

        if(dialogo.whosDialog == dialogoCondicao[0].whosDialog)
        {
            dialogoCondicao.RemoveAt(0);

            if(dialogoCondicao.Count == 0)
            {
                isPartOfDialogue = false;
            }
        }
    }

}
