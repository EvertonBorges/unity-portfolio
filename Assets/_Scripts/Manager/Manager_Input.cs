using System;
using System.Reflection;
using UnityEngine;

public static class Manager_Input
{

    private static InputActions inputActions;
    private static InputActions.GameplayActions gameplayActions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Setup()
    {
        HackFixForEditorPlayModeDelay();

        inputActions = new InputActions();
        gameplayActions = inputActions.Gameplay;

        gameplayActions.Interact.performed += ctx => Manager_Events.Player.OnInteract.Notify();
        gameplayActions.Move.performed += ctx => Manager_Events.Player.OnMove.Notify(ctx.ReadValue<Vector2>());
        gameplayActions.Run.performed += ctx => Manager_Events.Player.OnRun.Notify(ctx.ReadValue<float>() == 1f);
        gameplayActions.Look.performed += ctx => Manager_Events.Player.OnLook.Notify(ctx.ReadValue<Vector2>());

        inputActions.Enable();
    }

    private static void HackFixForEditorPlayModeDelay()
    {
#if UNITY_EDITOR
        // Using reflection, does this: InputSystem.s_SystemObject.exitEditModeTime = 0

        // Get InputSystem.s_SystemObject object
        FieldInfo systemObjectField = typeof(UnityEngine.InputSystem.InputSystem).GetField("s_SystemObject", BindingFlags.NonPublic | BindingFlags.Static);
        object systemObject = systemObjectField.GetValue(null);

        // Get InputSystemObject.exitEditModeTime field
        Assembly inputSystemAssembly = typeof(UnityEngine.InputSystem.InputSystem).Assembly;
        Type inputSystemObjectType = inputSystemAssembly.GetType("UnityEngine.InputSystem.InputSystemObject");
        FieldInfo exitEditModeTimeField = inputSystemObjectType.GetField("exitEditModeTime", BindingFlags.Public | BindingFlags.Instance);

        // Set exitEditModeTime to zero
        exitEditModeTimeField.SetValue(systemObject, 0d);
#endif
    }

}