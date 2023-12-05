using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PanelBoost : MonoBehaviour
{
    [SerializeField] private GameObject panelMoney;
    [SerializeField] private GameObject panelAmount;
    [SerializeField] private TextMeshProUGUI textMoney;
    [SerializeField] private TextMeshProUGUI textAmount;
    [SerializeField] private Image lockPanel;
    [SerializeField] private TextMeshProUGUI textLock;
    [SerializeField] private Button buttonBoost;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -1337, transform.localPosition.z);
        transform.DOLocalMoveY(-1050, 0.3f);
    }
    
    public void SetText(int amount, int money)
    {
        panelMoney.SetActive(amount <= 0);
        panelAmount.SetActive(amount > 0);

        textAmount.text = amount.ToString();
        textMoney.text = money.ToString();
    }

    public void LockBoost(bool isLock, int level)
    {
        if (isLock)
        {
            buttonBoost.gameObject.SetActive(false);
            lockPanel.gameObject.SetActive(true);
            textLock.text = $"LVL {level}";
        }
        else
        {
            buttonBoost.gameObject.SetActive(true);
            lockPanel.gameObject.SetActive(false);
        }
    }
}
