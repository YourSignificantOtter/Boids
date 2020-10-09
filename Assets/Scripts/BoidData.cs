using UnityEngine;

[CreateAssetMenu(fileName = "Boid Data", menuName = "ScriptableObjects/BoidData")]
public class BoidData : ScriptableObject
{
    
    public float FOV;
    public float viewDistance;

    public float localScale;

    public float bound;

    public float maxVelocity;
    public float maxAngularVel;

    public float seperationWeight;
    public float alignmentWeight;
    public float cohesionWeight;

    public int numRays;
    public Vector3[] rayDirections;
    
}