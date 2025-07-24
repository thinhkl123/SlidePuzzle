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
    }

    private void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(OnClickPauseBtn);
    }

    private void OnClickPauseBtn()
    {
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<PauseUI>();
    }
}
