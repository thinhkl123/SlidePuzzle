using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private Sprite unlockSpr;
    [SerializeField] private Sprite lockSpr;
    [SerializeField] private Button btn;
    public int levelId;

    private void OnEnable()
    {
        btn.onClick.AddListener(OnClickBtn);
    }

    private void OnDisable()
    {
        btn.onClick.RemoveListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        LoadingManager.instance.LoadScene("Board");
    }
}
