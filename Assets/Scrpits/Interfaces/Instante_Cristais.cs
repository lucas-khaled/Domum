using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instante_Cristais : MonoBehaviour
{
    public GameObject prefab;
    public static Instante_Cristais instante;
    public int Valor;

    private void Awake()
    {
        instante = this;
    }
    public void pegaValor(int value){
        Valor = value;
    }
    void Update(){
        PopulateCristal(Valor);//precisa de melhorar
    }

    public void PopulateCristal(int value)
    {

        GameObject Imagem;
        for(int i = 0; i < value; i++)
        {
            Imagem = (GameObject)Instantiate(prefab, transform);
        }
    }
    
}
