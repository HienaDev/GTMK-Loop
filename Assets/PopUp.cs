using UnityEngine;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutBack;


    private void Start()
    {
        // Start the popup animation when the script starts
        AnimateToYZero();
    }

    public void AnimateToYZero()
    {
        Vector3 currentPos = transform.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, 0f, currentPos.z);

        transform.DOLocalMove(targetPos, duration).SetEase(ease);
    }
}
