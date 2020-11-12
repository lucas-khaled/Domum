using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class QuestEditorWindow : EditorWindow
{
    List<Quest> existingQuests = new List<Quest>();
    const string PATHBASE = "Assets/Quests/";

    Quest questAtual;
    QuestEditor questEditorAtual;

    Vector2 selectionScrollPosition = Vector2.zero;
    Vector2 infoScrollPosition = Vector2.zero;

    [MenuItem("Window/Domum/Quest Manager")]
    public static void ShowWindow()
    {
        GetWindow<QuestEditorWindow>("Manager de Quest");
    }

    private void LoadAllExistingQuests()
    {
        existingQuests.Clear();
        Quest[] quests = GetAtPath<Quest>("Quests/");
        existingQuests.AddRange(quests);
    }

    public void MakeANewQuest()
    {
        Quest newQuest = new Quest();
        newQuest.name = "Nova Quest " + (existingQuests.Count+1);
        newQuest.nome = newQuest.name;
        Condicoes c = new Condicoes();

        string path = PATHBASE + newQuest.name + ".asset";

        AssetDatabase.CreateAsset(newQuest, path);
        existingQuests.Add(newQuest);
    }

    public void RemoveQuest(Quest q)
    {
        string path = AssetDatabase.GetAssetPath(q);
        questAtual = null;

        AssetDatabase.DeleteAsset(path);
        existingQuests.Remove(q);
    }

    private void OnEnable()
    {
        LoadAllExistingQuests();
    }

    private void DrawQuestSelection()
    {
        GUILayout.BeginVertical();
        selectionScrollPosition = GUILayout.BeginScrollView(selectionScrollPosition);

        GUILayout.Label("Quests", VerticalAlligment());
        GUILayout.Space(10);

        foreach(Quest q in existingQuests)
        {
            if (GUILayout.Button(q.nome))
            {
                questAtual = q;
            }
            GUILayout.Space(3);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void DrawSelectedQuest()
    {
        GUILayout.BeginVertical();
        infoScrollPosition = GUILayout.BeginScrollView(infoScrollPosition);

        if (questEditorAtual == null || (Quest)questEditorAtual.target != questAtual)
        {
            Editor editor = Editor.CreateEditor(questAtual);
            questEditorAtual = (QuestEditor)editor;
        }

        //questEditor.DrawPlaces(questAtual);

        questEditorAtual.OnInspectorGUI();

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        DrawQuestSelection();

        if(questAtual != null)
        {
            DrawSelectedQuest();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        DrawOptionsButtons();
        GUILayout.EndVertical();
    }

    void DrawOptionsButtons()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Criar Quest"))
        {
            MakeANewQuest();
        }

        if(questAtual != null)
        {
            if(GUILayout.Button("Deletar Quest"))
            {
                RemoveQuest(questAtual);
            }
        }

        GUILayout.EndHorizontal();
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += fileName.Substring(index);

            if (!localPath.Contains(".meta"))
            {
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

    private GUIStyle VerticalAlligment()
    {
        GUIStyle vertical = new GUIStyle();
        vertical.alignment = TextAnchor.UpperCenter;

        return vertical;
    }
}
