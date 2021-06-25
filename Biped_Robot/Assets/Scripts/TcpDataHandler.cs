using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;

using TCP;

public class TcpDataHandler : MonoBehaviour {

    public TcpComClient TcpClient = new TcpComClient();
    
    public Reward rewarder;
    public ResetScene reseter;
    public ParameterHandler parameterHandler;
    public ControlScript controller;

    private bool request;


    [Serializable]
    public class Telemetry {

        public float reward;
        public float[] parameters;
        public long date;
        public bool done;

    }

    [Serializable]
    public class Orders {

        public float[] torques;
        public bool startPermission = false;

    }

    public Orders currentOrders = new Orders();
    public Telemetry currentTelemetry = new Telemetry();

    void Start() {
        TcpClient.SetEventFunction(OnOrder);
        TcpClient.connect();

        request = false;

    }

    void FixedUpdate() {

        if (request) {
            request = false;

            TcpClient.joinedSend(GetTelemetryData());
            controller.UpdateJoints(currentOrders.torques);

            if (currentOrders.startPermission) {
                reseter.continuePermission = true;
                reseter.done = false;
            }
            
        }
    }


    string GetTelemetryData() {

        currentTelemetry.reward = rewarder.reward;
        currentTelemetry.done = reseter.done;

        currentTelemetry.parameters = parameterHandler.parameterArray;

        DateTimeOffset utcTime = DateTimeOffset.UtcNow;
        long unixTimeStampInMilliseconds = utcTime.ToUnixTimeMilliseconds();
        currentTelemetry.date = unixTimeStampInMilliseconds;

        return JsonUtility.ToJson(currentTelemetry);
    }

    void OnApplicationQuit() {
        TcpClient.disconnect();
    }

    void OnOrder(string data) {
        try {
            JsonUtility.FromJsonOverwrite(data, currentOrders);

        } catch (Exception e) {
            Debug.Log(data);
            Debug.Log("error on jsonutility in coming message"+ e);
        }
  
        request = true;

    }
}
