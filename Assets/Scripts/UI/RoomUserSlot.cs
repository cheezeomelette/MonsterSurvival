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
        // �۽�.
        if (stream.IsWriting)
        {
            stream.SendNext(userText.text);
        }
        // ����.
        if (stream.IsReading)
        {
            userText.text = (string)stream.ReceiveNext();
        }
    }
}
