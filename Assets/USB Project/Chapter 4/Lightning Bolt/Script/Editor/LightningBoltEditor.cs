using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightningBoltController))]
public class LightningBoltEditor : Editor
{
    LightningBoltController _target;
    private void OnEnable()
    {
        _target = (LightningBoltController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.Space();

        if (serializedObject.ApplyModifiedProperties())
        {
            _target.GenerateMesh();
        }
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.green;

        for (int i = 0; i < _target.Points.Length; i++)
        {
            Vector2 p = _target.transform.TransformPoint( _target.Points[i]);
#if UNITY_2022
            Vector2 newPosition = Handles.FreeMoveHandle(p, 0.08f, Vector3.zero, Handles.CircleHandleCap);
#else
    var fmh_50_53_638685222822851218 = Quaternion.identity; Vector2 newPosition = Handles.FreeMoveHandle(p, 0.08f, Vector3.zero, Handles.CircleHandleCap);
#endif

            if (p != newPosition)
            {
                _target.SetPointPosition(i, _target.transform.InverseTransformPoint(newPosition));
            }
        }

        Handles.color = Color.white;
        Vector2 p0 = _target.transform.TransformPoint(_target.Points[0]);
        Vector2 p1 = _target.transform.TransformPoint(_target.Points[1]);
        Vector2 p2 = _target.transform.TransformPoint(_target.Points[2]);
        Vector2 p3 = _target.transform.TransformPoint(_target.Points[3]);

        Handles.DrawDottedLine(p0, p1, 5f);
        Handles.DrawDottedLine(p2, p3, 5f);
        Handles.DrawBezier(p0, p3, p1, p2, Color.gray, null, 3f);        
    }


    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    public static void OnDrawGUI(LightningBoltController target, GizmoType gizmoType)
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < target.Points.Length; i++)
        {
            Vector2 p = target.transform.TransformPoint(target.Points[i]);
            Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
