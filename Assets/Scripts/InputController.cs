using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputController
{
    private const string _runXAxis = "RunX";
    private const string _runYAxis = "RunY";
    private const string _lookXAxis = "LookX";
    private const string _lookYAxis = "LookY";
    private const string _aimGunAxis = "AimGun";
    private const string _fireGunAxis = "FireGun";

    private const string _jumpButton = "Jump";
    private const string _interactButton = "Interact";
    private const string _holsterButton = "HolsterGun";
    private const string _punchButton = "Punch";

    public static float RunXAxis{get=> Input.GetAxis(_runXAxis); }
    public static float RunYAxis { get=> Input.GetAxis(_runYAxis); }

    public static float LookXAxis { get => Input.GetAxis(_lookXAxis); }
    public static float LookYAxis { get => Input.GetAxis(_lookYAxis); }

    public static float AimGunAxis { get => Input.GetAxis(_aimGunAxis); }
    public static float FireGunAxis { get => Input.GetAxis(_fireGunAxis); }

    public static bool JumpButtonDown { get => Input.GetButtonDown(_jumpButton); }
    public static bool InteractButtonDown { get => Input.GetButtonDown(_interactButton); }
    public static bool InteractButton { get => Input.GetButton(_interactButton); }

    public static bool HolsterButtonDown { get => Input.GetButtonDown(_holsterButton); }
    public static bool PunchButtonDown { get => Input.GetButtonDown(_punchButton); }

}
