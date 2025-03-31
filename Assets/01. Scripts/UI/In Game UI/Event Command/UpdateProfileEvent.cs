using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상대방의 이름, 프로필 사진을 초기화 하는 이벤트입니다.
/// StartState에서 실행됩니다.
/// </summary>
[CreateAssetMenu(fileName = "UpdateProfileEvent", menuName = "IOnEventSO/UpdateProfileEvent")]
public class UpdateProfileEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        // Null 체크 추가: 파괴된 오브젝트 참조 방지
        if (listenerObj == null) 
        {
            Debug.LogWarning("listenerObj가 null입니다. 이벤트를 건너뜁니다.");
            return;
        }

        // 기존 로직 유지
        var nicknameData = msg.GetParameter<string>();
        var spriteData = msg.GetParameter<Sprite>();

        if (listenerObj.TryGetComponent(out TextMeshProUGUI nickname))
        {
            nickname.text = nicknameData;
        }

        if (listenerObj.TryGetComponent(out Image profile))
        {
            profile.sprite = spriteData;
        }
    }
}
