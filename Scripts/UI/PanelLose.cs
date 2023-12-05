using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelLose : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Transform panel;
    [SerializeField] private TextMeshProUGUI textAmountMoves;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI textMoney;
    [SerializeField] private Button buttonAddMoves;
    [SerializeField] private Button buttonTryAgain;

    public void Activate(int moves, int money, bool isMoney)
    {
        buttonAddMoves.interactable = false;
        buttonTryAgain.interactable = false;
        
        textAmountMoves.text = $"+{moves}";
        text.text = $"ADD {moves} MOVES TO KEEP PLAYING";
        textMoney.text = money.ToString();

        background.color = new Color(0, 0, 0, 0);
        background.DOColor(new Color(0, 0, 0, 0.5f), 0.4f);

        panel.localPosition = new Vector3(0, 2000, 0);
        panel.DOLocalMoveY(-150, 0.2f).OnComplete(() =>
        {
            panel.DOLocalMoveY(0, 0.2f).OnComplete(() =>
            {
                buttonAddMoves.interactable = isMoney;
                buttonTryAgain.interactable = true;
            });
        });
    }

    public void Deactivate()
    {
        buttonAddMoves.interactable = false;
        buttonTryAgain.interactable = false;
        
        background.DOColor(new Color(0, 0, 0, 0), 0.4f);
        
        panel.DOLocalMoveY(-150, 0.2f).OnComplete(() =>
        {
            panel.DOLocalMoveY(2000, 0.2f).OnComplete(() => {gameObject.SetActive(false);});
        });
    }

    public void DeactivateNoAnim()
    {
        buttonAddMoves.interactable = false;
        buttonTryAgain.interactable = false;
        gameObject.SetActive(false);
    }
}
