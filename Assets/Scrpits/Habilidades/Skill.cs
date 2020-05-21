﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public TipoPlayer player;
    public string ObjectDeAcesso;
    public string VariavelDeAcesso;
    public float qntMudança;
    public string mudançaSeString;

    private bool ativo = false;

    public void AtivaSkill()
    {
        ativo = true;

        var objAcesso = GameObject.FindObjectOfType(Type.GetType(ObjectDeAcesso));
        Debug.Log(objAcesso.GetType());

        PropertyInfo varAcesso = objAcesso.GetType().GetProperty(VariavelDeAcesso);

        if(varAcesso == null)
        {
            Debug.Log("Cuzin");
        }

        if (varAcesso.PropertyType == typeof(int))
        {
            varAcesso.SetValue(objAcesso, Mathf.CeilToInt(qntMudança), null);
            Debug.Log(varAcesso.GetValue(objAcesso,null));
        }

        else if(varAcesso.PropertyType == typeof(float))
        {
            varAcesso.SetValue(objAcesso, qntMudança, null);
        }

        else if (varAcesso.PropertyType == typeof(bool))
        {
            bool muda = (qntMudança >= 1) ? true : false;
            varAcesso.SetValue(objAcesso, muda);
            Debug.Log("cuu");
        }

        else if(varAcesso.PropertyType == typeof(string))
        {
            varAcesso.SetValue(objAcesso, mudançaSeString, null);
        }

    }
}
