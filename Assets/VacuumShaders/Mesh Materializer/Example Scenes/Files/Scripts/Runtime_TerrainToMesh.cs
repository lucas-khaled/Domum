﻿using UnityEngine;
using System.Collections;

//Mesh Materializer API is here ↓
using VacuumShaders.MeshMaterializer;


[AddComponentMenu("VacuumShaders/Mesh Materializer/Example/Runtime TerrainToMesh")]
public class Runtime_TerrainToMesh : MonoBehaviour 
{
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Variables                                                                 //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////


    public Terrain targetTerrain;

    public bool assignBasemap;

    public bool attachMeshCollider;
    //Terrain To Mesh options
    public MMData_TerrainToMesh terrainToMesh;
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Unity Functions                                                           //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    void Start () 
    {
        if (targetTerrain == null)
            return;


      
        //Will contain bake results 
        //Need - array - as materializing terrain returns mesh array depending on chunks count described in 'terrainToMesh'
        Mesh[] newMesh = null;

        //Will contain baking reports, will help if something goes wrong
        MMGenerator.CONVERTION_INFO[] convertionInfo;

        //Same as above but with more detail info
        string[] convertionInfoString;
        


        //Terrain Materializer        
        newMesh = MMGenerator.MaterializeTerrain(targetTerrain, out convertionInfo, out convertionInfoString, terrainToMesh);

        //Check reports
        if (convertionInfoString != null)
            for (int i = 0; i < convertionInfoString.Length; i++)
            {
                Debug.LogWarning(convertionInfoString[i]);
            }


        //Successful conversation
        if (newMesh != null)
        {
            //Setup TerrainToMesh material
            Material newMaterial = null;
            if (assignBasemap)
                newMaterial = SetupTerrainToMeshMaterial_Basemap();
            else
                newMaterial = SetupTerrainToMeshMaterial_Splatmap();



            for (int i = 0; i < newMesh.Length; i++)
            {
                //Create new gameobject for each chunk
                GameObject chunk = new GameObject(newMesh[i].name);
                chunk.AddComponent<MeshFilter>().sharedMesh = newMesh[i];
                chunk.AddComponent<MeshRenderer>().sharedMaterial = newMaterial;

                
                //Move to parent
                chunk.transform.parent = this.gameObject.transform;
                chunk.transform.localPosition = Vector3.zero;

                if(attachMeshCollider)
                    chunk.AddComponent<MeshCollider>().sharedMesh = newMesh[i];
            }
        }
	}

    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Custom Functions                                                          //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    Material SetupTerrainToMeshMaterial_Splatmap()
    {
        Material newMaterial = null;


        //Export terrain splatmaps
        Texture2D[] splatMap = MMGenerator.ExtractTerrainSplatmaps(targetTerrain);
        if (splatMap == null || splatMap.Length == 0)
            return newMaterial;

        
        //Export diffuse/normal textures
        Texture2D[] diffuseTextures;
        Texture2D[] normalTextures;
        Vector2[] uvScale;
        Vector2[] uvOffset;
        float[] metalic;
        float[] smoothness;

        int usedTexturesCount = MMGenerator.ExtractTerrainTexturesInfo(targetTerrain, out diffuseTextures, out normalTextures, out uvScale, out uvOffset, out metalic, out smoothness);
        if (usedTexturesCount == 0 || diffuseTextures == null)
        {
            //Problems with terrain
            Debug.LogWarning("usedTexturesCount == 0");
           
            return newMaterial;
        }
        else if (usedTexturesCount == 1)
        {
            //There is no need to use TerrainToMesh shaders with one texture
            Shader shader = null;
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                shader = Shader.Find("Lightweight Render Pipeline/Lit");
            else
                shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader != null)
            {
                newMaterial = new Material(shader);

                //Texture
                newMaterial.mainTexture = diffuseTextures[0];

                //Scale & Offset
                newMaterial.mainTextureScale = uvScale[0];
                newMaterial.mainTextureOffset = uvOffset[0];
            }

            return newMaterial;
        }


        //Terrain To Mesh shaders support max 8 textures blend
        usedTexturesCount = Mathf.Clamp(usedTexturesCount, 2, 8);


