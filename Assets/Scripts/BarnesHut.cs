// TODO: It looks like GetAllObjects() isn't actually getting all the objects? 7 or 9 out of 10?
// The answer is the Insert isn't working properly. It's occassionally not splitting objects properly into Children nodes. Nt sure why...
// TODO: It seems that the MoveObjectsToChildren is inserting objects into nodes that already have objects in place, but then not splitting the node into children...
// Set up a broken test case and try to get debugging up and running... Work through a real case step by step and see where it's getting caught in a loop.

// Between 1,000 objects there are 500,000 potential interactions. Increasing theta to 0.9 reached 140,000 interactions and the results looked ok!

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Quadrant
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

public class BarnesHutSimulation : MonoBehaviour
{
    public float GravitationalConstant = 0.05f;
    
    private int interactionCount = 0;  // Counter variable

    // private float largestForce = 0.0f; // largest force so far

    private float smallestDistance = 2.50f;
    
    private float largestForcePossible = 15000.0f; // largest force allowed
    private SpaceObjectManager spaceObjectManager;

    // Method to initialize BarnesHutSimulation with a reference to SpaceObjectManager
    public void Initialize(SpaceObjectManager manager)
    {
        spaceObjectManager = manager;
    } 

    public Quadtree quadtree;

    public Quadtree gizmoquadtree;

    // Adjust these parameters based on your simulation
    [SerializeField] public float Theta = 6.0f;

    public int maxDepth = 30;

    public void Start()
    {
        
       // GravitationalConstant = randomGrav();

        createQuadtree();

    }

    private float simulationInterval = 0.0f; // Set the simulation interval (in seconds)

    private float timeSinceLastSimulation = 0f;

    private void Update()
    {
            // Accumulate time since the last simulation
        timeSinceLastSimulation += Time.deltaTime;

        // Check if enough time has passed to perform the simulation
        if (timeSinceLastSimulation >= simulationInterval)
        {
            
            
            // Perform Barnes–Hut simulation steps
            foreach (var spaceObject in quadtree.GetAllObjects())
            {
                ApplyForces(spaceObject, quadtree.Root);   
            } 

            Debug.Log("interactionCount = " + interactionCount);

            Debug.Log("interactionCountPerSecond = " + interactionCount/timeSinceLastSimulation);

            interactionCount = 0;

            // Reset the timer
            timeSinceLastSimulation = 0f;


            // Update last object positions
            UpdateLastObjectPositions();

            // Create/update the quadtree if needed */
           createQuadtreeIfNeeded();
        }
    }

    private void createQuadtree()
    {
         if (spaceObjectManager != null)
        {
            List<Rigidbody2D> allSpaceObjects = spaceObjectManager.planetRigidbodies; // Replace with your list
            
            // Initialize quadtree based on your simulation space
            Bounds simulationBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 0)); // Adjust Bounds accordingly

            quadtree = new Quadtree(simulationBounds);



        foreach (Rigidbody2D planet in allSpaceObjects.ToList())
        {
            
            if (planet != null)
            {
                quadtree.Insert(planet, maxDepth);
                
            }
            else
            {
                Debug.LogError("SpaceObject component not found on GameObject: " + planet.name);
            }

        }
        }
        else
        {
            Debug.LogError("SpaceObjectManager script not found on the GameObject.");
        }
       // gizmoquadtree = quadtree;
    }


private Dictionary<Rigidbody2D, Vector2> lastObjectPositions = new Dictionary<Rigidbody2D, Vector2>();
private float objectMoveThreshold = 1.00f; // Adjust this threshold based on your simulation scale

 /*  float randomGrav()
    {
        float randomGrav = UnityEngine.Random.Range(15, 25);
        Debug.Log("randomGrav = " + randomGrav);
        return randomGrav / 1000;
    }*/

private void createQuadtreeIfNeeded()
{
    if (spaceObjectManager == null)
    {
        Debug.LogError("SpaceObjectManager script not found on the GameObject.");
        return;
    }

    List<Rigidbody2D> allSpaceObjects = spaceObjectManager.planetRigidbodies;

    if (allSpaceObjects.Count == 0)
    {
        // No space objects, no need to create the quadtree
        return;
    }

    Bounds simulationBounds = CalculateSimulationBounds(allSpaceObjects);

    if (quadtree == null)
    {
        // Create the quadtree if it doesn't exist
        quadtree = new Quadtree(simulationBounds);
    }
    else
    {
        // Update the existing quadtree
        foreach (Rigidbody2D planet in allSpaceObjects.ToList())
        {
            if (planet != null && HasObjectMoved(planet))
            {
                quadtree.Insert(planet, maxDepth);
            }
        }
    }

    // Update the gizmoquadtree
    gizmoquadtree = quadtree;
}

