using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;

public class PingNetworkVar : NetworkBehaviour {
    [SerializeField] float pingIntervalSec = 0.5f;
    [SerializeField] Text pingText;

    NetworkVariable<float> time = new NetworkVariable<float>();
    float[] rttWindow = new float[5];
    uint pingId = 0;

    void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
    }

    public override void OnDestroy() {
        base.OnDestroy();
        StopAllCoroutines();

        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
        }

        if (IsClient) {
            time.OnValueChanged -= OnValueChanged;
        }
    }

    void OnConnected(ulong clientId) {
        if (IsClient) {
            time.OnValueChanged += OnValueChanged;
            StartCoroutine(PingRoutine());
        }
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

    [ServerRpc(Delivery = RpcDelivery.Unreliable, RequireOwnership = false)]
    void SetTimeServerRpc(float newTime) {
        time.Value = newTime;
    }
}
