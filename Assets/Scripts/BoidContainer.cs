using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidContainer : MonoBehaviour
{

    public BoidData data;
    public int numBoids;
    public float bound;
    private int currNumBoids;

    private List<GameObject> BoidList;


    public void Awake()
    {
        BoidList = new List<GameObject>();
        CalculateBoidRayDirections();
    }

    public void CalculateBoidRayDirections()
    {
        //https://github.com/SebLague/Boids/blob/master/Assets/Scripts/BoidHelper.cs
        int numRays = data.numRays;
        data.rayDirections = new Vector3[numRays];

        float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for(int i = 0; i < numRays; i++)
        {
            float t = (float) i / numRays;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            data.rayDirections[i] = new Vector3 (x, y, z);
        }
    }

    public void Start()
    {
        currNumBoids = 0;
        data.bound = bound;
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        bc.size = new Vector3(bound, bound, bound);
        bc.center = transform.position;
        spawnBoids();
    }

    public void Update()
    {
        if(currNumBoids != numBoids)
        {
            if(currNumBoids > numBoids)
            {
                removeBoids();
            }
            else
            {
                spawnBoids();
            }
        }
    }

    private void removeBoids()
    {
        //Remove the last n boids
        //where n = currNumBoids - numBoids;
        int n = currNumBoids - numBoids;

        for(; n > 0; n--)
        {   
            GameObject b = BoidList[BoidList.Count - 1];
            BoidList.RemoveAt(BoidList.Count - 1);
            Destroy(b);
        }
        currNumBoids = numBoids;
    }

    private void spawnBoids()
    {
        //Generate n new boids
        //where n = numBoids - currNumBoids
        //Add newly generated boids into the BoidList
        int n = numBoids - currNumBoids;
        for(; n > 0; n--)
        {
            GameObject newBoid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newBoid.name = "Boid_" + BoidList.Count;
            newBoid.AddComponent<Boid>();
            newBoid.GetComponent<Boid>().SetBoidData(data);
            newBoid.GetComponent<Boid>().Spawn();
            if(newBoid != null)
            {
                BoidList.Add(newBoid);
            }
        }
        currNumBoids = numBoids;
    }
}