private bool HasObjectMoved(Rigidbody2D spaceObject)
{
    if (lastObjectPositions.TryGetValue(spaceObject, out Vector2 lastPosition))
    {
        Vector2 currentPosition = spaceObject.position;
        float distance = Vector2.Distance(currentPosition, lastPosition);
        return distance > objectMoveThreshold;
    }
    else
    {
        // If the object is not in the dictionary, consider it moved
        return true;
    }
}

private Bounds CalculateSimulationBounds(List<Rigidbody2D> spaceObjects)
{
    if (spaceObjects == null || spaceObjects.Count == 0)
    {
        // Return a default bounds if there are no space objects
        return new Bounds(Vector3.zero, Vector3.one);
    }

    Vector3 min = spaceObjects[0].position;
    Vector3 max = spaceObjects[0].position;

    foreach (var spaceObject in spaceObjects)
    {
        if (spaceObject != null)
        {
            Vector3 position = spaceObject.position;
            min = Vector3.Min(min, position);
            max = Vector3.Max(max, position);
        }
    }

    Vector3 center = (min + max) / 2f;
    Vector3 size = max - min;

    return new Bounds(center, size);
}


private void UpdateLastObjectPositions()
{
    foreach (Rigidbody2D planet in spaceObjectManager.planetRigidbodies)
    {
        if (planet != null)
        {
            lastObjectPositions[planet] = planet.position;
        }
    }
}

private void ApplyForces(Rigidbody2D currentObject, QuadtreeNode currentNode)
{
    interactionCount++;
    
    if (currentNode == null)
    {
        return;
    }

    if (currentNode.Bounds.size.magnitude / ((Vector3)currentObject.position - currentNode.CenterOfMass).magnitude < Theta)
    {
        // Use Barnes–Hut approximation for external nodes or small cell size
            if((Vector3)currentObject.position != currentNode.CenterOfMass & currentNode.TotalMass > 0 & ((Vector3)currentObject.position - currentNode.CenterOfMass).magnitude > smallestDistance)
            {
                ApplyForce(currentObject, currentNode.CenterOfMass, currentNode.TotalMass);
            }
    }

    else
    {
        if (currentNode.IsExternal)
        {
            return;
        }
        else
        {
            // Recursively apply forces for internal nodes
            foreach (var child in currentNode.Children)
                {
                    ApplyForces(currentObject, child);
                }
        }
    }
}

private void ApplyForce(Rigidbody2D currentObject, Vector3 targetPosition, float targetMass)
{
    Vector3 direction = targetPosition - (Vector3)currentObject.position;
    float distanceSquared = direction.sqrMagnitude;

     // Smallest distance debugging check
    if (distanceSquared < smallestDistance)
    {
        return;
    }

    if (distanceSquared > 0f)
    {
        float forceMagnitude = GravitationalConstant * (currentObject.mass * targetMass) / distanceSquared;
        Vector3 force = forceMagnitude * direction.normalized;

        // Largest force debugging check
        if (force.magnitude > largestForcePossible)
        {
            return;
        }


        /*if (force.magnitude > largestForce)
        {
            largestForce = force.magnitude;
        }*/



        // Apply force to the current object (modify based on your SpaceObject class)
        currentObject.velocity += (Vector2)force  / currentObject.mass;
    }
}

}

public class Quadtree
{
    public QuadtreeNode Root { get; private set; }

    public Quadtree(Bounds Bounds)
    {

        Root = new QuadtreeNode(Bounds);

    }

    public void Insert(Rigidbody2D spaceObject, int maxDepth)
    {

        Root.Insert(spaceObject, 0, maxDepth);
    }

    public List<Rigidbody2D> GetAllObjects()
    {
        List<Rigidbody2D> objects = new List<Rigidbody2D>();
        Root.CollectObjects(objects);
        return objects;
    }
}

public class QuadtreeNode
{
   public Bounds Bounds { get; private set; }
    public Vector3 CenterOfMass { get; private set; }
    public float TotalMass { get; private set; }
    public List<Rigidbody2D> Objects { get; set; }
    public bool IsExternal => Children == null;
    private const float ChildWidthFactor = 0.5f;
    private const float ChildHeightFactor = 0.5f;

    public string QuadTreeNodeNumber { get; set; }

    // For demonstration purposes, assume a quadtree has four children
    public QuadtreeNode[] Children { get; private set; }

