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
    DrawQuesGizmo condHolder;
    Transform placesRoot;
    SerializedObject obj;
    SerializedProperty propertyToDraw = null;
    Condicoes selectedCondition = null;

    public bool isPlacesDrawn { get; private set; }

    private void OnEnable()
    {
        placesRoot = GameObject.Find("CondHolder").transform;
        obj = new SerializedObject(target);
        q = (Quest)target;
        
        isPlacesDrawn = false;
    }

    private void OnDisable()
    {
        UndrawPlaces();
    }

    void ChangeName(Quest quest, string newName)
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(quest), newName);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(obj.FindProperty("nome"));
        q.name = q.nome;

        if(GUILayout.Button("Apply Name"))
        {
            ChangeName(q, q.nome);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.PropertyField(obj.FindProperty("principal"));

        GUILayout.Space(5);
        EditorGUILayout.PropertyField(obj.FindProperty("isQuestAdditioner"));

        if (obj.FindProperty("isQuestAdditioner").boolValue)
        {
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(obj.FindProperty("questToAdd"));
            EditorGUILayout.PropertyField(obj.FindProperty("questGiverName"));
            

            /*if(obj.FindProperty("questToAdd").objectReferenceValue.name == q.name)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("A quest Que você vai adicionar é a mesma quest que essa. Tem certeza disso?", MessageType.Warning);
            }*/
            GUILayout.Space(10);
        }

        EditorGUILayout.PropertyField(obj.FindProperty("reward"));

        EditorGUILayout.PropertyField(obj.FindProperty("dialogo"));

        //ShowConditions(obj.FindProperty("condicoes"));
        DrawConditions(obj.FindProperty("condicoes"));

        if(selectedCondition != null)
            DrawActiveConditionPlace();

        obj.ApplyModifiedProperties();

    }

    void FindCondHolder()
    {
        if(SceneManager.GetActiveScene().name == "Level" && placesRoot == null)
            placesRoot = GameObject.Find("CondHolder").transform;
    }   

    void DrawConditions(SerializedProperty prop)
    {

        GUILayout.Space(20);
        var rect = EditorGUILayout.BeginVertical(); 

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
                    content.text = "Selcionada";
                
                if (GUILayout.Button(content))
                {
                    propertyToDraw = p;
                    selectedCondition = q.condicoes[i];
                }

                if (numOfButtons == 3 || i == prop.arraySize - 1)
                    GUILayout.EndHorizontal();

                numOfButtons = (numOfButtons == 3) ? 1 : numOfButtons + 1;
                i++;
         
            }

            EditorGUILayout.PropertyField(propertyToDraw);

            GUILayout.Space(10);

            if (GUILayout.Button("Trazer Condição aqui"))
            {
                Camera cam = SceneView.lastActiveSceneView.camera;
                Ray ray = cam.ScreenPointToRay(new Vector3(((float)(cam.pixelWidth - 1) / 2), ((float)(cam.pixelHeight - 1) / 2)));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
                {
                    propertyToDraw.FindPropertyRelative("local").vector3Value = hit.point;
                }
            }

            EditorGUILayout.EndVertical();

            Color c = Color.black;
            c.a = 0.1f;
            EditorGUI.DrawRect(new Rect(rect.x, rect.y-10, rect.width, rect.height+20), c);
        }

    }

    void ClearConditions()
    {
        foreach(Condicoes c in q.condicoes)
        {
            c.CleanUnsusedConditions();
        }
    }


    public void DrawActiveConditionPlace()
    {
        if (condHolder == null || condHolder.condHolder.descricao != selectedCondition.descricao)
        {
            UndrawPlaces();

            GameObject condicaoHolder = new GameObject(selectedCondition.descricao, typeof(DrawQuesGizmo));
            condicaoHolder.transform.SetParent(placesRoot);

            condicaoHolder.GetComponent<DrawQuesGizmo>().SetCondicaoOnHolder(selectedCondition);

            condHolder = condicaoHolder.GetComponent<DrawQuesGizmo>();
        }
    }

    /*public void DrawAllConditionPlaces(Quest q)
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
    }*/

    void UndrawPlaces()
    {
        if(placesRoot.childCount>0)
            DestroyImmediate(placesRoot.GetChild(0).gameObject);
        condHolder = null;
    }

    void ToSceneInteractableEditor(SerializedProperty p)
    {
       p.FindPropertyRelative("interagivel").objectReferenceValue = GameObject.Find(p.FindPropertyRelative("nameOnScene").stringValue).GetComponent<Interagivel>();       
       p.FindPropertyRelative("local").vector3Value = p.FindPropertyRelative("interagivel").FindPropertyRelative("transform").FindPropertyRelative("position").vector3Value;
    }

    //jeito antigo (sem o property drawer) de desenhar as condições
    /*void ShowConditions(SerializedProperty prop, bool foldout = true)
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
    }*/
}
