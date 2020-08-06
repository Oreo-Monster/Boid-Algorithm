using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidSpawner : MonoBehaviour{

    public GameObject boid;
    public GameObject flock;
    public int numBoids;

    void Start(){
        float range = boid.GetComponent<Boid>().range;

        for(int i = 0; i < numBoids; i++){
            Vector3 randomPosition = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            Quaternion randomRotation = Quaternion.Euler(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)); 
            GameObject currentBoid = Instantiate(boid, randomPosition, randomRotation);
            currentBoid.transform.SetParent(flock.transform, false);
        }
    }

}
