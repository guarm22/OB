using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TerrainScript : MonoBehaviour {
    public Terrain terrain;

    public TerrainData terrainData;

    void Awake() {
        Instantiate(terrain, transform);
        terrain.terrainData = terrainData;
    }

    void Start() {
        //terrain.terrainData = Resources.Load<TerrainData>("Terrain/tutorial_terrain.asset");
        terrainData.RefreshPrototypes();
        terrain.Flush();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
