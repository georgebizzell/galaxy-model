using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

// Implement Barnes–Hut_simulation using grids

public class SpaceObjectManager : MonoBehaviour
{
    
      // Reference to the BarnesHutSimulation component
    private BarnesHutSimulation barnesHutSimulationInstance;
    [SerializeField] float gravConstant = 1.00f;
    [SerializeField] float timeSpeed = 0.10f;
    [SerializeField] float sunMass = 1.00f;
    [SerializeField] float sunSize = 10f;
    [SerializeField] bool Orbit = false;
    [SerializeField] bool testMode = false;
    [SerializeField] float squareStart = 0f;
    [SerializeField] float squareSize = 1f;
    [SerializeField] Vector3 sunPosition = new Vector3(-10f, -10f, 0f);
    [SerializeField] Vector3 sunVelocity = new Vector3(0f, 0f, 0f);
    [SerializeField] int posRange = 100;
    [SerializeField] int velRange = 1;
    [SerializeField] float sizeRange = 20f;
    [SerializeField] int numberOfBeings = 4;

    [SerializeField] float testMultiplier = 1;

    public List<GameObject> spaceObjects = new List<GameObject>();
    public List<GameObject> planets = new List<GameObject>();

    // Initialize spriteList with the appropriate sprite names
    private List<string> spriteList = new List<string> { "planet_orange", "planet_red", "planet_earth", "planet_earth", "planet_yellow" };

    private Dictionary<string, float> spriteRadiusDictionary = new Dictionary<string, float>();

    public List<Rigidbody2D> planetRigidbodies = new List<Rigidbody2D>();


    private void Start()
    {
        
        spriteRadiusDictionary.Add("planet_orange", 1.60f);
        spriteRadiusDictionary.Add("planet_red", 1.60f);
        spriteRadiusDictionary.Add("planet_earth", 2.00f);
        spriteRadiusDictionary.Add("planet_yellow", 1.60f );

        CreateSun();

        for (int i = 0; i < numberOfBeings; i++)
        {
            if(!testMode)
            {
                CreateSpaceObject(i);
            }
            else
            {
                CreateSpaceObject(i, 1, 10, "planet_earth", new Vector3 (0,0,0), testPositions(i));
            }
            
        }

        foreach (GameObject planet in planets)
        {
        Rigidbody2D planetRigidbody = planet.GetComponent<Rigidbody2D>();
        
        if (planetRigidbody != null)
        {
            planetRigidbodies.Add(planetRigidbody);
        }  
    }
    InitializeBarnesHutSimulation();
        }

    private Vector3 testPositions(float i)
    {
        float xPos, yPos;

        xPos = i * squareSize + squareStart;
        yPos = i * squareSize + squareStart;

        return new Vector3(xPos, yPos, 0);

    }

    /*private void Update()
    {
       // UpdateSpaceObjects();
    }*/

    private void UpdateSpaceObjects()
    {
        foreach (GameObject spaceObjectPrefab in planets)
        {
            ObjectsLooper(spaceObjectPrefab);
        }
    }

    private void CreateSun()
    {
        string spriteType = "planet_yellow";
        int spriteIndex = spriteList.IndexOf(spriteType);
        if (spriteIndex >= 0)
        {
            CreateSpaceObject(100000, sunSize, sunMass, spriteType, sunVelocity, sunPosition, true);
        }
        else
        {
            Debug.LogError("Invalid spriteType: " + spriteType);
        }
    }

    private void CreateSpaceObject(int number, float? size = null, float? mass = null, string spriteType = "planet_earth", Vector3? startVelocity = null, Vector3? startPosition = null, bool fixedPosition = false)
    {

        Vector3 initialPosition = startPosition.HasValue ? startPosition.Value : InitialPositionSetter();

        Vector3 initialVelocity = new Vector3(0, 0, 0);
        
        if(!Orbit)
        {
            initialVelocity = startVelocity.HasValue ? startVelocity.Value : InitialVelocitySetter();
        }

        else 
        {
            initialVelocity = startVelocity.HasValue ? startVelocity.Value : orbitVelocity(initialPosition);
        }

        // Debug.Log("Initial velocity = " + initialVelocity);

        float initialSize = size.HasValue ? size.Value : UnityEngine.Random.Range(0, sizeRange);
        float initialMass = mass.HasValue ? mass.Value : (float)(Math.PI * initialSize * initialSize);
        Color initialColor = new Color(1f, 1f, 1f, 1f);

        GameObject spaceObjectPrefab = new GameObject("Celestial being " + number);
        Rigidbody2D rb = spaceObjectPrefab.AddComponent<Rigidbody2D>();
        rb.mass = initialMass;
        rb.velocity = initialVelocity;
        rb.gravityScale = 0;
        if(fixedPosition == true)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        spaceObjectPrefab.transform.position = initialPosition;

        // Set the local scale for the GameObject (will not affect the collider)
        spaceObjectPrefab.transform.localScale = new Vector3(1f, 1f, 1f);

        CircleCollider2D circleCollider = spaceObjectPrefab.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
        
        /*/ Set the collider radius based on the sprite name
        if (spriteRadiusDictionary.TryGetValue(spriteType, out float spriteRadius))

            {
                circleCollider.radius = spriteRadius;
            }

        else
            {
                Debug.LogError("Sprite radius not found for spriteType: " + spriteType);
            } */

        SpriteRenderer spriteRenderer = spaceObjectPrefab.AddComponent<SpriteRenderer>();
        string spriteFileName = "space_object";
        Sprite circleSprite = Resources.Load<Sprite>(spriteFileName);
        spriteRenderer.sprite = circleSprite;

        spaceObjectPrefab.transform.localScale = new Vector3(initialSize, initialSize, 1f);
        spaceObjectPrefab.tag = "Planet";
        spaceObjects.Add(spaceObjectPrefab);

        planets.Add(spaceObjectPrefab);
    
    }

