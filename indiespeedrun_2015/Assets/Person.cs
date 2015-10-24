using unityengine;
using system.collections;

public class behaviour : monobehaviour {

    /** definitions of persons' types */
    public enum enType {
        level_0 = 0,
        level_1,
        level_2,
        max
    };

    /** definitions of persons' states relative to the player */
    public enum enState {
        free = 0,
        bribed,
        persuaded,
        max
    };

    /** Current state of the person, relative to the player */
    public enState state;
    /** which of the classes the instance represent */
    public enType type;
    /** This object's color */
    public Color color;
    /** How many 'resources' are needed to bribe this person */
    public int price;
    /**
     * the object's parent color (i.e., which color gets cheaper to bribe as you
     * bribe objects from the same color as this one)
     */
    public Color parentColor;
    //public Color[] childrenColor;

    /*
     * TODO:
     *   - Add the person to a global counter, when they are bribed
     *   - Make sure a person
     *   - Create a list of children colors (for each parent color)
     */

	// use this for initialization
	void start () {

	}
	
	// update is called once per frame
	void update () {
	
	}
}
