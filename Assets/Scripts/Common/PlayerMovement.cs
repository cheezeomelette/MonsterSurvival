using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// �÷��̾��� �������� ���
public class PlayerMovement : MonoBehaviourPun
{
	[SerializeField] float moveSpeed;
	[SerializeField] float runSpeed;
	[SerializeField] float jumpHeight;

	[Range(1.0f, 4.0f)]
	[SerializeField] float gravityScale;        // �߷� ����.

	[SerializeField] float groundRadius;
	[SerializeField] LayerMask groundMask;

	// ĳ���� ��Ʈ�ѷ��� ����ؼ� �����δ�
	CharacterController controller;
	PlayerStatus status;

	Vector3 velocity;
	float GRAVITY => -9.8f * gravityScale;
	bool isGrounded;

	private void Start()
	{
		// ĳ��
		controller = GetComponent<CharacterController>();
		status = GetComponent<PlayerStatus>();
	}
	private void Update()
	{
		// ���� �÷��̾ ���� ����
		if (!photonView.IsMine)
			return;

		CheckGround();
		Movement();
		Jump();
		Gravity();
	}

	// �ٴڿ� ����ִ��� üũ
	void CheckGround()
	{
		isGrounded = Physics.CheckSphere(transform.position + (Vector3.up * 0.4f), groundRadius, groundMask);
	}

	// �յ��¿� ������
	void Movement()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");

		// stemina�� ����ϰ� shift�� ������ �޸��� ����
		bool isRun = Input.GetKey(KeyCode.LeftShift) && status.stemina > 1;
		
		// ���⸸ ��Ÿ���� ���� �������ͷ� ��ȯ
		Vector3 direction = ((transform.right * x) + (transform.forward * z).normalized);
		controller.Move(direction * (isRun ? runSpeed : moveSpeed) * GameManager.gameDeltaTime);

		// �޸��� �� ���׹̳� ���
		if (isRun && direction != Vector3.zero)
			status.UseStemina(10 * Time.deltaTime);
	}

	// ����
	void Jump()
	{
		if (isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			// ���̿� ���� �ӵ� ����
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY);
		}
	}
	// �߷� ����
	void Gravity()
	{
		// �ٴڿ� ��� ���� �� ����ó�� ������ ����
		if (isGrounded && velocity.y < 0.0f)
			velocity.y = -2f;

		velocity.y += GRAVITY * GameManager.gameDeltaTime;         // �߷� ���ӵ�.
		controller.Move(velocity * GameManager.gameDeltaTime);     // �Ʒ������� �̵�.
	}

	// �ٴ� ���� üũ�� ���� �����
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position + (Vector3.up * 0.4f), groundRadius);
	}
}
