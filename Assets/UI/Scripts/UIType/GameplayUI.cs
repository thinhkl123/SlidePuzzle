using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : UICanvas
{
    [SerializeField] private Button pauseBtn;

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

    private void Start()
    {
        
    }

}
