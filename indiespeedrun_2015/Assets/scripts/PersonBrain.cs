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
        black  = 0x00000000,
        red    = 0x00000001,
        green  = 0x00000002,
        blue   = 0x00000004,
        purple = 0x00000008,
        yellow = 0x00000010,
        white  = 0x40000000,
        // TODO Add more colors
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

    public void OnTriggerExit2D(Collider2D other) {
        PersonBrain otherBrain;

        otherBrain = other.GetComponent<PersonBrain>();
        if (otherBrain) {
            // TODO Check for parent/child color etc
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
            } break;
            case enAIState.getBribed: {

            } break;
            case enAIState.follow: {

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
            // TODO Check if the player is moving
            if (false) {
                this.aiState = enAIState.follow;
            }
            else {
                this.aiState = enAIState.idle;
            }
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
        Color sprColor;

        // Set the instance's properties
        this.type = type;
        this.color = color;

        // Make sure this person can be bribed/influenced
        this.state = enState.free;

        // Always start on idle
        forceAIState(enAIState.idle);

        // TODO Set this only on the shirt sprite
        switch (this.color) {
            case enColor.black: sprColor = Color.black; break;
            case enColor.red: sprColor = Color.red; break;
            case enColor.green: sprColor = Color.green; break;
            case enColor.blue: sprColor = Color.blue; break;
            case enColor.purple: sprColor = new Color(0.8f, 0.0f, 0.8f); break;
            case enColor.yellow: sprColor = Color.yellow; break;
            default: sprColor = Color.white; break;
        }
        GetComponent<SpriteRenderer>().color = sprColor;
        
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
            case enColor.red: return color == enColor.red;
            case enColor.green: return color == enColor.green;
            case enColor.blue: return color == enColor.blue;
            case enColor.purple: return color == enColor.purple || color == enColor.red ||
                                        color == enColor.blue;
            // TODO Add more Colors
            default: return false;
        }
    }

    /**
     * Return all colors this person has influence over
     */
    public bool sufferInfluence(enColor color) {
        switch (this.color) {
            case enColor.red: return color == enColor.red ||
                    color == enColor.purple || color == enColor.yellow;
            case enColor.green: return color == enColor.green ||
                    color == enColor.yellow;
            case enColor.blue: return color == enColor.blue ||
                    color == enColor.purple;
            case enColor.purple: return color == enColor.purple;
            // TODO Add more Colors
            default: return false;
        }
    }

    /**
     * Return the price for bribing this person, according to its color, type
     * and influence
     */
    public int getPrice() {
        return 0x7fffff;
    }
}
