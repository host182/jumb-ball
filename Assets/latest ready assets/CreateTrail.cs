using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The particle respawn time is constant, not the distance from the parent object
//The length of the particle system depends on the number of particles
//The thickness of the trail decays exponentially

public class CreateTrail : MonoBehaviour
{
    public int numberOfParticles;
    public float largestParticle;       //size of the largest particle, in units
    public float smallestParticle;      //size of the smallest particle, in units
    public float particleLifeTime;      //time from the spawn until it vanishes, in seconds
                                        //recommended to increase the number of particles when increasing the lifetime as the spawn rate is lifetime/number
    public GameObject trailDot;
    float residualTime;
    List<GameObject> trail = new List<GameObject>();
    List<Vector3> positions = new List<Vector3>();

    // Use this for initialization
    void Start()
    {
        residualTime = 0;
        trailDot.transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        float b = Mathf.Pow((largestParticle / smallestParticle), 0.125f);                         //b is the base of the exponensial eqn, smallest = largest * b ^ (-8)
        for (int i = 0; i < numberOfParticles; i++)
        {
            trail.Add(Instantiate(trailDot));
            positions.Add(new Vector3(transform.position.x, transform.position.y, 1));
            float scale = largestParticle * Mathf.Pow(b, -8 / (i + 1));
            trail[i].transform.localScale = new Vector3(scale, scale, scale);
        }
        Destroy(trailDot);
    }

    // Update is called once per frame
    void Update()
    {
        residualTime += Time.deltaTime;
        int newPositionCount = (int)(residualTime * numberOfParticles / particleLifeTime);           //the count increases by 1 every 1/particle number secs
        residualTime -= (float)newPositionCount * particleLifeTime / numberOfParticles;              //the used-up time is subtracted to get the residual time
        if (newPositionCount != 0)
        {
            UpdatePositions();
            for (int i = 0; i < newPositionCount; i++)
            {
                positions.RemoveAt(0);
                Vector3 tempPosition = positions[numberOfParticles - 2] + ((transform.position - positions[numberOfParticles - 2 - i]) / (newPositionCount - i));
                tempPosition += new Vector3(0, 0, 1);
                positions.Add(tempPosition);
            }
            for (int i = 0; i < numberOfParticles; i++)
                trail[i].transform.position = positions[i];
        }
    }

    void UpdatePositions()
    {
        for (int i = 0; i < numberOfParticles; i++)
            positions[i] -= new Vector3(0, 3 * Time.deltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "spikes")
            for (int i = 0; i < numberOfParticles; i++)
                Destroy(trail[i]);
    }
}