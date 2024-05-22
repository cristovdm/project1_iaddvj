using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpawner : MonoBehaviour
{
    public GameObject guardPrefab; // Prefab del guardia
    public int numberOfGuards = 10; // Número de guardias a instanciar
    public Vector2 mazeBoundsMin; // Coordenadas mínimas del laberinto
    public Vector2 mazeBoundsMax; // Coordenadas máximas del laberinto
    public float checkRadius = 0.5f; // Radio para verificar colisiones

    void Start()
    {
        SpawnGuards();
    }

    void SpawnGuards()
    {
        for (int i = 0; i < numberOfGuards; i++)
        {
            Vector2 spawnPosition = GetRandomPosition();
            Instantiate(guardPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomPosition()
    {
        Vector2 spawnPosition;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            float x = Random.Range(mazeBoundsMin.x, mazeBoundsMax.x);
            float y = Random.Range(mazeBoundsMin.y, mazeBoundsMax.y);
            spawnPosition = new Vector2(x, y);
            attempts++;
        } while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a valid spawn position for the guard.");
        }

        return spawnPosition;
    }

    bool IsPositionOccupied(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
}
