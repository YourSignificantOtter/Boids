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
        //The idea behind this is to create a raycast that looks forward in a cone
        //To raycast the cone we can simplify the math a little
        //A cone is an infinite number of circles of increasing radii stack on top of each other
        //To cast a line from the origin (the vertex of the cone) to a point on the last circle of the cone you 
        //have to cross through each preceding circle.
        //So we can ray cast to points on the "last" circle of the cone
        
        //Get the radius of the last cone
        float hypotonuse = data.viewDistance / Mathf.Cos(data.FOV / 2.0f);
        float radius = Mathf.Sqrt(hypotonuse * hypotonuse - data.viewDistance * data.viewDistance);
        float radiusSqr = radius * radius;
        
        //To get better coverage we will sample points within that entire circle rather than just on the perimiter
        //To do so we will imagine a shrinking circle starting at the max radius and slowly reducing down
        //At each iteration of the shrinking circle we will sample a number of points on that circles perimeter
        data.rayDirections = new Vector3[data.numRays];
        int circles = 5;
        int pointsPerCircle = data.numRays / circles;
        float goldenRatio = Mathf.Sqrt (5) / 2; //golden ratio without the leading 1

        //The golden ratio is useful to use as an irrational number for iteration.
        //This will help us reduce the amount of overlap and gaps in our ray casting

        int i = 0; //index into the data.rayDirections array

        float currRadius = radius;
        float currRadiusIt = currRadius / circles;
        float currRadiusSqr = currRadius * currRadius;

        Vector3 point = new Vector3();
        point.z = data.viewDistance;
        Vector3 dir = new Vector3();

        //Iterate over the number of "shrinking" circles
        for(int numCircles = circles; numCircles > 0; numCircles--)
        {
            float x = -1.0f * currRadius;
            for(int numPoints = pointsPerCircle / 2; numPoints > 0; numPoints--)
            {
                point.x = x;
                point.y = Mathf.Sqrt(Mathf.Abs(currRadiusSqr - x * x));
                dir = point - Vector3.zero;
                data.rayDirections[i++] = dir;
                point.y *= -1.0f;
                dir = point - Vector3.zero;
                data.rayDirections[i++] = dir;

                //When X >= +currRadius decrement by the irrational number
                //When X <= -currRadius increment by the irrational number
                x += goldenRatio;
                if(x >= currRadius)
                {
                    float temp = x % currRadius;
                    x = currRadius - temp;
                    goldenRatio *= -1.0f;
                }
                else if(x <= -currRadius)
                {
                    float temp = Mathf.Abs(x) % currRadius;
                    x = -currRadius + temp;
                    goldenRatio *= -1.0f;                    
                }
            }
            //Update the radius value for the circle shirnking
            currRadius -= currRadiusIt;
            currRadiusSqr = currRadius * currRadius;
            goldenRatio = Mathf.Abs(goldenRatio);
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
