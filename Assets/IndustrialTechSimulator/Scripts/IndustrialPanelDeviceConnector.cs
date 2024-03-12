using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IndustrialPanelDeviceConnector : MonoBehaviour
{
    public enum ConnectorType
    {
        None,
        Source,
        Sink,
        Cable
    }
    public enum ConnectorState
    {
        None,
        Underpowered,
        Overloaded
    }

    [SerializeField] private string _connectorName;
    [SerializeField] private float _propertyValue;
    [SerializeField] private float _maxCurrent;
    [SerializeField] private float _currentCurrent;
    [SerializeField] private float _minCurrent;
    [SerializeField] private float _maxVoltage;
    [SerializeField] private float _currentVoltage;
    [SerializeField] private float _minVoltage;
    [SerializeField] private ConnectorType _connectorType;
    [SerializeField] private ConnectorState _connectorState;

    [SerializeField] private IndustrialPanelDevice industrialDevice;
    [SerializeField] private IndustrialPanelDeviceConnector pairConnector;
    [SerializeField] private IndustrialPanelDeviceConnector connectedTo;

    public static Action OnConnectionChanged;
    public static Action OnPropertyValueChanged;
    public static Action OnConnectorStateChanged;


    private void Awake()
    {
        //Set industrialDevice parent.
        SetIndustrialDevice(GetComponentInParent<IndustrialPanelDevice>());

        //Set connector Name. Use IndustrialPanelDevice + gameobject.name (this Connector)
        SetConnectorName(industrialDevice.name + this.name);

        //Set pair connector. This action has to be made manually due to multiple connectors in devices.
    }

    #region SETGET
    public void SetConnectorName(string connectorName)
    {
        _connectorName = connectorName;
    }
    public string GetConnectorName()
    {
        return _connectorName;
    }
    public void SetPropertyValue( float value )
    {
        _propertyValue = value;
        OnPropertyValueChanged.Invoke();
    }
    public float GetPropertyValue()
    { 
        return _propertyValue;
    }
    public void SetMaxCurrent(float value)
    {
        _maxCurrent = value;
    }
    public float GetMaxCurrent()
    {
        return _maxCurrent;
    }
    public void SetCurrentCurrent(float value)
    {
        _currentCurrent = value;
    }
    public float GetCurrentCurrent()
    {
        return _currentCurrent; 
    }
    public void SetMinCurrent(float value)
    {
        _minCurrent = value;
    }
    public float GetMinCurrent()
    {
        return _minCurrent;
    }
    public void SetMaxVoltage(float value)
    {
        _maxVoltage = value;
    }
    public float GetMaxVoltage()
    {
        return _maxVoltage;
    }
    public void SetCurrentVoltage(float value)
    {
        _currentVoltage = value;
    }
    public float GetCurrentVoltage()
    {
        return _currentVoltage;
    }
    public void SetMinVoltage(float value)
    {
        _maxVoltage = value;
    }
    public float GetMinVoltage()
    {
        return _minVoltage;
    }
    public void SetConnectorType(ConnectorType connectorType)
    {
        _connectorType = connectorType;
    }
    public ConnectorType GetConnectorType()
    {
        return _connectorType;
    }
    public void SetConnectorState(ConnectorState newState)
    {
        _connectorState = newState;
        OnConnectorStateChanged.Invoke();
    }
    public ConnectorState GetConnectorState()
    {
        return _connectorState;
    }
    public void SetIndustrialDevice(IndustrialPanelDevice industrialPanelDevice)
    {
        industrialDevice = industrialPanelDevice;
    }
    public IndustrialPanelDevice GetIndustrialDevice() 
    { 
        return industrialDevice; 
    }
    public IndustrialPanelDeviceConnector GetPairConnector()
    {
        return pairConnector;
    }
    public IndustrialPanelDeviceConnector GetConnectedTo()
    {
        return connectedTo;
    }
    #endregion

    /// <summary>
    /// Method used for recursive execution of adding devices to a singular circuit (Industrial Device Source). Se sugiere agregar el device en vez del conector, y de él extraer la infromación. 
    /// Se manda de parametro la fuente inicial para que se asocie a la cadena de ella. Debe enviarse a ejecutar esta función para cada conector asociado a este (utilizar el interactor socket.
    /// </summary>
    public void AddSpiceSharpComponentToCircuit(int circuitIndex)
    {
        if (IndustrialCircuitSimulationManager.Instance.IsComponentInCircuit(circuitIndex, this))
        {
            return;
        }
        else
        {
            IndustrialCircuitSimulationManager.Instance.AddSpiceSharpComponentToCircuit(circuitIndex, this);
        }
        if (pairConnector.GetConnectedTo() != null)
        {
            pairConnector.GetConnectedTo().AddSpiceSharpComponentToCircuit(circuitIndex);
        }
    }

    //When XRSocketInteractor is Select Enter
    public void SetConnectedToConnector(SelectEnterEventArgs args)
    {
        if (this.gameObject.GetComponent<XRSocketInteractor>())
        {
            connectedTo = this.gameObject.GetComponent<XRSocketInteractor>().GetOldestInteractableSelected().transform.GetComponent<IndustrialPanelDeviceConnector>();
        }
        
        if (this.gameObject.GetComponent<XRGrabInteractable>())
        {
            connectedTo = this.gameObject.GetComponent<XRGrabInteractable>().GetOldestInteractorSelecting().transform.GetComponent<IndustrialPanelDeviceConnector>();
        }
        IndustrialPanelDeviceConnector.OnConnectionChanged();
    }

    //When XRSocketInteractor is Select Exit
    public void SetConnectedToNull(SelectExitEventArgs args)
    {
        connectedTo = null;
        IndustrialPanelDeviceConnector.OnConnectionChanged();
    }
    
}