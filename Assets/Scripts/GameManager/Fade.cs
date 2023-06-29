using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public RawImage ri;
    public float fadeSpeed;
    public float waitTime;

    public void BlackFade()
    {
        Debug.Log("BlackFade");
        StartCoroutine(BlackFaded()); // 启动协程
    }

    private void FadeToBlack()
    {
        ri.color = Color.Lerp(ri.color, Color.black, Time.deltaTime * fadeSpeed); // 插值
    }

    private void FadeToClear()
    {
        ri.color = Color.Lerp(ri.color, Color.clear, Time.deltaTime * fadeSpeed); // 插值
    }

    private IEnumerator BlackFaded()
    {
        ri.color = Color.clear; // 设置透明
        ri.enabled = true; // 启用
        while (ri.color.a < 0.99f)
        {
            FadeToBlack(); // 淡入
            yield return null; // 等待一帧
        }

        ri.color = Color.black; // 设置黑色
        yield return new WaitForSeconds(waitTime); // 等待 waitTime 秒
        while (ri.color.a > 0.01f)
        {
            FadeToClear(); // 淡出
            yield return null; // 等待一帧
        }

        ri.color = Color.clear; // 设置透明
        ri.enabled= false; // 禁用
    }
}