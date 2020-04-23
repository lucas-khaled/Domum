using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawQuesGizmo : MonoBehaviour
{
    Condicoes condHolder;

    public void SetCondicaoOnHolder(Condicoes c)
    {
        condHolder = c;
    }
    private void OnDrawGizmos()
    {
        if (condHolder != null)
        {
            Gizmos.DrawWireSphere(condHolder.local, condHolder.distanciaChegada);
        }
    }
}
