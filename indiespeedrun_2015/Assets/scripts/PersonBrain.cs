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
    public float maxIdleTime = 0.5f;
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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // TODO Set the animation according with the state

        if (!isRunningAI) {
            StartCoroutine("doAI");
        }
	}

    private IEnumerator doAI() {
        isRunningAI = true;
        switch (aiState) {
            case enAIState.idle: {
                float time;

                time = Random.Range(minIdleTime, maxIdleTime);

                yield return new WaitForSeconds(time);

                // TODO Select new state
            } break;
        }
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
        this.aiState = enAIState.idle;
        // Enable running the AI loop
        this.isRunningAI = false;

        // TODO Set this only on the shirt sprite
        GetComponent<SpriteRenderer>().color = color;

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
