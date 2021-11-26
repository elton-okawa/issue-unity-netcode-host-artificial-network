using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;   

public class PingRpc : NetworkBehaviour {

    [SerializeField] float pingIntervalSec = 0.5f;
    [SerializeField] Text pingText;

    // Ping
    const int bufferSize = 5;
    float[] rttWindow = new float[bufferSize];
    float[] pingHistory = new float[bufferSize];
    uint pingId = 0;

    // Cache
    Dictionary<ulong, ClientRpcParams> clientRpcParams = new Dictionary<ulong, ClientRpcParams>();

    void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
    }


    public override void OnDestroy() {
        base.OnDestroy();
        StopAllCoroutines();
        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
        }
    }

    void OnConnected(ulong clientId) {
        if (IsClient) {
            StartCoroutine(PingRoutine());
        }
    }

    IEnumerator PingRoutine() {
        while (true) {
            pingHistory[pingId % pingHistory.Length] = Time.realtimeSinceStartup;
            PingServerRpc(pingId);
            pingId++;

            pingText.text = $"{Mathf.RoundToInt(PingHelper.CalculateMeanRTT(rttWindow) * 1000)} ms";

            yield return new WaitForSeconds(pingIntervalSec);
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable, RequireOwnership = false)]
    public void PingServerRpc(uint pingId, ServerRpcParams serverRpcParams = default) {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (!clientRpcParams.ContainsKey(clientId)) {
            clientRpcParams.Add(clientId, new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }

        PongClientRpc(pingId, clientRpcParams[clientId]);
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    public void PongClientRpc(uint receivedId, ClientRpcParams clientRpcParams = default) {
        if (pingId >= receivedId + bufferSize) {
            Debug.LogWarning($"[PING] Ping took too long to return (currentId: {pingId}, receivedId: {receivedId})");
        }

        int index = (int)(receivedId % rttWindow.Length);
        rttWindow[index] = Time.realtimeSinceStartup - pingHistory[index];
    }
}
