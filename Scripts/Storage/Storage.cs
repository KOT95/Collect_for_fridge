using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public sealed class Storage : MonoBehaviour
{
    [SerializeField] private Shelf[] shelves;
    [SerializeField] private float timeMoveItem;
    [SerializeField] private float timeMoveShelf;
    [SerializeField] private Transform point;

    public event Action Win = default;

    private List<Shelf> _currentShelfs = new List<Shelf>();
    private bool _isWin = false;

    private void Start()
    {
        ItemMovementManager.Current.onMovementComplete += FinishMovingObjects;
    }

    public int CheckEmptySlotsType(TypeObject type)
    {
        int num = 0;

        foreach (var shelf in shelves)
        {
            if (shelf.Type == type && shelf.CheckEmptySlot() != 0)
                num += shelf.CheckEmptySlot();
        }

        return num;
    }

    public void SetObjectToSlot(ObjectItem item, float delayTimeItem)
    {
        foreach (Shelf shelf in shelves)
        {
            if (shelf.Type == item.Type && shelf.CheckEmptySlot() != 0)
            {
                _currentShelfs.Add(shelf);
                shelf.transform.DOKill();

                ShelfSlot slot = shelf.SlotEmpty();
                Transform obj = slot.transform.GetChild(0).transform;

                shelf.transform.GetChild(0).DOLocalMoveZ(0.43f, timeMoveShelf);
                shelf.transform.GetChild(0).DOLocalRotate(new Vector3(20, 0, 0), timeMoveShelf);

                NewMovement(item, slot);
                //OriginalMovement(item, obj, slot,shelf, delayTimeItem);
            }
        }
    }

    private void NewMovement(ObjectItem item, ShelfSlot slot)
    {
        ItemMovementManager.Current.StartObjectMovement(slot, item);

        slot.gameObject.SetActive(true);

        if (CheckEmptyShelfs() == 0)
        {
            _isWin = true;
            Board.Instance.IsActivate = false;
        }

    }
    private void OriginalMovement(ObjectItem item, Transform obj, ShelfSlot slot, Shelf shelf, float delayTimeItem)
    {
        Quaternion startRot = obj.transform.rotation;
        Vector3 startScale = obj.transform.localScale;

        slot.gameObject.SetActive(true);

        if (CheckEmptyShelfs() == 0)
        {
            _isWin = true;
            Board.Instance.IsActivate = false;
        }

        obj.SetParent(null);
        obj.transform.position = item.transform.GetChild(0).position;
        obj.transform.rotation = item.transform.GetChild(0).rotation;

        Vector3 s1 = item.transform.GetChild(0).localScale;
        Vector3 s2 = item.transform.parent.parent.parent.localScale;

        Vector3 scale = new Vector3(s1.x * s2.x, s1.y * s2.y, s1.z * s2.z);
        obj.transform.localScale = scale;

        var centerPos = Vector3.Lerp(item.transform.position, slot.transform.position, 0.5f);
        centerPos.z = point.position.z;

        obj.transform.DOScale(new Vector3(scale.x + 0.1f, scale.y + 0.1f, scale.z + 0.1f), delayTimeItem);
        obj.transform
            .DOLocalRotate(new Vector3(obj.transform.eulerAngles.x, -100, obj.transform.eulerAngles.z),
                delayTimeItem / 2).OnComplete(
                () =>
                {
                    obj.transform
                        .DOLocalRotate(
                            new Vector3(obj.transform.eulerAngles.x, 0, obj.transform.eulerAngles.z),
                            delayTimeItem / 2);
                });

        obj.transform.DOMove(new Vector3(centerPos.x, centerPos.y + 1, centerPos.z), timeMoveItem / 2).OnComplete(() =>
        {
            obj.transform.DOMove(slot.transform.position, timeMoveItem / 2).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    obj.SetParent(slot.transform);
                    obj.transform.DOLocalMove(Vector3.zero, timeMoveShelf / 3);
                    obj.transform.DOLocalRotateQuaternion(Quaternion.identity, timeMoveShelf / 3);

                    shelf.transform.GetChild(0).DOLocalMoveZ(0, timeMoveShelf).SetDelay(timeMoveShelf / 3);
                    shelf.transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 0), timeMoveShelf).SetDelay(timeMoveShelf / 3).OnComplete(
                        () =>
                        {
                            shelf.ActivateParticle();
                            
                            if (_isWin)
                            {
                                Win?.Invoke();
                            }
                        });
                });
            obj.transform.DORotateQuaternion(startRot, timeMoveItem / 2);
            obj.transform.DOScale(startScale, timeMoveItem / 2);
        }).SetEase(Ease.OutCubic).SetDelay(delayTimeItem);
    }


    private void FinishMovingObjects()
    {
        for (int i = _currentShelfs.Count - 1; i >= 0; i--)
        {
            Shelf shelf = _currentShelfs[i];
            _currentShelfs.RemoveAt(i);
            
            shelf.transform.GetChild(0).DOLocalMoveZ(0, timeMoveShelf).SetDelay(timeMoveShelf / 3);
            shelf.transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 0), timeMoveShelf).SetDelay(timeMoveShelf / 3).OnComplete(
                () =>
                {
                    if(shelf.CheckEmptySlot() == 0 && !_currentShelfs.Contains(shelf))
                        shelf.ActivateParticle();
                });
        }

        if (_isWin)
        {
            Win?.Invoke();
        }
    }

    private int CheckEmptyShelfs()
    {
        int num = 0;

        foreach (var shelf in shelves)
        {
            if (shelf.CheckEmptySlot() != 0)
                num++;
        }

        return num;
    }

    public void Reset()
    {
        foreach (var shelf in shelves)
            shelf.Reset();
    }

    public TypeObject RandomEmptyObjectType()
    {
        List<Shelf> EmptyShelves = new List<Shelf>();

        foreach (var shelf in shelves)
        {
            if (shelf.CheckEmptySlot() != 0)
                EmptyShelves.Add(shelf);
        }
        
        if(EmptyShelves.Count != 0)
            return EmptyShelves[Random.Range(0, EmptyShelves.Count)].Type;

        return shelves[Random.Range(0, shelves.Length)].Type;
    }
    public Shelf[] GetActiveShelves()
    {
        shelves = GetComponentsInChildren<Shelf>();
        return shelves;
    }
}
