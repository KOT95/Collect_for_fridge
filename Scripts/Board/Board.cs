using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private Row[] rows;
    [SerializeField] private Pool pool;
    [SerializeField, Range(0, 0.9f)] private float maxAmountRandomType;
    [SerializeField] private bool readingObjectsOnTheSides;
    [SerializeField] private bool fullRandom;
    [SerializeField] private float timeItem;
    [SerializeField] private float delayTimeItem;
    [SerializeField] private Map map;

    public Tile[,] Tiles { get; private set; }
    public int Width
    {
        get { return Tiles.GetLength(0); }
    }
    public int Height
    {
        get { return Tiles.GetLength(1); }
    }
    public bool IsSelected { get; private set; }
    public bool IsActivate { get; set; }
    public event Action Step;
    public bool IsBoost { get; set; }

    private Storage _storage;
    private TypeObject[] _typeObjects;
    private IEnumerable<IBoost> _boosts;
    private SelectItem _selectItem;
    private BoostRocket _boostRocket;

    private void Awake()
    {
        Instance = this;
        _boosts = GetComponentsInChildren<IBoost>();
        _selectItem = GetComponent<SelectItem>();
        _boostRocket = GetComponent<BoostRocket>();
    }

    public void Activate(Storage storage, TypeObject[] typeObjects)
    {
        _storage = storage;
        _typeObjects = typeObjects;
        foreach (var boost in _boosts) boost.CheckLevel(map.CurrentLevel);
        Tiles = new Tile[rows.Max(row => row.Tiles.Length), rows.Length];
        SetTiles(false);
        _selectItem.ResetSlider();
        IsActivate = true;
        IsSelected = true;
    }

    public void Deactivate()
    {
        IsSelected = false;
        IsActivate = false;
        _typeObjects = null;

        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var tile = rows[y].Tiles[x];

                tile.Type = TypeObject.Null;
                ReturnObject(tile);
            }
        }
    }

    private void SetTiles(bool isAnim)
    {
        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var tile = rows[y].Tiles[x];

                if (tile.Type == TypeObject.Null && !tile.IsRocket)
                {
                    tile.X = x;
                    tile.Y = y;

                    float RandomType = Random.Range(0f, 1f);

                    if (RandomType > maxAmountRandomType)
                    {
                        if (readingObjectsOnTheSides)
                        {
                            List<TypeObject> typeObjects = new List<TypeObject>();

                            typeObjects.Add(tile.Bottom != null ? tile.Bottom.Type : TypeObject.Null);
                            typeObjects.Add(tile.Left != null ? tile.Left.Type : TypeObject.Null);
                            typeObjects.Add(tile.Right != null ? tile.Right.Type : TypeObject.Null);

                            float num = RandomType - maxAmountRandomType;

                            if (num <= num / 3)
                                tile.Type = typeObjects[0] != TypeObject.Null
                                    ? typeObjects[0]
                                    : _typeObjects[Random.Range(0, _typeObjects.Length)];
                            else if (num > num / 3 && num <= num / 1.5f)
                                tile.Type = typeObjects[1] != TypeObject.Null
                                    ? typeObjects[1]
                                    : _typeObjects[Random.Range(0, _typeObjects.Length)];
                            else if (num >= num / 1.5f)
                                tile.Type = typeObjects[2] != TypeObject.Null
                                    ? typeObjects[2]
                                    : _typeObjects[Random.Range(0, _typeObjects.Length)];
                        }
                        else
                            tile.Type = _typeObjects[Random.Range(0, _typeObjects.Length)];
                    }
                    else
                    {
                        if(!fullRandom)
                            tile.Type = _storage.RandomEmptyObjectType();
                        else
                            tile.Type = _typeObjects[Random.Range(0, _typeObjects.Length)];
                    }

                    PoolSetObject(tile, tile.Type, isAnim);
                    Tiles[x, y] = tile;
                }
            }
        }
    }

    private void PoolSetObject(Tile tile, TypeObject type, bool isAnim)
    {
        pool.SetObject(tile, type, isAnim);
    }

    public void StartSetItemsStorage(List<Tile> items, Tile firstTile, bool isRocket)
    {
        StartCoroutine(SetItemsStorage(items, firstTile, isRocket));
    }

    private IEnumerator SetItemsStorage(List<Tile> items, Tile firstTile, bool isRocket)
    {
        print(items.Count);
        IsSelected = false;

        int num = _storage.CheckEmptySlotsType(firstTile.Type);
        bool isDelay = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (num != 0)
            {
                isDelay = true;
                _storage.SetObjectToSlot(items[i].ObjectItem, delayTimeItem);
                items[i].Type = TypeObject.Null;
                ReturnObject(items[i]);
                num--;
                yield return new WaitForSeconds(timeItem);
            }
            else
                break;
        }

        foreach (Tile tile in items)
        {
            if (tile.Type != TypeObject.Null)
            {
                ObjectItem objectItem = tile.ObjectItem;
                if (objectItem.MaterialLine != null && !tile.ParticleSystem.isPlaying)
                {
                    //tile.ParticleSystem.GetComponent<Renderer>().material = objectItem.MaterialLine;
                    //tile.ParticleSystem.Play();

                    ParticlesManager.Current.MakeDestroyParticles(tile.transform.position, objectItem.MaterialLine);
                }

                tile.Type = TypeObject.Null;
                ReturnObject(tile);
                yield return new WaitForSeconds(timeItem);
            }
        }

        if (isRocket)
        {
            Tile item = items[items.Count - 1];
            item.IsRocket = true;
            _boostRocket.ActivateMoves(item, delayTimeItem);
        }

        if (isDelay)
            yield return new WaitForSeconds(delayTimeItem);

        FallTiles();
        IsSelected = true;
        Step?.Invoke();
    }

    public void SetItemStorageBoost(Tile tile)
    {
        if (tile.Type != TypeObject.Null)
        {
            int num = _storage.CheckEmptySlotsType(tile.Type);

            ObjectItem objectItem = tile.ObjectItem;
            if (objectItem.MaterialLine != null && !tile.ParticleSystem.isPlaying)
            {
                tile.ParticleSystem.GetComponent<Renderer>().material = objectItem.MaterialLine;
                tile.ParticleSystem.Play();
            }

            if (num != 0)
            {
                _storage.SetObjectToSlot(tile.ObjectItem, 0);
            }

            tile.Type = TypeObject.Null;
            ReturnObject(tile);
        }
    }

    private void ReturnObject(Tile tile)
    {
        tile.ObjectItem.transform.SetParent(pool.transform);
        tile.ObjectItem.gameObject.SetActive(false);
        tile.ObjectItem = null;
    }

    public void FallTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height - 2; y >= 0; y--)
            {
                if (rows[y].Tiles[x].Type == TypeObject.Null) continue;

                if (rows[y].Tiles[x].Bottom.Type == TypeObject.Null && !rows[y].Tiles[x].Bottom.IsRocket)
                {
                    for (int i = y + 1; i < Height; i++)
                    {
                        if (rows[i].Tiles[x].Type != TypeObject.Null && !rows[i - 1].Tiles[x].IsRocket)
                        {
                            rows[i - 1].Tiles[x].Type = rows[y].Tiles[x].Type;
                            rows[y].Tiles[x].ObjectItem.transform.SetParent(rows[i - 1].Tiles[x].transform);
                            rows[i - 1].Tiles[x].ObjectItem = rows[y].Tiles[x].ObjectItem;
                            ObjectItem objectItem = rows[i - 1].Tiles[x].ObjectItem;
                            objectItem.transform.DOLocalMoveY(-0.3f, 0.15f).OnComplete(() =>
                            {
                                objectItem.transform.DOLocalMoveY(0, 0.15f);
                            });
                            rows[y].Tiles[x].Type = TypeObject.Null;
                            rows[y].Tiles[x].ObjectItem = null;
                            break;
                        }
                        else if (rows[i].Tiles[x].Type != TypeObject.Null && rows[i - 1].Tiles[x].IsRocket)
                        {
                            rows[i - 2].Tiles[x].Type = rows[y].Tiles[x].Type;
                            rows[y].Tiles[x].ObjectItem.transform.SetParent(rows[i - 2].Tiles[x].transform);
                            rows[i - 2].Tiles[x].ObjectItem = rows[y].Tiles[x].ObjectItem;
                            ObjectItem objectItem = rows[i - 2].Tiles[x].ObjectItem;
                            objectItem.transform.DOLocalMoveY(-0.3f, 0.15f).OnComplete(() =>
                            {
                                objectItem.transform.DOLocalMoveY(0, 0.15f);
                            });
                            rows[y].Tiles[x].Type = TypeObject.Null;
                            rows[y].Tiles[x].ObjectItem = null;
                            break;
                        }
                        else if (i == Height - 1 && rows[i].Tiles[x].Type == TypeObject.Null && !rows[i].Tiles[x].IsRocket)
                        {
                            rows[i].Tiles[x].Type = rows[y].Tiles[x].Type;
                            rows[y].Tiles[x].ObjectItem.transform.SetParent(rows[i].Tiles[x].transform);
                            rows[i].Tiles[x].ObjectItem = rows[y].Tiles[x].ObjectItem;
                            ObjectItem objectItem = rows[i].Tiles[x].ObjectItem;
                            objectItem.transform.DOLocalMoveY(-0.3f, 0.15f).OnComplete(() =>
                            {
                                objectItem.transform.DOLocalMoveY(0, 0.15f);
                            });
                            rows[y].Tiles[x].Type = TypeObject.Null;
                            rows[y].Tiles[x].ObjectItem = null;
                            break;
                        }
                    }
                }
            }
        }

        SetTiles(true);
    }

    public void BoostActivate(Tile tile)
    {
        foreach (var boost in _boosts) boost.Activate(tile);
    }

    public void DeactivateBoosts()
    {
        foreach (var boost in _boosts) boost.Deactivate();
        UIController.Instance.ActivatorBoostBlackPanel(false, 0);
        IsActivate = true;
        IsBoost = false;
    }
}
