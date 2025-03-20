using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TempNetworkManager : Singleton<TempNetworkManager>
{
    //TEMP CODE
    [SerializeField] private IOnEventSO logInEvent;
    void Start()
    {
        EventManager.Instance.AddListener("RequestLogIn", logInEvent ,gameObject);
    }
    
    public void RequestSignIn(IDataFormat data)
    {
        try
        {
            byte[] bodyRaw = DataSerialize(data.GetJsonData());

            using (UnityWebRequest www = new UnityWebRequest(Constants.SERVER_URL + "users/signup",
                       UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                www.SendWebRequest();
                
                var message = new EventMessage("ResponseLogIn");
            
                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error : " + www.error);

                    if (www.responseCode == 409)
                    {
                        //TODO: 중복 사용자 생성 팝업 표시
                        Debug.Log("중복 사용자");
                        
                        message.AddParameter<string>("Fail");
                        message.AddParameter<float>(2.5f);
        
                        EventManager.Instance.PushEventMessageEvent(message);
                    }
                }
                else
                {
                    var result = www.downloadHandler.text;
                    Debug.Log(result);
                
                    //TODO: 회원가입 성공 팝업 표시
        
                    message.AddParameter<string>("Success");
                    message.AddParameter<float>(2.5f);
        
                    EventManager.Instance.PushEventMessageEvent(message);
                }
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
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
                    var resultCode = JsonUtility.FromJson<SigninResult>(result);
                    
                    Debug.Log("Wow " + resultCode.result);
                    Debug.Log(result);
                
                    //TODO: 회원가입 성공 팝업 표시
        
                    // message.AddParameter<string>("Success");
                    // message.AddParameter<float>(2.5f);
                    //
                    // EventManager.Instance.PushEventMessageEvent(message);
                }
            }
    }
    
    private byte[] DataSerialize(string data)
    {
        return System.Text.Encoding.UTF8.GetBytes(data);
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