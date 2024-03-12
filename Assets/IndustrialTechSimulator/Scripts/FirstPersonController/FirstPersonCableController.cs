using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class FirstPersonCableController : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    [SerializeField] private XRRayInteractor _xRRayInteractor;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    private void Update()
    {
        AddAnchorToCable();
        RemoveAnchorToCable();
    }

    public void AddAnchorToCable()
    {
        if (_input.addAnchor == true && _xRRayInteractor.interactablesSelected.Count > 0)
        {
            if (_xRRayInteractor.interactablesSelected.First().transform.GetComponent<IndustrialPanelDeviceConnector>())
            {
                IndustrialPanelDevice cable = _xRRayInteractor.interactablesSelected.First().transform.GetComponent<IndustrialPanelDeviceConnector>().GetIndustrialDevice();
                cable.GetComponent<IndustrialCableVisualController>().AddKnot(_xRRayInteractor.interactablesSelected.First().transform.gameObject);
            }
            else
            {
                Debug.Log("This is not a cable. Can't add an anchor to a non-cable object.");
            }
        }
        _input.addAnchor = false;
    }

    public void RemoveAnchorToCable()
    {
        if (_input.removeAnchor == true && _xRRayInteractor.interactablesSelected.Count > 0)
        {
            if (_xRRayInteractor.interactablesSelected.First().transform.GetComponent<IndustrialPanelDeviceConnector>())
            {
                IndustrialPanelDevice cable = _xRRayInteractor.interactablesSelected.First().transform.GetComponent<IndustrialPanelDeviceConnector>().GetIndustrialDevice();
                cable.GetComponent<IndustrialCableVisualController>().RemoveKnot(_xRRayInteractor.interactablesSelected.First().transform.gameObject);
            }
            else
            {
                Debug.Log("This is not a cable. Can't remove an anchor to a non-cable object.");
            }
        }
        _input.removeAnchor = false;
    }
}
