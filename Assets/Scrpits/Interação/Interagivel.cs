using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interagivel : MonoBehaviour
{
    public GameObject worldCanvas;

    protected bool isPartOfDialogue;

    private List<Dialogo> dialogoCondicao = new List<Dialogo>();

    public void SetDialogoCondicao(Dialogo dialogo)
    {
        isPartOfDialogue = true;
        dialogoCondicao.Add(dialogo);
        EventsController.onDialogoTerminado += OnDialogoTerminado;
    }

    protected virtual void Start()
    {
        isPartOfDialogue = false;
        worldCanvas.SetActive(false);
    }

    public void SwitchImagemInteracao(bool isActual)
    {
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
        if(dialogo.whosDialog == dialogoCondicao[0].whosDialog)
        {
            dialogoCondicao.RemoveAt(0);

            Debug.Log(dialogoCondicao[1].whosDialog);
            if(dialogoCondicao.Count == 0)
            {
                isPartOfDialogue = false;
            }
        }
    }

}
