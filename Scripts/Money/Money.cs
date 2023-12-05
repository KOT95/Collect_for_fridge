using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int startMoney;
    
    public int MoneyAmount
    {
        get { return _moneyAmount; }
        private set
        {
            _moneyAmount = value;
            PlayerPrefs.SetInt("Money", value);
            UIController.Instance.SetTextMoney(value);
        }
    }

    private int _moneyAmount;

    private void Start()
    {
        MoneyAmount = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetInt("Money") : startMoney;
    }

    public void AddMoney(int amount)
    {
        MoneyAmount += amount;
    }
    
    public void RemoveMoney(int amount)
    {
        MoneyAmount -= amount;
    }
}
