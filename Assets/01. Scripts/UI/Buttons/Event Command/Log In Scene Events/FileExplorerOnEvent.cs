using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using FileWindows = System.Windows.Forms;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;
using UnityEngine.XR;

[CreateAssetMenu(fileName = "FileExplorerOnEvent", menuName = "IOnEventSO/FileExplorerOnEvent")]
public class FileExplorerOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        FileWindows.OpenFileDialog openFileDialog = new FileWindows.OpenFileDialog();
        openFileDialog.Filter = "이미지 파일 (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
        openFileDialog.Title = "파일 선택";
        
        if (openFileDialog.ShowDialog() == FileWindows.DialogResult.OK)
        {
            string filePath = openFileDialog.FileName;
            
            Debug.Log("선택한 파일 경로: " + filePath);
            
            TempNetworkManager.Instance.FileUpload(filePath);

            //var uiImage = msg.GetParameter<GameObject>().GetComponentInChildren<Image>();

            ////일반적으로 최소 사이즈(2,2)로 설정, 실제로는 실제 이미지 크기에 맞게 리사이즈 된다.
            //Texture2D texture2D = new Texture2D(2, 2);

            //if (texture2D.LoadImage(bytes) is true)
            //{
            //    uiImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);

            //    if (_aspectFitter is not null)
            //    {
            //        _aspectFitter.aspectRatio = (float)texture2D.width / texture2D.height;
            //    }
            //}
            //else
            //{
            //    throw new Exception("Invalid image format");
            //}
        }
    }

    private AspectRatioFitter _aspectFitter;
}