    public QuadtreeNode(Bounds Bounds)
    {
        this.Bounds = Bounds;
        CenterOfMass = Bounds.center;
        Objects = new List<Rigidbody2D>();
    }

public void Insert(Rigidbody2D spaceObject, int depthFactor, int maxDepth)

{


    Objects.Add(spaceObject);

    UpdateNodeProperties(spaceObject);

    if (depthFactor >= maxDepth)
    {
        // If the depth limit is reached, stop recursion
        return;
    }

    if (this.IsExternal) // If this is a leaf node without any children
    {
        if(Objects.Count > 1)
        {
        CreateChildren(depthFactor); // create 4 nodes
        MoveObjectsToChildren(depthFactor, maxDepth);
        }
    }

    else
    {
        MoveObjectsToChildren(depthFactor, maxDepth);
    }

}


private void MoveObjectsToChildren(int depthFactor, int maxDepth)
{
    depthFactor += 1;

    // Ensure that Children is not null and has been properly initialized
    if (Children != null)
    {
        // Create a temporary list to store the objects to avoid modification during iteration
        var objectsToMove = Objects.ToList();
        var objectsToRemove = new List<Rigidbody2D>(); // List to store objects to be removed

        foreach (var celestialBody in objectsToMove)
        {
            Quadrant quadrant = GetQuadrantForSpaceObject(celestialBody);
            Children[(int)quadrant].Insert(celestialBody, depthFactor, maxDepth);
            objectsToRemove.Add(celestialBody); // Add the object to the removal list
        }

        // Remove the objects from the current node after moving them
        foreach (var celestialBody in objectsToRemove)
        {
            Objects.Remove(celestialBody);
        } 
    }
}


   // Function to create four child nodes
    public void CreateChildren(int depthFactor)
    {

        float childWidth = Bounds.size.x / 2f;
        float childHeight = Bounds.size.y / 2f;

        Children = new QuadtreeNode[4];

        // Create bottom-left child
        Children[0] = new QuadtreeNode(new Bounds(new Vector3(Bounds.center.x - childWidth / 2f, Bounds.center.y - childHeight / 2f, 0f), new Vector3(childWidth, childHeight, Bounds.size.z)));

        
        // Create bottom-right child
        Children[1] = new QuadtreeNode(new Bounds(new Vector3(Bounds.center.x + childWidth / 2f, Bounds.center.y - childHeight / 2f, 0f), new Vector3(childWidth, childHeight, Bounds.size.z)));

        
        // Create top-left child
        Children[2] = new QuadtreeNode(new Bounds(new Vector3(Bounds.center.x - childWidth / 2f, Bounds.center.y + childHeight / 2f, 0f), new Vector3(childWidth, childHeight, Bounds.size.z)));

        
        // Create top-right child
        Children[3] = new QuadtreeNode(new Bounds(new Vector3(Bounds.center.x + childWidth / 2f, Bounds.center.y + childHeight / 2f, 0f), new Vector3(childWidth, childHeight, Bounds.size.z)));

}

public void CollectObjects(List<Rigidbody2D> collectedObjects)
{

    int collectObjectsCallCounter = 0;

    // If the node is external, simply add its objects to the collection
    if (IsExternal)
    {
        collectedObjects.AddRange(Objects);
    }
    else
    {
        // If the node is not external, recursively collect objects from each child
        for (int i = 0; i < 4; i++)
        {
            Children[i].CollectObjects(collectedObjects);
        }
    }

    collectObjectsCallCounter += 1;
}


    private Quadrant GetQuadrantForSpaceObject(Rigidbody2D spaceObject)
    {

        Vector3 objectPosition = spaceObject.position;
        
        Vector3 nodeCenter = Bounds.center;

        if (objectPosition.x < nodeCenter.x)
        {
            // Object is on the left side
            return (objectPosition.y < nodeCenter.y) ? Quadrant.BottomLeft : Quadrant.TopLeft;
        }
        else
        {
            // Object is on the right side
            return (objectPosition.y < nodeCenter.y) ? Quadrant.BottomRight : Quadrant.TopRight;
        }
    }

    private void UpdateNodeProperties(Rigidbody2D spaceObject)
    {
                
        // Update the center of mass and total mass of the node
        float totalMass = TotalMass + spaceObject.mass;
        Vector3 weightedCenter = (CenterOfMass * TotalMass + (Vector3)spaceObject.position * spaceObject.mass) / totalMass;

        TotalMass = totalMass;
        CenterOfMass = weightedCenter;

    }

}

