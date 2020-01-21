using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.UI;
using System.Timers;

public class PlayerMove : MonoBehaviour
{

    public int playerId;
    private int game_id;
    private bool playersReady = false;
    private bool isWalking = false;
    private char[] trim = { '"' };
    private int cnt = 0;

    public int playerRecord;

    // Cam look variables.
    [HideInInspector]
    private float rotSpeedX; // Mouse X sensitivity control, set in editor.
    [HideInInspector]
    private float rotSpeedY; // Mouse Y sensitivity control, set in editor.

    [HideInInspector]
    private float rotDamp; // Damping value for camera rotation.

    private float mY = 0f; // Mouse X.
    private float mX = 0f; // Mouse Y.

    // Player move variables.
    [HideInInspector]
    private float walkSpeed; // Walk (normal movement) speed, set in editor.
    [HideInInspector]
    private float runSpeed; // Run speed, set in editor.

    private float currentSpeed; // Stores current movement speed.

    [HideInInspector]
    private KeyCode runKey; // Run key, set in editor.

    private CharacterController cc; // Reference to attached CharacterController.

    [SerializeField]
    private GameObject playerCamera; // Player cam, set in editor.

    protected Joystick joystick;
    protected joybutton attackJoyButton;
    protected joybutton catchJoyButton;

    [HideInInspector]
    public bool attack;
    public bool catchChicken;

    Animator anim;

    int screenTouch;

    // Debugging purpose
    bool wasOverUI;

    //for Attack class
    AttackOrCatch attackOrCatch;
    [HideInInspector]
    public float shootVelocity;
    [HideInInspector]
    public Vector3 shootStartPoint;

    //attack trajectory
    LineRenderer lineVisual;
    [HideInInspector]
    public int lineSegment;

    //socket
    private SocketIOComponent socket;

    //for Energy
    public float health;

    public HealthBar healthBar;
    public Image DarkImage;

    //timer
    public Text TextTimer;
    public int TimerSeconds;
    private float timer;
    int resultSeconds;

    //audio
    [SerializeField]
    Audio audio;

    //color
    public Material material;
    Renderer rend;

    private void Start()
    {
        DarkImage.gameObject.SetActive(false);

        // Initialize player position   
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");
        if (playerId == 1)
        {
            transform.position = GameSettings.p1StartPos;
            transform.rotation = GameSettings.p1StartRot;
        }
        else
        {
            transform.position = GameSettings.p2StartPos;
            transform.rotation = GameSettings.p2StartRot;
        }

        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        rend.enabled = true;

        if (playerId == 2)
        {
            rend.material = material;
        }
        //color


        health = 100;

        // initialize values
        rotSpeedX = GameSettings.rotSpeedX;
        rotSpeedY = GameSettings.rotSpeedY;
        rotDamp = GameSettings.rotDamp;
        walkSpeed = GameSettings.walkSpeed;
        runSpeed = GameSettings.runSpeed;
        attack = GameSettings.attack;
        catchChicken = GameSettings.catchChicken;
        runKey = GameSettings.runKey;
        shootVelocity = GameSettings.shootVelocity;
        lineSegment = GameSettings.lineSegment;

        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        joystick = FindObjectOfType<Joystick>();
        joybutton[] joybuttons = FindObjectsOfType<joybutton>() as joybutton[];
        foreach (joybutton j in joybuttons)
        {
            if (j.name == "attackJoyButton")
            {
                attackJoyButton = j;
            }
            else if (j.name == "catchJoyButton")
            {
                catchJoyButton = j;
            }
        }

        currentSpeed = walkSpeed;
        screenTouch = -1;
        wasOverUI = true;

        attackOrCatch = GetComponent<AttackOrCatch>();

        lineVisual = GetComponent<LineRenderer>();

        lineVisual.SetVertexCount(lineSegment);

        // Socket settings
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("game_ready", (SocketIOEvent e) => {
            Debug.Log("game_ready recieved");
            var data = JSON.ParseString(e.data.ToString());
            if (int.Parse(data["ready"].CreateString().Trim(trim)) == 2)
            {
                playersReady = true;
                if (playerId == 1)
                {
                    Debug.Log("game_ready: 'if' entered");
                    var cs = FindObjectOfType<ChickenSpawner>();
                    cs.SpawnChickens();
                }
            }
        });
    }

