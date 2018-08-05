using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedTiles : MonoBehaviour
{

    public int score = new int();
    public float length = new float();
    public int index = new int();
    public float scrollSpeed = new float();
    public float lengthOfCamera = new float();
    public float spawnStartY = new float();
    public float distanceFromOriginX = new float();
    Color[] availableColors = new Color[4];
    int[] tileScores = new int[4];
    float[] tileLength = new float[4];

    // Use this for initialization
    void Start()
    {
        //setting up the colors
        availableColors[0] = Color.red;
        availableColors[1] = Color.green;
        availableColors[2] = Color.blue;
        availableColors[3] = Color.black;
        //setting up the scores
        tileScores[0] = 50;
        tileScores[1] = 150;
        tileScores[2] = 300;
        tileScores[3] = 0;          //if the returning score is 0, player dies
        //setting up the length
        tileLength[0] = 6;
        tileLength[1] = 2;
        tileLength[2] = 1;
        tileLength[3] = 1;

        score = tileScores[index];
        length = tileLength[index];
        gameObject.GetComponent<SpriteRenderer>().color = availableColors[index];
        gameObject.transform.position = new Vector3(transform.position.x, spawnStartY + length / 2);
        gameObject.transform.localScale = new Vector3(1, length, 1);
        Destroy(gameObject, (spawnStartY - Camera.main.transform.position.y + length + lengthOfCamera / 2) / scrollSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position -= new Vector3(0, scrollSpeed * Time.deltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Color objectColor = gameObject.GetComponent<SpriteRenderer>().color;
        if (collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0.5f);
        }
    }

    public void RevertCollision()
    {
        Color objectColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1);
    }
}
