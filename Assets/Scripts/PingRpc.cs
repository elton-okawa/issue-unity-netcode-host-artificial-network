using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;   

public class PingRpc : NetworkBehaviour {

    [SerializeField] float pingIntervalSec = 0.5f;
    [SerializeField] Text pingText;

    float[] rttWindow = new float[5];
    uint pingId = 0;

    // Cache
    Dictionary<ulong, ClientRpcParams> clientRpcParams = new Dictionary<ulong, ClientRpcParams>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(PingRoutine());
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        StopAllCoroutines();
    }

    IEnumerator PingRoutine() {
        while (true) {
            PingServerRpc(Time.realtimeSinceStartup);

            yield return new WaitForSeconds(pingIntervalSec);
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable, RequireOwnership = false)]
    public void PingServerRpc(float sentTime, ServerRpcParams serverRpcParams = default) {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (!clientRpcParams.ContainsKey(clientId)) {
            clientRpcParams.Add(clientId, new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }

        PongClientRpc(sentTime, clientRpcParams[clientId]);
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    public void PongClientRpc(float sentTime, ClientRpcParams clientRpcParams = default) {
        var currentTime = Time.realtimeSinceStartup;
        rttWindow[pingId % rttWindow.Length] = currentTime - sentTime;

        pingText.text = $"{Mathf.RoundToInt(PingHelper.CalculateMeanRTT(rttWindow) * 1000)} ms";

        pingId++;
    }
}
