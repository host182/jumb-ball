using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//it used to be called WallParticles

public class BallParticles : MonoBehaviour {

    public GameObject particle;
    public int numberOfParticles;
    public float particleLifeTime;
    public float initialRadiusOfParticleGroup;
    public float sizeOfParticles;
    public float forceOnParticles;
    List<GameObject> wallParticleSystem;

    // Use this for initialization
    void Start () {
        Debug.Log("particle position - " + particle.transform.position);
        Debug.Log("particle color - " + particle.GetComponent<SpriteRenderer>().color);
        float angles = (360 / numberOfParticles) * Mathf.Deg2Rad;
        particle.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        for (int i = 0; i < numberOfParticles; i++)
        {
            wallParticleSystem.Add(Instantiate(particle));
            Debug.Log("wall particle position - " + wallParticleSystem[i].transform.position);
            particle.transform.parent = wallParticleSystem[i].transform;
            wallParticleSystem[i].transform.localPosition = new Vector3(initialRadiusOfParticleGroup * Mathf.Cos(i * angles), initialRadiusOfParticleGroup * Mathf.Sin(i * angles));
        }
        particle.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void triggerParticles(Vector2 collisionPoint)
    {
        Destroy(particle, particleLifeTime);
        particle.transform.position = new Vector3(collisionPoint.x, collisionPoint.y);
        for (int i = 0; i < numberOfParticles; i++)
        {
            Vector2 forceVector = wallParticleSystem[i].transform.localPosition * forceOnParticles;
            wallParticleSystem[i].AddComponent<Rigidbody2D>();
            Rigidbody2D rgd = wallParticleSystem[i].GetComponent<Rigidbody2D>();
            rgd.AddForce(forceVector);
        }
    }
}