    private void Update()
    {
        //Timer

        timer += Time.deltaTime;
        resultSeconds = TimerSeconds - (int)timer;
        int min = resultSeconds / 60;
        int seconds = resultSeconds - min*60;

        if(playerId == 1 && resultSeconds == 0)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;

            if(playerRecord > FindObjectOfType<OpponentMove>().playerRecord)
            {
                data["winnerId"] = "1";
            }
            else if (playerRecord < FindObjectOfType<OpponentMove>().playerRecord)
            {
                data["winnerId"] = "2";
            }
            else
            {
                data["winnerId"] = "0";
            }
            socket.Emit("game_over", new JSONObject(data));
        }

        TextTimer.text = string.Format("{0:D2} : {1:D2}", min, seconds);


        // Do not start until all players are successfully connected to the server   
        if (!playersReady) return;

        // Set animation
        if (joystick.pressed && !isWalking)
        {
            anim.SetBool("isWalking", true);
            isWalking = true;

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            socket.Emit("player_walk", new JSONObject(data));
        }
        else if (!joystick.pressed && isWalking)
        {
            anim.SetBool("isWalking", false);
            isWalking = false;

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            socket.Emit("player_idle", new JSONObject(data));
        }

        if (!attack && attackJoyButton.pressed)
        {
            attack = true;
        }

        if (attack && attackJoyButton.pressed)
        {
            lineVisual.SetColors(Color.red, Color.red);

            shootVelocity += 0.4f;
            VisualizeLine(Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity);
        }

        if (attack && !attackJoyButton.pressed)
        {
            attack = false;
            anim.ResetTrigger("attack");
            anim.SetTrigger("attack");
            //attackOrCatch.ShootAttack(shootStartPoint, shootVelocity);

            Vector3 v = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity;

            Debug.Log("v: " + v);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            data["x"] = "" + shootStartPoint.x;
            data["y"] = "" + shootStartPoint.y;
            data["z"] = "" + shootStartPoint.z;
            data["vx"] = "" + v.x;
            data["vy"] = "" + v.y;
            data["vz"] = "" + v.z;
            socket.Emit("attack", new JSONObject(data));

            audio.shoot.Play();
            audio.shoot_1.Play();

            shootVelocity = 0;
            for (int i = 0; i < lineSegment; i++)
            {
                lineVisual.SetPosition(i, Vector3.zero);
            }
        }

        if (!catchChicken && catchJoyButton.pressed)
        {
            catchChicken = true;
        }
        if (catchChicken && catchJoyButton.pressed)
        {
            lineVisual.SetColors(Color.blue, Color.blue);

            shootVelocity += 0.2f;
            VisualizeLine(Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity);
        }
        if (catchChicken && !catchJoyButton.pressed)
        {
            catchChicken = false;
            anim.ResetTrigger("attack");
            anim.SetTrigger("attack");
            //attackOrCatch.ShootCatch(shootStartPoint, shootVelocity, playerId);

            Vector3 v = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity;
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            data["x"] = "" + shootStartPoint.x;
            data["y"] = "" + shootStartPoint.y;
            data["z"] = "" + shootStartPoint.z;
            data["vx"] = "" + v.x;
            data["vy"] = "" + v.y;
            data["vz"] = "" + v.z;
            socket.Emit("catch", new JSONObject(data));

            audio.shoot.Play();
            audio.shoot_2.Play();

            shootVelocity = 0;
            for (int i = 0; i < lineSegment; i++)
            {
                lineVisual.SetPosition(i, Vector3.zero);
            }
        }

        //사망정보

