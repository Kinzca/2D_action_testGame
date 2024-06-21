using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataSaveManager : MonoBehaviour
{
    public static DataSaveManager Instance;
    
    public int _maxWave;
    public float _maxScore;
    public bool firstScore;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果已经存在另一个实例，则销毁当前实例
            Destroy(gameObject);
        }
        
        Debug.Log("DataSaveManager.Instance 已经成功初始化。");
        // 在场景切换时不销毁该对象
        //DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        EventCenter.AddListener(EventType.SaveData,SaveData);
        EventCenter.AddListener(EventType.DeleteData,DeleteData);
        EventCenter.AddListener(EventType.SavePlayerRank,PlayerRank);
        EventCenter.AddListener(EventType.LoadDate,LoadData);
        EventCenter.AddListener(EventType.SaveRecord,RecordUpdate);
        
        LoadPlayerRecords();
        DataInitialize();
    }

    
    [Serializable]//将类序列化
    public class PlayerData
    {
        public int waveNum;
        public int enemyNum;
        public float playerScore;  
        public float playerHealth;
    }
    [Serializable]//将类序列化
    public class  PlayerRecord
    {
        public float MaxScore;
        public int MaxWave;
        public bool FirstScore;
    }
    
    private class  PlayerDataList
    {
        public List<PlayerData> PlayerDataSave = new List<PlayerData>();
        /*
        public void SetWaveNum(int waveNum)
        {
            foreach (PlayerData playerData in PlayerDataSave)// 遍历playerDataSave链表中的PlayerData
            {
                playerData.waveNum = waveNum;
            }
        }

        public void SetPlayerScore(float playerScore)
        {
            foreach (PlayerData playerData in PlayerDataSave)// 遍历playerDataSave链表中的PlayerData
            {
                playerData.playerScore = playerScore;
            }
        }
        
        public void SetPlayerHealth(float playerHealth)
        {
            foreach (PlayerData playerData in PlayerDataSave)// 遍历playerDataSave链表中的PlayerData
            {
                playerData.playerHealth = playerHealth;
            }
        }
        */
    }
    private PlayerDataList playerDataList = new PlayerDataList();//实例化该对象

    private class PlayerRecordsList
    {
        public List<PlayerRecord> PlayerRecordSave = new List<PlayerRecord>();
    }

    private PlayerRecordsList playerRecordsList = new PlayerRecordsList();
    
    private void LoadPlayerRecords()
    {
        string json;
        string filePath = Application.streamingAssetsPath + "/playerRecord.json";
        using (StreamReader sr = new StreamReader(filePath))
        {
            json = sr.ReadToEnd();
        }

        playerRecordsList = JsonUtility.FromJson<PlayerRecordsList>(json);
        Debug.Log(json);
    }
    
    private void SaveData()
    {
        // 清空列表
        playerDataList.PlayerDataSave.Clear();
        
        // 创建一个新的 PlayerData 对象
        PlayerData playerData = new PlayerData();
        playerData.waveNum = LevelManager.instance.thisWave;
        playerData.enemyNum = LevelManager.instance._thisWaveNum;
        playerData.playerScore = PlayerFSM.instance.paramenter.playerScore;
        playerData.playerHealth = PlayerFSM.instance.paramenter.health;
    
        // 向列表中添加该对象
        playerDataList.PlayerDataSave.Add(playerData);
    
        // 将列表序列化为 JSON 字符串
        string json = JsonUtility.ToJson(playerDataList);
        string filePath = Application.streamingAssetsPath + "/playerData.json";

        // 使用 StreamWriter 写入数据到文件
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.Write(json);
        }

        Debug.Log("已保存数据");
    }

    private void RecordUpdate()
    {
        playerRecordsList.PlayerRecordSave.Clear();
        
        // 创建一个新的 PlayerRecord 对象
        PlayerRecord playerRecord = new PlayerRecord();
        playerRecord.FirstScore = true;
        playerRecord.MaxScore = _maxScore;
        playerRecord.MaxWave = _maxWave;
        
        Debug.Log("FirstScore "+playerRecord.FirstScore+"MaxScore "+playerRecord.MaxScore+"MaxWave "+playerRecord.MaxWave);
        
        // 将新的玩家记录添加到列表中
        playerRecordsList.PlayerRecordSave.Add(playerRecord);

        // 将列表序列化为 JSON 字符串
        string json = JsonUtility.ToJson(playerRecordsList);
        string filePath = Application.streamingAssetsPath + "/playerRecord.json";
        
        // 使用 StreamWriter 写入数据到文件
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.WriteLine(json);
        }

        Debug.Log(json);
        //Debug.Log("FirstScore "+playerRecord.FirstScore+"MaxScore "+playerRecord.MaxScore+"MaxWave "+playerRecord.MaxWave);
        //Debug.Log("已保存玩家记录");
    }

    private void DataInitialize()
    {
        // 检查是否有玩家记录可用
        if (playerRecordsList.PlayerRecordSave.Count > 0)
        {
            // 获取最新的玩家记录
            PlayerRecord playerRecord = playerRecordsList.PlayerRecordSave[0];

            // 使用玩家记录中的数据来初始化变量
            firstScore = playerRecord.FirstScore;
            _maxScore = playerRecord.MaxScore;
            _maxWave = playerRecord.MaxWave;

            Debug.Log("FirstScore " + firstScore + " MaxScore " + _maxScore + " MaxWave " + _maxWave);
        }
        else
        {
            // 如果没有可用的玩家记录，则将变量初始化为默认值
            firstScore = false;
            _maxScore = 0f;
            _maxWave = 0;

            Debug.LogWarning("没有可用的玩家记录！");
        }
    }

    
    public void DeleteData()
    {
        /*
        playerDataList.PlayerDataSave.Clear();
    
        // 将空列表序列化为 JSON 字符串
        string json = JsonUtility.ToJson(playerDataList, true);
        string filePath = Application.streamingAssetsPath + "/playerData.json";

        // 使用 StreamWriter 写入空数据到文件（覆盖文件内容）
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.Write(json);
        }
        */
        
        // 创建一个空的 PlayerDataList 对象
        PlayerDataList emptyPlayerDataList = new PlayerDataList();
    
        // 将空对象序列化为 JSON 字符串
        string emptyJson = JsonUtility.ToJson(emptyPlayerDataList);
    
        // 指定 JSON 文件路径
        string filePath = Application.streamingAssetsPath + "/playerData.json";

        // 使用 StreamWriter 写入空内容到文件
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.Write(emptyJson);
        }

        // 提示信息
        Debug.Log("已成功删除 JSON 文件中的所有内容！");
    }
    
    
    public void LoadData()
    {
        SceneManager.LoadScene("GameScene");
        EventCenter.Broadcast(EventType.SetActive,true);
        
        string json;
        string filePath = Application.streamingAssetsPath + "/playerData.json";
        using (StreamReader sr= new StreamReader(filePath))
        {
            json = sr.ReadToEnd();
        }

        playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        Debug.Log(json);
        
/*
        if (playerDataList.PlayerDataSave != null && playerDataList.PlayerDataSave.Count > 0)
        {
            Debug.Log("thisWave:" + LevelManager.instance.thisWave + "\n" + LevelManager.instance._thisWaveNum +
                      "playerScore" +
                      PlayerFSM.instance.paramenter.playerScore + "\n" + "playHealth" +
                      PlayerFSM.instance.paramenter.health);
        }
        else
        {
            Debug.LogWarning("存档文件中没有数据！");
        }
*/
        //playerDataList是PlayerDataList类的实例化对象
        //playerDataSave是playerDataList链表的实例化对象

    }

    private void PlayerRank()
    {
        //创建一个新的playerData对象来存储数据
        PlayerData playerDataRank = new PlayerData();
        
        playerDataRank.waveNum = LevelManager.instance.thisWave;
        playerDataRank.playerScore = PlayerFSM.instance.paramenter.playerScore;
        //创建一个PlayerData对象来存储数据
        PlayerDataList playerDataRankList = new PlayerDataList();
        playerDataRankList.PlayerDataSave.Add(playerDataRank);
        
        // 将列表序列化为 JSON 字符串
        string json = JsonUtility.ToJson(playerDataRankList, true);
        string filePath = Application.streamingAssetsPath + "/playerRank.json";

        // 使用 StreamWriter 写入数据到文件
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.Write(json);
        }

        // 提示信息
        Debug.Log("玩家波次和分数已保存至 Player Rank 文件");
    }

    public bool NeedNewGame()
    {
        string json;
        string filePath = Application.streamingAssetsPath + "/playerData.json";
        using (StreamReader sr = new StreamReader(filePath))
        {
            json = sr.ReadToEnd();
        }

        // 去除字符串两端的空格和换行符
        json = json.Trim();

        // 反序列化json数据为对象
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log(playerData);

        if (json == "{\"PlayerDataSave\":[]}" )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

    
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.SaveData,SaveData);
        EventCenter.RemoveListener(EventType.DeleteData,DeleteData);
        EventCenter.RemoveListener(EventType.SavePlayerRank,PlayerRank);
        EventCenter.RemoveListener(EventType.LoadDate,LoadData);
        EventCenter.RemoveListener(EventType.SaveRecord,RecordUpdate);
    }
}
