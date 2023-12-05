using DG.Tweening;
using TMPro;
using UnityEngine;

public class PanelMoney : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isAnim;

    private void OnEnable()
    {
        if (isAnim)
        {
            transform.localPosition = new Vector3(700, transform.localPosition.y, transform.localPosition.z);
            transform.DOLocalMoveX(390, 0.3f);
        }
    }
    
    public void SetText(int amount)
    {
        if(amount >= 1000)
            text.text = (amount / 1000f).ToString("F1") + "K";
        else
            text.text = amount.ToString();
    }
}
