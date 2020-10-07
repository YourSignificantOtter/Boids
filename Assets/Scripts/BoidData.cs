using UnityEngine;

[CreateAssetMenu(fileName = "Boid Data", menuName = "ScriptableObjects/BoidData")]
public class BoidData : ScriptableObject
{
    
    public float FOV;
    public float viewDistance;

    public float bound;
    public BoxCollider boxCollider;

    public float maxVelocity;
    public float maxAngularVel;

    public float seperationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
}