        if (health <= 0)
        {
            audio.player_die.Play();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            socket.Emit("dead", new JSONObject(data));
            
            anim.ResetTrigger("Die");
            anim.SetTrigger("Die");
            DarkImage.gameObject.SetActive(true);
            joystick.gameObject.SetActive(false);
            attackJoyButton.gameObject.SetActive(false);
            catchJoyButton.gameObject.SetActive(false);
            StartCoroutine(RefreshAfterSeconds(10f));
        }

    }

    IEnumerator RefreshAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        health = 100;
        healthBar.UpdateEnergybar(100f);
        DarkImage.gameObject.SetActive(false);
        joystick.gameObject.SetActive(true);
        attackJoyButton.gameObject.SetActive(true);
        catchJoyButton.gameObject.SetActive(true);
        anim.ResetTrigger("Alive");
        anim.SetTrigger("Alive");
    }

    private void fixPosition()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["playerId"] = "" + playerId;
        data["game_id"] = "" + game_id;
        data["x"] = "" + transform.position.x;
        data["y"] = "" + transform.position.y;
        data["z"] = "" + transform.position.z;
        data["ry"] = "" + transform.eulerAngles.y;
        socket.Emit("fix_position", new JSONObject(data));
    }

    public Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;

        Vector3 result = shootStartPoint + Vxz * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + shootStartPoint.y;

        result.y = sY;
        return result;
    }

    public void VisualizeLine(Vector3 vo)
    {
        shootStartPoint = transform.position + Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 0, 1f) + new Vector3(0, 0.6f, 0);
        float finalTime = (2.0f * vo.y) / Mathf.Abs(Physics.gravity.y);

        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, finalTime * i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }

    private void LateUpdate()
    {
        // Do not start until all players are successfully connected to the server   
        if (!playersReady) return;

        touchManager();

        if (screenTouch != -1)
        {
            Touch touch = Input.GetTouch(screenTouch);

            if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log("d-mX = " + touch.deltaPosition.x * rotSpeedX);
                Debug.Log("d-mX = " + touch.deltaPosition.y * rotSpeedY);
                mX += touch.deltaPosition.x * rotSpeedX / 300;
                mY += -touch.deltaPosition.y * rotSpeedY / 300;
            }
        }

        // Debugging lines. If clicked...
        if (Input.GetMouseButtonDown(0))
        {
            wasOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        if (Input.GetMouseButton(0) && !wasOverUI && Input.touchCount == 0)
        {
            // Get mouse axis.
            mX += Input.GetAxis("Mouse X") * rotSpeedX * (Time.deltaTime * rotDamp) * 1.5f;
            mY += -Input.GetAxis("Mouse Y") * rotSpeedY * (Time.deltaTime * rotDamp) * 1.5f;
        }

        // Clamp Y so player can't 'flip'.
        mY = Mathf.Clamp(mY, -80, 80);

        // Adjust rotation of camera and player's body.
        // Rotate the camera on its X axis for up / down camera movement.
        playerCamera.transform.localEulerAngles = new Vector3(mY, 0f, 0f);
        // Rotate the player's body on its Y axis for left / right camera movement.
        Vector3 rotDisp = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0f, mX, 0f);
        rotDisp = transform.eulerAngles - rotDisp;

        // Get Hor and Ver input.
        //float hor = Input.GetAxis("Horizontal");
        //float ver = Input.GetAxis("Vertical");
        float hor = joystick.Horizontal;
        float ver = joystick.Vertical;

        // Set speed to walk speed.
        currentSpeed = walkSpeed;
        // If player is pressing run key and moving forward, set speed to run speed.
        if (Input.GetKey(runKey) && Input.GetKey(KeyCode.W)) currentSpeed = runSpeed;

        // Get new move position based off input.
        Vector3 moveDir = (transform.right * hor) + (transform.forward * ver) - (transform.up * 0.8f);

        // Move CharController. 
        // .Move will not apply gravity, use SimpleMove if you want gravity.
        Vector3 posDisp = transform.position;
        cc.Move(moveDir * currentSpeed * Time.deltaTime);
        posDisp = transform.position - posDisp;
        // Send data to socket: Displacement and Rotation   
        if (posDisp.x != 0 || posDisp.y != 0 || posDisp.z != 0 || rotDisp.y != 0)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            data["x"] = "" + posDisp.x;
            data["y"] = "" + posDisp.y;
            data["z"] = "" + posDisp.z;
            data["ry"] = "" + rotDisp.y;

            socket.Emit("move", new JSONObject(data));
        }

        cnt++;
        if (cnt % 10 == 9)
        {
            cnt = 0;
            fixPosition();
        }
    }

    private void touchManager()
    {
        //Debug.Log("screentouch = " + screenTouch);
        if (screenTouch == -1)
        {
            // If there is no screentouch yet, assign new screenTouch
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    screenTouch = i;
                    break;
                }
            }
        }
        else
        {
            // Update screenTouch value after any touch is removed
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Ended)
                {
                    // Remove deleted screenTouch
                    if (i == screenTouch)
                    {
                        screenTouch = -1;
                        break;
                    }
                    else if (i < screenTouch)
                    {
                        screenTouch--;
                    }
                }
            }
        }
    }
}