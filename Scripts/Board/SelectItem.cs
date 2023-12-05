using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class SelectItem : MonoBehaviour
{
    [SerializeField] private float distanceLine;
    [SerializeField] private int maxSelectedTiles;
    [SerializeField] private CursorTrigger trigger;
    [SerializeField] private int movesToRockets;
    [Header("Slider")]
    [SerializeField] private Image slider;
    [SerializeField] private Image slider2;
    [SerializeField] private float speedSlider;
    [SerializeField] private float timeSwitchColor;

    private List<Tile> _selectedTiles = new List<Tile>();
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    private Tile _firstTile;
    private LineRenderer _currentLine;
    private Camera _camera;
    private Plane _plane;
    private float _time;
    private float _timeColor;
    private long[] _vibration = {0, 8, 5, 8};

    private void Awake()
    {
        _camera = Camera.main;
        Vector3 planePoint = transform.position;
        Vector3 planeNormal = transform.forward;
        _plane = new Plane(planeNormal, planePoint);
    }

    private void Update()
    {
        if(!Board.Instance.IsSelected || !Board.Instance.IsActivate) return;

        if (Board.Instance.IsBoost)
        {
            if (InputController.IsTouchOnScreen(TouchPhase.Moved))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                _plane.Raycast(ray, out float X);
                Vector3 curPos = ray.GetPoint(X);
                float distance = 0;
                
                trigger.transform.position = curPos;
                trigger.gameObject.SetActive(true);

                if (trigger.Tiles.Count != 0)
                {
                    var tile = trigger.Tiles.Last();
                    Board.Instance.BoostActivate(tile);
                    trigger.gameObject.SetActive(false);
                    Board.Instance.IsBoost = false;
                }
            }
            return;
        }

        if (_selectedTiles.Count != 0 && InputController.IsTouchOnScreen(TouchPhase.Moved))
        {
            for (int i = 0; i < _selectedTiles.Count; i++)
            {
                _selectedTiles[i].ObjectItem.transform.localScale = new Vector4(Mathf.PingPong(Time.time, 0.15f) + 1f,
                    _selectedTiles[i].ObjectItem.transform.localScale.x, _selectedTiles[i].ObjectItem.transform.localScale.y,
                    _selectedTiles[i].ObjectItem.transform.localScale.z);
            }
        }
        
        if (_selectedTiles.Count >= movesToRockets)
        {
            _timeColor += Time.deltaTime;
            
            ObjectItem objectItem = _firstTile.ObjectItem;

            if (_timeColor >= timeSwitchColor)
            {
                if (slider.color == objectItem.colorSlider)
                {
                    slider.color = objectItem.colorSliderMax;
                    slider2.color = objectItem.colorSliderMax;
                }
                else
                {
                    slider.color = objectItem.colorSlider;
                    slider2.color = objectItem.colorSlider;
                }

                _timeColor = 0;
            }
        }

        if (InputController.IsTouchOnScreen(TouchPhase.Moved))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            _plane.Raycast(ray, out float X);
            Vector3 curPos = ray.GetPoint(X);
            float distance = 0;
            trigger.gameObject.SetActive(true);

            if (_currentLine != null)
            {
                Vector3 f = curPos - _currentLine.GetPosition(0);

                if (f.magnitude > distanceLine)
                {
                    f = f.normalized * distanceLine;
                }

                _currentLine.SetPosition(1, _currentLine.GetPosition(0) + f);
                trigger.transform.position = _currentLine.GetPosition(0) + f;
            }
            else
                trigger.transform.position = curPos;

            if (trigger.Tiles.Count != 0)
            {
                var tile = trigger.Tiles.Last();

                if (tile != null)
                {
                    if (_firstTile == null)
                    {
                        _firstTile = tile;
                        slider.color = _firstTile.ObjectItem.MaterialLine.color;
                        slider2.color = _firstTile.ObjectItem.MaterialLine.color;
                        StartLine();
                    }
                    else
                    {
                        if (tile.Type == _firstTile.Type && !_selectedTiles.Contains(tile))
                        {
                            Vibration.Vibrate(_vibration, -1);
                            _selectedTiles.Add(tile);

                            ObjectItem objectItem = tile.ObjectItem;
                            if (objectItem.MaterialLine != null && !tile.ParticleSystem.isPlaying)
                            {
                                //tile.ParticleSystem.GetComponent<Renderer>().material = objectItem.MaterialLine;
                                //tile.ParticleSystem.Play();

                                ParticlesManager.Current.MakeSelectParticles(tile.transform.position,
                                    objectItem.MaterialLine);

                            }
                            //tile.SpriteRenderer.gameObject.SetActive(true);
                            //tile.SpriteRenderer.color = objectItem.MaterialLine.color;

                            Vector3 p1 = new Vector3(tile.transform.position.x, tile.transform.position.y,
                                tile.transform.position.z) + _plane.normal * 0.04f;

                            if (_selectedTiles.Count > 1)
                                DrawLine(_currentLine.GetPosition(0), p1);

                            _currentLine.SetPosition(0, p1);
                            _time = 0;
                        }
                        else if (tile.Type == _firstTile.Type && _selectedTiles.Contains(tile) &&
                                 tile != _selectedTiles[_selectedTiles.Count - 1])
                        {
                            _time += Time.deltaTime;
                            if (_time >= 0.1f)
                            {
                                Vibration.Vibrate(_vibration, -1);
                                int currentId = _selectedTiles.FindIndex(x => x == tile);
                                int count = _selectedTiles.Count;
                                int id = count - currentId;
                                print(currentId);
                                for (int i = 0; i < id - 1; i++)
                                {
                                    var obj = _lineRenderers[currentId].gameObject;
                                    //_selectedTiles[currentId + 1].SpriteRenderer.gameObject.SetActive(false);
                                    _selectedTiles.Remove(_selectedTiles[currentId + 1]);
                                    _lineRenderers.Remove(_lineRenderers[currentId]);
                                    Destroy(obj);
                                }

                                Vector3 p1 = new Vector3(tile.transform.position.x, tile.transform.position.y,
                                    tile.transform.position.z) + _plane.normal * 0.04f;
                                _currentLine.SetPosition(0, p1);
                            }
                        }
                        else
                            _time = 0;
                        
                        float num = _selectedTiles.Count / (float)movesToRockets;
                        num /= 2.5f;
                        slider.fillAmount = Mathf.MoveTowards(slider.fillAmount, num, speedSlider * Time.deltaTime);
                        slider2.fillAmount = Mathf.MoveTowards(slider.fillAmount, num, speedSlider * Time.deltaTime);
                    }
                }
            }

            return;
        }
        else
            _time = 0;
        
        if (_firstTile != null && InputController.IsTouchOnScreen(TouchPhase.Ended))
        {
            foreach (var tile in _selectedTiles) tile.transform.localScale = Vector3.one;

            if (_selectedTiles.Count >= maxSelectedTiles)
            {
                List<Tile> tiles = new List<Tile>();

                foreach (var tile in _selectedTiles) tiles.Add(tile);

                Board.Instance.StartSetItemsStorage(tiles, _firstTile, tiles.Count >= movesToRockets);
            }
            
            trigger.gameObject.SetActive(false);
            //for (int i = 0; i < _selectedTiles.Count; i++)  _selectedTiles[i].SpriteRenderer.gameObject.SetActive(false);

            _selectedTiles.Clear();
            ResetSlider();
            Destroy(_currentLine.gameObject);
            foreach (var line in _lineRenderers) Destroy(line.gameObject);
            _lineRenderers.Clear();
            _firstTile = null;
        }
    }

    private void StartLine()
    {
        if(_firstTile == null) return;

        GameObject obj = new GameObject();
        obj.transform.SetParent(this.transform);
        _currentLine = obj.AddComponent<LineRenderer>();
        _currentLine.sortingLayerName = "OnTop";
        _currentLine.sortingOrder = -1;
        _currentLine.textureMode = LineTextureMode.Tile;
        _currentLine.materials[0].mainTextureScale = new Vector2(2f / _currentLine.widthMultiplier, 1f);
        ObjectItem objectItem = _firstTile.ObjectItem;
        if(objectItem.MaterialLine != null)
            _currentLine.material = objectItem.MaterialLine;
        _currentLine.SetVertexCount(2);
        _currentLine.SetPosition(0, _firstTile.transform.position);
        _currentLine.SetPosition(1, _firstTile.transform.position);
        _currentLine.widthMultiplier = 0.02f;
        _currentLine.useWorldSpace = true;
    }

    private void DrawLine(Vector3 pos, Vector3 pos2)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(this.transform);
        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.sortingLayerName = "OnTop";
        line.sortingOrder = -1;
        line.textureMode = LineTextureMode.Tile;
        line.materials[0].mainTextureScale = new Vector2(2f / _currentLine.widthMultiplier, 1f);
        ObjectItem objectItem = _firstTile.ObjectItem;
        if(objectItem.MaterialLine != null)
            line.material = objectItem.MaterialLine;
        line.SetVertexCount(2);
        line.SetPosition(0, pos);
        line.SetPosition(1, pos2);
        line.widthMultiplier = 0.02f;
        line.useWorldSpace = true;
        _lineRenderers.Add(line);
    }

    public void ResetSlider()
    {
        slider.fillAmount = 0;
        slider2.fillAmount = 0;
    }
}
