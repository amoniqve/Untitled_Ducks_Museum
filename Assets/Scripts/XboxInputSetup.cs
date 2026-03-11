#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Automatically configures Unity's legacy Input Manager with the correct
/// Xbox controller axis mappings whenever scripts are recompiled.
/// This only runs in the Editor and has zero runtime footprint.
/// </summary>
[InitializeOnLoad]
public static class XboxInputSetup
{
    // Axis names used across the project
    private const string HorizontalAxis  = "Horizontal";
    private const string VerticalAxis    = "Vertical";
    private const string RightStickXName = "RightStickX";
    private const string RightStickYName = "RightStickY";
    private const string DPadXName       = "DPadX";
    private const string DPadYName       = "DPadY";

    private enum AxisType { KeyOrMouseButton = 0, MouseMovement = 1, JoystickAxis = 2 }

    static XboxInputSetup()
    {
        SetupAxes();
    }

    private static void SetupAxes()
    {
        SerializedObject inputManager =
            new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

        SerializedProperty axes = inputManager.FindProperty("m_Axes");

        // Patch the joystick sub-entry of the built-in movement axes
        FixJoystickAxis(axes, HorizontalAxis, axisNumber: 1, invert: false);
        FixJoystickAxis(axes, VerticalAxis,   axisNumber: 2, invert: true);

        // Ensure right stick and D-pad axes exist
        EnsureJoystickAxis(axes, RightStickXName, axisNumber: 4, invert: false);
        EnsureJoystickAxis(axes, RightStickYName, axisNumber: 5, invert: true);
        EnsureJoystickAxis(axes, DPadXName,       axisNumber: 6, invert: false);
        EnsureJoystickAxis(axes, DPadYName,       axisNumber: 7, invert: false);

        // Remove joystick from Submit/Cancel so controller buttons don't double-fire
        StripJoystickFromButtonAxis(axes, "Submit");
        StripJoystickFromButtonAxis(axes, "Cancel");

        inputManager.ApplyModifiedProperties();
        Debug.Log("[XboxInputSetup] Input Manager axes configured.");
    }

    /// <summary>Finds the JoystickAxis entry for <paramref name="name"/> and patches axis number/inversion.</summary>
    private static void FixJoystickAxis(SerializedProperty axes, string name, int axisNumber, bool invert)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty a = axes.GetArrayElementAtIndex(i);
            if (a.FindPropertyRelative("m_Name").stringValue != name) continue;
            if ((AxisType)a.FindPropertyRelative("type").intValue != AxisType.JoystickAxis) continue;

            a.FindPropertyRelative("axis").intValue   = axisNumber - 1; // Unity is 0-indexed internally
            a.FindPropertyRelative("invert").boolValue = invert;
            a.FindPropertyRelative("dead").floatValue  = 0.19f;
            return;
        }
    }

    /// <summary>Creates a JoystickAxis entry if one with <paramref name="name"/> does not already exist.</summary>
    private static void EnsureJoystickAxis(SerializedProperty axes, string name, int axisNumber, bool invert)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty a = axes.GetArrayElementAtIndex(i);
            if (a.FindPropertyRelative("m_Name").stringValue == name &&
                (AxisType)a.FindPropertyRelative("type").intValue == AxisType.JoystickAxis)
            {
                // Already exists — just make sure values are correct
                a.FindPropertyRelative("axis").intValue    = axisNumber - 1;
                a.FindPropertyRelative("invert").boolValue = invert;
                a.FindPropertyRelative("dead").floatValue  = 0.19f;
                return;
            }
        }

        // Not found — append a new entry
        axes.InsertArrayElementAtIndex(axes.arraySize);
        SerializedProperty n = axes.GetArrayElementAtIndex(axes.arraySize - 1);

        n.FindPropertyRelative("m_Name").stringValue           = name;
        n.FindPropertyRelative("descriptiveName").stringValue  = "";
        n.FindPropertyRelative("descriptiveNegativeName").stringValue = "";
        n.FindPropertyRelative("negativeButton").stringValue   = "";
        n.FindPropertyRelative("positiveButton").stringValue   = "";
        n.FindPropertyRelative("altNegativeButton").stringValue = "";
        n.FindPropertyRelative("altPositiveButton").stringValue = "";
        n.FindPropertyRelative("gravity").floatValue           = 0f;
        n.FindPropertyRelative("dead").floatValue              = 0.19f;
        n.FindPropertyRelative("sensitivity").floatValue       = 1f;
        n.FindPropertyRelative("snap").boolValue               = false;
        n.FindPropertyRelative("invert").boolValue             = invert;
        n.FindPropertyRelative("type").intValue                = (int)AxisType.JoystickAxis;
        n.FindPropertyRelative("axis").intValue                = axisNumber - 1;
        n.FindPropertyRelative("joyNum").intValue              = 0; // any joystick
    }

    /// <summary>
    /// Removes the joystick button binding from an axis (e.g. Submit/Cancel)
    /// so the Xbox A/B buttons don't double-fire those events.
    /// </summary>
    private static void StripJoystickFromButtonAxis(SerializedProperty axes, string name)
    {
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty a = axes.GetArrayElementAtIndex(i);
            if (a.FindPropertyRelative("m_Name").stringValue != name) continue;
            if ((AxisType)a.FindPropertyRelative("type").intValue != AxisType.KeyOrMouseButton) continue;

            // Clear any joystick button bindings on the alt slots
            string pos = a.FindPropertyRelative("positiveButton").stringValue;
            string alt = a.FindPropertyRelative("altPositiveButton").stringValue;

            if (pos.StartsWith("joystick")) a.FindPropertyRelative("positiveButton").stringValue    = "";
            if (alt.StartsWith("joystick")) a.FindPropertyRelative("altPositiveButton").stringValue = "";
        }
    }
}
#endif
