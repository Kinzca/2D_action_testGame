using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("UI")]
    public Text wave;
    public Text tip;
    public Text score;
    public Text waveNum;
    [Header("结算界面")]
    public Text currentWave;
    public Text maxWave;
    public Text currentScore;
    public Text maxScore;
    public Text newScore;
    
    private float _maxHealth;
    
    public GameObject player;
    public GameObject pauseMenu;
    public GameObject finalMenu;
    
    private GameObject _firstEnemy;

    private bool _baseTip;
    private bool _attackTip;
    private bool _isAttacked;

    public bool isPause;
    
    [Serializable]//将类序列化
    public class PlayerData
    {
        public int waveNum;
        public float playerScore;  
        public float playerHealth;
        public int enemyNum;
    }

    private class  PlayerDataList
    {
        public List<PlayerData> PlayerDataSave = new List<PlayerData>();
    }
    
    private PlayerDataList playerDataList = new PlayerDataList();//实例化该对象

    
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        
        EventCenter.AddListener(EventType.WaveStart, WaveStart);
        EventCenter.AddListener(EventType.WaveEnd, WaveEnd);
        EventCenter.AddListener(EventType.FinalMenu,FinalMenu);
        
        string json;
        string filePath = Application.streamingAssetsPath + "/playerData.json";
        using (StreamReader sr = new StreamReader(filePath))
        {
            json = sr.ReadToEnd();
        }
        // 去除字符串两端的空格和换行符
        json = json.Trim();
        
        playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        
        if (json != "{\"PlayerDataSave\":[]}")
        {
            StartCoroutine(NextWave());
            LevelManager.instance._currentState = LevelManager.WaveState.Transition;
            
            PlayerData playerDate = playerDataList.PlayerDataSave[0];
            LevelManager.instance.thisWave = playerDate.waveNum;
            LevelManager.instance._thisWaveNum = playerDate.enemyNum;
            PlayerFSM.instance.paramenter.playerScore = playerDate.playerScore;
            PlayerFSM.instance.paramenter.health = playerDate.playerHealth;
            
            PlayerFSM.instance.paramenter.kills = playerDate.enemyNum;//将玩家击杀数设置到上一波此结束时阶段相等时
            
            EventCenter.Broadcast(EventType.P_takeDamage);
            
            Debug.Log("thisWave:" + LevelManager.instance.thisWave + "\n" + LevelManager.instance._thisWaveNum +
                      "playerScore" +
                      PlayerFSM.instance.paramenter.playerScore + "\n" + "playHealth" +
                      PlayerFSM.instance.paramenter.health);
        }
        //pauseMenu.SetActive(false);
        //finalMenu.SetActive(true);

        Time.timeScale = 1;
        
        StartCoroutine(GetFirstEnemy());
    }

    private void Update()
    {
        //TipState();
        if (LevelManager.instance.thisWave <= 0) 
        {
            FirstTip();
        }
        
        AttackDection();
        
        PauseMenu();
        
        UpdateScore();
        UpdateWavaNum();
    }

    private void WaveStart()
    {
        waveNum.gameObject.SetActive(true);
        score.gameObject.SetActive(true);
        
        wave.gameObject.SetActive(true);
        StartCoroutine(DisplayStartTextForSeconds());
    }

    private void WaveEnd()
    {
        wave.gameObject.SetActive(true);
        
        // Display the text for 2 seconds, then hide it
        StartCoroutine(DisplayTextForSeconds("完成！", 1f));
        
        StartCoroutine(NextWave());
    }

    private IEnumerator DisplayTextForSeconds(string text, float seconds)
    {
        wave.text = text;
        if (!string.IsNullOrEmpty(text)) // Check if the text is not empty
        {
            yield return new WaitForSeconds(seconds);
        }

        wave.gameObject.SetActive(false);
    }

    public IEnumerator NextWave()
    {
        yield return new WaitForSeconds(1);
        tip.gameObject.SetActive(true);
        if (LevelManager.instance._currentState != LevelManager.WaveState.Start)
        {
            tip.text = "按 G 进行下一波";
        }

        yield return new WaitForSeconds(2);

        tip.gameObject.SetActive(false);
    }
    
    private IEnumerator DisplayStartTextForSeconds()
    {
        wave.text = "第" + (LevelManager.instance.thisWave + 1) + "波";
        
        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);

        // Display the text for 2 seconds, then hide it
        StartCoroutine(DisplayTextForSeconds("开始!", 1f));
    }

    private IEnumerator GetFirstEnemy()
    {
        yield return new WaitForSeconds(4);

        _firstEnemy = GameObject.FindWithTag("Enemy");

        if (_firstEnemy != null)
        {
            Debug.Log("已查找到第一个敌人");
            
            EnemyFSM enemyFsm = _firstEnemy.GetComponent<EnemyFSM>();

            _maxHealth = enemyFsm.paramenter.health;
        }
    }


    public void FirstTip()
    {
        if ((_firstEnemy == null || Mathf.Abs(player.transform.position.x - _firstEnemy.transform.position.x) > 5.5f) && !_baseTip)
        {
            tip.text = "AD键移动，空格跳跃";
            _baseTip = true;
        }
        else if (_firstEnemy != null && Mathf.Abs(player.transform.position.x - _firstEnemy.transform.position.x) <= 5.5f && !_attackTip)  
        {
            tip.text = "按下 J 攻击";
            _attackTip = true;
        }
    }

    public void AttackDection()
    {
        if (_firstEnemy != null && _firstEnemy.GetComponent<EnemyFSM>().paramenter.health < _maxHealth &&
            !_isAttacked) 
        {
            tip.text="";
            _isAttacked = true;
        }
    }

    private void UpdateScore()
    {
        if (LevelManager.instance.thisWave > 0)
        {
            score.text = "Score:" + PlayerFSM.instance.paramenter.playerScore;
        }
        else
        {
            score.text = "Score:---";
        }
    }

    private void UpdateWavaNum()
    {
        if (LevelManager.instance.thisWave <= 0)
        {
            waveNum.text = "Wave:---";
        }
        else
        {
            waveNum.text = "Wave:" + LevelManager.instance.thisWave;
        }
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPause && !finalMenu.activeSelf) 
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPause = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPause)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPause = false;
        }
    }

    private void FinalMenu()
    {
        Time.timeScale = 0;
        CalculatedData();
        finalMenu.SetActive(true);
    }

    public void ReturnMainMenu()
    {
        EventCenter.Broadcast(EventType.SetActive,false);
        SceneManager.LoadScene("MainScene");
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    public void ReturnMain()
    {
        SceneManager.LoadScene("MainScene");
        newScore.gameObject.SetActive(false);
        EventCenter.Broadcast(EventType.SetActive,false);
    }

    #region 数据显示

    private void CalculatedData()
    {
        waveNum.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
        currentWave.text = "当前波次" + LevelManager.instance.thisWave;
        currentScore.text = "总分" + PlayerFSM.instance.paramenter.playerScore;

        if (!DataSaveManager.Instance.firstScore)//第一次必定为新记录
        {
            DataSaveManager.Instance._maxWave = LevelManager.instance.thisWave;//中间变量_maxWave
            DataSaveManager.Instance._maxScore = PlayerFSM.instance.paramenter.playerScore;//中间变量_maxScore

            DataSaveManager.Instance.firstScore = true;
            
            EventCenter.Broadcast(EventType.SaveRecord);
        }

        if (DataSaveManager.Instance._maxScore <= PlayerFSM.instance.paramenter.playerScore && DataSaveManager.Instance._maxWave <= LevelManager.instance.thisWave)//如果突破了新记录
        {
            DataSaveManager.Instance._maxScore = PlayerFSM.instance.paramenter.playerScore;
            DataSaveManager.Instance._maxWave = LevelManager.instance.thisWave;
            
            maxWave.text = "Max " + DataSaveManager.Instance._maxWave;
            maxScore.text = "Max " + DataSaveManager.Instance._maxScore;
            
            newScore.gameObject.SetActive(true);
            EventCenter.Broadcast(EventType.SaveRecord);
        }
        else
        {
            maxWave.text = "Max " + DataSaveManager.Instance._maxWave;
            maxScore.text = "Max " + DataSaveManager.Instance._maxScore;
        }
    }

    #endregion
    
    public void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.WaveStart, WaveStart);
        EventCenter.RemoveListener(EventType.WaveEnd, WaveEnd);
        EventCenter.RemoveListener(EventType.FinalMenu,FinalMenu);
    }
    
    /*
    private void TipState()
    {
        _firstEnemy =  FindFirstObjectByTag("Enemy");

        if (_firstEnemy != null)
        {
            StartTip();
        }
        
    }
    
    private GameObject FindFirstObjectByTag(string tag)
    {
        // 使用 GameObject.FindWithTag 方法查找具有指定标签的对象
        return GameObject.FindWithTag("Enemy");
    }

    private void StartTip()
    {
        float playerPositionX = player.transform.position.x;
        float enemyPositionX = _firstEnemy.transform.position.x;

        float _length = Mathf.Abs(playerPositionX - enemyPositionX);
        if (_length <= 5.5f || !_usedFirstTip)
        {
            FirstTip();
            _usedFirstTip = true;
        }
    }

    private void FirstTip()
    {
        wave.text = "按下 J 攻击";
        wave.fontSize = 36;
        
        if (Input.GetKeyDown(KeyCode.J)||!_usedSecondTip)
        {
            SecondTip();
            _usedSecondTip = true;
        }
    }

    private void SecondTip()
    {
        wave.text = "多次按下 J 进行多段攻击";
        wave.fontSize = 36;
        
        Animator playerAnimator = PlayerFSM.instance.paramenter.animator;
        AnimatorStateInfo currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("attack3")||!_usedThirdTip)
        {
            ThirdTip();
            _usedThirdTip = true;
        }
    }

    private void ThirdTip()
    {
        wave.text = "跳跃后 按J 进行下劈";
        wave.fontSize = 36;
        
        Animator playerAnimator = PlayerFSM.instance.paramenter.animator;
        AnimatorStateInfo currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("attack3"))
        {
            FinallyTip();
        }
    }

    private void FinallyTip()
    {
        string waveText = "请力尽全力在这片森林存活下去吧！";
        wave.fontSize = 36;
        
        StartCoroutine(DisplayTextForSeconds(waveText, 1f));
    }
    */
    
}