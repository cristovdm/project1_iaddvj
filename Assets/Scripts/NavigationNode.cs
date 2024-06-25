using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    public List<NavigationNode> neighbors = new List<NavigationNode>();
    public float distanceToNeighbor = 1.5f; // Configura esta distancia según sea necesario

    void Start()
    {
        FindNeighbors();
        DrawLines();
        Debug.Log("Node created at position: " + transform.position);
        Debug.Log("Number of neighbors: " + neighbors.Count);
    }

    public List<NavigationNode> GetNeighbors()
    {
        return neighbors;
    }

    void FindNeighbors()
    {
        foreach (NavigationNode node in FindObjectsOfType<NavigationNode>())
        {
            if (Vector2.Distance(transform.position, node.transform.position) < distanceToNeighbor && node != this)
            {
                neighbors.Add(node);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToNeighbor);
    }

    void DrawLines()
    {
        foreach (NavigationNode node in neighbors)
        {
            Debug.DrawLine(transform.position, node.transform.position, Color.red, 1000f);
        }
    }
}
