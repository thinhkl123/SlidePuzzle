using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;

    public void OnClickStartBtn()
    {
        LoadingManager.instance.LoadScene("Level 1");
    }

    public void OnClickQuitBtn()
    {
        Application.Quit();
    }
}
