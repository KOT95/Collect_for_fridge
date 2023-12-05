using System.Collections;
using DG.Tweening;
using UnityEngine;

public sealed class BoostBomb : Boost, IBoost
{
    [SerializeField] private int level;
    [SerializeField] private int startAmount;
    [SerializeField] private int moneyAmount;
    [SerializeField] private Money money;
    [SerializeField] private ObjectBomb boost;
    [SerializeField] private float radiusBomb;
    [SerializeField] private float time;

    private int Amount
    {
        get { return _amount; }
        set
        {
            _amount = value;
            PlayerPrefs.SetInt("BoostBomb", value);
            UIController.Instance.SetTextPanelBoostBomb(value, moneyAmount);
        }
    }
    
    private bool _selected;
    private float _timer;
    private ObjectBomb _boost;
    private int _amount;

    private void Start()
    {
        Amount = PlayerPrefs.HasKey("BoostBomb") ? PlayerPrefs.GetInt("BoostBomb") : startAmount;
    }

    public void ClickBoost()
    {
        if (Amount > 0 && Board.Instance.IsActivate)
        {
            Board.Instance.IsBoost = true;
            _selected = true;
            UIController.Instance.ActivatorBoostBlackPanel(true, 1);
        }
        else if (Amount <= 0 && money.MoneyAmount >= moneyAmount)
        {
            money.RemoveMoney(moneyAmount);
            Amount++;
        }
    }

    private IEnumerator Timer()
    {
        while (_timer < time)
        {
            _timer += Time.deltaTime;
            yield return null;
        }
        
        _selected = false;
        Destroy(_boost.gameObject);
        Board.Instance.FallTiles();
        Board.Instance.IsActivate = true;
    }

    public void Activate(Tile tile)
    {
        if (_selected)
        {
            Amount--;
            Board.Instance.IsActivate = false;
            UIController.Instance.ActivatorBoostBlackPanel(false, 0);
            _timer = 0;

            Vector3 pos = new Vector3(tile.transform.position.x, tile.transform.position.y,
                tile.transform.position.z - 0.1f);
            _boost = Instantiate(boost, pos, Quaternion.identity);
            _boost.GetComponent<SphereCollider>().radius = radiusBomb;
            _boost.transform.rotation = tile.transform.rotation;
            _boost.transform.DOMoveZ(tile.transform.position.z, 0.5f).OnComplete(() =>
            {
                _boost.Explosion();
                StartCoroutine(Timer());
            });
        }
    }

    public void Deactivate()
    {
        _selected = false;
    }

    public void CheckLevel(int currentLevel)
    {
        print("234324");
        if (currentLevel < level)
            UIController.Instance.LockBoostBomb(true, level);
        else
            UIController.Instance.LockBoostBomb(false, level);
    }
}
