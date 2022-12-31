using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// 포톤서버에서 사용할 닉네임 관리
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    // PlayerPrefs에서 검색 할 키
    const string playerNamePrefkey = "PlayerName";

    void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = this.GetComponent<InputField>();
        if(inputField!= null)
		{
            // PlayerPrefs에서 이전에 사용하던 닉네임 검색
            if (PlayerPrefs.HasKey(playerNamePrefkey))
			{
                defaultName = PlayerPrefs.GetString(playerNamePrefkey);
                inputField.text = defaultName;
			}
		}
        PhotonNetwork.NickName = defaultName;
    }

    // 입력이 감지될 때마다 닉네임 설정
    public void SetPlayerName(string value)
	{
        if(string.IsNullOrEmpty(value))
		{
            Debug.Log("비었다");
            return;
		}
        // 포톤네트워크의 닉네임 변경
        PhotonNetwork.NickName = value;
        // PlayerPrefs에 닉네임 저장
        PlayerPrefs.SetString(playerNamePrefkey, value);
	}
}
