using UnityEngine;

public sealed class Row : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;

    public Tile[] Tiles
    {
        get => tiles;
    }
}
