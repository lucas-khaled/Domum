using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InimigoBase : MonoBehaviour
{
    public int dano;
    public double velocidade;
    Animator anim;
    // criar array de itens d

    public int vida
    {
        get { return vida; }
        set { vida = value;
            if (vida <= 0)
                morrer();

        }
    }

    void Atacar()
    {

    }

    void Dano ()
    {
        
    }

    void morrer()
    {

    }

    void droparLoot()
    {

    }


}
