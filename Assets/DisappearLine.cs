using UnityEngine;
using DG.Tweening;

public class DisappearLine : MonoBehaviour
{
    [SerializeField] private float timeToDisappear = 2f;

    private LineRenderer lineRenderer;
    private Material lineMaterial;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = lineRenderer.material;
    }

    public void Disappear()
    {
        float initialStartWidth = lineRenderer.startWidth;
        float initialEndWidth = lineRenderer.endWidth;
        float targetStartWidth = initialStartWidth * 5f;
        float targetEndWidth = initialEndWidth * 5f;

        // Tween width to double
        DOTween.To(() => initialStartWidth, w =>
        {
            lineRenderer.startWidth = w;
        }, targetStartWidth, timeToDisappear);

        DOTween.To(() => initialEndWidth, w =>
        {
            lineRenderer.endWidth = w;
        }, targetEndWidth, timeToDisappear);

        // Fade out alpha of material color
        Color startColor = lineMaterial.color;
        DOTween.To(
            () => startColor.a,
            a =>
            {
                Color c = lineMaterial.color;
                c.a = a;
                lineMaterial.color = c;
            },
            0f, timeToDisappear
        ).OnComplete(() => Destroy(gameObject));
    }
}
