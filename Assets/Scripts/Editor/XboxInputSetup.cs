using UnityEngine;
using UnityEditor;

/// <summary>
/// Automatically configures Xbox controller axes in the Input Manager on every
/// Unity editor load/recompile. Also available via Tools > Setup Xbox Controller Axes.
///
/// Xbox One on Windows (XInput) axis mapping:
///   1 = Left stick X      2 = Left stick Y
///   3 = Triggers combined  4 = Right stick X   5 = Right stick Y
///   6 = D-pad X            7 = D-pad Y
/// </summary>
[InitializeOnLoad]
public static class XboxInputSetup
{
    private const string HorizontalAxis  = "Horizontal";
    private const string VerticalAxis    = "Vertical";
    private const string RightStickXName = "RightStickX";
    private const string RightStickYName = "RightStickY";
    private const string DPadXName       = "DPadX";
    private const string DPadYName       = "DPadY";

    static XboxInputSetup() => SetupAxes();

    [MenuItem("Tools/Setup Xbox Controller Axes")]
    public static void SetupAxes()
    {
        SerializedObject inputManager = GetInputManager();
        SerializedProperty axes       = inputManager.FindProperty("m_Axes");

        // Left stick — axis 1 (X, no invert) and axis 2 (Y, invert so up = +1 = forward)
        FixJoystickAxis(axes, HorizontalAxis, axisNumber: 1, invert: false);
        FixJoystickAxis(axes, VerticalAxis,   axisNumber: 2, invert: true);

        // Right stick — axis 4 (X) and axis 5 (Y). Axis 3 is the trigger on XInput.
        // X: left=-1, right=+1, no invert needed for correct yaw
        // Y: up=-1 raw, invert=true so up stick = +1 = look up
        EnsureJoystickAxis(axes, RightStickXName, axisNumber: 4, invert: false);
        EnsureJoystickAxis(axes, RightStickYName, axisNumber: 5, invert: true);

        // D-pad — axis 6 (X) and axis 7 (Y)
        EnsureJoystickAxis(axes, DPadXName, axisNumber: 6, invert: false);
        EnsureJoystickAxis(axes, DPadYName, axisNumber: 7, invert: false);

        // Strip joystick buttons from the built-in Submit/Cancel axes so
        // StandaloneInputModule never double-fires alongside MenuNavigator.
        // The axes still work for keyboard (return / escape).
        StripJoystickFromButtonAxis(axes, "Submit");
        StripJoystickFromButtonAxis(axes, "Cancel");

        inputManager.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    private static void FixJoystickAxis(SerializedProperty axes, string name, int axisNumber, bool invert)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty axis = axes.GetArrayElementAtIndex(i);
            if (axis.FindPropertyRelative("m_Name").stringValue != name) continue;
            if ((AxisType)axis.FindPropertyRelative("type").intValue != AxisType.JoystickAxis) continue;

            axis.FindPropertyRelative("axis").intValue          = axisNumber - 1;
            axis.FindPropertyRelative("invert").boolValue       = invert;
            axis.FindPropertyRelative("dead").floatValue        = 0.19f;
            axis.FindPropertyRelative("sensitivity").floatValue = 1f;
            return;
        }
        EnsureJoystickAxis(axes, name, axisNumber, invert);
    }

    private static void EnsureJoystickAxis(SerializedProperty axes, string name, int axisNumber, bool invert)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty a = axes.GetArrayElementAtIndex(i);
            if (a.FindPropertyRelative("m_Name").stringValue == name &&
                (AxisType)a.FindPropertyRelative("type").intValue == AxisType.JoystickAxis)
            {
                a.FindPropertyRelative("axis").intValue    = axisNumber - 1;
                a.FindPropertyRelative("invert").boolValue = invert;
                return;
            }
        }

        axes.InsertArrayElementAtIndex(axes.arraySize);
        SerializedProperty n = axes.GetArrayElementAtIndex(axes.arraySize - 1);

        n.FindPropertyRelative("m_Name").stringValue                  = name;
        n.FindPropertyRelative("descriptiveName").stringValue          = "";
        n.FindPropertyRelative("descriptiveNegativeName").stringValue  = "";
        n.FindPropertyRelative("negativeButton").stringValue           = "";
        n.FindPropertyRelative("positiveButton").stringValue           = "";
        n.FindPropertyRelative("altNegativeButton").stringValue        = "";
        n.FindPropertyRelative("altPositiveButton").stringValue        = "";
        n.FindPropertyRelative("gravity").floatValue                   = 0f;
        n.FindPropertyRelative("dead").floatValue                      = 0.19f;
        n.FindPropertyRelative("sensitivity").floatValue               = 1f;
        n.FindPropertyRelative("snap").boolValue                       = false;
        n.FindPropertyRelative("invert").boolValue                     = invert;
        n.FindPropertyRelative("type").intValue                        = (int)AxisType.JoystickAxis;
        n.FindPropertyRelative("axis").intValue                        = axisNumber - 1;
        n.FindPropertyRelative("joyNum").intValue                      = 0;
    }

    private static SerializedObject GetInputManager() =>
        new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

    /// <summary>
    /// Removes joystick button bindings from a KeyOrMouseButton axis so
    /// StandaloneInputModule only fires for keyboard, not controller.
    /// </summary>
    private static void StripJoystickFromButtonAxis(SerializedProperty axes, string name)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty a = axes.GetArrayElementAtIndex(i);
            if (a.FindPropertyRelative("m_Name").stringValue != name) continue;
            if ((AxisType)a.FindPropertyRelative("type").intValue != AxisType.KeyOrMouseButton) continue;

            // Clear only the alt binding — that's where Unity puts joystick button 0/1 by default
            SerializedProperty alt = a.FindPropertyRelative("altPositiveButton");
            string current = alt.stringValue;
            if (current.StartsWith("joystick"))
                alt.stringValue = "";
        }
    }

    private enum AxisType { KeyOrMouseButton = 0, MouseMovement = 1, JoystickAxis = 2 }
}


