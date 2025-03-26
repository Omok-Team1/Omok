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
