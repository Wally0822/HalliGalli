using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class JoimRoomMgr : MonoBehaviourPunCallbacks
{
    #region 싱글턴
    private static JoimRoomMgr instance;
    public static JoimRoomMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<JoimRoomMgr>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(JoimRoomMgr), typeof(JoimRoomMgr)).GetComponent<JoimRoomMgr>();
                }
            }
            return instance;
        }
    }

    #endregion
    [Header("StartPanel")]
    [SerializeField] GameObject startPanel = null;
    [SerializeField] GameObject joinRoomPanel = null;
    [SerializeField] TMP_InputField nickName = null;
    [SerializeField] Button joinRoomButton = null;

    [Header("PlayerMachingPanel")]
    [SerializeField] TextMeshProUGUI nickNametext1 = null;
    [SerializeField] Button gamestartButton = null;
    [SerializeField] Button isReadyButton = null;


    RoomOptions roomOptions = new RoomOptions();

    private void Awake()
    {
        // Set Screen Size 16 : 9 fullscreen false
        Screen.SetResolution(1920, 1080, true);
        // Improves the performance of the Photon network
        // Defines how many times per second PhotonNetwork should send packages
        PhotonNetwork.SendRate = 60;
        // PhotonView 들이 OnPhotonSerialize를 초당 몇회 호출할지
        // How many times per second PhotonViews call OnPhotonSerialize
        PhotonNetwork.SerializationRate = 30;
        // Automatically synchronized scenes from other clients when switching scenes from the master server.
        // 마스터 서버에서 장면을 전환할 때 다른 클라이언트의 장면을 자동으로 동기화합니다.
        PhotonNetwork.AutomaticallySyncScene = true;
        // Don't get a photonmessage when access the room.
        PhotonNetwork.IsMessageQueueRunning = false;
    }

    void Start()
    {
        joinRoomButton.interactable = false;

        PhotonNetwork.ConnectUsingSettings(); // 접속 시도
        Debug.Log($"PhotonNetwork.IsConnectedAndReady : {PhotonNetwork.IsConnectedAndReady}");
    }

    void Update()
    {
        if (nickName.text.Length > 4 && nickName.text.Length < 8)
        {
            Debug.Log("## nickName.Length : " + nickName.text.Length);
            joinRoomButton.interactable = true;
        }
        else if (nickName.text.Length == 0)
        {
            joinRoomButton.interactable = false;
        }
    }

    // 마스터 서버 접속 성공시에 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("## OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)  // Call when the master server is not connected
    {
        PhotonNetwork.ConnectUsingSettings();
        joinRoomButton.interactable = false;
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }


    public void OnClick_JoinRoomButton()
    {
        if (string.IsNullOrEmpty(nickName.text)) return;

        PhotonNetwork.LocalPlayer.NickName = nickName.text;

        //roomOptions.IsOpen = true;  // 방 보이게
        if (roomOptions.MaxPlayers == 0)
        {
            roomOptions.MaxPlayers = 4;
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }

        startPanel.SetActive(false);
        joinRoomPanel.SetActive(true);

        isReadyButton.interactable = false;
    }

    #region PlayerMatching
    int ReadyCount = 0;
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            gamestartButton.interactable = false;
        }
    }

    public void OnClick_ReadyButton()
    {
        ReadyCount += 1;
        isReadyButton.interactable = true;
    }

    // joinRoom Panel 에서의 뒤로 가기 버튼
    public void Onclick_BackButton()
    {
        joinRoomPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    #endregion
    // 실행 종료
    public void OnClick_ExitButton()
    {
        Application.Quit();
    }
}
