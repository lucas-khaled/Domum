using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Item : ScriptableObject
{
    public string nome;
    [TextArea]
    public string descricao;

    public int custoMoeda;
    public float peso;

    public Sprite icone;


}
