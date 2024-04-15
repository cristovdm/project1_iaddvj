using System.Collections.Generic;
using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    public GameObject bananaPrefab; // Prefab de la banana
    public GameObject boostPrefab; // Prefab del boost
    public GameObject rottenTomatoPrefab; // Prefab del rottenTomato
    public GameObject rottenPanPrefab; // Prefab del rottenPan

    // Lista de posiciones donde se colocarán los objetos
    private List<Vector2> positions = new List<Vector2>
    {
        new Vector2(330, 132),
        new Vector2(300, 188),
        new Vector2(300, 132),
        new Vector2(272, 240),
        new Vector2(272, 215),
        new Vector2(272, 190),
        new Vector2(245, 240),
        new Vector2(245, 160),
        new Vector2(245, 132),
        new Vector2(220, 240),
        new Vector2(190, 240),
        new Vector2(190, 215),
        new Vector2(190, 160),
        new Vector2(160, 240),
        new Vector2(160, 215),
        new Vector2(160, 190),
        new Vector2(160, 150),
        new Vector2(160, 240),
    };

    // Números a colocar (total 14)
    private int bananasCount = 3;
    private int boostCount = 4;
    private int rottenTomatoCount = 4;
    private int rottenPanCount = 3;

    void Start()
    {
        // Copia de las posiciones
        List<Vector2> availablePositions = new List<Vector2>(positions);

        // Para las bananas
        for (int i = 0; i < bananasCount; i++)
        {
            Vector2 position = GetRandomPosition(availablePositions);
            Instantiate(bananaPrefab, position, Quaternion.identity);
        }

        // Para los boosts
        for (int i = 0; i < boostCount; i++)
        {
            Vector2 position = GetRandomPosition(availablePositions);
            Instantiate(boostPrefab, position, Quaternion.identity);
        }

        // Para los rotten tomatoes
        for (int i = 0; i < rottenTomatoCount; i++)
        {
            Vector2 position = GetRandomPosition(availablePositions);
            Instantiate(rottenTomatoPrefab, position, Quaternion.identity);
        }

        // Para los rotten panes
        for (int i = 0; i < rottenPanCount; i++)
        {
            Vector2 position = GetRandomPosition(availablePositions);
            Instantiate(rottenPanPrefab, position, Quaternion.identity);
        }
    }

    private Vector2 GetRandomPosition(List<Vector2> availablePositions)
    {
        if (availablePositions.Count == 0)
        {
            Debug.LogError("No hay más posiciones disponibles para colocar objetos.");
            return Vector2.zero;
        }
        int index = Random.Range(0, availablePositions.Count);
        Vector2 position = availablePositions[index];
        availablePositions.RemoveAt(index);
        return position;
    }
}
