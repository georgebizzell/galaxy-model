using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObject : MonoBehaviour 

{
    public string Name { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public float Size { get; set; } // radius
    public Color ObjectColor { get; set; }
    public float Mass { get; set; }

    // Constructor to initialize the object with default values
    public SpaceObject(Vector3 position, Vector3 velocity, float size, Color objectColor, float mass)
    {
        Position = position;
        Velocity = velocity;
        Size = size;
        ObjectColor = objectColor;
        Mass = mass;
    }

    // Method to update the position based on the current velocity
    public void Move(float deltaTime)
    {
        Position += Velocity * deltaTime;
    }
}