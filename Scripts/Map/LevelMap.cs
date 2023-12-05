using DG.Tweening;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
    [SerializeField] private Transform panel;
    [SerializeField] private RectTransform whiteLine;
    [SerializeField] private LevelMapText close, current, passed;
    
    public Level Level { get; set; }
    public int Number
    {
        get { return _number; }
        set
        {
            _number = value;
            close.SetText(value);
            current.SetText(value);
            passed.SetText(value);
        }
    }

    private int _number;

    public void SetStatus(int num, bool isAnim)
    {
        if (isAnim && num == 1)
        {
            whiteLine.DOAnchorPosY(-150, 0.5f);
            whiteLine.DOSizeDelta(new Vector2(whiteLine.sizeDelta.x, 300), 0.5f).OnComplete(() =>
            {
                current.gameObject.SetActive(true);
                
                close.gameObject.SetActive(false);
                passed.gameObject.SetActive(false);
                
                panel.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.2f).OnComplete(() =>
                {
                    panel.DOScale(Vector3.one, 0.2f);
                });
            });
        }
        else
        {
            if (num != 1)
            {
                close.gameObject.SetActive(num == 0);
                passed.gameObject.SetActive(num == 2);
                current.gameObject.SetActive(false);
            }
            else
            {
                close.gameObject.SetActive(false);
                passed.gameObject.SetActive(false);
                current.gameObject.SetActive(true);

                if (whiteLine != null)
                {
                    whiteLine.sizeDelta = new Vector2(whiteLine.sizeDelta.x, 300);
                    whiteLine.anchoredPosition = new Vector2(whiteLine.anchoredPosition.x, -150);
                }
            }
        }
    }
}
