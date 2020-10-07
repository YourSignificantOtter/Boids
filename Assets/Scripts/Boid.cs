using System.Collections;
using System.Collections.Generic;
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
        //Check if we are going to collide with a non-boid object and avoid it

        //Perform seperation

        //Perform alignment

        //Perform cohesion
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
        //Spawn the boid with a random direction
        _rb.rotation = Random.rotation;
        //Spawn the boid with a random velocity
        //The max velocity is the MAGNITUDE of the velocity, not the max of a single component
        _rb.velocity = Random.insideUnitSphere * _bd.maxVelocity;
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
        if(maxIdx == 0)
        {
            pos.x = pos.x * -0.95f;
        }
        else if(maxIdx == 1)
        {
            pos.y = pos.y * -0.95f;
        }
        else
        {
            pos.z = pos.z * -0.95f;
        }
        transform.position = pos;
        _rb.isKinematic = false;
        _rb.velocity = vel;
    }
}