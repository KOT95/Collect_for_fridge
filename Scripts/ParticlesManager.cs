using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ParticlesManager : MonoBehaviour
{
    [HideInInspector] public static ParticlesManager Current;

    public ParticleSystem SelectParticle;
    private ParticleSystem[] _selectParticles;

    public ParticleSystem DestroyParticle;
    private ParticleSystem[] _destroyParticles;

    public ParticleSystem SmallConfettiParticle;
    private ParticleSystem[] _smallConfettiParticles;


    private Transform _parentTransform;

    private Vector3 _poolPositionVector = new Vector3(0, -10f, -20f);
    private Vector3 _poolRotationVector = new Vector3(-90f, 0, 0);
    private List<Vector3> _confettiParticlesPositions = new List<Vector3>();


    private ParticleSystem[] _tempParticles;
    private void Awake()
    {
        Current = this;
        _parentTransform = new GameObject("ParticlesPool").transform;

        _selectParticles = new ParticleSystem[15];
        for (int i = 0; i < _selectParticles.Length; i++)
        {
            _selectParticles[i] = Instantiate(SelectParticle, _poolPositionVector, Quaternion.Euler(_poolRotationVector), _parentTransform);
            _selectParticles[i].Stop();
        }

        _destroyParticles = new ParticleSystem[15];
        for (int i = 0; i < _destroyParticles.Length; i++)
        {
            _destroyParticles[i] = Instantiate(DestroyParticle, _poolPositionVector, Quaternion.Euler(_poolRotationVector), _parentTransform);
            _destroyParticles[i].Stop();
        }

        _smallConfettiParticles = new ParticleSystem[15];
        for (int i = 0; i < _smallConfettiParticles.Length; i++)
        {
            _smallConfettiParticles[i] = Instantiate(SmallConfettiParticle, _poolPositionVector, Quaternion.Euler(_poolRotationVector), _parentTransform);
            _smallConfettiParticles[i].Stop();
        }

    }

    public void MakeConfettiParticlesWithDelay(Vector3 position, float delay = 1)
    {
        _confettiParticlesPositions.Add(position);

        Invoke(nameof(MakeConfettiParticlesAfterDelay), delay);
    }
    private void MakeConfettiParticlesAfterDelay()
    {
        for (int i = 0; i < _selectParticles.Length; i++)
        {
            if (!_selectParticles[i].isPlaying || i == _selectParticles.Length - 1)
            {
                _selectParticles[i].transform.position = _confettiParticlesPositions[0];
                _confettiParticlesPositions.RemoveAt(0);
                _selectParticles[i].Play();
                break;
            }
        }
    }
    public void MakeSelectParticles(Vector3 position, Material material)
    {
        for (int i = 0; i < _selectParticles.Length; i++)
        {
            if (!_selectParticles[i].isPlaying || i == _selectParticles.Length - 1)
            {
                Color tempColor = material.GetColor("_BaseColor");
                _tempParticles = _selectParticles[i].GetComponentsInChildren<ParticleSystem>();
                for (int j = 0; j < _tempParticles.Length; j++)
                {
                    _tempParticles[j].startColor = tempColor;
                }
                _selectParticles[i].transform.position = position;
                _selectParticles[i].Play();
                break;
            }
        }
    }
    public void MakeDestroyParticles(Vector3 position, Material material)
    {
        for (int i = 0; i < _destroyParticles.Length; i++)
        {
            if (!_destroyParticles[i].isPlaying || i == _destroyParticles.Length - 1)
            {
                Color tempColor = material.GetColor("_BaseColor");
                _tempParticles = _destroyParticles[i].GetComponentsInChildren<ParticleSystem>();
                for (int j = 0; j < _tempParticles.Length; j++)
                {
                    _tempParticles[j].startColor = tempColor;
                }
                _destroyParticles[i].transform.position = position;
                _destroyParticles[i].Play();
                break;
            }
        }
    }
}