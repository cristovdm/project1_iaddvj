using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    public GameObject minePrefab;
    public int numberOfMines = 10;
    public Vector2 mazeBoundsMin;
    public Vector2 mazeBoundsMax;
    public Vector2 mazeRestrictedAreaMin;
    public Vector2 mazeRestrictedAreaMax;
    public float checkRadius = 0.5f;

    void Start()
    {
        SpawnMines();
    }

    void SpawnMines()
    {
        for (int i = 0; i < numberOfMines; i++)
        {
            Vector2 spawnPosition = GetRandomPosition();
            Instantiate(minePrefab, spawnPosition, Quaternion.identity);
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
        } while ((IsPositionOccupied(spawnPosition) || IsPositionInRestrictedArea(spawnPosition)) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("No se pudo encontrar una posición válida para la mina.");
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

    bool IsPositionInRestrictedArea(Vector2 position)
    {
        return position.x >= mazeRestrictedAreaMin.x && position.x <= mazeRestrictedAreaMax.x &&
               position.y >= mazeRestrictedAreaMin.y && position.y <= mazeRestrictedAreaMax.y;
    }
}
