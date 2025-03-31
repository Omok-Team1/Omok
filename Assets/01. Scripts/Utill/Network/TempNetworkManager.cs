using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TempNetworkManager : Singleton<TempNetworkManager>, INetworkManager
{
    //TEMP CODE
    [SerializeField] private IOnEventSO logInEvent;
    //TEMP CODE
    [SerializeField] private IOnEventSO signupEvent;

    [SerializeField] private IOnEventSO fileUploadEvent;
    [SerializeField] private IOnEventSO requestFileEvent;

    void Start()
    {
        EventManager.Instance.AddListener("RequestLogIn", logInEvent ,gameObject);
        EventManager.Instance.AddListener("RequestSignUp", signupEvent ,gameObject);
        EventManager.Instance.AddListener("FileUpload", fileUploadEvent, gameObject);
        EventManager.Instance.AddListener("RequestFile", requestFileEvent, gameObject);
    }

    // 파일 업로드
    public void FileUpload(string filePath)
    {
        StartCoroutine(Fileupload(filePath));
    }
    public IEnumerator Fileupload(string filePath)
    {
        //Server에 올릴 이미지 데이터
        byte[] bytes = File.ReadAllBytes(filePath);

        // 서버에 이미지 파일 업로드 요청
        using (UnityWebRequest www =
            new UnityWebRequest(Constants.SERVER_URL + "/users/profileImageUpload", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "multipart/form-data");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + www.error);
            }
            else
            {
                var message = new EventMessage("ImageUploaded");

                var filename = www.downloadHandler.text;
                Debug.Log("Result: " + filename);

                message.AddParameter<string>(filename);

                EventManager.Instance.PushEventMessageEvent(message);
                EventManager.Instance.PublishMessageQueue();
            }
        }
    }

    // 프로필 이미지 파일 요청
    public void RequestProfilePic(string filename)
    {
        StartCoroutine(Requestprofilepic(filename));
    }
    public IEnumerator Requestprofilepic(string filename)
    {
        string jsonString = JsonUtility.ToJson(new FilePathData(filename));
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        // 서버에 이미지 파일 요청
        using (UnityWebRequest www =
            new UnityWebRequest(Constants.SERVER_URL + "/users/profileImageUpload", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + www.error);
            }
            else
            {
                var message = new EventMessage("ImageLoaded");
                message.AddParameter<string>(filename);

                byte[] result = www.downloadHandler.data;

                EventManager.Instance.PushEventMessageEvent(message);
                EventManager.Instance.PublishMessageQueue();
            }
        }
    }
    public struct FilePathData
    {
        public string filename;
        public FilePathData(string filename) { this.filename = filename; }
    }

    public void RequestSignUp(SignUpData data)
    {
        StartCoroutine(RequestsignUp(data));
    }
    
    public IEnumerator RequestsignUp(SignUpData data)
    {
        string jsonString = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = 
            new UnityWebRequest(Constants.SERVER_URL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            var message = new EventMessage("ResponseSignUp");

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + www.error);
                if (www.responseCode == 400)
                {
                    Debug.Log("회원가입 실패");
                    var result = www.downloadHandler.text;
                    Debug.Log("Result: " + result);
                    message.AddParameter<string>(result);
                }
                else if (www.responseCode == 409)
                {
                    Debug.Log("이미 존재하는 사용자");
                    message.AddParameter<string>("Existing Username");
                }
            }
            else
            {
                message.AddParameter<string>("Success");

                var result = www.downloadHandler.text;
                Debug.Log("Result: " + result);
            }
            EventManager.Instance.PushEventMessageEvent(message);
            EventManager.Instance.PublishMessageQueue();
        }
    }

    public void RequestLogIn(SignInData data)
    {
        StartCoroutine(Requestsignin(data));
    }
    
    public IEnumerator Requestsignin(SignInData data)
    {
        string jsonString = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = 
            new UnityWebRequest(Constants.SERVER_URL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            EventMessage message = null;

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + www.error);

                message = new EventMessage("LogInFailed");
                message.AddParameter<string>(www.error);
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);
                Debug.Log("result: " + result.result);
                
                // 유저네임 유효하지 않음
                if (result.result == 0)
                {
                    message = new EventMessage("LogInFailed");
                    message.AddParameter<string>("Invaild Login");
                }
                // 로그인 성공
                else if (result.result == 1)
                {
                    message = new EventMessage("ResponseLogIn");
                    message.AddParameter<string>("Success");
                }
                // TOO_MANY_REQUEST
                else if (result.result == 2)
                {
                    message = new EventMessage("LogInFailed");
                    message.AddParameter<string>("Too Many Request");
                }
                // 중복 로그인
                else if (result.result == 3)
                {
                    message = new EventMessage("LogInFailed");
                    message.AddParameter<string>("duplicate login");
                }
            }
            EventManager.Instance.PushEventMessageEvent(message);
            EventManager.Instance.PublishMessageQueue();
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

public struct SignInData
{
    public string username;
    public string password;

    public SignInData(string username, string password)
    {
        this.username = username;
        this.password = password;
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
    public string imageFilename;

    public SignUpData(string inputString, string password, string nickname, string filename)
    {
        this.username = inputString;
        this.password = password;
        this.nickname = nickname;
        imageFilename = filename;
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