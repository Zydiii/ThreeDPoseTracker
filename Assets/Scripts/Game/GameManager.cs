using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 游戏类型
public enum GameType
{
    Single,
    Double
}

// 游戏难度
public enum GameLevel
{
    Easy,
    Difficult
}

// 游戏状态
public enum GameState
{
    Menu,     // 显示菜单界面
    Prepare,  // 游戏准备阶段
    Playing,  // 游戏过程中
    Stop,     // 游戏暂停
    Ending    // 游戏结束
}

public class GameManager : MonoBehaviour
{
    //---------------------------Instance----------------------------
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return _instance;
        }
    }
    //---------------------------Score Image----------------------------
    public List<RawImage> images;
    float imageTime = 0;
    bool show = false;
    //---------------------------Test----------------------------
    public bool test = true;

    // 菜单等UI------------------------------------------------------------------
    public GameObject menuPanel;
    public GameObject menuS;
    public GameObject StartButton;
    public GameObject scorePanel;
    public GameObject musicPanel;
    public GameObject informationPanel;
    public GameObject userSendPanel;
    public GameObject firstpage;
    //游戏中UI
    public GameObject midPanel;

    // 游戏中的结束按钮
    public GameObject stopWhenPlaying;
    // 游戏结束UI
    public GameObject stopPanel;
    public Text commentText;
    public Text scoreText;
    public Text scoreTextA;
    public Text scoreTextB;
    // 结束UI中与准备UI同样作用的组件
    public Text restartTimer;     // 单人计时
    public Text restartTimerA;    // 玩家A的计时
    public Text restartTimerB;    // 玩家B的计时
                                  // 显示错误提示
    public Text restartHint;  // 单人的提示
    public Text restartHintA; // 玩家A的提示
    public Text restartHintB; // 玩家B的提示
    public GameObject okAwhenRestart;
    public GameObject okBwhenRestart;

    // 准备阶段需要用到的一些变量------------------------------------------------
    // 准备动作保持的时间
    public Text prepareTimer;     // 单人计时
    public Text prepareTimerA;    // 玩家A的计时
    public Text prepareTimerB;    // 玩家B的计时
    float prepareTime = 3; // 准备确认时间
    // 显示错误提示
    public Text prepareHint;  // 单人的提示
    public Text prepareHintA; // 玩家A的提示
    public Text prepareHintB; // 玩家B的提示
    // 双人模式时一方准备完成的提示
    public GameObject okA;
    public GameObject okB;

    public GameObject preparePanel; // 准备阶段界面
    // 单人
    public GameObject preparePoseSingle; // 给用户提示的准备墙面 - T pose
    public List<Collider> prepareCollidersSingle; // 准备墙面的colliders，检测用户动作的标准
    // 双人
    public GameObject preparePoseDouble;
    public List<Collider> prepareCollidersDouble;

    // 玩家关键点有关的变量----------------------------------------------------------------
    public GameObject single; // 包含人关键点和判断的对象
    PoseManager poseManagerSingle;
    public GameObject playerA;
    public GameObject playerB;
    PoseManager poseManagerA;
    PoseManager poseManagerB;

    // 墙相关的变量------------------------------------------------------------------------
    public List<GameObject> singleEasyWalls; // 单人-简单
    public List<GameObject> singleDiffWalls; // 单人-困难
    public List<GameObject> doubleEasyWalls; // 双人-简单
    public List<GameObject> doubleDiffWalls; // 双人-困难

    public float comingTime = 6;     // 墙到达所需的时间
    float minTime = 3; // 墙移动所用的最短时间
    public float startZ = 15f;   // 墙出发的位置
    public float stopZ = 1.5f;   // 要开始进行判断的位置
    public float responseZ = -1f;// 要响应单次结果的位置
    public float destroyZ = -12f;// 墙要销毁的位置
    public GameObject emptyWall; // 生成的墙挂在这个对象下
    float speedOfWall; // 墙移动的速度

    public int numOfWall = 10; // 一轮游戏墙的数量

    // 总分等级设置
    float Terrible;
    float Soso;
    float Good;
    float Great;

    // 游戏设置和状态---------------------------------------------------------------------
    public GameType type = GameType.Single;   // 游戏的类型 - 单人、双人
    public GameLevel level = GameLevel.Easy; // 游戏的难度 - 简单、困难
    public GameState state = GameState.Menu; // 游戏状态，刚开始设置为菜单状态

    // 产生随机评论的对象
    Comments comments;

    // Start is called before the first frame update
    void Start()
    {
        // 获取poseManager，用于动作判断
        poseManagerSingle = single.GetComponent<PoseManager>();
        poseManagerA = playerA.GetComponent<PoseManager>();
        poseManagerB = playerB.GetComponent<PoseManager>();
        // 初始化评论列表
        comments = gameObject.GetComponent<Comments>();
    }

    // Update is called once per frame
    void Update()
    {
        //---------------------------Score Image----------------------------
        if (show)
            imageTime += Time.deltaTime;
        if (imageTime >= 1.0f)
        {
            images[0].GetComponent<CanvasGroup>().alpha = 0;
            images[1].GetComponent<CanvasGroup>().alpha = 0;
            images[2].GetComponent<CanvasGroup>().alpha = 0;
            images[3].GetComponent<CanvasGroup>().alpha = 0;
            imageTime = 0;
            show = false;
        }
        switch (state)
        {
            case GameState.Prepare:
                Prepare(preparePanel, prepareTimer, prepareHint,
                    prepareTimerA, prepareHintA, okA,
                    prepareTimerB, prepareHintB, okB);
                break;
            case GameState.Playing:
                ComingWall();
                break;
            case GameState.Stop:
                break;
            case GameState.Ending:
                // 重新进入准备阶段，UI参数与之前不同
                //Prepare(stopPanel, restartTimer, restartHint,
                //    restartTimerA, restartHintA, okAwhenRestart,
                //    restartTimerB, restartHintB, okBwhenRestart);
                return;
            default:
                break;
        }       
    }

    // 点击button选择游戏类型和难度--------------------------------------------------------

    public void SetGameTypeSingle(bool value)
    {
        if (value)
        {
            type = GameType.Single;
        }
        // 界面有什么变化吗？选择之后需要确认键吗？
    }

    public void SetGameTypeDouble(bool value)
    {
        if (value)
        {
            type = GameType.Double;
        }
    }

    //打开/关闭菜单
    public void openMenu()
    {
        menuS.SetActive(true);
        StartButton.SetActive(false);
    }
    public void closeMenu()
    {
        menuS.SetActive(false);
        StartButton.SetActive(true);
    }

    //设置栏跳转
    public void openScoreBoard()
    {
        scorePanel.SetActive(true);
    }
    public void closeScoreBoard()
    {
        scorePanel.SetActive(false);
    }
    public void openMusicBoard()
    {
        musicPanel.SetActive(true);
    }
    public void closeMusicBoard()
    {
        musicPanel.SetActive(false);
    }
    public void openInfoBoard()
    {
        informationPanel.SetActive(true);
    }
    public void closeInfoBoard()
    {
        informationPanel.SetActive(false);
    }
    public void openUserBoard()
    {
        userSendPanel.SetActive(true);
    }
    public void closeUserBoard()
    {
        userSendPanel.SetActive(false);
    }

    GameState tempState; // 临时存一下是准备状态还是游戏状态
    //暂停键功能
    public void stopMid()
    {
        //Debug.Log("CLICK STOP");
        midPanel.SetActive(true);
        tempState = state;
        state = GameState.Stop;
    }
    public void continuePlay()
    {
        midPanel.SetActive(false);
        state = tempState;
        //state = GameState.Playing;
    }
    public void rePlay()
    {
        // 需要清空游戏中的状态
        InitGame();
        // 复用开始游戏的function
        StartGame();
        midPanel.SetActive(false);
        state = GameState.Prepare;
    }
    public void backToMenu()
    {
        InitGame();
        midPanel.SetActive(false);
        stopWhenPlaying.SetActive(false); // 右上角的暂停键也不显示了
        state = GameState.Menu;
        firstpage.SetActive(true);
    }
    // 选择游戏类型之后确认开始游戏
    public void StartGame()
    {
        // 改变UI
        preparePanel.SetActive(true);
        menuPanel.SetActive(false);
        // 根据难度设置关卡数和等级标准
        if (level == GameLevel.Easy)
        {
            numOfWall = 10;
            Terrible = 79;
            Soso = 99;
            Good = 119;
            Great = 139;
        }
        else
        {
            numOfWall = 15;
            Terrible = 59;
            Soso = 104;
            Good = 149;
            Great = 194;
        }
        // 判断游戏类型
        if (type == GameType.Single)
        {
            prepareTimer.text = "0 s";
            prepareTimerA.text = "";
            prepareTimerB.text = "";
            preparePoseSingle.SetActive(true);
            preparePoseDouble.SetActive(false); // 从入口统一设置不显示另一类型的墙
        }
        else
        {
            prepareTimer.text = "";
            prepareTimerA.text = "0 s";
            prepareTimerB.text = "0 s";
            preparePoseDouble.SetActive(true);
            preparePoseSingle.SetActive(false); // 从入口统一设置不显示另一类型的墙
            // 双人的评分标准要*2
            Terrible *= 2;
            Soso *= 2;
            Good *= 2;
            Great *= 2;
        }
        // 清空原来可能有的计时
        time = 0;
        timeA = 0;
        timeB = 0;
        poseManagerSingle.ClearTimer();
        poseManagerA.ClearTimer();
        poseManagerB.ClearTimer();
        prepareHint.text = "";
        prepareHintA.text = "";
        prepareHintB.text = "";
        stopWhenPlaying.SetActive(true); // 显示游戏中的暂停按钮
        // 改变游戏状态
        state = GameState.Prepare;
    }

    // 按键
    //public void SetGameEasy()
    //{
    //    level = GameLevel.Easy;
    //    // 界面对应有什么变化吗？
    //}

    //public void SetGameDiff()
    //{
    //    level = GameLevel.Difficult;
    //}

    // 单选框
    public void SetGameEasy(bool value)
    {
        if (value)
        {
            level = GameLevel.Easy;
            // 界面对应有什么变化吗？
        }
    }

    public void SetGameDiff(bool value)
    {
        if (value)
        {
            level = GameLevel.Difficult;
        }
    }


    // 准备阶段---------------------------------------------------------------------------
    float time = 0, timeA = 0, timeB = 0;
    void Prepare(GameObject preparePanel, Text timer, Text hint,
                Text timerA, Text hintA, GameObject okA,
                Text timerB, Text hintB, GameObject okB)
    {
        if (type == GameType.Single)
        {
            timerA.text = "";
            timerB.text = "";
            // 更新时间
            time = Mathf.Round(poseManagerSingle.Prepare(prepareCollidersSingle));
            // 相应判断
            // 动作未到要求时间，更新显示
            if (time >= 0 && time < prepareTime)
            {
                timer.text = time.ToString() + " s";
            }
            // 准备完成，修改游戏UI和游戏状态
            else if (time >= prepareTime)
            {
                time = 0;
                poseManagerSingle.ClearTimer();
                preparePanel.SetActive(false);
                preparePoseSingle.SetActive(false);
                stopWhenPlaying.SetActive(true); // 显示右上角的暂停键
                // 清空现在的计时和提示显示
                timer.text = "";
                hint.text = "";
                state = GameState.Playing;
            }
            // 错误提示
            else if (time == -1)
            {
                timer.text = "0 s";
                hint.text = "请伸直手臂";
            }
            // 错误提示
            else
            {
                timer.text = "0 s";
                hint.text = "请对准墙面空洞";
            }
        }
        else
        {
            timer.text = "";
            if (timeA < prepareTime)
            {
                timeA = Mathf.Round(poseManagerA.Prepare(prepareCollidersDouble));
            }
            if (timeB < prepareTime)
            {
                timeB = Mathf.Round(poseManagerB.Prepare(prepareCollidersDouble));
            }
            // Player A 相应判断
            // 动作未到要求时间，更新显示
            if (timeA >= 0 && timeA < prepareTime)
            {
                timerA.text = timeA.ToString() + " s";
            }
            // 错误提示
            else if (timeA == -1)
            {
                timerA.text = "0 s";
                hintA.text = "请伸直手臂";
            }
            // 错误提示
            else if (timeA == -2)
            {
                timerA.text = "0 s";
                hintA.text = "请对准墙面空洞";
            }
            else
            {
                timerA.text = "3 s";
                hintA.text = "";
                okA.SetActive(true);
            }

            // Player B 判断
            if (timeB >= 0 && timeB < prepareTime)
            {
                timerB.text = timeB.ToString() + " s";
            }
            // 错误提示
            else if (timeB == -1)
            {
                timerB.text = "0 s";
                hintB.text = "请伸直手臂";
            }
            // 错误提示
            else if (timeB == -2)
            {
                timerB.text = "0 s";
                hintB.text = "请对准墙面空洞";
            }
            else
            {
                timerB.text = "3 s";
                hintB.text = "";
                okB.SetActive(true);
            }

            if (timeA >= prepareTime && timeB >= prepareTime)
            {
                timeA = 0;
                timeB = 0;
                poseManagerA.ClearTimer();
                poseManagerB.ClearTimer();
                okA.SetActive(false);
                okB.SetActive(false);
                preparePanel.SetActive(false);
                preparePoseDouble.SetActive(false);
                stopWhenPlaying.SetActive(true);
                timerA.text = "";
                hintA.text = "";
                timerB.text = "";
                hintB.text = "";
                state = GameState.Playing;
            }
        }
    }

    // 游戏阶段------------------------------------------------------------------------------
    GameObject wall;               // 当前正在移动的墙
    GameObject nextWall;           // 下一面即将移动的墙
    WallController wallController; // 控制墙移动
    bool hasWall = false;          // 当前场景中是否有墙
    bool waiting = false;          // 是否有等待的墙
    bool response = false;         // 是否已经给用户结果反馈
    float score = 0;               // 记录通过单次墙面的分数，不是累计分数
    float scoreA = 0;
    float scoreB = 0;
    float totalScore = 0;          //总分，显示用？
    float totalScoreA = 0;
    float totalScoreB = 0;
    int count = 0;                 // 记录已经玩过的关卡数

    // 控制墙创建、移动和销毁，计算得分
    void ComingWall()
    {
        // 创建第一个墙
        // 此时游戏关卡计数为0，且场景中没有墙
        if (count == 0 && !hasWall)
        {
            wall = CreateWall(count);
            hasWall = true;
            count++;
            // Debug.Log("first wall"+count);
        }
        // wall换成下一面墙
        // 此时关卡计数不是0，没有正在移动的墙，有墙等待
        else if (count < numOfWall && !hasWall && waiting)
        {
            // 把等待的墙赋值给正在移动的墙
            wall = nextWall;
            waiting = false;
            hasWall = true;
            count++;
            // Debug.Log("other wall"+count);
        }
        // 一轮游戏结束
        // 此时关卡计数达到目标数，并且场景中没有正在移动的墙了
        else if (count == numOfWall && !hasWall)
        {
            count = 0;
            // 调用游戏结束的处理函数
            GameOver();

            return;
        }

        // 每次调用都更新当前关卡墙的位置
        wallController = wall.GetComponent<WallController>();
        float posZ = wallController.move();
        // 比较墙是否到要进行计算的位置，这样在墙离玩家比较远时不用做碰撞检测
        if (posZ <= stopZ)
        {
            // 保留最大值
            if (type == GameType.Single)
            {
                // 调用poseManager的方法计算得分，保留最高得分
                score = Mathf.Max(poseManagerSingle.Play(wallController.colliders), score);
            }
            else
            {
                // A B玩家分别计算分数
                scoreA = Mathf.Max(poseManagerA.Play(wallController.colliders), scoreA);
                scoreB = Mathf.Max(poseManagerB.Play(wallController.colliders), scoreB);
            }

            // 应该有下一面墙但是还没有时，创建一个新的对象
            if (count < numOfWall && !waiting)
            {
                nextWall = CreateWall(count);
                waiting = true;
            }
        }

        // 墙到了终点，应该给玩家反馈
        // 反馈只需要一次，之后response为true，不会再给反馈
        if (posZ <= responseZ && !response)
        {
            // 根据分数做出反应
            // 这里还应该添加对应的声效和画面提示
            if (type == GameType.Single)
            {
                Debug.Log("score" + score);
                setImage(score);
            }
            else
            {
                Debug.Log("scoreA" + scoreA);
                Debug.Log("scoreB" + scoreB);
            }

            response = true;
        }

        // 墙到了要销毁的位置
        if (posZ < destroyZ)
        {
            Debug.Log("destroy" + count);
            // 销毁对象
            Destroy(wall);
            hasWall = false;
            response = false;

            // 计算总分，清空单次分数
            // 某一模式下另一模式的分数为0，所以加上也不影响正确性
            totalScore += (score + scoreA + scoreB);
            totalScoreA += scoreA;
            totalScoreB += scoreB;
            score = 0;
            scoreA = 0;
            scoreB = 0;
        }
    }

    // 创建墙,判断游戏类型和难度
    GameObject CreateWall(int count)
    {
        float time = comingTime; // 困难模式会改变墙移动的时间，从而提速
        GameObject wall;
        int index = GetRandomIndex();
        // int index = count % singleWalls.Count;
        if (type == GameType.Single && level == GameLevel.Easy)
        {
            wall = Instantiate(singleEasyWalls[index], new Vector3(0, 0, startZ), Quaternion.identity);
        }
        else if (type == GameType.Single && level == GameLevel.Difficult)
        {
            wall = Instantiate(singleDiffWalls[index], new Vector3(0, 0, startZ), Quaternion.identity);
            time = comingTime - (comingTime - minTime) * (count + 1) / (numOfWall); // 墙的数量一定不是0
        }
        else if (type == GameType.Double && level == GameLevel.Easy)
        {
            wall = Instantiate(doubleEasyWalls[index], new Vector3(0, 0, startZ), Quaternion.identity);
        }
        else
        {
            wall = Instantiate(doubleDiffWalls[index], new Vector3(0, 0, startZ), Quaternion.identity);
            time = comingTime - (comingTime - minTime) * (count + 1) / (numOfWall);
        }
        wall.transform.SetParent(emptyWall.transform);
        wall.transform.localScale = new Vector3(0.25f, 0.25f, 0.5f);
        wall.transform.position = new Vector3(0, 1.3f, startZ);     // 墙的初始位置
        WallController wallController = wall.GetComponent<WallController>();
        // 防止除零错误，正常情况下速度不是0
        if (comingTime > 0)
        {
            wallController.speed = (startZ - stopZ) / time;
        }
        else
        {
            wallController.speed = 0;
        }
        return wall;
    }

    // 从对应难度和类型的列表中随机选取一个索引
    // 不同难度等级使用不同的墙
    System.Random random = new System.Random();
    int GetRandomIndex()
    {
        int index = 0;
        if (type == GameType.Single && level == GameLevel.Easy)
        {
            index = random.Next(singleEasyWalls.Count);
        }
        else if (type == GameType.Single && level == GameLevel.Difficult)
        {
            index = random.Next(singleDiffWalls.Count);
        }
        else if (type == GameType.Double && level == GameLevel.Easy)
        {
            index = random.Next(doubleEasyWalls.Count);
        }
        else
        {
            index = random.Next(doubleDiffWalls.Count);
        }
        return index;
    }

    // 游戏暂停---------------------------------------------------------------------------


    // 游戏结束---------------------------------------------------------------------------
    void GameOver()
    {
        state = GameState.Ending;
        // 界面显示控制
        stopPanel.SetActive(true);
        stopWhenPlaying.SetActive(false);

        // 显示分数
        scoreText.text = "总分：" + totalScore;
        if (type == GameType.Single)
        {
            Debug.Log("单人得分：" + totalScore);
            // 重新显示准备墙面
            preparePoseSingle.SetActive(true);
        }
        else
        {
            Debug.Log("玩家A得分：" + totalScoreA);
            Debug.Log("玩家B得分：" + totalScoreB);
            Debug.Log("总分：" + totalScore);
            scoreTextA.text = "玩家1：" + totalScoreA;
            scoreTextB.text = "玩家2：" + totalScoreB;
            preparePoseDouble.SetActive(true);
        }

        // 根据总分给出不同评价，正向鼓励
        string comment = "";
        if (totalScore <= Terrible)
        {
            comment = comments.GetBadComments();
            //Debug.Log(comment);
        }
        else if (totalScore <= Soso)
        {
            comment = comments.GetSosoComments();
            //Debug.Log(comment);
        }
        else if (totalScore <= Good)
        {
            comment = comments.GetGoodComments();
            //Debug.Log(comment);
        }
        else if (totalScore <= Great)
        {
            comment = comments.GetGreatComments();
            //Debug.Log(comment);
        }
        else
        {
            comment = comments.GetPerfectComments();
            //Debug.Log(comment);
        }
        commentText.text = comment;
        // 清空记分
        totalScore = 0;
        totalScoreA = 0;
        totalScoreB = 0;
        
    }

    // 结束UI上的button点击操作
    public void BackToMenuWhenStop()
    {
        stopPanel.SetActive(false);
        menuPanel.SetActive(true);
        state = GameState.Menu;
    }

    private void InitGame()
    {
        //墙的计数清零
        count = 0;
        // 墙相关的状态初始化
        hasWall = false;
        waiting = false;
        response = false;
        // 分数清零
        score = 0;
        scoreA = 0;
        scoreB = 0;
        totalScore = 0;
        totalScoreA = 0;
        totalScoreB = 0;
        // 销毁可能存留的墙
        Destroy(wall);
        Destroy(nextWall);
    }

    void setImage(float score)
    {
        //---------------------------Set Score Image by Score----------------------------
        if (score < 5)
            images[0].GetComponent<CanvasGroup>().alpha = 1;
        else if (score < 8)
            images[1].GetComponent<CanvasGroup>().alpha = 1;
        else if (score < 12)
            images[2].GetComponent<CanvasGroup>().alpha = 1;
        else if (score <= 15)
            images[3].GetComponent<CanvasGroup>().alpha = 1;
        show = true;
    }
}


