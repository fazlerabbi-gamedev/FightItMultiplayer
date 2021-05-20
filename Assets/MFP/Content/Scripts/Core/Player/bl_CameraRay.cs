using UnityEngine;
using System.Collections;

public class bl_CameraRay : MonoBehaviour
{

    public float DistanceCheck = 2;

    private DetectType Detected = DetectType.None;
    private bl_SimpleDoor CacheSimpleDoor = null;

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        RayDetection();
        InputControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void RayDetection()
    {
        RaycastHit hit;

        Vector3 fwr = this.transform.forward;
        Debug.DrawRay(this.transform.position, fwr, Color.green);

        if (Physics.Raycast(this.transform.position, fwr, out hit, DistanceCheck))
        {
            if (hit.transform.GetComponent<bl_SimpleDoor>() != null)
            {
                CacheSimpleDoor = hit.transform.GetComponent<bl_SimpleDoor>();
                Detected = DetectType.Door;
                bl_UIManager.Instance.ShowInputText(true, "PRESS [F]");
            }
        }
        else
        {
            if (Detected != DetectType.None)
            {
                bl_UIManager.Instance.ShowInputText(false);
                Detected = DetectType.None;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void InputControl()
    {
        if(Detected == DetectType.Door && CacheSimpleDoor != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CacheSimpleDoor.Intercalate();
            }
        }
    }

    public enum DetectType
    {
        None = 0,
        Door = 1,
    }
}