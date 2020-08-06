using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour{
    
    public float initSpeed;
    public Vector2 speedMinMax;
    public float range;
    public float viewDistance;
    public float viewAngle;
    public float avoidRadius;
    public float edgeBuffer = 0.1f;

    public float alignmentWeight;
    public float separationWeight;
    public float cohesionWeight;
    public float targetWeight;

    Transform[] flock;
    Transform[] targets;
    bool isTarget;

    Vector3 velocity;


    void Start(){
        Transform flockTransfrom = GameObject.Find("Flock").transform;
        Transform targetTransforms= GameObject.Find("Targets").transform;
        flock = new Transform[flockTransfrom.childCount];
        
        for(int i = 0; i < flock.Length; i++){
            flock[i] = flockTransfrom.GetChild(i);
        }
        isTarget = targetTransforms.childCount != 0;
        if(isTarget){
            targets = new Transform[targetTransforms.childCount];
            for(int i = 0; i < targets.Length; i++){
                targets[i] = targetTransforms.GetChild(i);
            }
        }

        velocity = transform.forward * initSpeed;
    }

    void Update(){
        List<Transform> neighbors = findNeighbors(flock);
        Vector3 acceleration = calcAcceleration(neighbors, alignmentWeight, separationWeight, cohesionWeight);
        if(isTarget){
            acceleration += targetAcceleration(targets, targetWeight);
        }
        velocity += acceleration * Time.deltaTime;
        float speed = Mathf.Clamp(velocity.magnitude, speedMinMax.x, speedMinMax.y);
        velocity = velocity.normalized * speed;
        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;

        if (Mathf.Abs(transform.position.x) >= range){
            transform.position = new Vector3(-transform.position.x * edgeBuffer, transform.position.y, transform.position.z);
        }
        if (Mathf.Abs(transform.position.y) >= range){
            transform.position = new Vector3(transform.position.x, -transform.position.y* edgeBuffer, transform.position.z);
        }
        if (Mathf.Abs(transform.position.z) >= range){
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z * edgeBuffer);
        }

    }

    List<Transform> findNeighbors(Transform[] flock)
    {
        List<Transform> neighbors = new List<Transform>();
        for (int i = 0; i < flock.Length; i++)
        { 
            Vector3 toCurrent = flock[i].position - transform.position;
            if (toCurrent.magnitude < viewDistance && toCurrent.magnitude > 0)
            {
                if (Vector3.Angle(transform.forward, toCurrent) < viewAngle / 2)
                {
                    neighbors.Add(flock[i]);
                }
            }
        }
        return neighbors;
    }

    Vector3 calcAcceleration(List<Transform> neighbors, float alignmentWeight, float separationWeight, float cohesionWeight){
        Vector3 avgForward = Vector3.zero;
        Vector3 COM = Vector3.zero;
        Vector3 separationSum = Vector3.zero;

        if(neighbors.Count > 0){
            foreach(Transform b in neighbors){
                avgForward += b.forward;
                Vector3 disp = transform.position - b.position;
                COM -= disp;
                float sqrDist = disp.x * disp.x + disp.y * disp.y + disp.z * disp.z;
                if(sqrDist < avoidRadius * avoidRadius){
                    if(sqrDist > 0){
                        separationSum += disp / sqrDist;
                    }
                }


            }
            Vector3 alignmentOffset = (avgForward/neighbors.Count) - transform.forward;
            Vector3 separationOffset = (separationSum / neighbors.Count) - transform.forward;
            Vector3 cohesionOffset = (COM / neighbors.Count) - transform.forward;
            Vector3 acceleration = alignmentOffset * alignmentWeight + separationOffset * separationWeight + cohesionOffset * cohesionWeight;
            return acceleration;
        }

        return Vector3.zero;
    }

    Vector3 targetAcceleration(Transform[] targets, float targetWeight){
        Transform closestTarget = null;
        float closestTargetDistance = viewDistance * 100f;
        foreach(Transform target in targets){
            Vector3 displacement = target.position - transform.position;
            if(displacement.magnitude < viewDistance){
                if(Vector3.Angle(transform.forward, displacement) < viewAngle / 2){
                    if(displacement.magnitude < closestTargetDistance){
                        closestTarget = target;
                        closestTargetDistance = displacement.magnitude;
                    }
                }
            }
        }
        if(closestTarget != null){
            return (closestTarget.position - transform.position) * targetWeight;
        }
        return Vector3.zero;
    }
}
