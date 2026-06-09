using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Room Settings")]
    [SerializeField] private string roomName = "TestRoom";
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private NetworkRunner runner;

    async void Start()
    {
        await StartGame();
    }

    public async Task StartGame()
    {
        // Runner生成
        runner = gameObject.AddComponent<NetworkRunner>();

        // 入力を有効化
        runner.ProvideInput = true;

        // Callback登録
        runner.AddCallbacks(this);

        // SceneManager追加
        var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        Debug.Log("Starting Fusion...");

        // ゲーム開始
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            //Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            //SceneManager = sceneManager
        });

        if (result.Ok)
        {
            Debug.Log("Fusion Start Success");
        }
        else
        {
            Debug.LogError($"Fusion Start Failed: {result.ShutdownReason}");
            Debug.LogError($"Error Message: {result.ErrorMessage}");
        }
    }

    // =========================
    // Player Join / Leave
    // =========================

   public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
{
    Debug.Log($"Player Joined: {player}");

    if (player == runner.LocalPlayer)
    {
        Vector3 spawnPosition;

        if (player.RawEncoded == 1)
        {
            spawnPosition = new Vector3(0, 1, 0);
        }
        else
        {
            spawnPosition = new Vector3(2, 1, 0);
        }

        runner.Spawn(
            playerPrefab,
            spawnPosition,
            Quaternion.identity,
            player
        );
    }
}

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Left: {player}");
    }

    // =========================
    // Connection
    // =========================

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected To Server");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning($"Disconnected: {reason}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning($"Shutdown: {shutdownReason}");
    }

    // =========================
    // Empty Required Callbacks
    // =========================

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    
    public void OnReliableDataProgress(
    NetworkRunner runner,
    PlayerRef player,
    ReliableKey key,
    float progress)
{
    
}
}