using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    Camera mainCam;
    
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject buttonContinue;
    public GameObject exit1;
    public GameObject exit2;
    
    public Button continueButton;
    
    public float moveSpeed = 1f;
    private Vector3 lastXPos;

    public Transform nt1, nl1, mt1, ft1, fl1, fft1,map1;
    public Transform nt2, nl2, mt2, ft2, fl2, fft2,map2;
    public Transform nt3, nl3, mt3, ft3, fl3, fft3,map3;
    void Start()
    {
        Time.timeScale = 1;
        
        lastXPos = transform.position;
        mainCam = Camera.main;
        
        if (DataSaveManager.Instance.NeedNewGame())//true说明存档没内容，需要新游戏
        {
            Debug.Log("未检测到存档");
            
            buttonContinue.SetActive(false);
            exit1.SetActive(true);
            exit2.SetActive(false);
        }
        else
        {
            Debug.Log("检测到存档");
            
            buttonContinue.SetActive(true);
            exit1.SetActive(false);
            exit2.SetActive(true);
        }
        
        mainCam.transform.position = new Vector3(0, 0, -10);
        
        continueButton.onClick.AddListener(DataSaveManager.Instance.LoadData);
    }

    void Update()
    {
        float distanceX = moveSpeed * Time.deltaTime;
        float distanceY = 0f; // Assuming no vertical movement
        
        //Debug.Log(distanceX);
        
        Vector3 amountToMoveX = new Vector3(distanceX, distanceY, 0f);
        
        // Update background positions based on camera movement
        UpdateParallaxBackground(nt1, nl1, mt1, ft1, fl1, fft1,map1, amountToMoveX, 0.95f, 0.9f, 0.8f, 0.7f,1);
        UpdateParallaxBackground(nt2, nl2, mt2, ft2, fl2, fft2,map2, amountToMoveX, 0.95f, 0.9f, 0.8f, 0.7f,1);
        UpdateParallaxBackground(nt3, nl3, mt3, ft3, fl3, fft3,map3, amountToMoveX, 0.95f, 0.9f, 0.8f, 0.7f,1);
        // Move the main camera
        mainCam.transform.position += amountToMoveX;

        lastXPos = mainCam.transform.position;
    }

    void UpdateParallaxBackground(Transform nt, Transform nl, Transform mt, Transform ft, Transform fl, Transform fft, Transform map, Vector3 amountToMoveX, float ntSpeed, float mtSpeed, float ftSpeed, float fftSpeed, float mapSpeed)
    {
        nt.position += new Vector3(amountToMoveX.x * ntSpeed, amountToMoveX.y * ntSpeed, 0f) * 0.1f;
        nl.position += new Vector3(amountToMoveX.x * ntSpeed, amountToMoveX.y * ntSpeed, 0f) * 0.1f;
        map.position +=new Vector3(amountToMoveX.x * mapSpeed, amountToMoveX.y * mapSpeed, 0f) * 0.1f;
        
        mt.position += new Vector3(amountToMoveX.x * mtSpeed, amountToMoveX.y * mtSpeed, 0f) * 0.1f;

        ft.position += new Vector3(amountToMoveX.x * ftSpeed, amountToMoveX.y * ftSpeed, 0f) * 0.1f;
        fl.position += new Vector3(amountToMoveX.x * ftSpeed, amountToMoveX.y * ftSpeed, 0f) * 0.1f;

        fft.position += new Vector3(amountToMoveX.x * fftSpeed, amountToMoveX.y * fftSpeed, 0f) * 0.1f;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        EventCenter.Broadcast(EventType.SetActive,true);
        
        // Reset game parameters and load new game scene
        var enemyFsm = enemyPrefab.GetComponent<EnemyFSM>();
        enemyFsm.paramenter.health = 10;
        enemyFsm.paramenter.moveSpeed = 1;
        enemyFsm.paramenter.attackTimer = 2;
        
        var playerFsm = playerPrefab.GetComponent<PlayerFSM>();
        playerFsm.paramenter.isDead = false;
        playerFsm.paramenter.rigidbody2D = playerPrefab.GetComponent<Rigidbody2D>();
        playerFsm.paramenter.rigidbody2D = playerPrefab.GetComponent<Rigidbody2D>();
        
        DataSaveManager.Instance.DeleteData();
        EventCenter.Broadcast(EventType.ResetPlayer);
        
        SceneManager.LoadScene("GameScene");
    }
}
