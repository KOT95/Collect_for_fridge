using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelWin : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Transform panel;
    [SerializeField] private Button button;
    [SerializeField] private Transform moneyPanel;
    [SerializeField] private Transform bonusMoneyPanel;
    [SerializeField] private TextMeshProUGUI textMoney;
    [SerializeField] private TextMeshProUGUI textBonusMoney;
    [SerializeField] private TextMeshProUGUI textMoves;
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private float _delayTime;

    public void Activate(int money, int moves, int bonusMoney)
    {
        //button.interactable = false;
        textMoney.text = money.ToString();
        textMoves.text = $"{moves} MOVES";
        moneyPanel.localScale = Vector3.zero;
        bonusMoneyPanel.localScale = Vector3.zero;

        foreach (var particleSystem in particleSystems) particleSystem.Play();

        background.color = new Color(0, 0, 0, 0);
        background.DOColor(new Color(0, 0, 0, 0.5f), 0.5f).SetDelay(_delayTime);

        panel.localScale = Vector3.zero;
        panel.DOScale(Vector3.one + new Vector3(0.1f, 0.1f ,0.1f), 0.3f).OnComplete(() =>
        {
            panel.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                bonusMoneyPanel.DOScale(Vector3.one, 0.1f).OnComplete(() =>
                {
                    StartCoroutine(AnimBonusMoney(moves, bonusMoney));
                });
            });
        }).SetDelay(_delayTime);
    }

    private IEnumerator AnimBonusMoney(int moves, int bonusMoney)
    {
        int allBonusMoney = 0;
        
        while (moves != 0)
        {
            moves--;
            allBonusMoney += bonusMoney;
            textMoves.text = $"{moves} MOVES";
            textBonusMoney.text = allBonusMoney.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        
        textMoves.text = $"{moves} MOVES";
        textBonusMoney.text = allBonusMoney.ToString();
        moneyPanel.DOScale(Vector3.one, 0.3f).OnComplete(() =>
        {
            button.interactable = true;
        }).SetDelay(0.1f);
    }
}
