using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UICanvas
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button homeBtn;

    private void OnEnable()
    {
        continueBtn.onClick.AddListener(OnClickContinueBtn);
        replayBtn.onClick.AddListener(OnClickReplayBtn);
        homeBtn.onClick.AddListener(OnClickHomeBtn);
    }

    private void OnDisable()
    {
        continueBtn.onClick.RemoveListener(OnClickContinueBtn);
        replayBtn.onClick.RemoveListener(OnClickReplayBtn);
        homeBtn.onClick.RemoveListener(OnClickHomeBtn);
    }

    private void OnClickHomeBtn()
    {
        LoadingManager.instance.LoadScene("");
    }

    private void OnClickReplayBtn()
    {
        LoadingManager.instance.LoadScene("");
    }

    private void OnClickContinueBtn()
    {
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<PauseUI>();
    }
}
