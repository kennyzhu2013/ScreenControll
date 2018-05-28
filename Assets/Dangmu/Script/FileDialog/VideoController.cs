using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// Video controller.
/// referring: http://forum.china.unity3d.com/thread-25755-1-1.html
/// </summary>
public class VideoController : MonoBehaviour {
    public VideoPlayer vPlayer;
    public string playingName;

    //zy---增加进度条控制、时间显示、按钮
    public Slider videoSlider;
    public Text timeText;
    public Button playBtn;
    public Button pauseBtn;
    public Button loopBtn;

    //时 分的转换
    private int mint, secs;
    private float time;
    private float time_Count;
    private float time_Current;
    private string allTimeString;
    private string curTimeString;
    //是否在拖动中
    private bool isDrag;


    void Start() {
        //if (null == instance)
        //instance = this;

        if (null == vPlayer)
            vPlayer = GetComponentInChildren<VideoPlayer>();

        if (null == videoSlider)
            videoSlider = GetComponentInChildren<Slider>();


    }

    //zy--slider步进
    private void Update()
    {
        if (vPlayer.isPrepared && !isDrag)
        {
            //time_Count += Time.deltaTime;
            //if ((time_Count - time_Current) >= 0.5)
            {
                videoSlider.value = vPlayer.frame;
                //time_Current = time_Count;
                SetTimeTxt();
            }
        }
        if (vPlayer.isPrepared && isDrag)
        {
            vPlayer.frame = (long)videoSlider.value;
            //vPlayer.time = (long)videoSlider.value / vPlayer.frameRate;
            SetTimeTxt();
            //time_Current = time_Count;
        }
    }

    /*
    public void ChangeVideo()
    {
        if (vPlayer.isPrepared && isDrag)
        {
            vPlayer.frame = (long)videoSlider.value;
            vPlayer.Pause();
            vPlayer.Play();
            SetTimeTxt();
            isDrag = false;
        }
    }*/

    //判断slider是否处于Drag状态
    public void OnDrag()
    {
        isDrag = true;
    }
    public void EndDrag()
    {
        isDrag = false;
    }


    /// <summary>
    /// Starts the play.
    /// </summary>
    /// <param name="filePath">File path.</param>
    public void StartPlay(string filePath, VideoPlayer.EventHandler callBack) {
        Log.debug(this, "StartPlay==========filePath:" + filePath);
        if (vPlayer.isPlaying)
            vPlayer.Stop();
        //vPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
        //vPlayer.url = "file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4";
        //vPlayer.URL = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
        vPlayer.url = filePath;


        //当VideoPlayer全部设置好的时候调用
        if (null == callBack)
            vPlayer.prepareCompleted += Prepared;
        else
            vPlayer.prepareCompleted += callBack;

        //启动播放器
        vPlayer.Prepare();
        Init();
        vPlayer.Play();
    }

    //zy--初始化相关数值
    private void Init()
    {
        isDrag = false;
        time_Count = 0;
        time_Current = 0;

        videoSlider.maxValue = vPlayer.frameCount;
        videoSlider.value = vPlayer.frame;

        //帧数/帧速率=总时长    如果是本地直接赋值的视频，我们可以通过VideoClip.length获取总时长.单位秒
        time = vPlayer.frameCount / vPlayer.frameRate;
        mint = (int)time / 60;
        secs = (int)time % 60;
        allTimeString = string.Format("/{0:D2}:{1:D2}", mint.ToString(), secs.ToString());
        //videoSlider.onValueChanged.AddListener(delegate { ChangeVideo(videoSlider.value); });

 
    }

    //zy--设置时间字符串
    private void SetTimeTxt()
    {
        //当前时间
        time = (int)vPlayer.time;
        mint = (int)time / 60;
        secs = (int)time % 60;
        curTimeString = string.Format("{0:D2}:{1:D2}", mint.ToString(), secs.ToString());
        timeText.text = curTimeString + allTimeString;
    }

    //TODO: play, pause, stop, functionsabc and so on.
    void Prepared( VideoPlayer vPlayer )
	{      
        Debug.Log ("VideoPlayer Prepared Ended");
		vPlayer.Play ();
        Init();
    }

	public void Play()
	{   
        vPlayer.Play ();
        Init();
        playBtn.gameObject.SetActive(false);
        pauseBtn.gameObject.SetActive(true);
    }

	public void Paused()
	{
		vPlayer.Pause ();
        playBtn.gameObject.SetActive(true);
        pauseBtn.gameObject.SetActive(false);
    }

	public void Stop()
	{
		vPlayer.Stop ();
	}

    public void loop()
    {
        //vPlayer.
    }
}
