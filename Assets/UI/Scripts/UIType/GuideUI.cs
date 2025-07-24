using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : UICanvas
{
    [SerializeField] private Button closeBtn;

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnClickCloseBtn);
    }

    private void OnClickCloseBtn()
    {
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<GuideUI>();
    }
}
