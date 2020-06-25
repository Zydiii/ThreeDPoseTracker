using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PhoneCamera : MonoBehaviour
{
    //---------------------------Instance----------------------------
    public static PhoneCamera _instance;
    public static PhoneCamera Instance
    {
        get
        {
            if(null == _instance)
            {
                _instance = FindObjectOfType(typeof(PhoneCamera)) as PhoneCamera;
            }
            return _instance;
        }
    }
    //---------------------------Render Image for Test----------------------------
    public RawImage rawImage;
    Texture2D test;
    public RectTransform imageParent;
    public Text text;
    //---------------------------Camera----------------------------
    int index = 0;
    WebCamTexture currentWebCam;
    public bool openCamera = false;
    public bool hasOpen = false;
    public bool first = true;
    //---------------------------Image base64 To Send----------------------------
    public string url = "http://192.168.0.107:5000/";
    string base64;
    //---------------------------Result from OpenPose----------------------------
    public string result = "";
    //---------------------------Write File for Test----------------------------
    int fileIndex = 0;

    void Start()
    {
        //---------------------------Camera----------------------------
        StartCoroutine(Call());
    }

    public IEnumerator Call()
    {
        //---------------------------Ask For Camera Authority----------------------------
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam) && WebCamTexture.devices.Length > 0)
        {
            //---------------------------Front Camera----------------------------
            SwitchCamera();
        }
    }

    public void SwitchCamera()
    {
        //---------------------------Front Camera----------------------------
        if (WebCamTexture.devices.Length < 1)
            return;
        if (currentWebCam != null)
            currentWebCam.Stop();
        index++;
        index = index % WebCamTexture.devices.Length;        
    }
  
    private void FixedUpdate()
    {
        //---------------------------Open Camera When Prepare & Playing----------------------------
        if(!GameManager.Instance.test)
        {
            openCamera = (GameManager.Instance.state == GameState.Prepare || GameManager.Instance.state == GameState.Playing);
        }
        else
        {
            openCamera = false;
        }
        //---------------------------Use File for Test, Just Close Camera----------------------------
        if (openCamera)
        {
            //---------------------------Only Call Play() Once----------------------------
            if(first)
            {
                UseCamera();
                first = false;
            }
            if (!hasOpen)
            {
                currentWebCam.Play();
                hasOpen = true;
            }
            //---------------------------Texture To Texture2D----------------------------
            test = TextureToTexture2D(rawImage.texture);
            //---------------------------Texture2D To Base64----------------------------
            base64 = Texture2DToBase64(test);
            //---------------------------Send base64 To OpenPose----------------------------
            if(base64.Length < 20)
            {
                Debug.Log("Camera Can't Open");
                return;
            }
            StartCoroutine(sendPost(base64));
            //---------------------------Write File for Test----------------------------
            //writeFile(base64);
        }
        else
        {
            //---------------------------Only Call Stop() Once----------------------------
            if (hasOpen)
            {
                currentWebCam.Stop();
                hasOpen = false;
            }          
        }
    }

    public void UseCamera()
    {
        //---------------------------Create Texture for Camera----------------------------
        currentWebCam = new WebCamTexture(WebCamTexture.devices[index].name, Screen.width, Screen.height, 60);
        rawImage.texture = currentWebCam;
        currentWebCam.Play();
        //---------------------------Camera must Rotate, and after Play()----------------------------
        rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -currentWebCam.videoRotationAngle);
        hasOpen = true;
    }


    private Texture2D TextureToTexture2D(Texture texture)
    {
        //---------------------------Texture To Texture2D----------------------------
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    public string Texture2DToBase64(Texture2D t2d)
    {
        //---------------------------Texture2D To Base64----------------------------
        byte[] bytesArr = t2d.EncodeToJPG();
        string strbaser64 = System.Convert.ToBase64String(bytesArr);
        return strbaser64;
    }

    IEnumerator sendPost(string base64)
    {
        //---------------------------Add base64 To WWWForm----------------------------
        WWWForm form = new WWWForm();
        form.AddField("base64", base64);
        //---------------------------Send base64 To OpenPose----------------------------
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            //---------------------------Bad Request----------------------------
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                result = "";
            }
            //---------------------------Good Request----------------------------
            else
            {
                result = www.downloadHandler.text;
                //---------------------------Write File for Test----------------------------
                //writeFile(result);
            }
            text.text = result;
        }
    }

    public void writeFile(string data)
    {
        //---------------------------Write File for Test----------------------------
        string path = Application.dataPath + "/PoseData/DemoPose1/" + "MyTest" + fileIndex.ToString() + ".txt";
        fileIndex++;
        //---------------------------Create File For the First Time----------------------------
        if (!System.IO.File.Exists(path))
        {
            string createText = data + System.Environment.NewLine;
            System.IO.File.WriteAllText(path, createText);
            return;
        }
        //---------------------------Append File----------------------------
        string appendText = data + System.Environment.NewLine;
        System.IO.File.AppendAllText(path, appendText);
    }
}
