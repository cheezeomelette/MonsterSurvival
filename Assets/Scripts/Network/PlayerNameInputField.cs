using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// ���漭������ ����� �г��� ����
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    // PlayerPrefs���� �˻� �� Ű
    const string playerNamePrefkey = "PlayerName";

    void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = this.GetComponent<InputField>();
        if(inputField!= null)
		{
            // PlayerPrefs���� ������ ����ϴ� �г��� �˻�
            if (PlayerPrefs.HasKey(playerNamePrefkey))
			{
                defaultName = PlayerPrefs.GetString(playerNamePrefkey);
                inputField.text = defaultName;
			}
		}
        PhotonNetwork.NickName = defaultName;
    }

    // �Է��� ������ ������ �г��� ����
    public void SetPlayerName(string value)
	{
        if(string.IsNullOrEmpty(value))
		{
            Debug.Log("�����");
            return;
		}
        // �����Ʈ��ũ�� �г��� ����
        PhotonNetwork.NickName = value;
        // PlayerPrefs�� �г��� ����
        PlayerPrefs.SetString(playerNamePrefkey, value);
	}
}
