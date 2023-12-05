using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Lose : MonoBehaviour
{
    [SerializeField] private int startMoney;
    [SerializeField] private int startMoves;
    [SerializeField] private Money money;
    [SerializeField] private Image blackPanel;
    [SerializeField] private Analytics analytics;
    [SerializeField] private Map map;

    private Level _currentLevel;
    private int _amount;
    private int _moves;
    private int _money;

    public void ShowLose(Level level)
    {
        if (_currentLevel != level)
            _amount = 1;
        else
            _amount++;

        
        _currentLevel = level;
        _money = startMoney * _amount;
        _moves = startMoves * _amount;
        UIController.Instance.ShowLosePanel(_moves, _money, money.MoneyAmount >= _money);
    }

    public void ClickAddMoves()
    {
        if(money.MoneyAmount < _money) return;

        money.RemoveMoney(_money);
        _currentLevel.AddMoves(_moves);
        Board.Instance.IsActivate = true;
        UIController.Instance.HideLosePanel();
    }

    public void ClickResetLevel()
    {
        blackPanel.color = new Color(0, 0, 0, 0);
        blackPanel.gameObject.SetActive(true);
        
        analytics.EndLevel(map.CurrentLevel, LevelFinishedResult.lose);
        analytics.StartLevel(map.CurrentLevel);
        
        blackPanel.DOColor(new Color(0, 0, 0, 1), 0.5f).OnComplete(() =>
        {
            _currentLevel.Reset();
            _currentLevel = null;
            Board.Instance.IsActivate = true;
            UIController.Instance.HideLosePanelNoAnim();
            
            blackPanel.DOColor(new Color(0, 0, 0, 0), 0.5f)
                .OnComplete(() =>
                {
                    blackPanel.gameObject.SetActive(false);
                });
        });
    }
}
