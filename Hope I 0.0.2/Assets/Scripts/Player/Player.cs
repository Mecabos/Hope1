using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{

    //**** Public variables
    [Header("Physics parameters")]
    public float acceleration = 25f;
    public float maxSpeed = 20f;
    public float jumpHeight = 4;            //how far to jump 
    public float timeToJumpApex = 0.4f;     //how long to get to the heighest point
    public float playerGravity = -20f;
    public float accelerationTimeAirborne = 0.2f;
    public float accelerationTimeGrounded = 0.1f;
    [Space]
    [Header("Other variables")]
    public float fireRate = 0.2f;
    public GameObject shot;
    public Transform shotSpawnTransform;

    [System.NonSerialized]
    public MovementInfo movementInfo = new MovementInfo();
    [System.NonSerialized]
    public PlayerInfo playerInfo = new PlayerInfo();
    [System.NonSerialized]
    public SoundController soundController;


    //**** Private variables
    private Rigidbody2D playerRB;
    private Transform playerTransform;
    private Animator playerAnim;
    private Vector3 playerVelocity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private Vector3 originalScale;

    

    void Start()
    {
        //**Initialisations
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponent<Animator>();
        playerTransform = gameObject.GetComponent<Transform>();
        soundController = new SoundController();
        originalScale = playerTransform.localScale;
        movementInfo.isRuning = false;
        movementInfo.isGrounded = true;
        //physics laws
        playerGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(playerGravity) * timeToJumpApex;

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

        handleInput();
        handleFixedCheck();


    }

    
   //This methode handles capturing user input
    private void handleInput()
    {
        //blocks input while being hit
        if (playerInfo.isHurting)
        {
            finishedLanding();
            return;
        }

        // Input for walking
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        PlayerAction.walk(this,input);

        // Input for Jumping
        if (Input.GetButtonDown("Jump") && movementInfo.isGrounded)
        {
            PlayerAction.jump(this);
        }

        // Input for firing
        if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("LandingShoot"))
        {
            if (Input.GetButton("Fire1")) movementInfo.isShooting = true;
            else movementInfo.isShooting = false;

            if (Input.GetButton("Fire1") && Time.time > playerInfo.nextFire)
            {
                //Debug.Log("At input: Time = " + Time.time + "playerInfo.nextFire" + playerInfo.nextFire);
                playerInfo.nextFire = Time.time + fireRate;
                PlayerAction.shoot(this);
            }
        }
        
        


    }

    //this methode handles all the tests needed for every frame
    private void handleFixedCheck()
    {
        //dealing with player's gravity
        if (!movementInfo.isGrounded)
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y + playerGravity * Time.deltaTime);
        //animation for landing
        if (playerRB.velocity.y < 0 && !movementInfo.isGrounded)
            movementInfo.isLanding = true;
        playerAnim.SetBool(parameterIsLandingHash, movementInfo.isLanding);
        playerAnim.SetBool(parameterIsGroundedHash, movementInfo.isGrounded);
        playerAnim.SetBool(parameterIsRuningHash, (Mathf.Abs(playerRB.velocity.x) > 0.01f) && movementInfo.isRuning);
        playerAnim.SetBool(parameterIsShootingHash, movementInfo.isShooting);
        playerAnim.SetBool(parameterIsJumpingHash, movementInfo.isJumping);

        movementInfo.velocity = playerRB.velocity;
    }

    
    //this class contains all player actions
    public static class PlayerAction
    {

        public static void walk(Player player, Vector2 input)
        {
            
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            Transform playerTransform = player.GetComponent<Transform>();
            Animator playerAnim = player.GetComponent<Animator>();
            playerRB.AddForce(Vector3.right * player.acceleration * input.x);
            player.movementInfo.directionX = Mathf.Sign(playerRB.velocity.x);
            if (!player.movementInfo.isJumping)
                player.movementInfo.isRuning = input.x != 0;
            //Orientation of running and shooting animations 
            if (input.x != 0)
            {
                player.movementInfo.facingDirection = Mathf.Sign(input.x);
                playerTransform.localScale = new Vector3(player.originalScale.x * player.movementInfo.facingDirection, player.originalScale.y, player.originalScale.z);
            }
            
            //limiting speed 
            if (Mathf.Abs(playerRB.velocity.x) > player.maxSpeed)
                playerRB.velocity = new Vector2(player.maxSpeed * player.movementInfo.directionX, playerRB.velocity.y);

            //animation Speed
            playerAnim.SetFloat(player.parameterSpeedHash, Mathf.Clamp(Mathf.Abs(playerRB.velocity.x) / player.maxSpeed, 0.5f, 1.0f) );
        }

        public static void jump(Player player)
        {
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            Animator playerAnim = player.GetComponent<Animator>();
            playerRB.AddForce(Vector2.up * player.jumpVelocity, ForceMode2D.Impulse);
            //animation for jumping
            playerAnim.SetTrigger(player.parameterJumpHash);
            player.movementInfo.isJumping = true;
            player.movementInfo.isRuning = false;

        }

        public static void shoot(Player player)
        {

            Animator playerAnim = player.GetComponent<Animator>();
            if (!player.movementInfo.isRuning && player.movementInfo.isGrounded )
            {
                playerAnim.SetTrigger(player.parameterShootStillOnLandHash);
                playerAnim.ResetTrigger(player.parameterJumpShootHash);
                player.shootBolt();
            }
            else if(player.movementInfo.isRuning && player.movementInfo.isGrounded  )
            {
                playerAnim.SetTrigger(player.parameterShootRunOnLandHash);
                playerAnim.ResetTrigger(player.parameterShootStillOnLandHash);
                playerAnim.ResetTrigger(player.parameterJumpShootHash);
                player.shootBolt();
            }
            else if (!player.movementInfo.isGrounded)
            {
                playerAnim.SetTrigger(player.parameterJumpShootHash);
                player.shootBolt();
            }
        }

        public static void hurt (Player player)
        {
            Animator playerAnim = player.GetComponent<Animator>();
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            playerAnim.SetTrigger(player.parameterHurtHash);
            player.playerInfo.isHurting = true;
            playerRB.AddForce(new Vector2(1 * (-8 * player.movementInfo.directionX),5)  , ForceMode2D.Impulse);
        }
        
    }

    //class contains the movement infos
    public class MovementInfo
    {
        public float directionX;
        public float facingDirection = 1f;
        public bool isLanding;
        public bool isGrounded;
        public bool isRuning;
        public bool isJumping;
        public bool isShooting;
        public Vector2 velocity;

    }

    //class contains the player stats
    public class PlayerInfo
    {
        public float nextFire = 0.0f;
        public bool isHurting = false;
        

    }

    //Methods for animation events

    public void finishedLanding()
    {
        movementInfo.isLanding = false;
        movementInfo.isJumping = false;
    }

    public void shootBolt()
    {
        //Debug.Log("*At Shot: Time = " + Time.time + "playerInfo.nextFire" + playerInfo.nextFire);
        if (Time.time < playerInfo.nextFire)
        {
            GameObject newShot = Instantiate(shot, shotSpawnTransform.position, shotSpawnTransform.rotation);
            playerAnim.SetTrigger(parameterBangHash);
            newShot.transform.parent = shotSpawnTransform;
        }
        
    }

    public void endPain()
    {
        playerInfo.isHurting = false;
    }





    //comparing hash is quicker than comparing strings plus this gives a way to quickly change parameter names
    //float parameters
    int parameterSpeedHash = Animator.StringToHash("speed");
    //trigger parameters
    int parameterJumpHash = Animator.StringToHash("jump");
    int parameterJumpShootHash = Animator.StringToHash("jumpShoot");
    int parameterShootStillOnLandHash = Animator.StringToHash("shootStillOnLand");
    int parameterShootRunOnLandHash = Animator.StringToHash("shootRunOnLand");
    int parameterBangHash = Animator.StringToHash("bang");
    int parameterHurtHash = Animator.StringToHash("hurt");
    //bool parameters
    int parameterIsLandingHash = Animator.StringToHash("isLanding");
    int parameterIsGroundedHash = Animator.StringToHash("isGrounded");
    int parameterIsRuningHash = Animator.StringToHash("isRuning");
    int parameterIsShootingHash = Animator.StringToHash("isShooting");
    int parameterIsJumpingHash = Animator.StringToHash("isJumping");
    
    //int stateRunHash = Animator.StringToHash("Base Layer.Run");
}


