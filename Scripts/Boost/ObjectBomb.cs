using UnityEngine;

public sealed class ObjectBomb : MonoBehaviour
{
    [SerializeField] private GameObject objBomb;
    [SerializeField] private ParticleSystem particleSystem;

    public void Explosion()
    {
        objBomb.SetActive(false);
        GetComponent<Collider>().enabled = true;
        particleSystem.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tile tile))
        {
            Board.Instance.SetItemStorageBoost(tile);
        }
    }
}
