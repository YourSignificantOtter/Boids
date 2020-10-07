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
