using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(IndustrialPanelDevice))]
public class IndustrialPanelDeviceEditor : Editor
{
    SerializedProperty _commonConnectorIndex;

    private void OnEnable()
    {
        _commonConnectorIndex = serializedObject.FindProperty("_commonConnectorIndex");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        IndustrialPanelDevice industrialPanelDevice = (IndustrialPanelDevice)target;

        if (GUILayout.Button("Update Dropdown Options"))
        {
            industrialPanelDevice.UpdateIndustrialPanelDeviceConnectorList();
        }

        GUIContent connectorList = new GUIContent("GND Connector");
        _commonConnectorIndex.intValue = EditorGUILayout.Popup(connectorList, _commonConnectorIndex.intValue, industrialPanelDevice.GetIndustrialPanelDeviceConnectorsNameList().ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}
