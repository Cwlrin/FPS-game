using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] public Canvas canvas;
    [SerializeField] public Button startButton;
    [SerializeField] public Button settingButton;
    [SerializeField] public Button quitButton;
    public void StartGame(Button button)
    {
        Debug.Log("Start Game");
        StartCoroutine(LoadSceneDelayed("SampleScene", 1.0f)); // 加载场景
    }

    public void SettingGame(Button button)
    {
        Debug.Log("Setting Game");
        canvas.gameObject.SetActive(true);
        Debug.Log("Destroy Button");
        startButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    public void QuitGame(Button button)
    {
        Application.Quit(); // 退出游戏
    }

    IEnumerator LoadSceneDelayed(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // 等待 delayTime 秒
        SceneManager.LoadScene(sceneName); // 加载场景
    }
}