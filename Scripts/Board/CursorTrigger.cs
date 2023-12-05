using System;
using System.Collections.Generic;
using UnityEngine;

public class CursorTrigger : MonoBehaviour
{
    public List<Tile> Tiles = new List<Tile>();

    private void OnEnable()
    {
        Tiles.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tile tile))
        {
            Tiles.Add(tile);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Tile tile))
        {
            Tiles.Remove(tile);
        }
    }
}
