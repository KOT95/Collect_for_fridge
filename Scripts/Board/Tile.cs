using UnityEngine;

public sealed class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    
    public int X { get; set; }
    public int Y { get; set; }
    
    private TypeObject _type;

    public TypeObject Type
    {
        get => _type;
        set
        {
            if(_type == value) return;

            _type = value;
        }
    }

    public ObjectItem ObjectItem;

    public Tile Left => X > 0 ? Board.Instance.Tiles[X - 1, Y] : null;
    public Tile Top => Y > 0 ? Board.Instance.Tiles[X, Y - 1] : null;
    public Tile Right => X < Board.Instance.Width - 1 ? Board.Instance.Tiles[X + 1, Y] : null;
    public Tile Bottom => Y < Board.Instance.Height - 1 ? Board.Instance.Tiles[X, Y + 1] : null;
    
    public ParticleSystem ParticleSystem => TryGetComponent(out ParticleSystem particleSystem) ? particleSystem : null;
    public bool IsRocket;
}
