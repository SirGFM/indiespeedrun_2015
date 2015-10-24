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

    /** Current state of the person, relative to the player */
    public enState state;
    /** Which of the classes the instance represent */
    public enType type;
    /** This object's color */
    public Color color;
    private enAIState aiState;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    // TODO Set a
	}

    public void initInstance(enType type, Color color) {
        this.type = type;
        this.color = color;
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
