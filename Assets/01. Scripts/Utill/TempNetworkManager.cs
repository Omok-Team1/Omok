using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TempNetworkManager : Singleton<TempNetworkManager>
{
    //TEMP CODE
    [SerializeField] private IOnEventSO logInEvent;
    //TEMP CODE
    [SerializeField] private IOnEventSO signupEvent;
    
    void Start()
    {
        EventManager.Instance.AddListener("RequestLogIn", logInEvent ,gameObject);
        EventManager.Instance.AddListener("RequestSignUp", signupEvent ,gameObject);
    }

    public void requestsignup(SignUpData data)
    {
        StartCoroutine(RequestSignUp(data));
    }
    
    public IEnumerator RequestSignUp(SignUpData data)
    {
        Debug.Log(GetJsonData(data) + " : Sign Up Data");
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
                    Debug.Log(result);
                    Debug.Log("Success");
                    
                    var message = new EventMessage("ResponseSignUp");
                        
                    message.AddParameter<string>("Success");

                    EventManager.Instance.PushEventMessageEvent(message);
                    EventManager.Instance.PublishMessageQueue();
                
                    //TODO: 회원가입 성공 팝업 표시
                }
            }
    }

    public void RequestLogIn(SigninData data)
    {
        StartCoroutine(Requestsignin(data));
    }
    
    public IEnumerator Requestsignin(SigninData data)
    {
            byte[] bodyRaw = DataSerialize(data.GetJsonData());

            using (UnityWebRequest www = new UnityWebRequest(Constants.SERVER_URL + "/users/signin",
                       UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error : " + www.error);

                    if (www.responseCode == 400)
                    {
                        //TODO: 중복 사용자 생성 팝업 표시
                        Debug.Log("아이디와 비밀번호 둘 다 입력해주세요");
                        
                        var message = new EventMessage("LogInFailed");
                        
                        message.AddParameter<string>(www.error);

                        EventManager.Instance.PushEventMessageEvent(message);
                        EventManager.Instance.PublishMessageQueue();
                    }
                }
                else
                {
                    var result = www.downloadHandler.text;
                    Debug.Log(result);
                    //var resultCode = JsonUtility.FromJson<SigninResult>(result);
                    
                    //Debug.Log("Wow " + resultCode.result);
                
                    //TODO: 회원가입 성공 팝업 표시
        
                    // message.AddParameter<string>("Success");
                    // message.AddParameter<float>(2.5f);
                    //
                    // EventManager.Instance.PushEventMessageEvent(message);
                    
                    var message = new EventMessage("ResponseLogIn");
                        
                    message.AddParameter<string>("Success");

                    EventManager.Instance.PushEventMessageEvent(message);
                    EventManager.Instance.PublishMessageQueue();
                }
            }
    }
    
    private byte[] DataSerialize(string data)
    {
        return System.Text.Encoding.UTF8.GetBytes(data);
    }
    public string GetJsonData(SignUpData data)
    {
        return JsonUtility.ToJson(data);
    }
}

public struct SigninResult
{
    public int result;
}

public struct SigninData
{
    public string username;
    public string password;

    public SigninData(string inputString, string s)
    {
        username = inputString;
        password = s;
    }
    
    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }
}

public struct SignUpData
{
    public string username;
    public string password;
    public string nickname;
}
// public struct SignUpData
// {
//     public string username;
//     public string password;
//     public string nickname;
//     //public byte[] image;
//
//     // public SignUpData(string inputString, string s, byte[] image)
//     // {
//     //     username = inputString;
//     //     password = s;
//     //     this.image = image;
//     // }
//     
//     public SignUpData(string inputString, string s, string s1)
//     {
//         username = inputString;
//         password = s;
//         nickname = s1;
//     }
//     
//     public string GetJsonData()
//     {
//         return JsonUtility.ToJson(this);
//     }
// }

public interface IDataFormat
{
    string GetJsonData();
}

public class RequestData : IDataFormat
{
    private string _username;
    private string _nickname;
    private string _password;

    public RequestData(string username, string nickname, string password)
    {
        _username = username;
        _nickname = nickname;
        _password = password;
    }
    
    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }
}