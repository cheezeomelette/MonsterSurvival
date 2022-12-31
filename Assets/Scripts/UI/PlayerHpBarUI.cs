using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 플레이어 머리위에 띄울 hpBar
public class PlayerHpBarUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] Text playerName;
    // 머리위에 띄우기 위한 간격
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    PlayerStatus target;
    float characterControllerHeight = 0f;
    Transform targetTransform;
    Vector3 targetPosition;

    private void Awake()
	{
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
	}

    // 타겟 설정
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

        // hp 적용
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

            // 스크린 좌표로 변환
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPosition);
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
