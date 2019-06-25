using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputController
{
    private const string _leftJoystickX = "LeftJoystickX";
    private const string _leftJoystickY = "LeftJoystickY";
    private const string _rightJoystickX = "RightJoystickX";
    private const string _rightJoystickY = "RightJoystickY";
    private const string _triggerLeft = "TriggerLeft";
    private const string _triggerRight = "TriggerRight";

    private const string _jumpButton = "Jump";
    private const string _interactButton = "Interact";
    private const string _holsterButton = "HolsterGun";
    private const string _punchButton = "Punch";

    public static float LeftJoystickX{get=> Input.GetAxis(_leftJoystickX); }
    public static float LeftJoystickY { get=> Input.GetAxis(_leftJoystickY); }

    public static float RightJoystickX { get => Input.GetAxis(_rightJoystickX); }
    public static float RightJoystickY { get => Input.GetAxis(_rightJoystickY); }

    public static float LeftTrigger { get => Input.GetAxis(_triggerLeft); }
    public static float RightTrigger { get => Input.GetAxis(_triggerRight); }

    public static bool JumpButtonDown { get => Input.GetButtonDown(_jumpButton); }
    public static bool InteractButtonDown { get => Input.GetButtonDown(_interactButton); }
    public static bool InteractButton { get => Input.GetButton(_interactButton); }

    public static bool HolsterButtonDown { get => Input.GetButtonDown(_holsterButton); }
    public static bool PunchButtonDown { get => Input.GetButtonDown(_punchButton); }

}
