using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Player {
    
    enum STATE { IDLE, ROAM, HUNT, GETHATBACK, RUNONLY };
    STATE currentState = STATE.ROAM;

    [Header("Modes")]
    public bool doesRun = false;
    public bool doesJump = false;
    public bool doesThrow = false;
    [Space(5)]

    [Header("Jump Properties")]
    public float jumpTimeMin = 0.05f;
    public float jumpTimeMax = 1.5f;
    float jumpTimer;
    float stopTimer;
    [Space(5)]
    
    [Header("Change Direction Properties")]
    public float changeDirectionTimeMin = 1f;
    public float changeDirectionTimeMax = 5f;
    float changeDirectionTimer;
    int xDir = 1;
    int yDir = 0;
    [Space(5)]

    [Header("Throw Properties")]
    public Vector2 throwDirection = Vector3.zero;
    public float minThrowTime = 1f;
    public float maxThrowTime = 3f;
    [HideInInspector]
    public float throwTimer;
    float throwStopTimer;
    bool wearingHat = true;

    [HideInInspector]
    public bool isWearingHat { get { return wearingHat; } set { wearingHat = value; } }

    PlayerInput inputController;
    CharacterColorChanger colorSwap;

    GameObject[] playersInGame;
    GameObject closestTarget = null;

    public override void Start() {

        base.Start();

        Init();

    }

    void Init() {

        inputController = GetComponent<PlayerInput>();
        colorSwap = GetComponent<CharacterColorChanger>();

        DetermineNewJumpTime();
        DetermineNewChangeDirectionTime();
        DetermineNewThrowTime();

        GiveRandomSkinAndColor();

    }

    void SetAIProperties(bool _doesRun, bool _doesJump, bool _doesThrow) {
        doesRun = _doesRun;
        doesJump = _doesJump;
        doesThrow = _doesThrow;
    }

    void FindPlayers() {

        int numberOfPlayers = GameController.instance.game.currentPlayers.Length;

        playersInGame = new GameObject[numberOfPlayers];

        for(int i = 0; i < playersInGame.Length; i++) {
            playersInGame[i] = GameController.instance.game.currentPlayers[i].inputReader.gameObject;
        }

    }

    void GiveRandomSkinAndColor() {
        int randomSkinIndex = Random.Range(0, colorSwap.alternateSkins.Length);
        int randomColorIndex = Random.Range(0, colorSwap.alternateSkins[randomSkinIndex].altColors.Length);
        colorSwap.Init(randomSkinIndex, randomColorIndex);
    }

    public override void FixedUpdate() {

        base.FixedUpdate();

        if (closestTarget == null) currentState = STATE.ROAM;
        if (currentState == STATE.GETHATBACK && hat.isCurrentlyAttached) currentState = STATE.ROAM;


        if (doesRun) UpdateMoving();
        if (doesJump) UpdateJumping();

    }


    public void SetStateToGetHatBack() {
        currentState = STATE.GETHATBACK;
    }

    public void SetStateToHunt() {
        currentState = STATE.HUNT;
    }

    public void AIGetsAKill() {
        if (!wearingHat) currentState = STATE.GETHATBACK;
        else currentState = STATE.ROAM;
    }

    public override void DieAtTheHandsOf(Player otherPlayer) {
        PlayDeathEffect();    

        hat.gameObject.SetActive(false);
        controller.SetDeadMask();
        
        if(GameController.instance.game.currentGameMode == Game.Mode.SURVIVAL) {
            otherPlayer.kills.Add(name);
            GameController.instance.game.UpdateSurvivalModeScore();          
        }

        BeginDeathFreeze();

    }

    void UpdateJumping() {
        if (hat == null) return;

        switch (currentState) {
            case STATE.IDLE:

                break;

            case STATE.RUNONLY:

                break;

            case STATE.ROAM:
                
                if (Time.time > jumpTimer) {

                    OnJumpInputDown();
                    DetermineNewJumpTime();
                    stopTimer = Time.time + Random.Range(0.05f, timeToJumpApex);

                }
                break;
            case STATE.HUNT:
                
                if(Mathf.Abs(transform.position.x - closestTarget.transform.position.x) <= 2 
                && Mathf.Abs(transform.position.y - closestTarget.transform.position.y) <= 2) {
                    OnJumpInputDown();
                    stopTimer = Time.time + Random.Range(timeToJumpApex / 10, timeToJumpApex / 5);
                } else {
                    if (Time.time > jumpTimer) {

                        OnJumpInputDown();
                        DetermineNewJumpTime();
                        stopTimer = Time.time + Random.Range(0.05f, timeToJumpApex);

                    }
                }
                
                break;

            case STATE.GETHATBACK:
                if (Time.time > jumpTimer) {

                    OnJumpInputDown();
                    DetermineNewJumpTime();
                    stopTimer = Time.time + Random.Range(0.05f, timeToJumpApex);

                }
                break;
            default:
                break;
        }

        
            
        if(Time.time > stopTimer) {

          OnJumpInputUp();

        }

    }

    void DetermineNewJumpTime() {
        jumpTimer = Time.time + Random.Range(jumpTimeMin, jumpTimeMax);
    }

    void UpdateMoving() {

        switch (currentState) {
            case STATE.IDLE:

                break;

            case STATE.ROAM:
                Roam();
                break;

            case STATE.HUNT:
                MoveToClosestTarget();
                CheckIfHatOrTargetIsCloser();
                break;

            case STATE.GETHATBACK:
                CheckIfHatOrTargetIsCloser();               
                WalkToHat();
                break;

            default:
                break;
        }

    }

    void Roam() {
        if (Time.time > changeDirectionTimer) {
            xDir = -xDir;
            SetDirectionalInput(new Vector2(xDir, 0));
            DetermineNewChangeDirectionTime();
        } else {

        }
    }


    void WalkToHat() {
        if (hat == null) {
            wearingHat = true;
            DetermineNewThrowTime();
            return;
        } 

        int _direction = (hat.transform.position.x > transform.position.x) ? 1 : -1;
        SetDirectionalInput(new Vector2(_direction, 0));

    }

    void DetermineNewChangeDirectionTime() {
        changeDirectionTimer = Time.time + Random.Range(changeDirectionTimeMin, changeDirectionTimeMax);
    }

    public override void GetStunned(float timeAmount) {

        base.GetStunned(timeAmount);

        wearingHat = false;
        currentState = STATE.GETHATBACK;

    }

    public void DetermineNewThrowTime() {
        throwTimer = Time.time + Random.Range(minThrowTime, maxThrowTime);
    }
       
    void MoveToClosestTarget() {
        xDir = (transform.position.x < closestTarget.transform.position.x) ? 1 : -1;

        if (Mathf.Abs(transform.position.x - closestTarget.transform.position.x) <= 1) yDir = -1;
        else yDir = 0;

        SetDirectionalInput(new Vector2(xDir, yDir));
    }

    public void CheckIfCloserTarget(GameObject _newestPlayer) {
        if (closestTarget == null) closestTarget = _newestPlayer;
        if (_newestPlayer == closestTarget) return;

        if(Vector2.Distance(transform.position, _newestPlayer.transform.position) < Vector2.Distance(transform.position, closestTarget.transform.position)
        && !_newestPlayer.GetComponent<Hat>().owner.isInvulnerable) {
            closestTarget = _newestPlayer.GetComponent<Hat>().gameObject;
        }

    }

    void CheckIfHatOrTargetIsCloser() {
        
        currentState = (Vector2.Distance(transform.position, hat.transform.position) < Vector2.Distance(transform.position, closestTarget.transform.position) && !hat.isCurrentlyAttached) ? STATE.GETHATBACK : STATE.HUNT;

    }

}
