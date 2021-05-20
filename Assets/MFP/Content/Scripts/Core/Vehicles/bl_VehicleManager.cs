using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;

public class bl_VehicleManager : bl_PhotonHelper, IPunObservable
{
    [Separator("Settings")]
    public VehicleType m_VehicleType = VehicleType.Car;
    public KeyCode EnterKey = KeyCode.E;
    public List<MonoBehaviour> VehicleScripts = new List<MonoBehaviour>();

    [Separator("Driver")]
    public bool PlayerVisibleInside = true;
    public Vector3 DriverPosition;
    public Vector3 DriverRotation;
    [Range(0.01f, 1)] public float SteeringHandSpace = 0.1f;
    [Range(10, 180)] public float SteeringWheelMaxAngle = 90;
    [Separator("References")]
    [SerializeField] GameObject VehicleCamera;
    [SerializeField] Transform ExitPoint;
    public Transform SteeringWheelPoint;
    public Transform PlayerHolder;

    private bool LocalInVehicle = false;
    private bool RemoteInVehicle = false;

    private CarController CarScript;
    private AeroplaneController JetScript;
    private PhotonView view;
    public GameObject Player { get; set; }
    private Animator m_Animator;
    private bl_VehicleUI VehicleUI;

    private float Vertical;
    private float Horizontal;
    public float Input1 { get; set; }
    private float Input2;
    private bool LocalOnTrigger = false;
    private bool AirBreak;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        view = PhotonView.Get(this);
        VehicleUI = FindObjectOfType<bl_VehicleUI>();
        if (m_VehicleType == VehicleType.Car)
        {
            CarScript = GetComponent<CarController>();
        }
        else if (m_VehicleType == VehicleType.Jet)
        {
            JetScript = GetComponent<AeroplaneController>();
            m_Animator = GetComponent<Animator>();
        }
        if (!view.ObservedComponents.Contains(this))
        {
            view.ObservedComponents.Add(this);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (PhotonNetwork.IsMasterClient && view.Owner == null)
        {
            view.RequestOwnership();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Player != null)
        {
            Inputs();
            Speedometer();
        }
         PositionControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void PositionControl()
    {
        if (m_VehicleType == VehicleType.Car)
        {
            CarPosition();
        }
        else if (m_VehicleType == VehicleType.Jet)
        {
            JetPosition();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CarPosition()
    {
        if (isPlayerIn)
        {
            Horizontal = CarScript.m_steerin;
            Vertical = CarScript.m_accel;
            Input1 = CarScript.m_brakeInput;
            Input2 = CarScript.m_handbrake;
        }
        else if (inMyControl && !LocalInVehicle)
        {
            CarScript.Move(0, 0, 0, 0, false);
        }
        else
        {
            CarScript.Move(Horizontal, Vertical, Input1, Input2, true);
        }
        SteeringWheelControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void SteeringWheelControl()
    {
        if (SteeringWheelPoint == null)
            return;

        Vector3 v = SteeringWheelPoint.localEulerAngles;
        v.z = -(Horizontal * SteeringWheelMaxAngle);
        SteeringWheelPoint.localEulerAngles = v;

    }

    /// <summary>
    /// 
    /// </summary>
    void JetPosition()
    {
        if (isPlayerIn)
        {
            Input1 = JetScript.RollInput;
            Input2 = JetScript.PitchInput;
            AirBreak = JetScript.AirBrakes;
            Horizontal = JetScript.YawInput;
            Vertical = JetScript.ThrottleInput;
        }
        else if (inMyControl && !LocalInVehicle)
        {
            JetScript.Move(0, 0, 0, 0, false);
            m_Animator.SetInteger("GearState", 1);
        }
        else
        {
            JetScript.Move(Input1, Input2, Horizontal, Vertical, AirBreak);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Speedometer()
    {
        if (!isPlayerIn)
            return;
        float speed = 0;
        if (m_VehicleType == VehicleType.Car)
        {
            speed = CarScript.CarVelocity;
            VehicleUI.UpdateSpeedometer(speed, m_VehicleType, CarScript.m_SpeedType);
        }
        else if (m_VehicleType == VehicleType.Jet)
        {
            speed = JetScript.ForwardSpeed;
            VehicleUI.UpdateSpeedometer(speed, m_VehicleType);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Inputs()
    {
        if (Input.GetKeyDown(EnterKey))
        {
            if (isPlayerIn)
            {
                OnExit();
            }
            else if(LocalOnTrigger && !RemoteInVehicle)
            {
                OnEnter();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void OnEnter()
    {
        if (view.Owner.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            view.RequestOwnership();
        }
        bl_EventHandler.OnLocalPlayerVehicle(true, m_VehicleType);
        foreach (MonoBehaviour m in VehicleScripts)
        {
            m.enabled = true;
        }
        if(m_VehicleType == VehicleType.Jet) { JetScript.Reset(); }
        Player.transform.parent = PlayerHolder;
        Player.transform.localPosition = DriverPosition;
        Player.transform.localEulerAngles = DriverRotation;

        VehicleCamera.SetActive(true);
        Player.GetComponent<bl_PlayerCar>().OnEnterLocal(this);
        LocalInVehicle = true;
        LocalOnTrigger = false;
        VehicleUI.SetEnterUI(false);
        VehicleUI.OnEnter(m_VehicleType);
        LocalPlayerView.RPC("NetworkCarEvent", RpcTarget.OthersBuffered, true, m_VehicleType, photonView.ViewID, PlayerVisibleInside);
        view.RPC("InAndOutEvent", RpcTarget.OthersBuffered, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnExit(bool byDeath = false)
    {
        foreach (MonoBehaviour m in VehicleScripts)
        {
            m.enabled = false;
        }
        if (m_VehicleType == VehicleType.Jet) { JetScript.Immobilize(); }
        VehicleCamera.SetActive(false);

        Player.transform.parent = null;
        Player.transform.position = ExitPoint.position;
        Vector3 r = ExitPoint.eulerAngles;
        r.y = transform.eulerAngles.y;
        Player.transform.rotation = Quaternion.Euler(r);

        Player.GetComponent<bl_PlayerCar>().OnExitLocal(this, byDeath);
        LocalInVehicle = false;
        RemoteInVehicle = false;
        VehicleUI.OnExit(m_VehicleType);
        bl_EventHandler.OnLocalPlayerVehicle(false, m_VehicleType);
        LocalPlayerView.RPC("NetworkCarEvent", RpcTarget.OthersBuffered, false, m_VehicleType, photonView.ViewID, PlayerVisibleInside);
        view.RPC("InAndOutEvent", RpcTarget.OthersBuffered, false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnCollision(float impact)
    {
        if (isPlayerIn)
        {
            int damage = (int)(impact / 5);
            Player.GetComponent<bl_PlayerDamage>().DoDamage(damage);
        }
    }

    [PunRPC]
    void InAndOutEvent(bool GetIn)
    {
        RemoteInVehicle = GetIn;
    }

    public void OnEnterDetector()
    {
        if (isPlayerIn)
            return;

        LocalOnTrigger = true;
        VehicleUI.SetEnterUI(true, EnterKey);
    }

    public void OnExitDetector()
    {
        LocalOnTrigger = false;
        VehicleUI.SetEnterUI(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (m_VehicleType == VehicleType.Car)
            {
                stream.SendNext(Horizontal);
                stream.SendNext(Vertical);
                stream.SendNext(Input1);
                stream.SendNext(Input2);
            }
            else if (m_VehicleType == VehicleType.Jet)
            {
                stream.SendNext(Input1);
                stream.SendNext(Input2);
                stream.SendNext(AirBreak);
                stream.SendNext(Horizontal);
                stream.SendNext(Vertical);
            }
        }
        else
        {
            if (m_VehicleType == VehicleType.Car)
            {
                Horizontal = (float)stream.ReceiveNext();
                Vertical = (float)stream.ReceiveNext();
                Input1 = (float)stream.ReceiveNext();
                Input2 = (float)stream.ReceiveNext();
            }
            else if (m_VehicleType == VehicleType.Jet)
            {
                Input1 = (float)stream.ReceiveNext();
                Input2 = (float)stream.ReceiveNext();
                AirBreak = (bool)stream.ReceiveNext();
                Horizontal = (float)stream.ReceiveNext();
                Vertical = (float)stream.ReceiveNext();
            }
        }
    }

    void OnEnable()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        bl_EventHandler.LocalPlayerSpawnEvent += OnLocalPlayerSpawn;
        bl_PhotonCallbacks.Instance.OnPlayerEnter+=(OnPhotonPlayerConnected);
        bl_PhotonCallbacks.Instance.OnPlayerLeft+=(OnPhotonPlayerDisconnected);
    }
    void OnDisable()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        bl_EventHandler.LocalPlayerSpawnEvent -= OnLocalPlayerSpawn;
        bl_PhotonCallbacks.Instance.OnPlayerEnter-=(OnPhotonPlayerConnected);
        bl_PhotonCallbacks.Instance.OnPlayerLeft-=(OnPhotonPlayerDisconnected);
    }

    public PhotonView LocalPlayerView { get; set; }
    void OnLocalPlayerSpawn(GameObject player)
    {
        Player = player;
        LocalPlayerView = Player.GetComponent<PhotonView>();
    }

    public  void OnPhotonPlayerDisconnected(Player otherPlayer)
    {
        if (view == null)
            return;
        if (view.Owner == null || view.Owner.NickName == "Scene")
        {
            LocalInVehicle = false;
            view.RPC("InAndOutEvent", RpcTarget.OthersBuffered, false);
            return;
        }
        //is the player in car.
        if (view.Owner.NickName == otherPlayer.NickName)
        {
            LocalInVehicle = false;
            view.RPC("InAndOutEvent", RpcTarget.OthersBuffered, false);
        }
    }
    public void OnPhotonPlayerConnected(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && view.Owner == null)
        {
            view.RequestOwnership();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (PlayerHolder == null || !PlayerVisibleInside)
            return;

        Gizmos.matrix = PlayerHolder.localToWorldMatrix;
        Color c = Color.green;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(DriverPosition, 0.5f);
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawSphere(DriverPosition, 0.5f);
        Gizmos.matrix = Matrix4x4.identity;
        if (SteeringWheelPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(SteeringWheelPoint.position, 0.1f);
            Gizmos.color = Color.yellow;
            Vector3 center = SteeringWheelPoint.position;
            Gizmos.DrawSphere(center + (SteeringWheelPoint.right * SteeringHandSpace), 0.1f);
            Gizmos.DrawSphere(center - (SteeringWheelPoint.right * SteeringHandSpace), 0.1f);
        }
    }

    public bool inMyControl
    {
        get
        {
            bool b = false;
            if (view.IsMine)
            {
                b = true;
            }
            return b;
        }
    }
    public bool isPlayerIn { get { return inMyControl && LocalInVehicle; } }

    private Transform _heatLook;
    public Transform HeatLook
    {
        get
        {
            if (_heatLook == null)
            {
                _heatLook = VehicleCamera.transform.GetChild(0);
            }
            return _heatLook;
        }
    }
    public Vector3 SteeringHandRight { get { return SteeringWheelPoint.position + (SteeringWheelPoint.right * SteeringHandSpace); } }
    public Vector3 SteeringHandLeft { get { return SteeringWheelPoint.position - (SteeringWheelPoint.right * SteeringHandSpace); ; } }
}