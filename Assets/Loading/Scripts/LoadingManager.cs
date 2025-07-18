
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    public static LoadingManager instance;


    public Animator animator;
    public bool isLoadingScene = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        effect.SetActive(true);
        isLoadingScene = true;
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(sceneName);
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        isLoadingScene = false;
        effect.SetActive(false);
    }
}
