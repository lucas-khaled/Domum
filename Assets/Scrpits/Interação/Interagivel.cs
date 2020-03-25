﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interagivel : MonoBehaviour
{
    public GameObject worldCanvas;

    private void Start()
    {
        worldCanvas.SetActive(false);
    }

    public void SwitchCanvas(bool isActual)
    {
        worldCanvas.SetActive(isActual);
    }

    public virtual void Interact()
    {
        Debug.Log("Interagi" + this.transform.name);
    }

}
