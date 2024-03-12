using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static IndustrialPanelDeviceConnector;
using UnityEngine.XR.Interaction.Toolkit;

public class IndustrialPanelDevice : MonoBehaviour
{
    public enum DeviceState
    {
        None,
        Turn_On,
        Turn_Off,
        Damaged
    }

    [HideInInspector]
    [SerializeField] private int _commonConnectorIndex;

    [SerializeField] private string _deviceName;
    [SerializeField] private float _deviceCurrent;
    [SerializeField] private float _deviceVoltage;
    [SerializeField] private DeviceState _deviceState;

    [SerializeField] private List<IndustrialPanelDeviceBehaviour> _industrialPanelDiviceBehavioursList;
    [SerializeField] private List<IndustrialPanelDeviceConnector> _industrialPanelDeviceConnectorsList;

    public static Action OnDeviceStateChange;

    private void Awake()
    {
        SetDeviceName(gameObject.name);
        SetDeviceState(DeviceState.Turn_Off);
    }

    #region SETGET
    public void SetDeviceName(string deviceName)
    {
        _deviceName = deviceName;
    }
    public string GetDeviceName()
    {
        return _deviceName;
    }
    public void SetDeviceState(DeviceState newState)
    {
        _deviceState = newState;
        OnDeviceStateChange?.Invoke();
    }
    public DeviceState GetDeviceState()
    {
        return _deviceState;
    }
    public void SetDeviceCurrent(float value)
    {
        _deviceCurrent = value;
    }
    public float  GetDeviceCurrent()
    {
        return _deviceCurrent;
    }
    public void SetDeviceVoltage(float value)
    {
        _deviceVoltage = value;
    }
    public float GetDeviceVoltage()
    {
        return _deviceVoltage;
    }
    public void SetCommonConnectorName()
    {
        //When user pick up Common connector, set conncetor name to 0. Only if Device/Connector is source
    }
    #endregion

#if UNITY_EDITOR
    /// <summary>
    /// Method used for populate IndustrialPanelDeviceEditor dropdown with the connectors options.
    /// </summary>
    /// <returns>A list of strings with the industrial panel device connectors associated.</returns>
    public List<string> GetIndustrialPanelDeviceConnectorsNameList()
    {
        List<string> IndustrialPanelDeviceConnectorsNameList = new List<string>();
        foreach (IndustrialPanelDeviceConnector connector in _industrialPanelDeviceConnectorsList)
        {
            IndustrialPanelDeviceConnectorsNameList.Add(connector.name);
        }
        return IndustrialPanelDeviceConnectorsNameList;
    }

    /// <summary>
    /// Method used for update the industrial panel device connector list.
    /// </summary>
    public void UpdateIndustrialPanelDeviceConnectorList()
    {
        _industrialPanelDeviceConnectorsList.Clear();
        foreach (IndustrialPanelDeviceConnector connectorGO in GetComponentsInChildren<IndustrialPanelDeviceConnector>())
        {
            _industrialPanelDeviceConnectorsList.Add(connectorGO);
        }
    }

#endif
}


