using UnityEngine;
using System.Collections;

public class PersonBrain : MonoBehaviour {

    static public LevelManager lvlManager;

    /** Definitions of people's types */
    public enum enType {
        level_0 = 0,
        level_1,
        level_2,
        max
    };

    /** Definitions of people's states relative to the player */
    public enum enState {
        free = 0,
        bribed,
        influenced,
        max
    };

    /** Definitions for the people's AI states */
    public enum enAIState {
        idle = 0,
        walk,
        talk,
        getBribed,
        follow,
        pursue,
        max
    };

    /** Definitions for the people's colors (so it can be used in a switch) */
    public enum enColor {
        white   = 0x00000000,
        red     = 0x00000001,
        green   = 0x00000002,
        blue    = 0x00000004,
        magenta = 0x00000010,
        cyan    = 0x00000020,
        yellow  = 0x00000040,
        black   = 0x00000100,
        max
    };

    /** Minimum time the person will stand still if idle */
    public float minIdleTime = 0.5f;
    /** Maximum time the person will stand still if idle */
    public float maxIdleTime = 2.0f;
    /** Minimum time the person will walk */
    public float minWalkTime = 2.0f;
    /** Maximum time the person will walk */
    public float maxWalkTime = 5.0f;
    /** Leftmost position the person can go to */
    public float minHorPosition = -10.0f;
    /** Rightmost position the person can go to */
    public float maxHorPosition = 10.0f;
    /** The horizontal speed */
    public float horizontalSpeed = 3.5f;
    /** For how long influence state should run */
    public float influenceTime = 2.0f;
    /** Maximum distance from the player */
    public float maxDistanceFromPlayer = 0.5f;

    /** Current state of the person, relative to the player */
    public enState state;
    /** Which of the classes the instance represent */
    public enType type;
    /** This object's color */
    public enColor color;

    /** Current state of the AI */
    public enAIState aiState;
    /** Probably unneeded, but avoids calling the coroutine if it's already running */
    protected bool isRunningAI;
    /** The currently running coroutine (duh!) */
    private Coroutine runningCoroutine = null;
    /** The rigid body */
    protected Rigidbody2D rbody;
    private bool isWhite;

