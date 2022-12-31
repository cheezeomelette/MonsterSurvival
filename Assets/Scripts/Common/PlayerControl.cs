using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Photon.Pun;

// �÷��̾��� �������� ������ ����
public class PlayerControl : MonoBehaviourPun, IInputManager
{
	// ���漭������ �ڽ��� �÷��̾ �����ϱ� ���� ����
	public static GameObject player;	

	[SerializeField] WeaponManager weaponManager;
	[SerializeField] Transform verticalPivot;
	[SerializeField] Transform weaponPivot;
	[SerializeField] Camera minimapCam;
	[SerializeField] float moveSpeed;
	[SerializeField] float interactionRadius;

	// ���� ������ ���� ����(��������)
	public float seneitivity = 1f;	

	private new Transform transform;
	// ī�޶� ȸ�� �ӵ�
	private float rotateSpeed = 300f;
	// ī�޶� ȸ�� �� �ִ밢���� �����ϱ� ���� ���簢��
	private float currentVertical;
	// ���� ���� �� ȸ���ӵ��� �ٲ� �ֱ����� ī�޶� ȸ�� �ӵ�
	private float currentRotateSpeed;

	// IInputManager ����
	public string IName { get ; set; }
	const string scriptName = "PControl";

	private void Awake()
	{
		if (photonView.IsMine)
		{
			// ���漭���� ���� �ڽ��� ������Ʈ�� ������ �� �Ҹ�
			PlayerControl.player = this.gameObject;
		}
		else
			Destroy(minimapCam.gameObject);		// �̴ϸ��� �÷��̾�� �������ֱ� ������ �ڽ��� �� �� ����� �ı��Ѵ�

		IName = scriptName;
		IInputManager.nameStack.Push(IName);	// IInputManager�� ���ÿ� �÷��̾� ������ ����Ѵ�.
	}
	private void Start()
	{
		if (photonView.IsMine)
		{
			// �ڽ��� �÷��̾� ������Ʈ�� ī�޶� �����ϱ� ���� �۾�
			Transform cam = Camera.main.transform;
			cam.position = verticalPivot.position;
			cam.rotation = verticalPivot.rotation;
			cam.SetParent(verticalPivot);
		}

		transform = base.transform;
		currentRotateSpeed = rotateSpeed;

		// ���콺�� �����
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		// �������� �÷��̾� ������Ʈ �� �ڽ��� �� �̿��� ������Ʈ�� ������ �� ���� �Ѵ�
		if (!photonView.IsMine)
			return;
		CameraRotate();
		CheckInteraction();
		MouseClick();
		// �κ��丮�� ���� �տ� ���
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			weaponManager.SwapWeapon(0);
			currentRotateSpeed = rotateSpeed;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			weaponManager.SwapWeapon(1);
			currentRotateSpeed = rotateSpeed;
		}
		// ������
		if (Input.GetKeyDown(KeyCode.R))
			weaponManager.Reloading();
	}

	// ī�޶� ȸ��
	private void CameraRotate()
	{
		if (!CanInput())
			return;
		float x = -Input.GetAxis("Mouse Y") * GameManager.gameDeltaTime * currentRotateSpeed * seneitivity;
		float y = Input.GetAxis("Mouse X") * GameManager.gameDeltaTime * currentRotateSpeed * seneitivity;

		Vector3 dirHorizontal = new Vector3(0, y, 0);

		currentVertical += x;
		currentVertical = Mathf.Clamp(currentVertical, -90, 90);

		verticalPivot.localEulerAngles = new Vector3(currentVertical, 0f, 0f);

		transform.eulerAngles += dirHorizontal;
	}

	// �������̽� ĳ���� �ֺ����� IInteraction�� ������ ������Ʈ�� Ž���Ѵ�
	private void CheckInteraction()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);
		// colliders�� IInteraction�� �����ִ� ���� �Ÿ������� �����Ѵ�.
		IInteraction[] interactions = colliders.
			Select(collider => collider.GetComponent<IInteraction>()).
			Where(c => c != null).
			OrderBy(x => Vector3.Distance(x.Position, transform.position)).ToArray();
		if (interactions.Count() <= 0)
		{
			InteractionUI.Instance.OffUI();
			return;
		}
		// ���尡��� ������Ʈ�� ��ȣ�ۿ� ui�� �Ҵ�.
		IInteraction findInteraction = interactions[0];
		InteractionUI.Instance.UpdateUI(findInteraction);
		// ��ȣ�ۿ�
		if (Input.GetKeyDown(KeyCode.F))
			findInteraction.OnInteraction();
	}

	private void MouseClick()
	{
		// �Է��� �������� Ȯ��
		if (!CanInput())
			return;
		// ��Ŭ�� �� �߻�
		if (Input.GetMouseButton(0))
		{
			weaponManager.Shoot();
		}
		// ��Ŭ�� ���� & �ܾƿ�
		if (Input.GetMouseButtonDown(1))
		{
			if (weaponManager.currentWeapon == null)
				return;

			// ������ Ŀ������ ȸ�� ������ ���߱� ���� ����
			float magnitude = weaponManager.ZoomIn();
			if (weaponManager.currentWeapon.isAim)
				currentRotateSpeed = rotateSpeed * magnitude;
			else
				currentRotateSpeed = rotateSpeed;
		}
	}

	// IInputManager�� �Լ�
	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}