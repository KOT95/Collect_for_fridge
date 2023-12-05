using System;
using UnityEngine;

public class ObjectRocket : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private float speed;
    [SerializeField] private Transform[] rockets;
    [SerializeField] private ParticleSystem[] particleSystems;

    public event Action Finish = default;

    public bool Activate
    {
        get { return _activate; }
        set
        {
            _activate = value;

            foreach (var particle in particleSystems)
            {
                particle.Play();
            }
        }
    }

    private float _timer;
    private bool _isFinish;
    private bool _activate;

    private void Update()
    {
        if (Activate)
        {
            if (!_isFinish)
            {
                _timer += Time.deltaTime;

                if (_timer >= time)
                {
                    Finish?.Invoke();
                    _isFinish = true;
                }
            }

            foreach (var rocket in rockets)
            {
                rocket.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tile tile))
        {
            _timer = 0;
            Board.Instance.SetItemStorageBoost(tile);
        }
    }
}
