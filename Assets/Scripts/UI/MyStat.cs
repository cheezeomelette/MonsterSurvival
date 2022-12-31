using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStat : Singleton<MyStat>
{
    [SerializeField] Image hpImage;
    [SerializeField] Text hpText;
    [SerializeField] Image steminaImage;
    [SerializeField] Text steminaText;

	// 체력 업데이트
	public void UpdateHp(float currentHp, float maxHp)
	{
		hpImage.fillAmount = currentHp / maxHp;
		// 소수점 올림
		hpText.text = Mathf.FloorToInt(currentHp).ToString();
	}

	public void UpdateStemina(float currentStemina, float maxStemina)
	{
		steminaImage.fillAmount = currentStemina / maxStemina;
		// 소수점 올림
		steminaText.text = Mathf.FloorToInt(currentStemina).ToString();
	}
}
