using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;

public class PingNetworkVar : NetworkBehaviour {
    [SerializeField] float pingIntervalSec = 0.5f;

    Text pingText;
    NetworkVariable<float> time = new NetworkVariable<float>();
    float[] rttWindow = new float[5];
    uint pingId = 0;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
        {
            PingManager pingManager = FindObjectOfType<PingManager>();
            pingText = pingManager.networkVarPingText;

            time.OnValueChanged += OnValueChanged;
            StartCoroutine(PingRoutine());
        }
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        time.OnValueChanged -= OnValueChanged;
        StopAllCoroutines();
    }


    IEnumerator PingRoutine() {
        while (true) {
            var currentTime = Time.realtimeSinceStartup;
            SetTimeServerRpc(currentTime);
           
            yield return new WaitForSeconds(pingIntervalSec);
        }
    }

    void OnValueChanged(float oldValue, float newValue) {
        var currentTime = Time.realtimeSinceStartup;
        rttWindow[pingId % rttWindow.Length] = currentTime - newValue;

        pingText.text = $"{Mathf.RoundToInt(PingHelper.CalculateMeanRTT(rttWindow) * 1000)} ms";

        pingId++;
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    void SetTimeServerRpc(float newTime) {
        time.Value = newTime;
    }
}
