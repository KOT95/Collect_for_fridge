using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class PanelStep : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(-790, transform.localPosition.y, transform.localPosition.z);
        transform.DOLocalMoveX(-390, 0.3f);
    }

    public void SetText(int num)
    {
        text.text = num.ToString();
    }
}
