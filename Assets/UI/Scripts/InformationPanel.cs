using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    public void SetInfomation(Sprite icon, string des)
    {
        this.icon.sprite = icon;
        this.description.text = des;
    }
}
