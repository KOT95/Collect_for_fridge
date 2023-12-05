using System.Collections;
using DG.Tweening;
using UnityEngine;

public sealed class BoostRocket : Boost, IBoost
{
    [SerializeField] private int level;
    [SerializeField] private int startAmount;
    [SerializeField] private int moneyAmount;
    [SerializeField] private Money money;
    [SerializeField] private ObjectRocket boost;
    [SerializeField] private Vector3 scaleBomb;

    private int Amount
    {
        get { return _amount; }
        set
        {
            _amount = value;
            PlayerPrefs.SetInt("BoostRocket", value);
            UIController.Instance.SetTextPanelBoostRocket(value, moneyAmount);
        }
    }
    
    private bool _selected;
    private float _timer;
    private ObjectRocket _boost;
    private int _amount;

    private void Start()
    {
        Amount = PlayerPrefs.HasKey("BoostRocket") ? PlayerPrefs.GetInt("BoostRocket") : startAmount;
    }
    
    public void ClickBoost()
    {
        if (Amount > 0 && Board.Instance.IsActivate)
        {
            Board.Instance.IsBoost = true;
            _selected = true;
            UIController.Instance.ActivatorBoostBlackPanel(true, 2);
        }        
        else if (Amount <= 0 && money.MoneyAmount >= moneyAmount)
        {
            money.RemoveMoney(moneyAmount);
            Amount++;
        }
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
            _boost.transform.localScale = scaleBomb;
            _boost.transform.rotation = tile.transform.rotation;
            _boost.transform.DOMoveZ(tile.transform.position.z, 0.5f).OnComplete(() =>
            {
                _boost.Finish += OnFinish;

                _boost.transform.DORotate(new Vector3(_boost.transform.eulerAngles.x, _boost.transform.eulerAngles.y, -10), 0.1f).OnComplete(() =>
                {
                    _boost.transform.DORotate(new Vector3(_boost.transform.eulerAngles.x, _boost.transform.eulerAngles.y, 0), 0.1f).OnComplete(() =>
                    {
                        _boost.Activate = true;
                    });
                });
            });
        }
    }

    public void ActivateMoves(Tile tile, float delayTime)
    {
        Board.Instance.IsActivate = false;
        UIController.Instance.ActivatorBoostBlackPanel(false, 0);
        _timer = 0;

        Vector3 pos = new Vector3(tile.transform.position.x, tile.transform.position.y,
            tile.transform.position.z - 0.1f);
        _boost = Instantiate(boost, pos, Quaternion.identity);
        _boost.transform.localScale = scaleBomb;
        _boost.transform.rotation = tile.transform.rotation;
        _boost.transform.DOMoveZ(tile.transform.position.z, 0.5f).OnComplete(() =>
        {
            _boost.Finish += OnFinish;

            _boost.transform
                .DORotate(new Vector3(_boost.transform.eulerAngles.x, _boost.transform.eulerAngles.y, -10), 0.1f)
                .OnComplete(() =>
                {
                    _boost.transform
                        .DORotate(new Vector3(_boost.transform.eulerAngles.x, _boost.transform.eulerAngles.y, 0),
                            0.1f).OnComplete(() => { StartCoroutine(TimeToMove(delayTime, tile)); });
                });
        });
    }

    private IEnumerator TimeToMove(float delayTime, Tile tile)
    {
        yield return new WaitForSeconds(delayTime);
        _boost.Activate = true;
        tile.IsRocket = false;
    }

    private void OnFinish()
    {
        _boost.Finish -= OnFinish;
        _selected = false;
        Destroy(_boost.gameObject);
        Board.Instance.FallTiles();
        Board.Instance.IsActivate = true;
    }
    
    public void Deactivate()
    {
        _selected = false;
    }

    public void CheckLevel(int currentLevel)
    {
        print("234324");
        if (currentLevel < level)
            UIController.Instance.LockBoostRocket(true, level);
        else
            UIController.Instance.LockBoostRocket(false, level);
    }
}
