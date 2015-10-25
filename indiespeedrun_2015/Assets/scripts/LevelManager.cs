using UnityEngine;
using System.Collections.Generic;

public class LevelManager: MonoBehaviour {

    /** Default prefab used to spawn persons */
    public Transform personPrefab = null;
    /** Player prefab, to be spawned right (and only) at the start of the game */
    public Transform playerPrefab = null;
    /** Background prefab, to be spawned right (and only) at the start of the game */
    public Transform bgPrefab = null;

    /** Store the current level, should be increased on level transition */
    public int curLevel;
    /** The player */
    public Transform player;
    /** Current width of the level */
    public float width;


    public Sprite bebedouro;
    public Sprite cadeira;
    public Sprite cafeteira;
    public Sprite cenario;
    public Sprite elevador_off;
    public Sprite elevador_on;
    public Sprite retrato;
    public Sprite escrivaninha;
    public Sprite lixo;
    public Sprite mesa;
    public Sprite planta;

    /** List of currently active persons in the level */
    private List<Transform> personsInUse = null;
    /**
     * List of currently inactive persons that may be recycled on the next
     * level
     */
    private List<Transform> personsRecycled = null;
    /** All the background's sprites currently being rendered */
    private List<Transform> bgInUse = null;
    /** All the background's sprites recyled for later use */
    private List<Transform> bgRecycled = null;
    private SpriteRenderer elevatorSpr = null;

    // Use this for initialization
    void Start () {
        // Initialize the static reference to this
        PersonBrain.lvlManager = this;

        personsInUse = new List<Transform>();
        personsRecycled = new List<Transform>();

        bgInUse = new List<Transform>();
        bgRecycled = new List<Transform>();

        // Spawn player
        player = Instantiate(playerPrefab);

        curLevel = -1;
        startLevel(curLevel);
    }
	
	// Update is called once per frame
	void Update () {
        bool objectiveDone;

        objectiveDone = false;
        if (objectiveDone) {
            elevatorSpr.sprite = elevador_on;
        }
	}

    public int countFreeWithColor(PersonBrain.enColor color, PersonBrain.enType type) {
        int count;

        count = 0;
        foreach (Transform item in this.personsInUse) {
            PersonBrain brain;

            brain = item.GetComponent<PersonBrain>();
            if (brain && brain.state == PersonBrain.enState.free &&
                    brain.type <= type && brain.sufferInfluence(color)) {
                count++;
            }
        }

        return count;
    }

    public PersonBrain getNextInfluentiable(PersonBrain.enColor color, PersonBrain.enType type) {
        foreach (Transform item in this.personsInUse) {
            PersonBrain brain;

            brain = item.GetComponent<PersonBrain>();
            if (brain && brain.state == PersonBrain.enState.free &&
                    brain.type <= type && brain.sufferInfluence(color)) {
                return brain;
            }
        }
        return null;
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
     * Retrieve a person either from the recycled list or spawn a new one, if
     * needed
     */
    private Transform getNewBG() {
        if (bgRecycled.Count > 0) {
            Transform foundBG;

            // Ugly hack for getting an object
            foundBG = null;
            foreach (var item in bgRecycled) {
                foundBG = item;
                break;
            }

            if (foundBG) {
                bgRecycled.Remove(foundBG);
                return foundBG;
            }
        }

        return Instantiate(bgPrefab);
    }

    /**
     * Spawn (either recycled or instantiated) a new person of the desired type
     * and color
     */
    private bool spawnNewPerson(PersonBrain.enType type, PersonBrain.enColor color) {
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
        float x, width;
        SpriteRenderer spr;

        // Should stop before an erro happens later on
        if (!personPrefab) {
            throw new System.Exception("Set the default prefab for every person!!");
        }

        // Add all items previously in use to the recycled list
        foreach (Transform item in personsInUse) {
            personsRecycled.Add(item);
            item.GetComponent<PersonBrain>().forceStop();
        }
        personsInUse.Clear();

        // Add all items previously in use to the recycled list
        foreach (Transform item in bgInUse) {
            bgRecycled.Add(item);
            item.gameObject.SetActive(false);
        }
        bgInUse.Clear();

        // Set default width
        width = 9 * 3 - 4.5f; // This means 3 bgs of 9 unities
        PersonBrain.minHorPosition = -4.4f;

        // Spawn every player for that level
        switch (level) {
            case 0: {
                // TODO set width
                PersonBrain.maxHorPosition = width - 4.5f;
                // TODO Spawn stuff
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.green);
                spawnNewPerson(PersonBrain.enType.level_2, PersonBrain.enColor.red);
                } break;
            default: {
                // TODO set width
                PersonBrain.maxHorPosition = width - 4.5f;
                // TODO Spawn more stuff
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.magenta);
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.magenta);
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.cyan);
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.cyan);
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.yellow);
                spawnNewPerson(PersonBrain.enType.level_0, PersonBrain.enColor.yellow);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.red);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.red);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.blue);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.blue);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.green);
                spawnNewPerson(PersonBrain.enType.level_1, PersonBrain.enColor.green);
                spawnNewPerson(PersonBrain.enType.level_2, PersonBrain.enColor.black);
            } break;
        }
        
        x = 0;
        spr = null;
        while (x < width) {
            Transform bg;

            bg = getNewBG();
            spr = bg.GetComponent<SpriteRenderer>();
            spr.transform.position = new Vector3(x, 0.0f, 0.0f);

            spr.sprite = cenario;
            spr.sortingOrder = 0;
            x += spr.sprite.bounds.extents.x * 2f - 0.1f;

            this.width = x - spr.sprite.bounds.extents.x * 3;
        }
        PersonBrain.maxHorPosition = this.width + spr.sprite.bounds.extents.x * 2;
        // Get the last rendered scenario store its sprite, so it's texture can be changed
        if (spr != null) {
            spr.sprite = elevador_off;
            elevatorSpr = spr;
        }

        player.GetComponent<PlayerControler>().clear();
    }
}
