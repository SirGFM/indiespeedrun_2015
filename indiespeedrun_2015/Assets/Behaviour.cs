using UnityEngine;
using System.Collections;

public class Behaviour : MonoBehaviour {

    /** Definitions of objects' types */
    public enum enType {
        floppy = 0,
        cd,
        pendrive,
        boss,
        max
    };
    /** Which of the classes the instance represent */
    public enType type;
    /** This object's color */
    public Color color;
    /**
     * The object's parent color (i.e., which color gets cheaper to bribe as you
     * bribe objects from the same color as this one)
     */
    public Color parentColor;
    /** Every of the classes percentags (i.e., how easy it's to bribe 'em) */
    static private double[] percentage;

	// Use this for initialization
	void Start () {
	    if (percentage != null) {
            percentage = new double[(int)enType.max];
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
