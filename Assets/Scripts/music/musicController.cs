using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicController : MonoBehaviour
{
    public AudioClip music;
    public AudioSource back;
    public Slider slider;
    public Slider sliderG;
    public AudioSource backG;
    //游戏界面
    public GameObject MenuPanel;
    public GameObject MusicPanel;
    public GameObject EndPanel;
   // public GameObject BeginBuntton;
    public GameObject stopMid;
    public GameObject backButton;
    public GameObject backButton2;
    public GameObject replayButton;
    void Start()
    {
        back.loop = true; //设置循环播放  
        back.volume = 0.2f;//设置音量最大，区间在0-2之间
        backG.volume = 0.2f;
        back.clip = music;
        back.Play(); //播放背景音乐
    }

    void Update()
    {
        if (MusicPanel.activeInHierarchy == true)
        {
            back.volume = slider.value;
            backG.volume = sliderG.value;
        }
        if (MenuPanel.activeInHierarchy == false)
        {
            back.Stop();
        }
        
    }
    public void musicPlay()
    {
        backG.loop = true; //设置循环播放
        backG.Play(); //播放背景音乐
    }
    public void MenuPlay()
    {
        backG.Stop();
        back.Play();
    }
    public void midStopMusic()
    {
        backG.Pause();
    }
    public void backToGame()
    {
        backG.Play();
    }
    public void replayMusic()
    {
        backG.Stop();
        backG.Play();
    }
}
