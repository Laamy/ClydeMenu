namespace ClydeMenu.Engine.Components.Visuals;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BoundingBoxOutline : MonoBehaviour
{
    public bool enabledOutline = true;
    private LineRenderer line;

    void Start()
    {
        if (!enabledOutline) return;

        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.widthMultiplier = 0.02f;
        line.loop = false;

        if (line.material == null)
            line.material = new Material(Shader.Find("Unlit/Color"));

        line.startColor = Color.red;
        line.endColor = Color.red;
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;

        // depthjh test opff
        line.material.SetInt("_ZWrite", 0);
        line.material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        Vector3 s = transform.lossyScale * 0.5f;
        Vector3 p = transform.position;

        Vector3[] c = new Vector3[8];
        c[0] = p + transform.rotation * new Vector3(-s.x, -s.y, -s.z);
        c[1] = p + transform.rotation * new Vector3(s.x, -s.y, -s.z);
        c[2] = p + transform.rotation * new Vector3(s.x, -s.y, s.z);
        c[3] = p + transform.rotation * new Vector3(-s.x, -s.y, s.z);
        c[4] = p + transform.rotation * new Vector3(-s.x, s.y, -s.z);
        c[5] = p + transform.rotation * new Vector3(s.x, s.y, -s.z);
        c[6] = p + transform.rotation * new Vector3(s.x, s.y, s.z);
        c[7] = p + transform.rotation * new Vector3(-s.x, s.y, s.z);

        line.positionCount = 16;
        line.SetPosition(0, c[0]);
        line.SetPosition(1, c[1]);
        line.SetPosition(2, c[2]);
        line.SetPosition(3, c[3]);
        line.SetPosition(4, c[0]);
        line.SetPosition(5, c[4]);
        line.SetPosition(6, c[5]);
        line.SetPosition(7, c[1]);
        line.SetPosition(8, c[5]);
        line.SetPosition(9, c[6]);
        line.SetPosition(10, c[2]);
        line.SetPosition(11, c[6]);
        line.SetPosition(12, c[7]);
        line.SetPosition(13, c[3]);
        line.SetPosition(14, c[7]);
        line.SetPosition(15, c[4]);
    }

    void OnValidate()
    {
        if (line != null)
            line.enabled = enabledOutline;
    }
}