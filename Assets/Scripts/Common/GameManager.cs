using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

// ������ �帧 ����
public class GameManager : MonoBehaviourPunCallbacks, IInputManager
{
	// ���� ���� �� �������� ���߱� ���� �ٲ� �� �ִ� Time ����
	public static float gameDeltaTime => Time.deltaTime * magnification;
	// �ð��� ������ ���� ����
	public static float magnification;

	[SerializeField] GameObject playerPrefab;
	[SerializeField] GameObject optionWindow;
	[SerializeField] GameObject mineralPrefab;
	[SerializeField] Transform playerSpawnPoint;
	[SerializeField] Transform mineralsTransform;
	[SerializeField] Timer timer;
	[SerializeField] Option option;
	[SerializeField] Score score;

	Mineral[] minerals;
	// ���� ���� ����
	int interval = 30;
	int index = 0;
	bool isEnd = false;
	// �Է¸Ŵ���
	public string IName { get; set; }
	const string scriptName = "GameManager";
	const string scoreName = "Score";

	private void Start()
	{
		Debug.Log("Sync" + PhotonNetwork.AutomaticallySyncScene);
		IName = scriptName;
		// ���� Time�� �����ϰ� �ð� ����
		magnification = 1f;
		if (playerPrefab == null)
		{
			Debug.Log("������ ����");
			return;
		}
		else
		{
			// ���� �ε�Ǿ��� �� PlayerControl�� static������ player�� �������� �ʾҴٸ�
			if (PlayerControl.player == null)
			{
				// �÷��̾� ����
				GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.position, Quaternion.identity, 0);
				PlayerControl player = playerObject.GetComponent<PlayerControl>();
				// �ɼ�â�� ������ ���� ����
				option.Setup(player);
			}
		}

		// ȣ��Ʈ�� ���� ����
		if (PhotonNetwork.IsMasterClient)
		{
			List<Mineral> list = new();
			// ������ ������ Transform���� ���� ��ġ�� �ΰ� �ϳ��� ������
			foreach (Transform mt in mineralsTransform)
			{
				// ��ġ�� ����
				GameObject m = PhotonNetwork.Instantiate(mineralPrefab.name, mt.position, mt.rotation);
				Mineral mineral = m.GetComponent<Mineral>();
				// ������ �����ϱ� ���� ����Ʈ ����
				list.Add(mineral);
			}
			minerals = list.ToArray();
		}
		optionWindow.SetActive(false);
	}

	private void Update()
	{
		// �ɼ�â ����
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			optionWindow.SetActive(!optionWindow.activeSelf);
			// Ŀ�� Ȱ��ȭ
			Cursor.lockState = optionWindow.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
			Cursor.visible = optionWindow.activeSelf;
			// �ѱ� �Է� ����
			if (optionWindow.activeSelf)
				IInputManager.nameStack.Push(IName);
			else
				IInputManager.nameStack.Pop();
		}

		// ���� ���� �˸�
		if (timer.time > 300f && !isEnd)
		{
			isEnd = true;
			// �ð� ������ 0���� �ٲ㼭 �����
			magnification = 0f;
			// ���â ����
			score.ShowResult();
			// �Է� ����
			IInputManager.nameStack.Push(scoreName);
			// Ŀ�� Ȱ��ȭ
			Cursor.lockState = score.Panel.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
			Cursor.visible = score.Panel.activeSelf;
			return;
		}
		// ȣ��Ʈ�� ���� ����
		if (!PhotonNetwork.IsMasterClient)
			return;

		// interval���� ���� �����
		if (timer.time > interval * index)
		{
			foreach (Mineral mineral in minerals)
				mineral.gameObject.SetActive(true);
			index++;
		}
	}
	// �絵��
	public void Retry()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Network");
		}
	}
	// ��������
	public void QuitGame()
	{
		Application.Quit();
	}

	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}
