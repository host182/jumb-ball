using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will only be attached to the primary tile

public class Tiles : MonoBehaviour
{

    public float spawnStartY;
    public float distanceFromOriginX;   //input a positive value
    public float lengthOfCamera;
    public float scrollSpeed;
    public GameObject tile;
    GameObject[] twoTiles = new GameObject[2];      //0 is in positive x axis and 1 is negative x axis
    bool[] makeCopies = new bool[2];
    /*//the following variables are for the particles
    public GameObject particle;
    public int numberOfParticles;
    public float particleLifeTime;
    public float initialRadiusOfParticleGroup;
    public float sizeOfParticles;
    public float forceOnParticles;*/

    // Use this for initialization
    void Start()
    {
        tile.transform.localScale = new Vector3(1, 1, 1);
        tile.transform.position = new Vector3(distanceFromOriginX, spawnStartY + 0.5f);
        twoTiles[0] = tile;
        twoTiles[1] = Instantiate(tile);
        twoTiles[1].transform.position = new Vector3(-distanceFromOriginX, tile.transform.position.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            makeCopies[i] = true;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2(twoTiles[i].transform.position.x, twoTiles[i].transform.position.y), new Vector2(1, 0.8f), 0);
            for (int j = 1; j < colliders.Length; j++)
            {
                if (colliders[j].gameObject.tag == "tiles")
                    makeCopies[i] = false;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (makeCopies[i])
            {
                int index = (int)(Random.value * 4) % 4;
                //setting up the clone properties
                GameObject cloneTile = Instantiate(twoTiles[i]);
                {
                    cloneTile.AddComponent<InstantiatedTiles>();
                    InstantiatedTiles cloneScript = cloneTile.GetComponent<InstantiatedTiles>();
                    cloneScript.index = index;
                    cloneScript.scrollSpeed = scrollSpeed;
                    cloneScript.lengthOfCamera = lengthOfCamera;
                    cloneScript.spawnStartY = spawnStartY;
                    cloneScript.distanceFromOriginX = distanceFromOriginX;
                }
            }
        }
    }
}
