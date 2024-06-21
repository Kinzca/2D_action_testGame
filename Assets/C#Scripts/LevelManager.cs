using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    public GameObject monsterPrefab;
    public GameObject playerPrefab;
    
    private EnemyFSM _enemy;//private 命名使用 "_"开头
    private PlayerFSM _player;

    private float spawInterval;//生成的间隔时间
    private float _spawnTimer;//计算生成的间隔时间

    private int _lastKills ;//玩家上一波的击杀数，默认0
    public int _thisWaveNum = 1;//当前波次的怪物数
    public int thisWave ;//当前波次数
    private int _currentNum;//当前生成的怪物数
    
    public float ll, lr;
    public float rr, rl;

    private bool _isEnd =false ;
    
    [Header("难度曲线因子")]
    [Range(0, 1f)]
    public float speedVariable = 0.5f;
    [Range(0, 1f)]
    public float numIncreaseSpeed = 0.5f;
    public enum WaveState
    {
        Start,Transition
    }//管理波次状态

    public WaveState _currentState = WaveState.Start;//枚举的变量名使用大写

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //playerPrefab = FindObjectOfType<PlayerFSM>();
        playerPrefab = GameObject.FindWithTag("Player");
        
        _enemy =monsterPrefab.GetComponent<EnemyFSM>();//获取敌人数据
        _player = playerPrefab.GetComponent<PlayerFSM>();//获取玩家数据

        spawInterval = _enemy.paramenter.spanInterval;
    }
    private void Update()
    {
        WaveEnd();
        
        switch (_currentState)
        {
            case WaveState.Start:
                WaveStart();
                break;
            case WaveState.Transition:
                WaveTransition();
                break;
        }
    }
    /*
    private void OnGUI()
    {
        // 设置文本样式
        GUIStyle style = new GUIStyle();
        style.fontSize = 35;
        style.normal.textColor =Color.white;
        
        //绘制文字
        GUI.Label(position: new Rect(45,25,580,120),"当前波次："+thisWave,style);
        GUI.Label(new Rect(45, 65, 580, 120), "玩家分数：" + (PlayerFSM.instance.paramenter.kills * 1000),style);
        GUI.Label(new Rect(45,105,290,120),"当前波次信息："+(PlayerFSM.instance.paramenter.kills - _lastKills),style);
        GUI.Label(new Rect(320,105,290,120),"/ "+_thisWaveNum,style);

    }
    */
    private void WaveStart()
    {
        if (_currentNum < _thisWaveNum)
        {
            SpawnMonster();
        }
        else
        {
            _isEnd = true;
            _currentState = WaveState.Transition; // Set the state to transition
        }
    }
    
    public void SpawnMonster()
    {
        if (_spawnTimer < spawInterval)
        {
            _spawnTimer += Time.deltaTime;
        }
        else
        {
            float transX = _player.transform.position.x;

            float randomX = (Random.Range(0, 2) == 0) ? Random.Range(transX - ll, transX - lr): Random.Range(transX + rl, transX +rr);

            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.x);

            Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);//实例化敌人
            _currentNum++;//当前敌人数加一
            _spawnTimer = 0;//重置时间
        }
    }
    
    public void WaveTransition()
    {
        var player = PlayerFSM.instance.paramenter;
        int killCount = player.kills - _lastKills;
        //Debug.Log("_lastKills "+_lastKills);
        //Debug.Log("player.kills "+player.kills);
        
        if (Input.GetKeyDown(KeyCode.G) && _thisWaveNum == killCount) 
        {
            EventCenter.Broadcast(EventType.WaveStart);
            
            //pool.Clear();
            thisWave++;
            
            _thisWaveNum = Mathf.Clamp(_thisWaveNum+Random.Range(1,10), 3, 100);  // 确保在范围内
            
            _enemy.paramenter.health = Mathf.Clamp(_enemy.paramenter.health * 1.05f, 10, 60);
            
            _enemy.paramenter.moveSpeed = Mathf.Clamp(_enemy.paramenter.moveSpeed * 1.05f, 1f, 3f);
            
            _enemy.paramenter.attackTimer *= 0.95f;

            spawInterval = Mathf.Clamp(spawInterval * 0.95f, 4f, 2f);
            
            _lastKills = player.kills;//记录上一波玩家的击杀数
            _currentNum = 0;//重置当前当前波次的击杀数
            
            _currentState = WaveState.Start; // Set the state back to start
        }
    }

    public void WaveEnd()
    {
        if (PlayerFSM.instance.paramenter.kills - _lastKills != 0 && _thisWaveNum != 0 &&
            PlayerFSM.instance.paramenter.kills - _lastKills == _thisWaveNum && _isEnd) 
        {
            
            EventCenter.Broadcast(EventType.SaveData);
            EventCenter.Broadcast(EventType.WaveEnd);
            _isEnd = false;
        }
    }
}
