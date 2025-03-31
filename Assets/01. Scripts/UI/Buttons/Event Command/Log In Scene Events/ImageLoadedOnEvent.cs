using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ImageLoadedOnEvent", menuName = "IOnEventSO/ImageLoadedOnEvent")]
public class ImageLoadedOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        byte[] bytes = msg.GetParameter<byte[]>();
        var uiImage = msg.GetParameter<GameObject>().GetComponentInChildren<Image>();

        //�Ϲ������� �ּ� ������(2,2)�� ����, �����δ� ���� �̹��� ũ�⿡ �°� �������� �ȴ�.
        Texture2D texture2D = new Texture2D(2, 2);

        if (texture2D.LoadImage(bytes) is true)
        {
            uiImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);

            if (_aspectFitter is not null)
            {
                _aspectFitter.aspectRatio = (float)texture2D.width / texture2D.height;
            }
        }
        else
        {
            throw new System.Exception("Invalid image format");
        }
    }

    private AspectRatioFitter _aspectFitter;
}
