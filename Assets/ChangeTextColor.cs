using UnityEngine;
using TMPro;
using DG.Tweening; // Make sure DOTween is imported

public class ChangeTextColor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color changeToColor;

    private Color defaultColor;
    private Vector3 originalScale;

    [SerializeField] private float scaleUpMultiplier = 1.2f;
    [SerializeField] private float tweenDuration = 0.3f;

    private string defaultText;

    private void Start()
    {
        if (text != null)
        {
            defaultColor = text.color;
            originalScale = text.transform.localScale;
            defaultText = text.text;
        }
    }

    public void ChangeToColor()
    {
        if (text != null)
        {
            text.color = changeToColor;
            text.text = ">" + defaultText + "<";
            text.transform.DOScale(originalScale * scaleUpMultiplier, tweenDuration).SetEase(Ease.InOutQuad);
        }
    }

    public void ChangeToDefaultColor()
    {
        if (text != null)
        {
            text.color = defaultColor;
            text.text =  defaultText;
            text.transform.DOScale(originalScale, tweenDuration).SetEase(Ease.InOutQuad);
        }
    }
}
