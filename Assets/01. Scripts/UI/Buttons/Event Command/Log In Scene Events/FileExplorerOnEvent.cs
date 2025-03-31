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
        }
    }
}
