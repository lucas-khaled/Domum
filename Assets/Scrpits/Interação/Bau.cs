using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bau : Interagivel
{
    public List<Item> itens = new List<Item>();
    [SerializeField]
    Animator anim;

    public bool someSeVazia;

    public override void Interact()
    {
        base.Interact();

        if (anim != null)
        {
            anim.SetBool("Aberto", true);
        }

        BauUI.bauUI.SetBau(this);

        if(someSeVazia)
        {
            StartCoroutine(Some());
        }

    }

    private IEnumerator Some()
    {
        yield return new WaitForSeconds(0.01f);
        if (itens.Count == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
