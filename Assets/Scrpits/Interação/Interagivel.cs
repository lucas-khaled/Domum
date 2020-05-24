using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interagivel : MonoBehaviour
{
    public GameObject worldCanvas;

    protected virtual void Start()
    {
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
    }

}
