using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �÷��̾� �Ӹ����� ��� hpBar
public class PlayerHpBarUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] Text playerName;
    // �Ӹ����� ���� ���� ����
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    PlayerStatus target;
    float characterControllerHeight = 0f;
    Transform targetTransform;
    Vector3 targetPosition;

    private void Awake()
	{
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
	}

    // Ÿ�� ����
	public void SetTarget(PlayerStatus target)
    {
        if (target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        this.target = target;
        targetTransform = target.transform;

        if (playerName != null)
        {
            playerName.text = target.photonView.Owner.NickName;
        }

        CharacterController characterController = target.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // hp ����
        if (hpBar != null)
        {
            hpBar.fillAmount = target.hp/target.maxHp;
        }
    }
    void LateUpdate()
    {
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;

            // ��ũ�� ��ǥ�� ��ȯ
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPosition);
            // ��ǥ ��ȯ �� �ڵ����� �� �ݴ��� �ִ� ���Ͱ� ���̴� ����
            if (screenPoint.z < 0f)
            {
                // ȭ������� �̵����Ѽ� �Ⱥ��̰� ��
                screenPoint = new Vector3(-100, -100, 0);
            }
            // Ÿ�� �������� �Ӹ����� ����
            transform.position = screenPoint + screenOffset;
        }
    }
}
