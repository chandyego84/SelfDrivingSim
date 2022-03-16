using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;

    [Range(-1f, 1f)]
    // acceleration and turning speed
    public float a, t;

    public float timeSinceStart = 0f;

    [Header("Fitness")]
    // how far car goes, how fast it is
    public float overallFitness;
    public float distanceMultiplier = 1.5f; // importance of distance in fitness
    public float avgSpeedMultiplier = 0.2f; // importance of average speed in fitness
    public float sensorMultiplier = 0.1f; // importance of staying in middle of track in fitness

    // used to calculate fitness
    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    // contains distance between origin and ray hit (inputs to NN)
    private float aSensor, bSensor, cSensor; 

    /*Initialize starting values*/
    private void Awake() {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    /*Reset car values when it dies*/
    public void Reset() {
        timeSinceStart = 0f; 
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    /*When car collides with object*/
    private void OnCollisionEnter(Collision collision) {
        // car dies as soon as it hits something 
        Reset();
    }

    private void FixedUpdate() {
        InputSensors();
        lastPosition = transform.position;

        // NN here for a and t values

        MoveCar(a, t);
        timeSinceStart += Time.deltaTime;

        CalculateFitness();  

        // a = 0;
        // t = 0;
    }

    /*Fitness Function*/
    private void CalculateFitness() {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        overallFitness = (distanceMultiplier * totalDistanceTravelled) + 
                        (avgSpeedMultiplier * avgSpeed) + 
                        (sensorMultiplier * ((aSensor + bSensor + cSensor)/3));
        
        if (timeSinceStart > 20 && overallFitness < 40) {
            // car is not doing much
            Reset();
        }

        if (overallFitness >= 1000) {
            // TODO: save to JSON!
            // car is doing well
            Reset();
        }
    }

    /*Sensors in front of car, inputs to NN are distances between car and raycast hit*/
    private void InputSensors() {
        Vector3 a = (transform.forward+transform.right); // 45 degrees right
        Vector3 b = (transform.forward); // straight ahead
        Vector3 c = (transform.forward-transform.right); // 45 degrees left

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit)){
            aSensor = hit.distance/60; // divide to normalize when passed to NN
            Debug.DrawRay(transform.position, a*hit.distance, Color.green);
            print("A: " + aSensor); // should be around val of 0-1
        }

        r.direction = b;
        if (Physics.Raycast(r, out hit)){
            bSensor = hit.distance/45;
            Debug.DrawRay(transform.position, b*hit.distance, Color.green);
            print("B: " + bSensor);
        }

        r.direction = c;
        if(Physics.Raycast(r, out hit)){
            cSensor = hit.distance/60;
            Debug.DrawRay(transform.position, c*hit.distance, Color.green);
            print("C: " + cSensor);
        }
    }

    /* Moving the car: v = acceleration, h = rotation/steering*/
    private Vector3 inp; 
    public void MoveCar(float v, float h) {
        // moving car from (0,0,0) --> (0,0,v*11.4f) @ 0.02f rate
        inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v*11.4f), 0.02f);
        inp = transform.TransformDirection(inp); // convert to car's local space
        transform.position += inp;

        // turning car from (0,0,0) --> (0,0,h*90f) @ 0.02f rate
        transform.eulerAngles += new Vector3(0, (h*90)*0.02f, 0); 
    }
}
