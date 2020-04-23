using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    Quest q;
    int selecionado = 0;
    string[] options;
    List<GameObject> condHolders = new List<GameObject>();
    Transform placesRoot;

    private void OnEnable()
    {
        q = (Quest)target;
        placesRoot = GameObject.Find("CondHolder").transform;
        DrawPlaces(q);
    }

    private void OnDisable()
    {
        UndrawPlaces();
    }

    public override void OnInspectorGUI()
    {      
        base.OnInspectorGUI();
    }


    private void DrawPlaces(Quest q)
    {
        if (q.condicoes.Count > 0)
        {
            int i = 1;
            foreach (Condicoes c in q.condicoes)
            {
                GameObject condicaoHolder = new GameObject("Cond"+i, typeof(DrawQuesGizmo));
                condicaoHolder.transform.SetParent(placesRoot);

                condicaoHolder.GetComponent<DrawQuesGizmo>().SetCondicaoOnHolder(c);

                condHolders.Add(condicaoHolder);
                i++;
            }
        }
    }

    private void UndrawPlaces()
    {
        if (condHolders.Count > 0)
        {
            foreach(GameObject go in condHolders)
            {
                DestroyImmediate(go);
                Debug.Log("Distrui");
            }

            condHolders.Clear();
        }
    }
}
