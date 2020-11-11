using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GetLODEditor : EditorWindow
{

    private GameObject destinyObj;
    private int numeroLOD;

    [MenuItem("Window/Auxiliar MeshCombiners")]
    public static void ShowWindow()
    {
        GetWindow<GetLODEditor>("Auxiliar de Mesh Combines");
    }

    private void OnGUI()
    {
        GUILayout.Label("Origem:");
        destinyObj = (GameObject)EditorGUILayout.ObjectField(destinyObj, typeof(GameObject));
        GUILayout.Space(5);
        numeroLOD = EditorGUILayout.IntField("LOD Index", numeroLOD);

        if(Selection.activeGameObject != null)
        {
            GUILayout.Space(30);
            if (GUILayout.Button("Ativar todos os renders de: " + Selection.activeObject.name))
            {
                AtivarRenders(Selection.activeTransform, true);
            }
            if (GUILayout.Button("Desativar todos os renders de: " + Selection.activeObject.name))
            {
                AtivarRenders(Selection.activeTransform, false);
            }

            GUILayout.Space(10);
            GUILayout.Label("Qnt de filhos de : " + Selection.activeGameObject.name + " = " + Selection.activeTransform.childCount);

            if (destinyObj == null)
            {
                GUILayout.Space(10);
                GUILayout.Label("Escolha uma Origem");
                return;
            }

            GUILayout.Space(30);

            GUILayout.Label("Objeto Ativo: " + Selection.activeGameObject.name);

            GUILayout.Space(30);


            if(GUILayout.Button("Adicionar LOD's à origem"))
            {
                AdicionarAOrigem(destinyObj.transform,Selection.activeTransform, this.numeroLOD);
            }

            GUILayout.Space(20);

            if(GUILayout.Button("Fazer Mesh Combines com LOD"))
            {
                PrepararArvores(Selection.activeTransform);
            }
        }

        if (Selection.gameObjects.Length > 0)
        {
            if(destinyObj == null)
            {
                if (destinyObj == null)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Escolha uma Origem");
                    return;
                }
            }

            GUILayout.Space(30);

            if(GUILayout.Button("Parentear seleções à origem"))
            {
                ParentearAOrigem(Selection.gameObjects);
                Selection.activeObject = destinyObj;
            }

            GUILayout.Label("Qnt Selecionados: " + Selection.gameObjects.Length);
        }
    }

    void PrepararArvores(Transform selecionado)
    {
        for(int i = 0; i<selecionado.childCount; i++)
        {
            GameObject obj = new GameObject(selecionado.GetChild(i).name + "_Combined");
            obj.AddComponent<LODGroup>();
            obj.transform.SetParent(destinyObj.transform);

            GameObject lod0 = new GameObject("LOD0");
            GameObject lod1 = new GameObject("LOD1");
            GameObject lod2 = new GameObject("LOD2");

            lod0.AddComponent<MeshCombiner>().DestroyCombinedChildren = true;
            lod1.AddComponent<MeshCombiner>().DestroyCombinedChildren = true;
            lod2.AddComponent<MeshCombiner>().DestroyCombinedChildren = true;

            lod0.GetComponent<MeshCombiner>().FolderPath = "Prefabs/MeshCombine/" + selecionado.name + "/" + selecionado.GetChild(i).name;
            lod1.GetComponent<MeshCombiner>().FolderPath = "Prefabs/MeshCombine/" + selecionado.name + "/" + selecionado.GetChild(i).name;
            lod2.GetComponent<MeshCombiner>().FolderPath = "Prefabs/MeshCombine/" + selecionado.name + "/" + selecionado.GetChild(i).name;

            lod0.transform.SetParent(obj.transform);
            lod1.transform.SetParent(obj.transform);
            lod2.transform.SetParent(obj.transform);

            AdicionarAOrigem(lod0.transform, selecionado.GetChild(i), 0);
            AdicionarAOrigem(lod1.transform, selecionado.GetChild(i), 1);
            AdicionarAOrigem(lod2.transform, selecionado.GetChild(i), 2);

            lod0.GetComponent<MeshCombiner>().CombineMeshes(true);
            lod1.GetComponent<MeshCombiner>().CombineMeshes(true);
            lod2.GetComponent<MeshCombiner>().CombineMeshes(true);

            obj.GetComponent<LODGroup>().SetLODs(
                new LOD[]
                {
                    new LOD(0.6f, new Renderer[] { lod0.GetComponent<MeshRenderer>() }),
                    new LOD(0.3f, new Renderer[] { lod1.GetComponent<MeshRenderer>() }),
                    new LOD(0.1f, new Renderer[] { lod2.GetComponent<MeshRenderer>() })
                }
            );
        }
    }

    void AtivarRenders(Transform selecionado, bool liga)
    {
        Renderer renderer = selecionado.GetComponent<Renderer>();
        if(renderer != null)
        {
            renderer.enabled = liga;
        }

        if (selecionado.childCount > 0)
        {
            for(int i = 0; i<selecionado.childCount; i++)
            {
                AtivarRenders(selecionado.GetChild(i), liga);
            }
        }
    }

    void ParentearAOrigem(GameObject[] objs)
    {
        foreach(GameObject go in objs)
        {
            go.transform.SetParent(destinyObj.transform, true);
        }
    }

    private void AdicionarAOrigem(Transform origem, Transform selected, int numeroLOD)
    {
        LODGroup paiLod = selected.GetComponent<LODGroup>();
        if(paiLod != null)
        {
            if(paiLod.lodCount > numeroLOD)
            {
                LOD lod  = paiLod.GetLODs()[numeroLOD];

                foreach(Renderer r in lod.renderers)
                {
                    r.enabled = true;

                    Vector3 originPosition = r.gameObject.transform.position;
                    Quaternion originRotation = r.gameObject.transform.rotation;
                    Vector3 originScale = r.gameObject.transform.lossyScale;

                    GameObject obj = Instantiate(r.gameObject);
                    obj.transform.SetParent(origem.transform, true);

                    obj.transform.position = originPosition;
                    obj.transform.rotation = originRotation;
                    obj.transform.localScale = originScale;

                    r.enabled = false;
                }
            }
            return;
        }

        Renderer renderer = selected.GetComponent<Renderer>();
        if(renderer != null && numeroLOD == 0)
        {
            renderer.enabled = true;

            Vector3 originPosition = renderer.gameObject.transform.position;
            Quaternion originRotation = renderer.gameObject.transform.rotation;
            Vector3 originScale = renderer.gameObject.transform.lossyScale;

            GameObject obj = Instantiate(renderer.gameObject);
            obj.transform.SetParent(origem.transform);

            obj.transform.position = originPosition;
            obj.transform.rotation = originRotation;
            obj.transform.localScale = originScale;
            renderer.enabled = false;
        }

        if (selected.childCount > 0)
        {
            for (int i = 0; i < selected.childCount; i++)
            {
                AdicionarAOrigem(origem,selected.GetChild(i), numeroLOD);
            }
        }
    }

}
