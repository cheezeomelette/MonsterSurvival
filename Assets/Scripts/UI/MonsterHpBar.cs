using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 몬스터 hpUI
public class MonsterHpBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    // 몬스터의 머리위에 띄우기 위한 offset
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    MonsterStatus target;

    float characterControllerHeight;
    Transform targetTransform;
    Vector3 targetPosition;
    Camera cam;

    private void Awake()
    {
        // HpBar만 관리하는 캔버스를 부모로 설정
        transform.SetParent(GameObject.Find("HpCanvas").GetComponent<Transform>(), false);
    }

    // 타겟설정 함수
    public void SetTarget(MonsterStatus target)
    {
        cam = Camera.main;
        if (target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        // 타겟 설정
        this.target = target;
        targetTransform = target.transform;

        // 머리위에 띄우기 위한 높이
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

        // hp 적용
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

            // 스크린 좌표로 변환
			Vector3 screenPoint = cam.WorldToScreenPoint(targetPosition);
            // 좌표 변환 시 뒤돌았을 때 반대편에 있는 몬스터가 보이는 현상
			if (screenPoint.z < 0f)
			{
                // 화면밖으로 이동시켜서 안보이게 함
				screenPoint = new Vector3(-100, -100, 0);
			}
            // 타겟 포지선의 머리위로 설정
            transform.position = screenPoint + screenOffset;
        }
    }
}
