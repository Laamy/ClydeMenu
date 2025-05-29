namespace Hot_reload.Components;

using System;

using UnityEngine;

public class HotReloadBehaviour : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Entry.Log("Reloading the clydemenu assemblies");
            Entry.Reload();
            Entry.Log("Successfully reloaded the clydemenu assemblies");
        }

        if (Entry.isLoaded)
            Entry.modUpdate?.Invoke(null, null);
    }
    public void LateUpdate()
    {
        if (Entry.isLoaded)
            Entry.modLateUpdate?.Invoke(null, null);
    }

    public void FixedUpdate()
    {
        if (Entry.isLoaded)
            Entry.modFixedUpdate?.Invoke(null, null);
    }

    public void OnGUI()
    {
        if (Entry.isLoaded)
            Entry.modOnGUI?.Invoke(null, null);
    }
}
