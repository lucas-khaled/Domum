using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiverAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    public void Interagir()
    {
        if (Random.Range(0,2) == 1)
        anim.SetTrigger("Interacao");
        else
        anim.SetTrigger("Interacao2");
    }

    public void AceitarQuest()
    {
        anim.SetTrigger("QuestAceita");
    }
}
