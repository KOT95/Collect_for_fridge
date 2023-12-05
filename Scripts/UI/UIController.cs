using UnityEngine;

public sealed class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    
    [SerializeField] private GameObject uiGame;
    [Space]
    [SerializeField] private PanelStep panelStep;
    [SerializeField] private PanelWin panelWin;
    [SerializeField] private PanelLose panelLose;
    [SerializeField] private PanelMoney panelMoney;
    [SerializeField] private PanelMoney panelMoneyMap;

    [Header("Boosts")] 
    [SerializeField] private PanelBoost panelBoostBomb;
    [SerializeField] private PanelBoost panelBoostRocket;
    [SerializeField] private BoostBlackPanel boostBlackPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void SetUIGame(bool isActivate) => uiGame.SetActive(isActivate);

    public void SetTextStep(int num) => panelStep.SetText(num);

    public void ShowWinPanel(int money, int moves, int bonusMoney)
    {
        panelWin.gameObject.SetActive(true);
        panelWin.Activate(money, moves, bonusMoney);
    }

    public void HideWinPanel() => panelWin.gameObject.SetActive(false);

    public void ShowLosePanel(int moves, int money, bool isMoney)
    {
        panelLose.gameObject.SetActive(true);
        panelLose.Activate(moves, money, isMoney);
    }

    public void HideLosePanel() => panelLose.Deactivate();
    public void HideLosePanelNoAnim() => panelLose.DeactivateNoAnim();

    public void SetTextMoney(int amount)
    {
        panelMoney.SetText(amount);
        panelMoneyMap.SetText(amount);
    }

    public void SetTextPanelBoostBomb(int amount, int money) => panelBoostBomb.SetText(amount, money);
    public void SetTextPanelBoostRocket(int amount, int money) => panelBoostRocket.SetText(amount, money);
    public void LockBoostBomb(bool isLock, int level) => panelBoostBomb.LockBoost(isLock, level);
    public void LockBoostRocket(bool isLock, int level) => panelBoostRocket.LockBoost(isLock, level);

    public void ActivatorBoostBlackPanel(bool isActivate, int numPanel)
    {
        boostBlackPanel.gameObject.SetActive(isActivate);
        boostBlackPanel.ActivatePanel(numPanel);
    } 
}
