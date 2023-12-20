using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
   [SerializeField] float sunMass = 10.00f;
   [SerializeField] float earthMass = 1.00f;
   [SerializeField] float sunRight = 1.00f;
   [SerializeField] float earthRight = 1.00f;
   [SerializeField] float sunUp = 1.00f;
   [SerializeField] float earthUp = 1.00f;
   [SerializeField] float gravConstant = 1.00f;
    public GameObject Earth;
    public GameObject Sun;

    Vector3 earthMovementVector = new Vector3(0,0,0);
    Vector3 sunMovementVector = new Vector3(0,0,0);

    Vector3 earthAccelerationVector;
    [SerializeField] float timeSpeed = 0.10f;
   
   
    // Start is called before the first frame update 
    void Start()
    {
                earthMovementVector = new Vector3(earthRight,earthUp,20);
                sunMovementVector = new Vector3(sunRight,sunUp,0);
    }

    // Update is called once per frame
    void Update()
    {
         //Get the positions;
        Vector3 sunPosition = Sun.transform.position;
        Vector3 earthPosition = Earth.transform.position;

        float distanceBetween = Vector3.Distance(sunPosition, earthPosition);
        Vector3 earthGravityDirection = (sunPosition - earthPosition).normalized;
        Vector3 sunGravityDirection = - earthGravityDirection;

         // Calculate the force of gravity
        float gravityForce = (gravConstant * sunMass * earthMass)/ (distanceBetween * distanceBetween);

        // Find gravitational vector
        Vector3 earthGravityVector = earthGravityDirection * gravityForce;
        Vector3 sunGravityVector = sunGravityDirection * gravityForce;

        //Find acceleration due to gravity
        Vector3 earthAcceleration = earthGravityVector / earthMass;
        Vector3 sunAcceleration = sunGravityVector / sunMass;

        // Integrate acceleration to get velocity
        earthMovementVector += earthAcceleration;
        sunMovementVector += sunAcceleration;

        // Apply the velocity to move the Earth
        Earth.transform.Translate(earthMovementVector * timeSpeed * Time.deltaTime);
        Sun.transform.Translate(sunMovementVector * timeSpeed * Time.deltaTime);

        // Logging
        Debug.Log(Sun.transform.position);
        Debug.Log(Earth.transform.position);
        Debug.Log("earth's mass = " + earthMass); 
        Debug.Log("gravity force = " + gravityForce); 
        Debug.Log("earth movement vector = " + earthMovementVector); 
        Debug.Log("earth acceleration vector = " + earthAcceleration); 

     
    }


}
