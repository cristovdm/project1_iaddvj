using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    public float amplitude = 0.5f;      // Amplitud del movimiento horizontal
    public float frequency = 2.0f;      // Frecuencia del movimiento horizontal
    public float scaleAmplitude = 0.1f; // Amplitud del escalado
    public float scaleFrequency = 3.0f; // Frecuencia del escalado
    public float verticalAmplitude = 0.5f; // Amplitud del movimiento vertical
    public float verticalFrequency = 2.0f; // Frecuencia del movimiento vertical

    private Vector3 startPosition;
    private Vector3 startScale;

    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
    }

    void Update()
    {
        // Movimiento horizontal
        float x = startPosition.x + Mathf.Sin(Time.time * frequency) * amplitude;
        
        // Movimiento vertical
        float y = startPosition.y + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

        transform.position = new Vector3(x, y, startPosition.z);

        // Escalado
        float scale = 1 + Mathf.Sin(Time.time * scaleFrequency) * scaleAmplitude;
        transform.localScale = startScale * scale;
    }
}
