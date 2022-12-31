using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Timer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Text minuteText;
    [SerializeField] Text secontText;

	public float time;

	int minute;
	int second;

	private void Update()
	{
		time += Time.deltaTime;
		UpdateUI();
	}

	private void UpdateUI()
	{
		minute = (int)(time / 60);
		second = (int)(time % 60);
		minuteText.text = minute.ToString("0#");
		secontText.text = second.ToString("0#");
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
			stream.SendNext(time);
		else
			time = (float)stream.ReceiveNext();
	}
}
