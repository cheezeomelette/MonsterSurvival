using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// ���� hpUI
public class MonsterHpBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    // ������ �Ӹ����� ���� ���� offset
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    MonsterStatus target;

    float characterControllerHeight;
    Transform targetTransform;
    Vector3 targetPosition;
    Camera cam;

    private void Awake()
    {
        // HpBar�� �����ϴ� ĵ������ �θ�� ����
        transform.SetParent(GameObject.Find("HpCanvas").GetComponent<Transform>(), false);
    }

    // Ÿ�ټ��� �Լ�
    public void SetTarget(MonsterStatus target)
    {
        cam = Camera.main;
        if (target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        // Ÿ�� ����
        this.target = target;
        targetTransform = target.transform;

        // �Ӹ����� ���� ���� ����
         NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            characterControllerHeight = agent.height;
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
            hpBar.fillAmount = target.hp / target.maxHp;
        }
    }
    void LateUpdate()
    {
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;

            // ��ũ�� ��ǥ�� ��ȯ
			Vector3 screenPoint = cam.WorldToScreenPoint(targetPosition);
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
