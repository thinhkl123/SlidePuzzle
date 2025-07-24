using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : UICanvas
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button tutorialBtn;

    private void Start()
    {
        int curLevelId = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        if (curLevelId == 3)
        {
            tutorialBtn.gameObject.SetActive(true);
        }
        else
        {
            tutorialBtn.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
        tutorialBtn.onClick.AddListener(OnClickTutorialBtn);
    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(OnClickPauseBtn);
        tutorialBtn.onClick.RemoveListener(OnClickTutorialBtn);
    }

    private void OnClickPauseBtn()
    {
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<PauseUI>();
    }

    private void OnClickTutorialBtn()
    {
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<GuideUI>();
    }
}
