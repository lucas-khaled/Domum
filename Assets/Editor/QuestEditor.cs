using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    Quest q;
    int selecionado = 0;
    string[] options;
    List<GameObject> condHolders = new List<GameObject>();
    Transform placesRoot;
    SerializedObject obj;
    SerializedProperty propertyToDraw = null;

    public bool isPlacesDrawn { get; private set; }

    private void OnEnable()
    {
        obj = new SerializedObject(target);
        q = (Quest)target;
        
        isPlacesDrawn = false;
    }


    public override void OnInspectorGUI()
    {
        FindCondHolder();
        q.nome = EditorGUILayout.TextField("Título: ", q.nome);
        q.principal = EditorGUILayout.Toggle("Principal: ", q.principal);
        EditorGUILayout.PropertyField(obj.FindProperty("reward"));

        EditorGUILayout.PropertyField(obj.FindProperty("dialogo"));

        //ShowConditions(obj.FindProperty("condicoes"));
        DrawConditions(obj.FindProperty("condicoes"));
        obj.ApplyModifiedProperties();

        //base.OnInspectorGUI();

    }

    void FindCondHolder()
    {
        if(SceneManager.GetActiveScene().name == "Level" && placesRoot == null)
            placesRoot = GameObject.Find("CondHolder").transform;
    }   

    void DrawConditions(SerializedProperty prop)
    {
        if (prop.isArray)
        {
            prop.arraySize = EditorGUILayout.IntField("Quantidade de Condições: ", prop.arraySize);
        }

        
        if (prop.arraySize > 0)
        {
            int numOfButtons = 1;
            int i = 0;
            
            foreach (SerializedProperty p in prop)
            {
                if (propertyToDraw == null)
                    propertyToDraw = p;

                if (numOfButtons == 1)
                    GUILayout.BeginHorizontal("Buttons " + i+1%3);

                GUIContent content = new GUIContent((i + 1).ToString());
                content.tooltip = p.FindPropertyRelative("descricao").stringValue;

                if (propertyToDraw.FindPropertyRelative("descricao").stringValue == p.FindPropertyRelative("descricao").stringValue)
                {
                    content.text = "Selcionada";
                    if (GUILayout.Button(content))
                    {

                    }
                }

                else
                {
                    if (GUILayout.Button(content))
                    {
                        propertyToDraw = p;
                    }
                }

                if (numOfButtons == 3 || i == prop.arraySize - 1)
                    GUILayout.EndHorizontal();

                numOfButtons = (numOfButtons == 3) ? 1 : numOfButtons + 1;
                i++;
         
            }

            EditorGUILayout.PropertyField(propertyToDraw);
        }

    }

    void ClearConditions()
    {
        foreach(Condicoes c in q.condicoes)
        {
            c.CleanUnsusedConditions();
        }
    }

    void ShowConditions(SerializedProperty prop, bool foldout = true)
    {
        if (prop.isArray)
        {
            prop.arraySize = EditorGUILayout.IntField("Quantidade de Condições: ", prop.arraySize);
        }

        foreach(SerializedProperty p in prop)
        {
            string descricao = p.FindPropertyRelative("descricao").stringValue;
            if (string.IsNullOrEmpty(descricao))
                descricao = "Condição: ";

            GUILayout.BeginHorizontal();
            p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, descricao);
            GUILayout.EndHorizontal();

            if (p.isExpanded)
            {
                EditorGUILayout.PropertyField(p.FindPropertyRelative("tipoCondicao"));
                Condicoes.TipoCondicao tipoCond = (Condicoes.TipoCondicao)p.FindPropertyRelative("tipoCondicao").enumValueIndex;


                EditorGUILayout.PropertyField(p.FindPropertyRelative("local"));
                EditorGUILayout.PropertyField(p.FindPropertyRelative("descricao"));

                if (tipoCond == Condicoes.TipoCondicao.COMBATE)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("inimigosDaCondicao"));
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("raioDeSpawn"));
                }

                bool onScene = false;
                if (tipoCond == Condicoes.TipoCondicao.INTERACAO || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM || tipoCond == Condicoes.TipoCondicao.FALA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("isOnScene"));
                    onScene = p.FindPropertyRelative("isOnScene").boolValue;

                    if (onScene)
                    {
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("nameOnScene"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("interagivelPrefab"));
                    }
                }

                if (tipoCond == Condicoes.TipoCondicao.PEGA_ITEM || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("itemDaCondicao"));
                }

                if (tipoCond == Condicoes.TipoCondicao.IDA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("distanciaChegada"));
                }

                if(tipoCond == Condicoes.TipoCondicao.FALA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("dialogoDaCondição"));
                }

                if (onScene)
                {
                    if (GUILayout.Button("Levar Condição até o interagível"))
                    {
                        ToSceneInteractableEditor(p);
                    }
                }

                else
                {
                    if (GUILayout.Button("Trazer Condição aqui"))
                    {
                        Camera cam = SceneView.lastActiveSceneView.camera;
                        Ray ray = cam.ScreenPointToRay(new Vector3(((float)(cam.pixelWidth-1)/2), ((float)(cam.pixelHeight-1)/2)));
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
                        {
                            p.FindPropertyRelative("local").vector3Value = hit.point;
                        }
                    }
                }
            }          
        }
    }

    public void DrawPlaces(Quest q)
    {
        UndrawPlaces();
        if (q.condicoes.Count > 0 && !isPlacesDrawn)
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
            isPlacesDrawn = true;
        }
    }

    void UndrawPlaces()
    {
        if (condHolders.Count > 0)
        {
            foreach(GameObject go in condHolders)
            {
                go.GetComponent<DrawQuesGizmo>().Clean();
                DestroyImmediate(go);
                
            }

            condHolders.Clear();
        }
    }

     void ToSceneInteractableEditor(SerializedProperty p)
    {
       p.FindPropertyRelative("interagivel").objectReferenceValue = GameObject.Find(p.FindPropertyRelative("nameOnScene").stringValue).GetComponent<Interagivel>();       
       p.FindPropertyRelative("local").vector3Value = p.FindPropertyRelative("interagivel").FindPropertyRelative("transform").FindPropertyRelative("position").vector3Value;
    }
}
