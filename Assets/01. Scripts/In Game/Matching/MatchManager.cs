using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : MonoBehaviour
{
    public void RequestMatch()
    {
        
    }
    
    private IEnumerator RequestMatching(SignUpData data)
    {
        byte[] bodyRaw = DataSerialize(GetJsonData(data));

        using (UnityWebRequest www = new UnityWebRequest(Constants.SERVER_URL + "/users/signup",
                   UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Started Requesting Sign Up");
            yield return www.SendWebRequest();
            Debug.Log("End Requesting Sign Up");

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + www.error);

                var result = www.downloadHandler.text;
                // var resultCode = JsonUtility.FromJson<SigninResult>(result);
                //
                // Debug.Log("Wow " + resultCode.result);
                Debug.Log(result);

                if (www.responseCode == 409)
                {
                    //TODO: 중복 사용자 생성 팝업 표시
                    Debug.Log("중복 사용자");
                }
            }
            else
            {
                var result = www.downloadHandler.text;

                var message = new EventMessage("ResponseSignUp");

                message.AddParameter<string>("Success");

                EventManager.Instance.PushEventMessageEvent(message);
                EventManager.Instance.PublishMessageQueue();

                //TODO: 회원가입 성공 팝업 표시
            }
        }
    }
    
    private byte[] DataSerialize(string data)
    {
        return System.Text.Encoding.UTF8.GetBytes(data);
    }
    
    private string GetJsonData(SignUpData data)
    {
        return JsonUtility.ToJson(data);
    }
}