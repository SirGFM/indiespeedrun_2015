using UnityEngine;
using System.Collections.Generic;

public class LevelManager: MonoBehaviour {

    /** Default prefab used to spawn persons */
    public Transform personPrefab = null;
    /** Player prefab, to be spawned right (and only) at the start of the game */
    public Transform playerPrefab = null;

    /** Store the current level, should be increased on level transition */
    public int curLevel;
    /** List of currently active persons in the level */
    private List<Transform> personsInUse = null;
    /**
     * List of currently inactive persons that may be recycled on the next
     * level
     */
    private List<Transform> personsRecycled = null;

    // Use this for initialization
    void Start () {
        personsInUse = new List<Transform>();
        personsRecycled = new List<Transform>();

        // TODO Spawn player!

        curLevel = -1;
        startLevel(curLevel);
    }
	
	// Update is called once per frame
	void Update () {

	}

    /**
     * Retrieve a person either from the recycled list or spawn a new one, if
     * needed
     */
    private Transform getNewPerson() {
        if (personsRecycled.Count > 0) {
            Transform foundPerson;

            // Ugly hack for getting an object
            foundPerson = null;
            foreach (var item in personsRecycled) {
                foundPerson = item;
                break;
            }

            if (foundPerson) {
                personsRecycled.Remove(foundPerson);
                return foundPerson;
            }
        }

        return Instantiate(personPrefab);
    }

    /**
     * Spawn (either recycled or instantiated) a new person of the desired type
     * and color
     */
    private bool spawnNewPerson(PersonBrain.enType type, Color color) {
        Transform newPerson;
        PersonBrain personScript;

        newPerson = getNewPerson();
        personScript = newPerson.GetComponent<PersonBrain>();

        if (personScript) {
            personScript.initInstance(type, color);

            personsInUse.Add(newPerson);
        }
        else {
            Destroy(newPerson);
            return false;
        }

        return true;
    }

    void startLevel(int level) {
        // Should stop before an erro happens later on
        if (!personPrefab) {
            throw new System.Exception("Set the default prefab for every person!!");
        }

        // Add all items previously in use to the recycled list
        foreach (Transform item in personsInUse) {
            personsRecycled.Add(item);
            item.gameObject.SetActive(false);
        }
        personsInUse.Clear();

        // Spawn every player for that level
        switch (level) {
            case 0: {
                // TODO Spawn stuff
                spawnNewPerson(PersonBrain.enType.level_1, Color.black);
                spawnNewPerson(PersonBrain.enType.level_2, Color.red);
                } break;
            default: {
                // TODO Spawn more stuff
                spawnNewPerson(PersonBrain.enType.level_0, Color.blue);
            } break;
        }
    }
}
