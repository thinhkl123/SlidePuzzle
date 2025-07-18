using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : UICanvas
{
    [SerializeField] private InformationPanel informationPanelPrefab;
    [SerializeField] private GameObject scrollView;
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
        UIManager.Instance.CloseUI<GuideUI>();
    }

    private void Start()
    {
        
    }
}
