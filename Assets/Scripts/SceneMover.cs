using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// �� �̵� �� ���̵��� ���̵�ƿ� ���
public class SceneMover : MonoBehaviour
{
    [SerializeField] Image blindImage;

    [SerializeField] float fadeTime;
    [SerializeField] bool isStopFadeIn;

	IEnumerator Start()
    {
        
        if (isStopFadeIn)
            blindImage.enabled = false;

        else
        {
            blindImage.enabled = true;
            Color color = new Color(0f, 0f, 0f, 1f);
            blindImage.color = color;
            float time = fadeTime;

            while (time > 0)
            {
                time = Mathf.Clamp(time - Time.deltaTime, 0f, fadeTime);
                color.a = time / fadeTime;
                blindImage.color = color;
                yield return null;
            }
            blindImage.enabled = false;
        }
    }
    // �� �̵� �� ���̵� �ƿ�
    public void LoadScene(string sceneName)
    {
        StartCoroutine(Load(fadeTime));
    }
    IEnumerator Load(float fadeTime)
    {
        blindImage.enabled = true;
        Color color = new Color(0f, 0f, 0f, 0f);
        blindImage.color = color;
        float time = 0;

        while (time < fadeTime)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0f, fadeTime);
            color.a = time / fadeTime;
            blindImage.color = color;
            yield return null;
        }
        blindImage.enabled = false;
    }
}