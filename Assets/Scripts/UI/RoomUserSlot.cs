using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class RoomUserSlot : MonoBehaviour
{
    [SerializeField] Text userText;
    
    public void Setup(string userName)
	{
        userText.text = userName;
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 송신.
        if (stream.IsWriting)
        {
            stream.SendNext(userText.text);
        }
        // 수신.
        if (stream.IsReading)
        {
            userText.text = (string)stream.ReceiveNext();
        }
    }
}
