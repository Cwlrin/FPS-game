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
        StartCoroutine(BlackFaded());
    }

    private void FadeToBlack()
    {
        ri.color = Color.Lerp(ri.color, Color.black, Time.deltaTime * fadeSpeed);
    }

    private void FadeToClear()
    {
        ri.color = Color.Lerp(ri.color, Color.clear, Time.deltaTime * fadeSpeed);
    }

    private IEnumerator BlackFaded()
    {
        ri.color = Color.clear;
        ri.enabled = true;
        while (ri.color.a < 0.99f)
        {
            FadeToBlack();
            yield return null;
        }

        ri.color = Color.black;
        yield return new WaitForSeconds(waitTime);
        while (ri.color.a > 0.01f)
        {
            FadeToClear();
            yield return null;
        }

        ri.color = Color.clear;
        ri.enabled= false;
    }
}