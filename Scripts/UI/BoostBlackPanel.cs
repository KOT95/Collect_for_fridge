using UnityEngine;

public class BoostBlackPanel : MonoBehaviour
{
    [SerializeField] private Transform bomb;
    [SerializeField] private Transform rocket;

    public void ActivatePanel(int num)
    {
        bomb.gameObject.SetActive(num == 1);
        rocket.gameObject.SetActive(num == 2);
    }
}
