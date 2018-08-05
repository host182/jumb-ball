using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//even if the life time of the path is too high, there wont be more than 2 paths

public class LineTrail : MonoBehaviour
{
    bool isStart;
    public GameObject trail;
    public float pathLifeTime;
    public float lineSize;
    public float scrollSpeed;
    public float distanceFromOriginToWallCollider;
    List<GameObject> fullTrail = new List<GameObject>();
    List<Vector2> directions = new List<Vector2>();

    // Use this for initialization
    void Start()
    {
        isStart = true;
        scrollSpeed = 3;
        trail.transform.position = transform.position;
        for (int i = 0; i < 2; i++)
            fullTrail.Add(Instantiate(trail));
        fullTrail[0].transform.localScale = new Vector3(0, 0, 0);
        Destroy(trail);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            ConfigureTrails();
        }
    }

    public void afterCollision(Vector2 direction)
    {
        directions.Add(direction);
        if (directions.Count > 2)
            directions.RemoveAt(0);
        isStart = false;
    }

    void ConfigureTrails()
    {
        Vector3 firstDirection = directions[directions.Count - 1];
        float firstX = transform.position.x - (scrollSpeed * pathLifeTime) / 2 * (firstDirection.x / firstDirection.y);
        float firstY = transform.position.y - (scrollSpeed * pathLifeTime) / 2;
        float zAngle = Mathf.Atan2(firstDirection.y, firstDirection.x);
        //should not forget to position the trail at z=1 so that it is shown behind the parent object
        fullTrail[1].transform.position = new Vector3(firstX, firstY, 1);
        fullTrail[1].transform.localScale = new Vector3((scrollSpeed * pathLifeTime) / Mathf.Sin(zAngle), lineSize);
        fullTrail[1].transform.eulerAngles = new Vector3(0, 0, zAngle * Mathf.Rad2Deg);
        
        if(directions.Count==2)
        {
            float heightAfterCollision = Mathf.Abs((distanceFromOriginToWallCollider + transform.position.x * Mathf.Sign(firstDirection.x)) * (firstDirection.y / firstDirection.x));
            float zAngle2 = Mathf.Atan2(directions[0].y, directions[0].x);
            float scale = scrollSpeed * pathLifeTime - heightAfterCollision;
            if (scale < 0)
                scale = 0;
            /*
            float secondY = transform.position.y - heightAfterCollision - scale / 2;
            float secondX = -Mathf.Sign(directions[1].x) * (distanceFromOriginToWallCollider - (scale / 2) * Mathf.Abs((directions[0].x / directions[0].y)));
            //always put z=1 for the same reason as above
            fullTrail[0].transform.position = new Vector3(secondX, secondY, 1);
            fullTrail[0].transform.localScale = new Vector3(scale / Mathf.Sin(zAngle2), lineSize);
            fullTrail[0].transform.eulerAngles = new Vector3(0, 0, zAngle2 * Mathf.Rad2Deg);
            */
            float secondY = transform.position.y - heightAfterCollision;
            float secondX = -Mathf.Sign(directions[1].x) * distanceFromOriginToWallCollider;
            fullTrail[0].transform.position = new Vector3(secondX, secondY, 1);
            fullTrail[0].transform.localScale = new Vector3(2 * scale / Mathf.Sin(zAngle2), lineSize);
            fullTrail[0].transform.eulerAngles = new Vector3(0, 0, zAngle2 * Mathf.Rad2Deg);
        }
    }

    public void DeleteTrail()
    {
        Destroy(fullTrail[0]);
        Destroy(fullTrail[1]);
    }
}
