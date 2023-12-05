using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class Shelf : MonoBehaviour
{
    [SerializeField] private TypeObject type;
    [SerializeField] private ShelfSlot[] points;
    [SerializeField] private Canvas[] canvas;
    [SerializeField] private TextMeshProUGUI[] textAmount;
    [SerializeField] private ParticleSystem particleSystem;

    public TypeObject Type { get { return type;} }

    private int _emptySlots;
    private Canvas _canvas;

    private void Awake()
    {
        foreach (var point in points) point.gameObject.SetActive(false);
        foreach (var c in canvas)
        {
            if (c.gameObject.activeSelf)
                _canvas = c;
        }
        _emptySlots = CheckEmptySlot();
    }

    public int CheckEmptySlot()
    {
        int num = 0;
        
        foreach (var point in points)
        {
            if (!point.gameObject.activeSelf)
                num++;
        }

        _emptySlots = num;
        SetText();

        return num;
    }

    private void SetText()
    {
        if (_emptySlots != 0)
        {
            foreach (var text in textAmount)
            {
                text.text = _emptySlots.ToString();
            }
        }
        else
        {
            foreach (var c in canvas)
            {
                c.gameObject.SetActive(false);
            }
        }
    }

    public ShelfSlot SlotEmpty()
    {
        foreach (var point in points)
            if (!point.gameObject.activeSelf)
                return point;

        return null;
    }

    public void Reset()
    {
        foreach (var point in points)
            point.gameObject.SetActive(false);
        
        _canvas.gameObject.SetActive(true);
        
        _emptySlots = CheckEmptySlot();
    }

    public void ActivateParticle()
    {
        if(!particleSystem.isPlaying)
            particleSystem.Play();
    }
}