    // Use this for initialization
    void Start() {
        this.rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // TODO Set the animation according with the state

        // Forcibly reverse the velocity, if going out of bounds
        if (this.transform.position.x <= this.minHorPosition) {
            this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
        }
        else if (this.transform.position.x >= this.maxHorPosition) {
            this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
        }

        // Reactivate the AI, if it has finished running
        if (!isRunningAI) {
            this.runningCoroutine = StartCoroutine(doAI());
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        PersonBrain otherBrain;

        otherBrain = other.GetComponent<PersonBrain>();
        if (otherBrain) {
            if (this.state == enState.bribed &&
                    otherBrain.state == enState.free &&
                    isFriendColor(otherBrain.color)) {
                otherBrain.state = enState.influenced;
                otherBrain.forceAIState(enAIState.talk);
                forceAIState(enAIState.talk);
            }
        }
    }

    private IEnumerator doAI() {
        this.isRunningAI = true;

        if (this.state == enState.bribed && this.aiState != enAIState.talk) {
            int num;
            
            num = PersonBrain.lvlManager.countFreeWithColor(this.color,
                    this.type);

            if (num > 0) {
                this.aiState = enAIState.pursue;
            }
            else if (!isWhite) {
                // Gradually change the player color to white
                StartCoroutine(fadeToWhite());
            }
        }

        switch (aiState) {
            case enAIState.idle: {
                float time;
                
                this.rbody.velocity = Vector2.zero;

                time = Random.Range(minIdleTime, maxIdleTime);

                Debug.Log("Idle for " + time.ToString() + "s");
                yield return new WaitForSeconds(time);
                Debug.Log("Exiting idle!");

                // Select new state
                if (Random.Range(0, 1) == 0) {
                    this.aiState = enAIState.walk;
                }
                else {
                    this.aiState = enAIState.idle;
                }
            } break;
            case enAIState.walk:
            {
                float time;

                time = Random.Range(minWalkTime, maxWalkTime);

                // Set speed
                if (this.transform.position.x <= this.minHorPosition) {
                    this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                }
                else if (this.transform.position.x >= this.maxHorPosition) {
                    this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                }
                else if (Random.Range(0, 1) == 0) {
                    this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                }
                else {
                    this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                }

                Debug.Log("Walking for " + time.ToString() + "s");
                yield return new WaitForSeconds(time);
                Debug.Log("Exiting walk!");

                this.rbody.velocity = Vector2.zero;
    
                // Always go back to idle, after this
                this.aiState = enAIState.idle;
             } break;
            case enAIState.talk: {
                this.rbody.velocity = Vector2.zero;
                // Since both people must talk for the same amount of time they
                // should wait for the same amount of time
                yield return new WaitForSeconds(this.influenceTime);

                if (this.state == enState.influenced) {
                    // Gradually change the player color to white
                    StartCoroutine(fadeToWhite());
                }
            } break;
            case enAIState.getBribed: {
                this.rbody.velocity = Vector2.zero;
                // Since both people must talk for the same amount of time they
                // should wait for the same amount of time
                yield return new WaitForSeconds(this.influenceTime);

                this.state = enState.bribed;
            } break;
            case enAIState.follow: {
                Transform player;

                player = PersonBrain.lvlManager.player;
                while (Vector3.Distance(player.transform.position,
                        this.transform.position) > this.maxDistanceFromPlayer) {
                    // Go after the player
                    if (player.transform.position.x < this.transform.position.x &&
                            this.rbody.velocity.x >= 0) {
                        this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                        Debug.Log("Player is to my left");
                    }
                    else if (player.transform.position.x > this.transform.position.x &&
                            this.rbody.velocity.x <= 0) {
                        this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                        Debug.Log("Player is to my right");
                    }
                    else {
                        Debug.Log("Going the right direction!");
                    }

                    yield return null;
                }
            } break;
            case enAIState.pursue: {
                PersonBrain target;

                target = PersonBrain.lvlManager.getNextInfluentiable(this.color,
                    this.type);
                Debug.Log("Entered pursue!");
                
                while (target.state == enState.free) {
                    // Go after that person
                    if (target.transform.position.x < this.transform.position.x &&
                            this.rbody.velocity.x >= 0) {
                        this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                        Debug.Log("Target is to my left");
                    }
                    else if (target.transform.position.x > this.transform.position.x &&
                            this.rbody.velocity.x <= 0) {
                        this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                        Debug.Log("Target is to my right");
                    }
                    else {
                        Debug.Log("Going the right direction!");
                    }

                    // Wait until this person overlap its target or the target
                    // gets influenced/bribed by another
                    yield return null;
                }
            } break;
        }
        this.isRunningAI = false;
        this.runningCoroutine = null;
        
        if (this.state != enState.free) {
            if (Vector3.Distance(PersonBrain.lvlManager.player.transform.position,
                    this.transform.position) > this.maxDistanceFromPlayer) {
                this.aiState = enAIState.follow;
            }
            else {
                this.aiState = enAIState.idle;
            }
        }
    }

    private IEnumerator fadeToWhite() {
        SpriteRenderer spr;
        
        isWhite = true;
        spr = this.GetComponent<SpriteRenderer>();
        while (spr.color != Color.white) {
            float b = spr.color.b;
            float g = spr.color.g;
            float r = spr.color.r;

            if (r < 1.0f) {
                r += Time.deltaTime;
                if (r > 1.0f) {
                    r = 1.0f;
                }
            }
            if (g < 1.0f) {
                g += Time.deltaTime;
                if (g > 1.0f) {
                    g = 1.0f;
                }
            }
            if (b < 1.0f) {
                b += Time.deltaTime;
                if (b > 1.0f) {
                    b = 1.0f;
                }
            }

            spr.color = new Color(r, g, b);

            yield return null;
        }
    }

    /**
     * Removes the AI loop and force this to be inactive
     */
    public void forceStop() {
        // Enable the next coroutine to start
        this.isRunningAI = false;
        // Switch the state to idle
        this.aiState = enAIState.idle;
        // Stop the current coroutine
        if (this.runningCoroutine != null) {
            StopCoroutine(this.runningCoroutine);
        }
        this.runningCoroutine = null;
        this.gameObject.SetActive(false);
    }

    /**
     * Bribe that person
     */
    public void doBribe() {
        forceAIState(enAIState.getBribed);
    }

    /**
     * Force this person to switch its state
     */
    protected void forceAIState(enAIState aiState) {
        // Enable the next coroutine to start
        this.isRunningAI = false;
        // Switch the state
        this.aiState = aiState;
        // Stop the current coroutine
        if (this.runningCoroutine != null) {
            StopCoroutine(this.runningCoroutine);
        }
        this.runningCoroutine = null;
    }

    /**
     * Initialize a interactive instance
     * 
     * @param type  The type of the person
     * @param color The color of the person
     */
    virtual public void initInstance(enType type, enColor color) {
        Color32 c32;

        // Set the instance's properties
        this.type = type;
        this.color = color;

        // Make sure this person can be bribed/influenced
        this.state = enState.free;
        this.isWhite = false;

        // Always start on idle
        forceAIState(enAIState.idle);

        // TODO Set this only on the shirt sprite
        switch (this.color) {
            case enColor.red:     c32 = new Color32(0xed, 0x6a, 0x6a, 0xff); break;
            case enColor.green:   c32 = new Color32(0x3e, 0xb7, 0x79, 0xff); break;
            case enColor.blue:    c32 = new Color32(0x54, 0x57, 0x9e, 0xff); break;
            case enColor.magenta: c32 = new Color32(0xda, 0x71, 0xb0, 0xff); break;
            case enColor.cyan:    c32 = new Color32(0x69, 0xb6, 0xd3, 0xff); break;
            case enColor.yellow:  c32 = new Color32(0xe1, 0xda, 0x4f, 0xff); break;
            case enColor.black:   c32 = new Color32(0x00, 0x00, 0x00, 0xff); break;
            case enColor.white:   c32 = new Color32(0xff, 0xff, 0xff, 0xff); break;
            default:              c32 = new Color32(0xff, 0xff, 0xff, 0xff); break;
        }
        GetComponent<SpriteRenderer>().color = new Color((float)c32.r / 255.0f,
                (float)c32.g / 255.0f, (float)c32.b / 255.0f);
        
        // Randomize the position
        this.transform.position = new Vector3(Random.Range(
                this.minHorPosition + 0.5f, this.maxHorPosition - 0.5f), 0, 0);
        
        // Activate the object's physics/behavious/etc
        this.gameObject.SetActive(true);
    }

    /**
     * Return all colors this person has influence over
     */
    public bool isFriendColor(enColor color) {
        switch (this.color) {
            case enColor.magenta: return color == enColor.magenta;
            case enColor.cyan: return color == enColor.cyan;
            case enColor.yellow: return color == enColor.yellow;
            case enColor.red: return color == enColor.red ||
                    color == enColor.magenta || color == enColor.yellow;
            case enColor.green: return color == enColor.green ||
                    color == enColor.yellow || color == enColor.cyan;
            case enColor.blue: return color == enColor.blue ||
                    color == enColor.cyan || color == enColor.magenta;
            case enColor.black: return true;
            default: return false;
        }
    }

    /**
     * Return all colors this person has influence over
     */
    public bool sufferInfluence(enColor color) {
        switch (this.color) {
            case enColor.magenta: return color == enColor.magenta ||
                    color == enColor.red || color == enColor.blue ||
                    color == enColor.black;
            case enColor.cyan: return color == enColor.cyan ||
                    color == enColor.blue || color == enColor.green ||
                    color == enColor.black;
            case enColor.yellow: return color == enColor.yellow ||
                    color == enColor.green || color == enColor.red ||
                    color == enColor.black;
            case enColor.red: return color == enColor.red ||
                    color == enColor.black;
            case enColor.green: return color == enColor.green ||
                    color == enColor.black;
            case enColor.blue: return color == enColor.blue ||
                    color == enColor.black;
            case enColor.black: return color == enColor.black;
            default: return false;
        }
    }

    /**
     * Return the price for bribing this person, according to its color, type
     * and influence
     */
    public int getPrice() {
        // TODO If was bribed, set the price to 0x7fffffff
        if (this.state == enState.bribed) {
            return 0x7fffffff;
        }
        else {
            switch (this.type) {
                // TODO Define the prices
                case enType.level_0: return 2;
                case enType.level_1: return 4;
                case enType.level_2: return 8;
                default: return 0x7fffffff;
            }
        }
    }
}
