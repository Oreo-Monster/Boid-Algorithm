using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed;
    public float range;
    public float veiwDistance;
    public float veiwAngle;

    Transform flockParent;

    Transform[] flock;
    
    void Start(){
        flockParent = GameObject.Find("Flock").transform;
        flock = new Transform[flockParent.childCount];
        for(int i = 0; i < flockParent.childCount; i++){
            flock[i] = flockParent.GetChild(i);
        }
    }

    void Update(){
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        transform.position += transform.forward * speed * Time.deltaTime;
        if(Mathf.Abs(transform.position.x) >= range){
            transform.position = new Vector3(-transform.position.x/1.2f, transform.position.y, transform.position.z);
        } 
        if(Mathf.Abs(transform.position.y ) >= range){
            transform.position = new Vector3(transform.position.x, -transform.position.y/1.2f, transform.position.z);
        }
        if(Mathf.Abs(transform.position.z) >= range){
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z/1.2f);
        }
        List<Transform> neighbors = findNeighbors(flock);
        foreach(Transform closeBoid in neighbors){
            Debug.DrawLine(transform.position, closeBoid.position, Color.green);
        }

    }

    List<Transform> findNeighbors(Transform[] flock){
        List<Transform> neighbors = new List<Transform>();
        for(int i = 0; i < flock.Length; i++){
            Vector3 toCurrent = flock[i].position - transform.position;
            if(toCurrent.magnitude < veiwDistance){
                if(Vector3.Angle(transform.forward, toCurrent) < veiwAngle/2){
                    neighbors.Add(flock[i]);
                }
            }
        }
        return neighbors;
    }
}
