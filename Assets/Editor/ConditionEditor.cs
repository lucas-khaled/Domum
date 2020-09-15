using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Condicoes))]
public class ConditionEditor : PropertyDrawer
{

    float arrayLines;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string desc = property.FindPropertyRelative("descricao").stringValue;
        string labelzinha = (desc.Replace(" ", string.Empty) == string.Empty) ? "Nova Condicao" : desc;

        EditorGUI.BeginProperty(position, label, property);
        GUI.backgroundColor = Color.yellow;

        //Rect labelsRect = new Rect(position.x - 100, position.y + 40, position.width, position.height);
        Rect valuesRect = new Rect(position.x, position.y + 25, position.width, 20);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(labelzinha));


        EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("descricao"));

        valuesRect.y += 45;
        EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("local"));

        valuesRect.y += 45;
        EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("tipoCondicao"));



        Condicoes.TipoCondicao qualCondicao = (Condicoes.TipoCondicao)property.FindPropertyRelative("tipoCondicao").enumValueIndex;

        if(qualCondicao == Condicoes.TipoCondicao.COMBATE)
        {
            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("inimigosDaCondicao"));
            arrayLines = DrawArrayValues(ref valuesRect, property.FindPropertyRelative("inimigosDaCondicao"), "Inimigo ");

            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("raioDeSpawn"));
        }

        if(qualCondicao == Condicoes.TipoCondicao.INTERACAO || qualCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM || qualCondicao == Condicoes.TipoCondicao.FALA)
        {
            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("isOnScene"));
            bool isOnScene = property.FindPropertyRelative("isOnScene").boolValue;

            valuesRect.y += 25;
            if (isOnScene)
                EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("nameOnScene"));
            else
                EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("interagivelPrefab"));
        }

        if(qualCondicao == Condicoes.TipoCondicao.PEGA_ITEM || qualCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM)
        {
            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("itemDaCondicao"));
        }

        if (qualCondicao == Condicoes.TipoCondicao.IDA)
        {
            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect,property.FindPropertyRelative("distanciaChegada"));
        }

        if (qualCondicao == Condicoes.TipoCondicao.FALA || qualCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM)
        {
            valuesRect.y += 25;
            EditorGUI.PropertyField(valuesRect, property.FindPropertyRelative("dialogoDaCondicao"));
            arrayLines = DrawDialog(ref valuesRect, property.FindPropertyRelative("dialogoDaCondicao"));
        }

        EditorGUI.EndProperty();
    }

    float DrawDialog(ref Rect valueRect, SerializedProperty dialogProperty)
    {
        float lines = 0;
        if (dialogProperty.isExpanded)
            lines = DrawArrayValues(ref valueRect, dialogProperty.FindPropertyRelative("dialogueLines"), "Fala ", false);
        
        return lines;
    }

    private float DrawArrayValues(ref Rect valueRect, SerializedProperty arrayProperty, string label = "Element", bool checkExpanded = true)
    {
        float initialValue = valueRect.y;

        if (arrayProperty.isExpanded || !checkExpanded) {
            valueRect.y += 25;
            valueRect.x += 10;
            valueRect.width -= 10;

            arrayProperty.arraySize = EditorGUI.IntField(valueRect, new GUIContent("Size"),arrayProperty.arraySize);

            if (arrayProperty.arraySize > 0) {

                valueRect.x += 20;
                valueRect.width -= 20;

                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    valueRect.y += 25;
                    EditorGUI.PropertyField(valueRect, arrayProperty.GetArrayElementAtIndex(i), new GUIContent(label+i));
                }

                valueRect.width += 20;
                valueRect.x -= 20;
            }

            valueRect.x -= 10;
            valueRect.width += 10;

        }

        return valueRect.y - initialValue;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Condicoes.TipoCondicao condition = (Condicoes.TipoCondicao)property.FindPropertyRelative("tipoCondicao").enumValueIndex;

        int valor = 0;

        switch (condition)
        {
            case Condicoes.TipoCondicao.COMBATE:
                valor = 50;
                break;
            case Condicoes.TipoCondicao.DEVOLVE_ITEM:
                valor = 100;
                break;
            case Condicoes.TipoCondicao.INTERACAO:
                valor = 50;
                break;
            case Condicoes.TipoCondicao.FALA:
                valor = 75;
                break;
            case Condicoes.TipoCondicao.PEGA_ITEM:
                valor = 25;
                break;
            case Condicoes.TipoCondicao.IDA:
                valor = 25;
                break;
        }

        return base.GetPropertyHeight(property, label) + 115 + valor + arrayLines;
    }

    /*public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        container.Add(new PropertyField(property.FindPropertyRelative("descricao")));
        container.Add(new PropertyField(property.FindPropertyRelative("local")));
        container.Add(new PropertyField(property.FindPropertyRelative("tipoCondicao")));

        var qualCondicao = (Condicoes.TipoCondicao) property.FindPropertyRelative("tipoCondicao").enumValueIndex;

        if(qualCondicao == Condicoes.TipoCondicao.COMBATE)
        {
            container.Add(new PropertyField(property.FindPropertyRelative("inimigosDaCondicao")));
            container.Add(new PropertyField(property.FindPropertyRelative("raioDeSpawn")));
        }

        bool onScene;

        if(qualCondicao == Condicoes.TipoCondicao.INTERACAO || qualCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM || qualCondicao == Condicoes.TipoCondicao.FALA)
        {
            container.Add(new PropertyField(property.FindPropertyRelative("isOnScene")));
            onScene = property.FindPropertyRelative("isOnScene").boolValue;

            if (onScene)
                container.Add(new PropertyField(property.FindPropertyRelative("nameOnScene")));
            else
                container.Add(new PropertyField(property.FindPropertyRelative("interagivelPrefab")));
        }

        if (qualCondicao == Condicoes.TipoCondicao.PEGA_ITEM || qualCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM)
        {
            container.Add (new PropertyField(property.FindPropertyRelative("itemDaCondicao")));
        }

        if (qualCondicao == Condicoes.TipoCondicao.IDA)
        {
            container.Add(new PropertyField(property.FindPropertyRelative("distanciaChegada")));
        }

        if (qualCondicao == Condicoes.TipoCondicao.FALA)
        {
            container.Add(new PropertyField(property.FindPropertyRelative("dialogoDaCondição")));
        }

        container.styleSheets.Add(Resources.Load<StyleSheet>("BuildingEditor"));
        return container;
    }*/


    /*
        if (onScene)
        {
            if (GUILayout.Button("Levar Condição até o interagível"))
            {
                ToSceneInteractableEditor(obj);
            }
        }

        else
        {
            if (GUILayout.Button("Trazer Condição aqui"))
            {
                Camera cam = SceneView.lastActiveSceneView.camera;
                Ray ray = cam.ScreenPointToRay(new Vector3(((float)(cam.pixelWidth - 1) / 2), ((float)(cam.pixelHeight - 1) / 2)));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
                {
                    obj.FindProperty("local").vector3Value = hit.point;
                }
            }
        }

        obj.ApplyModifiedProperties();
    }

    void ToSceneInteractableEditor(SerializedObject obj)
    {
        obj.FindProperty("interagivel").objectReferenceValue = GameObject.Find(obj.FindProperty("nameOnScene").stringValue).GetComponent<Interagivel>();
        obj.FindProperty("local").vector3Value = obj.FindProperty("interagivel").FindPropertyRelative("transform").FindPropertyRelative("position").vector3Value;
    }*/
}
