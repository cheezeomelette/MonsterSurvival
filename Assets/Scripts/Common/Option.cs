using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 볼륨과 감도를 조절하는 옵션창
public class Option : MonoBehaviour
{
	private PlayerControl player;

    public void Setup(PlayerControl player)
	{
		AudioListener.volume = 0.5f;
		player.seneitivity = 0.5f;
		this.player = player; 
	}

    public void ChangeVolume(float value)
	{
		AudioListener.volume = value;
		Debug.Log("ChangeVolume"+value);
	}

    public void ChangeSensitivity(float value)
	{
		player.seneitivity = value;
	}
}
