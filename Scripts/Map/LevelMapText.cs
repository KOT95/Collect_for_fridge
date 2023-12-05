using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelMapText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(int num)
    {
        text.text = num.ToString();
    }
}
