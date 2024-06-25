using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gerard : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private List<NavigationNode> path = new List<NavigationNode>();
    private int targetIndex;
    private bool playerDetected;
    private AudioSource audioSource;
    private GameObject canvasChangeScene;
    private NavigationNode currentNode;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private CircleCollider2D triggerCollider;
    [SerializeField] private float detectionRange = 1f;

    private List<NavigationNode> nodes = new List<NavigationNode>();
    private Vector2 previousDirection = Vector2.right; // Inicializar a la derecha

    void OnEnable()
    {
        canvasChangeScene = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "CanvasChangeScene");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<CircleCollider2D>();
        }

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            Debug.Log("Trigger collider radius: " + triggerCollider.radius);
        }

        FindAllNodes();
        FindClosestNodeToGerard();

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (playerDetected)
        {
            MoveAlongPath();
        }
    }

    private void FindAllNodes()
    {
        nodes.Clear();
        foreach (NavigationNode node in FindObjectsOfType<NavigationNode>())
        {
            nodes.Add(node);
        }
        Debug.Log("Total nodes found: " + nodes.Count);
    }

    private void FindClosestNodeToGerard()
    {
        float closestDistance = float.MaxValue;
        foreach (NavigationNode node in nodes)
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                currentNode = node;
                closestDistance = distance;
            }
        }
        Debug.Log("Closest node to Gerard found at: " + currentNode.transform.position);
    }

    private NavigationNode FindClosestNodeToPlayer()
    {
        float closestDistance = float.MaxValue;
        NavigationNode closestNode = null;

        foreach (NavigationNode node in nodes)
        {
            float distance = Vector2.Distance(player.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }
        Debug.Log("Closest node to Player found at: " + closestNode.transform.position);
        return closestNode;
    }

    private IEnumerator UpdatePath()
    {
        while (true)
        {
            if (player != null)
            {
                NavigationNode endNode = FindClosestNodeToPlayer();
                path = FindPath(currentNode, endNode);
                targetIndex = 0;
                playerDetected = path.Count > 0;
                Debug.Log("Path updated. Path count: " + path.Count);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void MoveAlongPath()
    {
        if (path != null && targetIndex < path.Count)
        {
            NavigationNode targetNode = path[targetIndex];
            if (Vector2.Distance(transform.position, targetNode.transform.position) > 0.1f)
            {
                Vector2 direction = (targetNode.transform.position - transform.position).normalized;
                Vector2 newPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;
                transform.position = newPosition;

                // Invertir el sprite basado en la dirección
                if (direction.x != 0)
                {
                    if (Mathf.Sign(direction.x) != Mathf.Sign(previousDirection.x))
                    {
                        Vector3 newScale = transform.localScale;
                        newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(-direction.x); // Cambiar a -direction.x para invertir la lógica
                        transform.localScale = newScale;
                        previousDirection = direction;
                    }
                }

                Debug.Log("Moving towards: " + targetNode.transform.position);
            }
            else
            {
                currentNode = targetNode;
                targetIndex++;
                Debug.Log("Reached node: " + currentNode.transform.position + ", moving to next node.");
            }
        }
    }

    private List<NavigationNode> FindPath(NavigationNode startNode, NavigationNode endNode)
    {
        List<NavigationNode> path = new List<NavigationNode>();
        List<NavigationNode> visited = new List<NavigationNode>();
        List<NavigationNode> unvisited = new List<NavigationNode>();
        NavigationNode currentNode = startNode;
        unvisited.Add(currentNode);

        Dictionary<NavigationNode, NavigationNode> cameFrom = new Dictionary<NavigationNode, NavigationNode>();
        Dictionary<NavigationNode, float> gScore = new Dictionary<NavigationNode, float>();
        Dictionary<NavigationNode, float> fScore = new Dictionary<NavigationNode, float>();

        foreach (var node in nodes)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }
        gScore[currentNode] = 0;
        fScore[currentNode] = calculateHValue(currentNode, endNode);

        while (unvisited.Count > 0)
        {
            currentNode = getLowestFScoreNode(unvisited, fScore);

            if (currentNode == endNode)
            {
                reconstructPath(cameFrom, currentNode, path);
                Debug.Log("Path found!");
                break;
            }

            unvisited.Remove(currentNode);
            visited.Add(currentNode);

            foreach (NavigationNode neighbor in currentNode.GetNeighbors())
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[currentNode] + Vector2.Distance(currentNode.transform.position, neighbor.transform.position);

                if (!unvisited.Contains(neighbor))
                {
                    unvisited.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = currentNode;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + calculateHValue(neighbor, endNode);
            }
        }

        return path;
    }

    private NavigationNode getLowestFScoreNode(List<NavigationNode> nodes, Dictionary<NavigationNode, float> fScore)
    {
        float lowestFScore = float.MaxValue;
        NavigationNode lowestFScoreNode = null;

        foreach (var node in nodes)
        {
            if (fScore[node] < lowestFScore)
            {
                lowestFScore = fScore[node];
                lowestFScoreNode = node;
            }
        }
        return lowestFScoreNode;
    }

    private void reconstructPath(Dictionary<NavigationNode, NavigationNode> cameFrom, NavigationNode currentNode, List<NavigationNode> path)
    {
        List<NavigationNode> totalPath = new List<NavigationNode> { currentNode };
        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            totalPath.Add(currentNode);
        }
        totalPath.Reverse();
        path.AddRange(totalPath);
    }

    private float calculateHValue(NavigationNode node, NavigationNode endNode)
    {
        return Vector2.Distance(node.transform.position, endNode.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canvasChangeScene != null)
            {
                Money moneyComponent = canvasChangeScene.GetComponent<Money>();
                if (moneyComponent != null)
                {
                    moneyComponent.UpdateAllUI();
                    canvasChangeScene.SetActive(true);
                }
                else
                {
                    Debug.LogError("Money component not found on the GameObject.");
                }
            }
            else
            {
                Debug.LogError("Canvas Change Scene not found");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
