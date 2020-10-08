using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidData _bd;
    private Rigidbody _rb;
    private float _bound;

    public void Awake()
    {
        gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //Set the collider to use a trigger
        Collider c = gameObject.GetComponent<Collider>();
        c.isTrigger = true;
        //Create and add the rigidbody component
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.mass = Random.Range(2.0f, 5.0f);
    }

    public void FixedUpdate()
    {
        //To perform the checks for all the following things we need to do a raycase from the front of the boid
        //That ray cast needs to cover our FOV and our distance data. Essentially we want to ray cast a conical shape
        //Sadly we have to use math to figure out how to do that

        //Check if we are going to collide with a non-boid object and avoid it

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

        //Spawn the boid at a random location
        Vector3 spawnPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * (_bd.bound / 2);
        transform.position = spawnPos;
        //TODO: How to handle moving in random rotations?
        //Spawn the boid with a random direction
        //_rb.rotation = Random.rotation;
        //Spawn the boid with a random velocity
        _rb.velocity = transform.forward * Random.Range(0.5f, _bd.maxVelocity);
    }

    void OnTriggerExit(Collider other)
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
}