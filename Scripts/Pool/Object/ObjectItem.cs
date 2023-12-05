using UnityEngine;

public sealed class ObjectItem : MonoBehaviour
{
    public TypeObject Type;
    public Material MaterialLine;
    public Color colorSlider;
    public Color colorSliderMax;
    public Vector3 ItemScale;

    private void Start()
    {
        ItemScale = transform.GetChild(0).localScale;
    }
}
