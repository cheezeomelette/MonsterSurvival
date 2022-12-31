using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : Singleton<PlayerHud>
{
	[SerializeField] Image redPanel;
	[SerializeField] Text respawnText;
	[SerializeField] Animation anim;

	private void Start()
	{
		respawnText.text = string.Empty;
	}

	public void GetDamaged()
	{
		StartCoroutine(Damaged());
	}

	public void CountDown(float time)
	{
		StartCoroutine(CCountDown(time));
	}
	IEnumerator Damaged()
	{
		Color color = new Color();
		color = redPanel.color;
		float currentRate = 0.3f;
		while (currentRate > 0f)
		{
			color.a = currentRate;
			currentRate -= Time.deltaTime;
			redPanel.color = color;
			yield return null;
		}
	}

	IEnumerator CCountDown(float time)
	{
		float currentTime = time;
		while (currentTime > 0)
		{
			anim.Play();                                // 돌아가는 애니메이션
			respawnText.text = Mathf.Ceil(currentTime).ToString();
			currentTime -= Time.deltaTime;
			yield return null;
		}
	}
}
