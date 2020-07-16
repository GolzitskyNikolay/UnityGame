using UnityEngine;
using System.Collections;

public class ObjectManipulations: MonoBehaviour
{
    [SerializeField] public Vector3 scaleMin;
    [SerializeField] public Vector3 scaleMax;
    [SerializeField] public float timeToMin;
    [SerializeField] public float timeToMax;

    public void Scale(Transform transform)
    {
        StartCoroutine(ForScale(transform));
    }

    IEnumerator ForScale(Transform transform)
    {
        var startScale = transform.localScale;

        float elapsedTime = 0.0f;

        while ((elapsedTime += Time.deltaTime) <= timeToMax)
        {
            transform.localScale = Vector3.Lerp(startScale, scaleMax, elapsedTime / timeToMax);

            yield return null;
        }

        startScale = transform.localScale;

        elapsedTime = 0.0f;

        while ((elapsedTime += Time.deltaTime) <= timeToMin)
        {
            transform.localScale = Vector3.Lerp(startScale, scaleMin, elapsedTime / timeToMin);

            yield return null;
        }

        transform.localScale = scaleMin;
    }
}
