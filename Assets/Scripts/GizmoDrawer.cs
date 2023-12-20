/*

using Unity.VisualScripting;
using UnityEngine;



public class QuadtreeGizmosDrawer : MonoBehaviour
{
    public Quadtree quadtree; // Reference to your quadtree

    public BarnesHutSimulation simulationScript;
    
    public void Start()
    {
        // Find the GameObject with BarnesHutSimulation script
        GameObject simulationObject = GameObject.Find("BarnesHutSimulationInstance");

        // Access the Quadtree instance from the BarnesHutSimulation script
        if (simulationObject != null)
        {
            simulationScript = simulationObject.GetComponent<BarnesHutSimulation>();

            
            if (simulationScript != null)
            {
                
                quadtree = simulationScript.gizmoquadtree;

            }
        }
    }
    
    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        
        // Find the GameObject with BarnesHutSimulation script
        GameObject simulationObject = GameObject.Find("BarnesHutSimulationInstance");

        // Access the Quadtree instance from the BarnesHutSimulation script
        if (simulationObject != null)
        {
            simulationScript = simulationObject.GetComponent<BarnesHutSimulation>();
            
            if (simulationScript != null)
            {    
                quadtree = simulationScript.gizmoquadtree;

            }
        }

        if (!Application.isPlaying)
        {
            return;
        }


        quadtree = simulationScript.gizmoquadtree;
            Gizmos.color = Color.blue;
            DrawQuadtreeGizmos(quadtree.Root);

    }

    #endif

   private void Update()
    {
        if (simulationScript != null && simulationScript.gizmoquadtree != null)
        {
            quadtree = simulationScript.gizmoquadtree;

            // Update or modify the quadtree as needed

            // Example: Clear Gizmos before drawing
            Gizmos.color = Color.green;

            OnDrawGizmos();
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (quadtree != null)
        {
            Gizmos.color = Color.blue;
            DrawQuadtreeGizmos(quadtree.Root);
        }
    }

    private void DrawQuadtreeGizmos(QuadtreeNode node)
    {
        if (node != null)
        {
            Gizmos.DrawWireCube(node.Bounds.center, node.Bounds.size);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DrawQuadtreeGizmos(child);
                }
            }
        }
    }

}
*/