using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private float offscreenY = -1000f;

    [Header("Events")]
    public UnityEvent onBeforePopUp;
    public UnityEvent onPopDownComplete;

    private void Start()
    {
        AnimateToYZero();
    }

    public void AnimateToYZero()
    {
        onBeforePopUp?.Invoke();

        Vector3 currentPos = transform.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, 0f, currentPos.z);

        transform.DOLocalMove(targetPos, duration)
            .SetEase(ease)
            .SetUpdate(true); // unscaled time
    }

    public void AnimateToYOffscreen()
    {
        Vector3 currentPos = transform.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, offscreenY, currentPos.z);

        transform.DOLocalMove(targetPos, duration)
            .SetEase(Ease.InBack) // Customize if needed
            .SetUpdate(true)
            .OnComplete(() => onPopDownComplete?.Invoke());
    }
}