    private Vector3 InitialVelocitySetter()
    {
        int xVelocity = UnityEngine.Random.Range(-velRange, velRange);
        int yVelocity = UnityEngine.Random.Range(-velRange, velRange);
        return new Vector3(xVelocity, yVelocity, 0);
    }

private Vector3 InitialPositionSetter()
{
    float minDistance = sunSize + 1; // Minimum distance from the sun

    float xPos, yPos;

        xPos = UnityEngine.Random.Range(-posRange, posRange);
        yPos = UnityEngine.Random.Range(-posRange, posRange);
    
    /* while 
    (Vector3.Distance(new Vector3(xPos, yPos, 0), sunPosition) < minDistance); */

    return new Vector3(xPos, yPos, 0);

}


    private void ObjectsLooper(GameObject spaceObjectPrefab)
    {
        Rigidbody2D objRigidbody = spaceObjectPrefab.GetComponent<Rigidbody2D>();
        Vector3 beingPosition = spaceObjectPrefab.transform.position;

        if (objRigidbody != null)
        {
            foreach (Rigidbody2D otherRigidbody in planetRigidbodies)
            {
                if (otherRigidbody != objRigidbody)
                {
                    Vector3 otherBeingPosition = otherRigidbody.transform.position;
                    float distanceBetween = Vector3.Distance(beingPosition, otherBeingPosition);
                    Vector3 gravityDirection = (otherBeingPosition - beingPosition).normalized;

                    float gravityForce = (gravConstant * objRigidbody.mass * otherRigidbody.mass) / (distanceBetween * distanceBetween);
                    Vector3 gravityVector = gravityDirection * gravityForce;
                    Vector3 acceleration = gravityVector / objRigidbody.mass;

                    objRigidbody.velocity += (Vector2)acceleration;
                }
            }

            spaceObjectPrefab.transform.position += (Vector3)objRigidbody.velocity * timeSpeed * Time.deltaTime;
        }
    }

    private Vector3 orbitVelocity(Vector3 initialPosition)
    {
        // Sun distance
        float sunDistance = UnityEngine.Vector3.Distance(initialPosition, sunPosition);
        
        // Perpendicular to Sun's gravity pull direction
        Vector3 gravityDirection = (sunPosition - initialPosition).normalized;

        Vector3 orbitDirection = PerpendicularClockwise(gravityDirection);

        //Velocity magnitude required in perpendicular direction
        float velocityMagnitude = Mathf.Sqrt(gravConstant * sunMass / sunDistance);

        Vector3 orbitalVelocity = orbitDirection * velocityMagnitude * testMultiplier;

        return orbitalVelocity;

    }

private Vector3 PerpendicularClockwise(Vector3 vector3)
    {
        return new Vector3(vector3[1], -vector3[0], vector3[2]);
    }

private Vector3 PerpendicularCounterClockwise(Vector3 vector3)
    {
        return new Vector3(-vector3[1], vector3[0], vector3[2]);
    }


    public void InitializeBarnesHutSimulation()
    {
        // Instantiate a new GameObject
        GameObject simulationGameObject = new GameObject("BarnesHutSimulationInstance");

        // Attach the BarnesHutSimulation script to the new GameObject
        barnesHutSimulationInstance = simulationGameObject.AddComponent<BarnesHutSimulation>();

        // Pass a reference to SpaceObjectManager to BarnesHutSimulation
        barnesHutSimulationInstance.Initialize(this);

        // Access and modify parameters as needed
        // barnesHutSimulationInstance.GravitationalConstant = 0.05f;
        // barnesHutSimulationInstance.Theta = 0.5f;
    }

}
