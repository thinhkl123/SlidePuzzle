using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseUI : UICanvas
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button homeBtn;

    private void OnEnable()
    {
        replayBtn.onClick.AddListener(OnClickReplayBtn);
        homeBtn.onClick.AddListener(OnClickHomeBtn);
    }

    private void OnDisable()
    {
        replayBtn.onClick.RemoveListener(OnClickReplayBtn);
        homeBtn.onClick.RemoveListener(OnClickHomeBtn);
    }

    private void OnClickHomeBtn()
    {
        LoadingManager.instance.LoadScene("Menu");
    }

    private void OnClickReplayBtn()
    {
        LoadingManager.instance.LoadScene("Board");
    }
}
