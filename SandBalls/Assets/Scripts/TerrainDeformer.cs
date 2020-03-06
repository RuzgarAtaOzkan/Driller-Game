// TerrainDeformer - Demonstrating a method modifying terrain in real-time. Changing height and texture
//
// released under MIT License
// http://www.opensource.org/licenses/mit-license.php
//
//@author		Devin Reimer
//@website 		http://blog.almostlogical.com
//Copyright (c) 2010 Devin Reimer
/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainDeformer : MonoBehaviour
{
    public int terrainDeformationTextureNum = 1;
    private Terrain terr; // terrain to modify
    protected int hmWidth; // heightmap width
    protected int hmHeight; // heightmap height
    protected int alphaMapWidth;
    protected int alphaMapHeight;
    protected int numOfAlphaLayers;
    protected const float DEPTH_METER_CONVERT = 0.05f;
    protected const float TEXTURE_SIZE_MULTIPLIER = 1.25f;
    private float[,] heightMapBackup;
    private float[,,] alphaMapBackup;

    // todo my part
    [SerializeField] Transform driller;
    DrillerPathfinding[] drillerPathdings;

    List<int> xPositions = new List<int>();
    List<int> zPositions = new List<int>();

    [System.Obsolete]
    void Start()
    {
        terr = this.GetComponent<Terrain>();
        hmWidth = terr.terrainData.heightmapResolution;
        hmHeight = terr.terrainData.heightmapResolution;
        alphaMapWidth = terr.terrainData.alphamapWidth;
        alphaMapHeight = terr.terrainData.alphamapHeight;
        numOfAlphaLayers = terr.terrainData.alphamapLayers;
        if (Debug.isDebugBuild)
        {
            heightMapBackup = terr.terrainData.GetHeights(0, 0, hmWidth, hmHeight);
            alphaMapBackup = terr.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
        }
        ProcessDrillerBotCounter();
        ProcessTerrainNormalization();
    }

    //this has to be done because terrains for some reason or another terrains don't reset after you run the app
    void OnApplicationQuit()
    {
        if (Debug.isDebugBuild)
        {
            terr.terrainData.SetHeights(0, 0, heightMapBackup);
            terr.terrainData.SetAlphamaps(0, 0, alphaMapBackup);
        }
    }

    public float inds;
    public Transform go;

    private void Update()
    {
        DeformTerrain(driller.position, inds);
        ProcessDrillerBots();
    }

    //Driller bots management part >
    IEnumerator KeepTrackDrillerBotsInHierarchy()
    {
        while (true)
        {
            drillerPathdings = FindObjectsOfType<DrillerPathfinding>();
            yield return new WaitForSeconds(1f);
        }
    }

    private void ProcessDrillerBots()
    {
        foreach (DrillerPathfinding drillerPathfinding in drillerPathdings)
        {
            DeformTerrain(drillerPathfinding.transform.position, inds);
        }
    }

    private void ProcessDrillerBotCounter()
    {
        StartCoroutine(KeepTrackDrillerBotsInHierarchy());
    }
    //============================================

    //Terrain normalization part >
    IEnumerator NormalizeDrillerPositionsOnTerrain()
    {
        while (true)
        {
            NormalizeTerrain(driller.position, inds);
            for (int i = 0; i < xPositions.Count; i++)
            {
                xPositions.RemoveAt(i);
                zPositions.RemoveAt(i);
            }
            yield return new WaitForSeconds(15f);
        }
    }

    private void ProcessTerrainNormalization()
    {
        StartCoroutine(NormalizeDrillerPositionsOnTerrain());
    }

    protected void NormalizeTerrain(Vector3 pos, float craterSizeInMeters)
    {
        //get the heights only once keep it and reuse, precalculate as much as possible
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, terr, hmWidth, hmHeight); //terr.terrainData.heightmapResolution/terr.terrainData.heightmapWidth
        int heightMapCraterWidth = (int)(craterSizeInMeters * (hmWidth / terr.terrainData.size.x));
        int heightMapCraterLength = (int)(craterSizeInMeters * (hmHeight / terr.terrainData.size.z));
        int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2));

        float[,] heights = terr.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        float depthMultiplier;

        float deformationDepth = (craterSizeInMeters / 3.0f) / terr.terrainData.size.y;

        // we set each sample of the terrain in the size to the desired height
        for (int i = 0; i < heightMapCraterLength; i++) //width
        {
            for (int j = 0; j < heightMapCraterWidth; j++) //height
            {
                circlePosX = (j - (heightMapCraterWidth / 2)) / (hmWidth / terr.terrainData.size.x);
                circlePosY = (i - (heightMapCraterLength / 2)) / (hmHeight / terr.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                //convert back to values without skew

                if (distanceFromCenter < (craterSizeInMeters / 4.0f))
                {
                    depthMultiplier = ((craterSizeInMeters / 4.0f - distanceFromCenter) / (craterSizeInMeters / 2.0f));

                    depthMultiplier += 0.1f;
                    depthMultiplier += Random.value * .1f;

                    depthMultiplier = Mathf.Clamp(depthMultiplier, 0, 1);
                    heights[i, j] = 1;//Mathf.Clamp(heights[i, j] - deformationDepth * depthMultiplier, 0, 1);
                }

            }
        }

        for (int i = 0; i < xPositions.Count; i++)
        {
            terr.terrainData.SetHeights(xPositions[i], zPositions[i], heights);
        }
    }
    //=====================================================

    public void DestroyTerrain(Vector3 pos, float craterSizeInMeters)
    {
        DeformTerrain(pos, craterSizeInMeters);
        TextureDeformation(pos, craterSizeInMeters * 1.5f);
    }

    protected void DeformTerrain(Vector3 pos, float craterSizeInMeters)
    {
        //get the heights only once keep it and reuse, precalculate as much as possible
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, terr, hmWidth, hmHeight); //terr.terrainData.heightmapResolution/terr.terrainData.heightmapWidth
        int heightMapCraterWidth = (int)(craterSizeInMeters * (hmWidth / terr.terrainData.size.x));
        int heightMapCraterLength = (int)(craterSizeInMeters * (hmHeight / terr.terrainData.size.z));
        int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2));

        float[,] heights = terr.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        float depthMultiplier;

        float deformationDepth = (craterSizeInMeters / 3.0f) / terr.terrainData.size.y;

        // we set each sample of the terrain in the size to the desired height
        for (int i = 0; i < heightMapCraterLength; i++) //width
        {
            for (int j = 0; j < heightMapCraterWidth; j++) //height
            {
                circlePosX = (j - (heightMapCraterWidth / 2)) / (hmWidth / terr.terrainData.size.x);
                circlePosY = (i - (heightMapCraterLength / 2)) / (hmHeight / terr.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                //convert back to values without skew

                if (distanceFromCenter < (craterSizeInMeters / 4.0f))
                {
                    depthMultiplier = ((craterSizeInMeters / 4.0f - distanceFromCenter) / (craterSizeInMeters / 2.0f));

                    depthMultiplier += 0.1f;
                    depthMultiplier += Random.value * .1f;

                    depthMultiplier = Mathf.Clamp(depthMultiplier, 0, 1);
                    heights[i, j] = 0;//Mathf.Clamp(heights[i, j] - deformationDepth * depthMultiplier, 0, 1);
                }

            }
        }
        // set the new height
        terr.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
        xPositions.Add(heightMapStartPosX);
        zPositions.Add(heightMapStartPosZ);
    }

    protected void TextureDeformation(Vector3 pos, float craterSizeInMeters)
    {
        Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, terr, alphaMapWidth, alphaMapHeight);
        int alphaMapCraterWidth = (int)(craterSizeInMeters * (alphaMapWidth / terr.terrainData.size.x));
        int alphaMapCraterLength = (int)(craterSizeInMeters * (alphaMapHeight / terr.terrainData.size.z));

        int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
        int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));

        float[,,] alphas = terr.terrainData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);

        float circlePosX;
        float circlePosY;
        float distanceFromCenter;

        for (int i = 0; i < alphaMapCraterLength; i++) //width
        {
            for (int j = 0; j < alphaMapCraterWidth; j++) //height
            {
                circlePosX = (j - (alphaMapCraterWidth / 2)) / (alphaMapWidth / terr.terrainData.size.x);
                circlePosY = (i - (alphaMapCraterLength / 2)) / (alphaMapHeight / terr.terrainData.size.z);

                //convert back to values without skew
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));


                if (distanceFromCenter < (craterSizeInMeters / 2.0f))
                {
                    for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++)
                    {
                        //could add blending here in the future
                        if (layerCount == terrainDeformationTextureNum)
                        {
                            alphas[i, j, layerCount] = 1;
                        }
                        else
                        {
                            alphas[i, j, layerCount] = 0;
                        }
                    }
                }
            }
        }

        terr.terrainData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
    }

    protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        //code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;

        return coord;
    }

    protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        // get the position of the terrain heightmap where this game object is
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }
}
