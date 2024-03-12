using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using StarterAssets;
using UnityEngine.InputSystem;
using Dreamteck.Splines;

public class IndustrialCableVisualController : MonoBehaviour
{
    [SerializeField] private SplineComputer _cableSpline;

    [SerializeField] private GameObject _connector1;
    [SerializeField] private GameObject _connector2;

    [SerializeField] private List<SplinePoint> _splinePoints;

    private void Start()
    {
        _cableSpline = GetComponentInChildren<SplineComputer>();

        SplinePoint iniPoint = new SplinePoint(new Vector3(_connector1.transform.position.x, _connector1.transform.position.y, _connector1.transform.position.z));
        SplinePoint endPoint = new SplinePoint(new Vector3(_connector2.transform.position.x, _connector2.transform.position.y, _connector2.transform.position.z));
        _splinePoints = new List<SplinePoint>() { iniPoint, endPoint };
    }

    private void Update()
    {
        SplinePoint iniPoint = new SplinePoint(new Vector3(_connector1.transform.position.x, _connector1.transform.position.y, _connector1.transform.position.z));
        SplinePoint endPoint = new SplinePoint(new Vector3(_connector2.transform.position.x, _connector2.transform.position.y, _connector2.transform.position.z));

        _splinePoints[0] = iniPoint;
        _splinePoints[_splinePoints.Count - 1] = endPoint;

        _cableSpline.SetPoints(_splinePoints.ToArray());
    }

    public void AddKnot(GameObject connector)
    {
        if (connector == _connector1)
        {
            SplinePoint newSplinePointAtConnectorPosition = new SplinePoint(connector.transform.position);
            _splinePoints.Insert(1, newSplinePointAtConnectorPosition);
        }
        else if (connector == _connector2)
        {
            SplinePoint newSplinePointAtConnectorPosition = new SplinePoint(connector.transform.position);
            _splinePoints.Insert(_splinePoints.Count - 1, newSplinePointAtConnectorPosition);
        }
    }

    public void RemoveKnot(GameObject connector)
    {
        if (_splinePoints.Count > 2)
        {
            if (connector == _connector1)
            {
                _splinePoints.RemoveAt(1);
            }
            else if (connector == _connector2)
            {
                _splinePoints.RemoveAt(_splinePoints.Count - 2);
            }
        }
    }
}
