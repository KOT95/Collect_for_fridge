using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovementManager : MonoBehaviour
{
    [HideInInspector] public static ItemMovementManager Current;

    [SerializeField] private Board _board;
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float _curveHieght;
    [SerializeField] private float _forwardDistance;
    [SerializeField] private float _movementMinSpeed;
    [SerializeField] private float _movementSecondSpeed;
    [SerializeField] private float _movementMultipliedSpeed;
    [SerializeField] private float _maxDistance;

    private List<ShelfSlot> objectItems = new List<ShelfSlot>();
    private Vector3 _boardScale;
    private bool _movementStarted;

    public event Action onMovementComplete;

    private void Awake()
    {
        Current = this;
    }
    private void Start()
    {
        _boardScale = _board.GetComponent<RectTransform>().localScale;
    }
    private void FixedUpdate()
    {
        for (int i = objectItems.Count - 1; i >= 0; i--)
        {
            if (objectItems[i].IsMoving)
            {
                objectItems[i].MoveItem();
            }
            else
            {
                objectItems.RemoveAt(i);
            }
        }
        if (objectItems.Count <= 0 && _movementStarted)
        {
            _movementStarted = false;
            onMovementComplete?.Invoke();
        }
    }
    public void StartObjectMovement(ShelfSlot item, ObjectItem startItem)
    {
        if (!_movementStarted)
            _movementStarted = true;
        //item.gameObject.SetActive(true);
        item.PrepareForMovement(_maxDistance, _movementMinSpeed, _movementSecondSpeed, _movementMultipliedSpeed, _curveHieght, _board.transform.forward * -1f * _forwardDistance, startItem.transform.position, startItem.transform.GetChild(0).rotation, GetMultipliedScale(_boardScale, startItem.transform.GetChild(0).localScale), _movementCurve);
        objectItems.Add(item);
    }

    private Vector3 GetMultipliedScale(Vector3 vector1, Vector3 vector2)
    {
        return new Vector3(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z);
    }
}