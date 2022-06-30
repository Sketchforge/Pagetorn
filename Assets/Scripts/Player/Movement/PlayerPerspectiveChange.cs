using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerspectiveChange : MonoBehaviour
{
    // Start is called before the first frame update
    public bool thirdPerson = false; //change this to Event later
    [SerializeField] GameObject _thirdPersonCam;
    [SerializeField] GameObject _firstPersonCam;
    [SerializeField] MouseRotation _playerRotator;
    [SerializeField] MouseRotation _headRotator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Debug.Log("Changed P");
            TogglePerspective();
        }
    }

    void TogglePerspective()
    {
        thirdPerson = !thirdPerson;

        if (!thirdPerson) //if not third person, make it third person.
        {
            if (!_thirdPersonCam.activeSelf) //if camera is not active, activate it
                _thirdPersonCam.SetActive(true);
            if (_playerRotator.isActiveAndEnabled) //if player rotator is enabled, disable it.
            {
                _playerRotator.enabled = false;
            }
            if (_headRotator.rotateX == false)
            {
                _headRotator.rotateX = true;
            }  
        }

        else if (thirdPerson) //if in third person, make it first person.
        {
            if (_thirdPersonCam.activeSelf) //if 3rd-person camera is active, deactivate it
                _thirdPersonCam.SetActive(false);
            if (!_playerRotator.isActiveAndEnabled) //if player rotator is disabled, enable it.
            {
                _playerRotator.enabled = true;
            }
            if (_headRotator.rotateX == true)
            {
                _firstPersonCam.transform.forward = _headRotator.transform.forward;
                _firstPersonCam.transform.position = _headRotator.transform.position;
                _headRotator.rotateX = false;
            }
            
        }
    }
}
