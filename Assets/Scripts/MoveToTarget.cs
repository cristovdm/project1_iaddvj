using System.Collections;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Vector3 targetPosition;
    private float itemSpeed = 35f;

    void Start()
    {
        StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * itemSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
