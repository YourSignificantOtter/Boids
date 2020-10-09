using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidData _bd;
    private Rigidbody _rb;
    private float _bound;

    private List<RaycastHit> hits;

    public void Awake()
    {
        gameObject.tag = "Boid";
        //Set the collider to use a trigger
        Collider c = gameObject.GetComponent<Collider>();
        c.isTrigger = true;
        //Create and add the rigidbody component
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.mass = Random.Range(2.0f, 5.0f);

        hits = new List<RaycastHit>();
    }

    public void FixedUpdate()
    {
        //To perform the checks for all the following things we need to do a raycast from the front of the boid
        //That ray cast needs to cover our FOV and our distance data. Essentially we want to ray cast a conical shape
        //Sadly we have to use math to figure out how to do that
        
        //Begin by raycasting the cone and getting a list of all hitInfos;
        RaycastAll();

        //Check if we are going to collide with a non-boid object and avoid it
        CheckCollisions();

        //Perform seperation

        //Perform alignment

        //Perform cohesion
    }

    //Visualize the rays that are being cast from this Boid if it is selected
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Ray ray = new Ray();
        ray.origin = transform.position;
        
        for(int i = 0; i < _bd.numRays; i++)
        {
            ray.direction = _bd.rayDirections[i];
            Gizmos.DrawRay(ray);
        }
    }

    public void SetBoidData(BoidData bd)
    {
        _bd = bd;
    }

    public void Spawn()
    {
        if(_bd == null)
            return;

        gameObject.transform.localScale = new Vector3(_bd.localScale, _bd.localScale, _bd.localScale);

        //Spawn the boid at a random location
        Vector3 spawnPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * (_bd.bound / 2);
        transform.position = spawnPos;
        //TODO: How to handle moving in random rotations?
        //Spawn the boid with a random direction
        _rb.rotation = Random.rotation;
        //Spawn the boid with a random velocity
        _rb.velocity = transform.forward * Random.Range(0.5f, _bd.maxVelocity);
    }

    public void RaycastAll()
    {
        hits.Clear();

        Ray ray = new Ray();
        ray.origin = transform.position;

        RaycastHit thisHit;

        for(int i = 0; i < _bd.numRays; i++)
        {
            ray.direction = _bd.rayDirections[i];
            if(Physics.Raycast(ray, out thisHit, _bd.viewDistance))
            {
                //There was a hit add it to the list
                hits.Add(thisHit);
            }
        }
    }

    public void CheckCollisions()
    {
        //Exit early if no hits this frame
        if(hits.Count == 0)
            return;

        RaycastHit closestHit = hits[0];
        Vector3 directionToTarget = new Vector3();
        float closestDistanceSqr = Mathf.Infinity;
        float thisDistanceSqr = 0.0f;
        
        foreach(RaycastHit h in hits)
        {
            //Check the tag to see if it is NOT "Boid"
            if(!h.transform.gameObject.CompareTag("Boid"))
            {
                directionToTarget = h.point - transform.position;
                thisDistanceSqr = directionToTarget.sqrMagnitude;
                if(thisDistanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = thisDistanceSqr;
                    closestHit = h;
                }
            }
        }

        if(closestDistanceSqr != Mathf.Infinity)
        {
            //Avoid the object by steering away from it
            //We have the point hit, now get the point closest to that on the collider's bounding box's edges
            _rb.isKinematic = true;

            Vector3 avoidance_force = Vector3.Normalize((transform.position + _rb.velocity) - closestHit.collider.bounds.center);
            //Scale the avoidance force. Scaling factor should be related to the distance between the boid and the collision point
            avoidance_force *= Mathf.Min(_bd.maxVelocity, Mathf.Sqrt(closestDistanceSqr));
            _rb.velocity = avoidance_force;
            _rb.isKinematic = false;

            /*
            Vector3 incomingVec = closestHit.point - transform.position;
            Vector3 reflectVec = Vector3.Reflect(incomingVec, closestHit.normal);
            Quaternion tempQ = new Quaternion();
            tempQ.eulerAngles = reflectVec;

            Vector3 v = _rb.velocity;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, tempQ, 45);
            _rb.velocity = transform.forward * v.magnitude;
            */
        }        
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Respawn"))
        {
            //We collided with the wall (or another object but not likely)
            //teleport us to the opposite side like pac-man
            Vector3 vel = _rb.velocity;
            _rb.isKinematic = true;
            //Which wall did we trigger on? +-x, +-y, +-z?
            //Get the maximum of the abs of each component to find out
            Vector3 pos = transform.position;
            float max = 0.0f;
            int maxIdx = -1;
            for(int i = 0; i < 3; i++)
            {
                if(Mathf.Abs(pos[i]) >= max)
                {
                    maxIdx = i;
                    max = Mathf.Abs(pos[i]);
                }
            }
            pos[maxIdx] *= -0.95f;
            transform.position = pos;
            _rb.isKinematic = false;
            _rb.velocity = vel;
        }

        if(other.gameObject.CompareTag("Boid"))
        {
            //We collided with another boid, seperate the two of them a little
            Vector3 vel = _rb.velocity;
            _rb.isKinematic = true;

            Vector3 pos = transform.position;
            pos.x += Random.Range(transform.localScale.x, transform.localScale.x * 5);
            pos.y += Random.Range(transform.localScale.y, transform.localScale.y * 5);
            pos.z += Random.Range(transform.localScale.z, transform.localScale.z * 5);

            vel = transform.forward * Random.Range(0.5f, _bd.maxVelocity);

            transform.position = pos;
            _rb.isKinematic = false;
            _rb.velocity = vel;
        }
    }
}