using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndustrialCircuitSimulationManager : MonoBehaviour
{
    public static IndustrialCircuitSimulationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private List<Circuit> circuitsList = new List<Circuit>();
    [SerializeField] private List<IndustrialPanelDeviceConnector> industrialConnectorsList = new List<IndustrialPanelDeviceConnector>();
    [SerializeField] private List<IndustrialPanelDeviceConnector> sourceConnectorsList = new List<IndustrialPanelDeviceConnector>();
    [SerializeField] private List<NodeSimulationResults> resultsList = new List<NodeSimulationResults>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TestCircuitManager();
        }
    }

    /// <summary>
    /// Exeecutes all simulation. It runs with IndustrialPanelDeviceConnector events: OnPropertyValueChanged, OnConnectorStateChanged, OnConnectionChanged
    /// </summary>
    private void RunSpiceSharpCircuitSimulations()
    {
        //Clear data
        ClearSpiceSharpCircuitSimulations();

        //Create circuits based on Connector type.
        CreateCircuits();

        //Add connectors to circuits;
        PopulateCircuits();
        
        //Runs SpiceSharp simulation        
        ExecuteSpiceSharpSimulation();

        //Assign simulation results based on Connecot.Component.Name and NodeSimulationResults.Name
        AssignCircuitSimulationResults();

        //Debug Circuits connections.
        ListComponentsInCircuit();
    }

    #region MainSimulationMethods
    private void ClearSpiceSharpCircuitSimulations() 
    {
        circuitsList.Clear();
        industrialConnectorsList.Clear();
        sourceConnectorsList.Clear();
        resultsList.Clear();
    }
    private void CreateCircuits() 
    {
        industrialConnectorsList = new List<IndustrialPanelDeviceConnector>(FindObjectsOfType<IndustrialPanelDeviceConnector>());
        foreach (IndustrialPanelDeviceConnector connector in industrialConnectorsList)
        {
            if (connector.GetConnectorType() == IndustrialPanelDeviceConnector.ConnectorType.Source)
            {
                sourceConnectorsList.Add(connector);
                circuitsList.Add(new Circuit());
            }
        }
    }
    private void PopulateCircuits()
    {
        int circuitIndex = 0;
        foreach (IndustrialPanelDeviceConnector sourceConnector in sourceConnectorsList)
        {
            //BuildCircuit();
            AddSpiceSharpComponentToCircuit(circuitIndex, sourceConnector);
            if (sourceConnector.GetConnectedTo())
            {
                sourceConnector.GetConnectedTo().AddSpiceSharpComponentToCircuit(circuitIndex);
            }
            circuitIndex++;
        }
    }
    private void ExecuteSpiceSharpSimulation() 
    {
        foreach(Circuit circuit in circuitsList)
        {
            //Create circuit
            var dc = new DC(circuit.ToString(), circuit.ElementAt(0).Name, circuitsList[0].ElementAt(0).GetProperty<double>("dc"), circuitsList[0].ElementAt(0).GetProperty<double>("dc"), 0.1f);

            dc.ExportSimulationData += (sender, args) =>
            {
                foreach (SpiceSharp.Components.Component component in circuit)
                {
                    foreach (var node in component.Nodes)
                    {
                        resultsList.Add(new NodeSimulationResults
                        {
                            NodeName = node.ToString(),
                            NodeVoltage = (float)args.GetVoltage(component.Nodes[0]),
                            NodeCurrent = 0//(float)(args.GetVoltage(component.Nodes[0]) - args.GetVoltage(component.Nodes[1]) / component.GetProperty<double>("Resistance")) ADD CONDITIONS. CURRENT CALCULATIONS BASED ON CONNECTOR TYPE. SINK => R = deltaV/R. Source => propertyValue
                        });
                    }
                }
            };

            dc.Run(circuit);
        }        
    }
    private void AssignCircuitSimulationResults() 
    {
        foreach (NodeSimulationResults result in resultsList)
        {
            foreach (IndustrialPanelDeviceConnector connector in industrialConnectorsList)
            {
                if (result.NodeName == connector.GetConnectorName())
                {
                    connector.SetCurrentCurrent(result.NodeCurrent);
                    connector.SetCurrentVoltage(result.NodeVoltage);
                }
            }
        }
        
    }

    #region AuxiliarySimulationMethods
    public void ListComponentsInCircuit()
    {
        Debug.Log("Circuits Count: " + circuitsList.Count);
        int circuitIndex = 0;

        foreach (Circuit circuit in circuitsList)
        {
            foreach (SpiceSharp.Components.Component component in circuit)
            {
                Debug.Log("Circuit Number" + circuitIndex);
                Debug.Log(component.Name);
                Debug.Log(component.Nodes[0]);
                Debug.Log(component.Nodes[1]);
                Debug.Log(component.GetType());
            }
            circuitIndex++;
        }
    }
    private SpiceSharp.Components.Component BuildConnectorComponent(IndustrialPanelDeviceConnector deviceConnector)
    {
        if (deviceConnector.GetConnectorType() == IndustrialPanelDeviceConnector.ConnectorType.Source)
        {
            SpiceSharp.Components.Component connectorComponent = new VoltageSource(
                deviceConnector.GetIndustrialDevice().GetDeviceName(),
                deviceConnector.GetConnectorName(),
                deviceConnector.GetPairConnector().GetConnectorName(),
                deviceConnector.GetPropertyValue());
            return connectorComponent;
        }

        else if (deviceConnector.GetConnectorType() == IndustrialPanelDeviceConnector.ConnectorType.Sink)
        {
            SpiceSharp.Components.Component connectorComponent = new Resistor(
                deviceConnector.GetIndustrialDevice().GetDeviceName(),
                deviceConnector.GetConnectorName(),
                deviceConnector.GetPairConnector().GetConnectorName(),
                deviceConnector.GetPropertyValue());
            return connectorComponent;
        }
        else if (deviceConnector.GetConnectorType() == IndustrialPanelDeviceConnector.ConnectorType.Cable)
        {
            if (deviceConnector.GetConnectedTo() && deviceConnector.GetPairConnector().GetConnectedTo())
            {
                SpiceSharp.Components.Component connectorComponent = new Resistor(
                deviceConnector.GetIndustrialDevice().GetDeviceName(),
                deviceConnector.GetConnectedTo().GetConnectorName(),
                deviceConnector.GetPairConnector().GetConnectedTo().GetConnectorName(),
                deviceConnector.GetPropertyValue());
                return connectorComponent;
            }
        }
        return null;
    }
    public bool IsComponentInCircuit(int circuitIndex, IndustrialPanelDeviceConnector deviceConnector)
    {
        SpiceSharp.Components.Component newComponent = BuildConnectorComponent(deviceConnector);

        if (newComponent != null)
        {
            if (circuitsList[circuitIndex].Contains(newComponent.Name))
            {
                //Debug.Log("Component " + newComponent.Name + " of type " + newComponent.GetType() + " is already on circuit " + circuitIndex);
                return true;
            }
            else
            {
                //Debug.Log("Component " + newComponent.Name + " of type " + newComponent.GetType() + " added to circuit " + circuitIndex);
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    public void AddSpiceSharpComponentToCircuit(int circuitIndex, IndustrialPanelDeviceConnector deviceConnector)
    {
        if (IsComponentInCircuit(circuitIndex, deviceConnector))
        {
            return;
        }
        else
        {
            SpiceSharp.Components.Component newComponent = BuildConnectorComponent(deviceConnector);
            circuitsList[circuitIndex].Add(newComponent);
        }
    }
    #endregion
    #endregion


    public void TestCircuitManager()
    {
        var ckt = new Circuit(
            new VoltageSource("V1", "in", "0", 1.0),
            new Resistor("R1", "in", "out", 1.0e4),
            new Resistor("R2", "out", "0", 2.0e4)
        );

        // Create a DC simulation that sweeps V1 from -1V to 1V in steps of 100mV
        var dc = new DC("DC 1", "V1", 1.0, 1.0, 0.1f);

        // Create exports
        var V1_in_Export = new RealVoltageExport(dc, "in");
        var V1_gnd_Export = new RealVoltageExport(dc, "0");
        var R1_pos_Export = new RealVoltageExport(dc, "in");
        var R1_neg_Export = new RealVoltageExport(dc, "out");
        var R2_pos_Export = new RealVoltageExport(dc, "out");
        var R2_neg_Export = new RealVoltageExport(dc, "0");

        // Catch exported data
        dc.ExportSimulationData += (sender, args) =>
        {
            var V1_in = V1_in_Export.Value;
            var V1_gnd = V1_in_Export.Value;
            var R1_pos = R1_pos_Export.Value;
            var R1_neg = R1_neg_Export.Value;
            var R2_pos = R2_pos_Export.Value;
            var R2_neg = R2_neg_Export.Value;
            Debug.Log(V1_in);
            Debug.Log(V1_gnd);
            Debug.Log(R1_pos);
            Debug.Log(R1_neg);
            Debug.Log(R2_pos);
            Debug.Log(R2_neg);
        };
        dc.Run(ckt);
    }
    public void UpdateVFXAtConnector() { }
    public void UpdateSFXAtConnector() { }

    private void OnEnable()
    {
        IndustrialPanelDevice.OnDeviceStateChange += RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnConnectionChanged += RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnConnectorStateChanged += RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnPropertyValueChanged += RunSpiceSharpCircuitSimulations;
    }
    private void OnDestroy()
    {
        IndustrialPanelDevice.OnDeviceStateChange -= RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnConnectionChanged -= RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnConnectorStateChanged -= RunSpiceSharpCircuitSimulations;
        IndustrialPanelDeviceConnector.OnPropertyValueChanged -= RunSpiceSharpCircuitSimulations;
    }
}


public class NodeSimulationResults
{
    public string NodeName { get; set; }
    public float NodeVoltage { get; set; }
    public float NodeCurrent { get; set; }
}

