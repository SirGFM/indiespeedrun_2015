using UnityEngine;
using System.Collections;

public class PersonBrain : MonoBehaviour {

    /** Definitions of persons' types */
    public enum enType
    {
        level_0 = 0,
        level_1,
        level_2,
        max
    };

    /** Definitions of persons' states relative to the player */
    public enum enState
    {
        free = 0,
        bribed,
        persuaded,
        max
    };

    /** Definitions for the persons' AI states */
    private enum enAIState {
        idle = 0,
        walk,
        talk,
        getBribed,
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
    /** Current state of the person, relative to the player */
    public enState state;
    /** Which of the classes the instance represent */
    public enType type;
    /** This object's color */
    public Color color;
    /** Current state of the AI */
    private enAIState aiState;
    /** Probably unneeded, but avoids calling the coroutine if it's already running */
    private bool isRunningAI;
    /** The currently running coroutine (duh!) */
    private Coroutine runningCoroutine = null;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        // TODO Set the animation according with the state

        // TODO Check if trying to go out of borders and stop (i.e., force idle)

        if (!isRunningAI) {
            this.runningCoroutine = StartCoroutine(doAI());
        }
	}

    private IEnumerator doAI() {
        this.isRunningAI = true;
        switch (aiState) {
            case enAIState.idle: {
                float time;

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

                // TODO Set speed

                Debug.Log("Walking for " + time.ToString() + "s");
                yield return new WaitForSeconds(time);
                Debug.Log("Exiting walk!");
    
                // Always go back to idle, after this
                this.aiState = enAIState.idle;
             } break;
            case enAIState.talk: {

            } break;
            case enAIState.getBribed: {

            } break;
        }
        this.isRunningAI = false;
        this.runningCoroutine = null;
    }

    /**
     * Force this person to switch its state
     */
    private void forceAIState(enAIState aiState) {
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
     * Initialize the instance
     * 
     * @param type  The type of the person
     * @param color The color of the person
     */
    public void initInstance(enType type, Color color) {
        // Set the instance's properties
        this.type = type;
        this.color = color;

        // Always start on idle
        forceAIState(enAIState.idle);

        // TODO Set this only on the shirt sprite
        GetComponent<SpriteRenderer>().color = this.color;

        // TODO Randomize the position
        this.transform.position.Set(0, 0, 0);

        this.gameObject.SetActive(true);
    }

    /**
     * Return all children colors for this person
     */
    public Color[] getChildrenColor()
    {
        return null;
    }
    /**
     * Return the parent of this person's color
     */
    public Color getParentColor()
    {
        return Color.black;
    }

    /**
     * Return the price for bribing this person, according to its color, type
     * and influence
     */
    public int getPrice()
    {
        return 0x7fffff;
    }
}