        //Select proper shader - One Directional Light
        string shaderName = string.Empty;
        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            shaderName = string.Format("VacuumShaders/Terrain To Mesh/Lightweight Render Pipeline/Lit" + "/{0} Textures", usedTexturesCount);
        else
        {
            if (usedTexturesCount < 5)
                shaderName = string.Format("VacuumShaders/Terrain To Mesh/One Directional Light/Bumped/{0} Textures", usedTexturesCount);
            else
                shaderName = string.Format("VacuumShaders/Terrain To Mesh/One Directional Light/Diffuse/{0} Textures", usedTexturesCount);
        }

        Shader ttmShader = Shader.Find(shaderName);
        if (ttmShader == null)
        {
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                Debug.LogWarning("Shader not found: '" + shaderName + "'.\nLightweight Render Pipeline shaders (http://u3d.as/1jFw) are not installed.\n");
            else
                Debug.LogWarning("Shader not found: " + shaderName);

            return newMaterial;
        }


        //Select shader
        newMaterial = new Material(ttmShader);

        //Set up controll textures
        if (splatMap.Length == 1)
        {
            newMaterial.SetTexture("_V_T2M_Control", splatMap[0]);
        }
        else
        {
            if (splatMap.Length > 2)
                Debug.Log("TerrainToMesh shaders support max 2 control textures. Current terrain uses " + splatMap.Length);

            newMaterial.SetTexture("_V_T2M_Control", splatMap[0]);
            newMaterial.SetTexture("_V_T2M_Control2", splatMap[1]);
        }


        //Assign textures
        for (int i = 0; i < usedTexturesCount; i++)
        {
            //Texture
            newMaterial.SetTexture(string.Format("_V_T2M_Splat{0}", i + 1), diffuseTextures[i]);

            //Scale
            newMaterial.SetFloat(string.Format("_V_T2M_Splat{0}_uvScale", i + 1), uvScale[i].x);

        }

        //Metalic & Gloss
        for (int i = 0; i < metalic.Length; i++)
        {
            newMaterial.SetFloat(string.Format("_V_T2M_Splat{0}_Metallic", i + 1), metalic[i]);
            newMaterial.SetFloat(string.Format("_V_T2M_Splat{0}_Glossiness", i + 1), smoothness[i]);
        }


        //Bumpmap
        for (int i = 0; i < normalTextures.Length; i++)
        {
            string propName = string.Format("_V_T2M_Splat{0}_bumpMap", i + 1);

            if(newMaterial.HasProperty(propName))
                newMaterial.SetTexture(propName, normalTextures[i]);
        }


        return newMaterial;
    }

    Material SetupTerrainToMeshMaterial_Basemap()
    {
        //Export basemaps (diffuse and normal)
        Texture2D basemapDiffuse = null;
        Texture2D basemapNormal = null;

        MMGenerator.ExtractTerrainBasemap(targetTerrain, out basemapDiffuse, out basemapNormal, 1024, 1024);


        Material newMaterial = null;

        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
        {
            Shader srpShader = Shader.Find("Lightweight Render Pipeline/Lit");
            if (srpShader == null)
                srpShader = Shader.Find("VacuumShaders/Terrain To Mesh/SRP Default");

            newMaterial = new Material(srpShader);
        }

        else
            newMaterial = new Material(Shader.Find(basemapNormal != null ? "Legacy Shaders/Bumped Diffuse" : "Legacy Shaders/Diffuse"));


        if (newMaterial.HasProperty("_MainTex"))
            newMaterial.SetTexture("_MainTex", basemapDiffuse);
        if (newMaterial.HasProperty("_BaseMap"))
            newMaterial.SetTexture("_BaseMap", basemapDiffuse);

        if (basemapNormal != null)
        {
            if (newMaterial.HasProperty("_BumpMap"))
                newMaterial.SetTexture("_BumpMap", basemapNormal);
        }

        if (newMaterial.HasProperty("_BaseColor"))
            newMaterial.SetColor("_BaseColor", Color.white);
        if (newMaterial.HasProperty("_Color"))
            newMaterial.SetColor("_Color", Color.white);

        if (newMaterial.HasProperty("_Smoothness"))
            newMaterial.SetFloat("_Smoothness", 1);

        return newMaterial;

    }
}
