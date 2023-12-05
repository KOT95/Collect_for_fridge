using DG.Tweening;
using UnityEngine;

public sealed class Pool : MonoBehaviour
{
    [SerializeField] private ObjectItem[] objectsArray;
    [SerializeField] private int poolCount = 3;
    [SerializeField] private bool autoExpand = false;
    [SerializeField] private Transform spawnPoint;

    private PoolMono<ObjectItem> _pool;

    private void Awake()
    {
        _pool = new PoolMono<ObjectItem>(objectsArray, poolCount, transform);
        
        _pool.AutoExpand = autoExpand;
        Application.targetFrameRate = 60;
    }

    public void SetObject(Tile tile, TypeObject typeObject, bool isAnim)
    {
        var obj = _pool.GetFreeElement(typeObject);
        obj.transform.SetParent(tile.transform);
        Vector3 spawnPos = new Vector3(tile.transform.position.x, spawnPoint.position.y, tile.transform.position.z);
        obj.transform.localScale = Vector3.one;
        tile.ObjectItem = obj;
        obj.gameObject.SetActive(true);
        
        if (isAnim)
        {
            obj.transform.position = spawnPos;
            obj.transform.DOMoveY(tile.transform.position.y - 0.03f, 0.15f).OnComplete(() =>
            {
                obj.transform.DOMoveY(tile.transform.position.y, 0.15f);
            });
        }
        else
        {
            obj.transform.position = tile.transform.position;
        }
    }
}
