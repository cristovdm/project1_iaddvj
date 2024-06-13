using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 2.0f;
    public float scaleAmplitude = 0.1f;  
    public float scaleFrequency = 3.0f;  

    private Vector3 startPosition;
    private Vector3 startScale;

    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
    }

    void Update()
    {
        float x = startPosition.x + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(x, startPosition.y, startPosition.z);
        float scale = 1 + Mathf.Sin(Time.time * scaleFrequency) * scaleAmplitude;
        transform.localScale = startScale * scale;
    }
}
