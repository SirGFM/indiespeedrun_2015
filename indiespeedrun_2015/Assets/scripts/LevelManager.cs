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
    public Sprite poster;
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
    private int lastFurniture;
    private int lastFurniture2;
    private int lastFurniture3;

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

    void spawnFurniture (float centerX) {
        bool isSecond;
        int newFurniture, repeat;
        Transform bg;
        SpriteRenderer spr;

        repeat = 1;
        isSecond = false;
        while (repeat > 0) {
            bg = getNewBG();
            spr = bg.GetComponent<SpriteRenderer>();
            spr.sortingOrder = 2;

            spr.transform.localScale = Vector3.one;
            newFurniture = this.lastFurniture;
            while (newFurniture == this.lastFurniture ||
                    newFurniture == this.lastFurniture2 ||
                    newFurniture == this.lastFurniture3 ||
                    (isSecond && (newFurniture == 3 || newFurniture == 0))) {
                newFurniture = Random.Range(0, 5);
            }
            switch (newFurniture) {
                case 0:{
                    spr.transform.position = new Vector3(centerX + 4.64f, -0.29f, 0.0f);
                    spr.sprite = bebedouro;
                } break;
                case 1: {
                    float x = Random.Range(-2.14f, 2.14f);
                    spr.transform.position = new Vector3(centerX + x, -2.62f, 0.0f);
                    spr.sprite = lixo;
                } break;
                case 2: {
                    spr.transform.position = new Vector3(centerX, -0.86f, 0.0f);
                    spr.sprite = cafeteira;
                } break;
                case 3: {
                    spr.transform.position = new Vector3(centerX + 4.51f, 2.172f, 0.0f);
                    spr.transform.localScale = new Vector3(0.15f, 0.15f);
                    spr.sprite = poster;
                    spr.sortingOrder = 1;
                    // Always add another sprite together with the poster
                    repeat++;
                } break;
                case 4: {
                    float x;
                    x = Random.Range(0, 4.5f);
                    spr.transform.position = new Vector3(centerX + x, -0.15f, 0.0f);
                    spr.sprite = planta;
                } break;
                case 5: {
                    float x;
                    x = Random.Range(-1.25f, 2.5f);
                    spr.transform.position = new Vector3(centerX + x, -1.6f, 0.0f);
                    spr.sprite = mesa;
                } break;
            }
            this.lastFurniture3 = this.lastFurniture2;
            this.lastFurniture2 = this.lastFurniture;
            this.lastFurniture = newFurniture;
            repeat--;
            isSecond = true;
        }
    }

    /**
     * Spawn a new level
     * 
     * @param [in]width           The width of the level in "scene chunks"
     * @param [in]initiaMoney     How much money the player has to bribe people
     * @param [in]targetFollowers How many follower the player must have to
     *                            advance
     * @param [in]people          Array of people's colors
     */
    private void spawnLevel(int width, int initialMoney, int targetFollowers,
            PersonBrain.enColor[] people) {
        int i;

        // Temporarially store this width so it can be returned
        this.width = width * 9 * 20 - 4.5f;

        // TODO Store the initial money and target followers
        this.player.GetComponent<PlayerControler>().currentMoney = initialMoney;

        // Set the limits for the RNG
        PersonBrain.minHorPosition = -4.4f;
        PersonBrain.maxHorPosition = this.width - 4.5f;
        // Spawn everything
        i = 0;
        while (i < people.Length) {
            PersonBrain.enColor color;
            PersonBrain.enType type;

            color = people[i];
            switch (color) {
                case PersonBrain.enColor.magenta: type = PersonBrain.enType.level_0; break;
                case PersonBrain.enColor.cyan: type = PersonBrain.enType.level_0; break;
                case PersonBrain.enColor.yellow: type = PersonBrain.enType.level_0; break;
                case PersonBrain.enColor.red: type = PersonBrain.enType.level_1; break;
                case PersonBrain.enColor.green: type = PersonBrain.enType.level_1; break;
                case PersonBrain.enColor.blue: type = PersonBrain.enType.level_1; break;
                case PersonBrain.enColor.black: type = PersonBrain.enType.level_2; break;
                case PersonBrain.enColor.white: type = PersonBrain.enType.level_2; break;
                default: type = PersonBrain.enType.level_0; break;
            }
            spawnNewPerson(type, color);

            i++;
        }
    }

    void startLevel(int level) {
        float x, width;
        SpriteRenderer spr;
        int initialMoney, targetFollowers;
        PersonBrain.enColor[] people;

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
        width = 9 * 20 - 4.5f; // This means 3 bgs of 9 unities

        // Spawn every player for that level
        switch (level) {
            case 0: {
                width = 3;
                initialMoney = 4;
                targetFollowers = 3;
                people = new PersonBrain.enColor[] {
                        PersonBrain.enColor.magenta,
                        PersonBrain.enColor.yellow
                    };
            } break;
            default: {
                width = 5;
                initialMoney = 10;
                targetFollowers = 5;
                people = new PersonBrain.enColor[] {
                        PersonBrain.enColor.magenta,
                        PersonBrain.enColor.magenta,
                        PersonBrain.enColor.cyan,
                        PersonBrain.enColor.cyan,
                        PersonBrain.enColor.yellow,
                        PersonBrain.enColor.yellow,
                        PersonBrain.enColor.red,
                        PersonBrain.enColor.red,
                        PersonBrain.enColor.blue,
                        PersonBrain.enColor.blue,
                        PersonBrain.enColor.green,
                        PersonBrain.enColor.green,
                        PersonBrain.enColor.black
                    };
            } break;
        }
        // Spawn the actual level
        spawnLevel((int)width, initialMoney, targetFollowers, people);
        // Retrieve the value previously calculated
        width = this.width;

        x = 0;
        spr = null;
        lastFurniture = -1;
        lastFurniture2 = -2;
        lastFurniture3 = -3;
        while (x < width) {
            Transform bg;

            bg = getNewBG();
            spr = bg.GetComponent<SpriteRenderer>();
            spr.transform.position = new Vector3(x, 0.0f, 0.0f);
            spr.transform.localScale = Vector3.one;

            spr.sprite = cenario;
            spr.sortingOrder = 0;

            if (x + spr.sprite.bounds.extents.x * 2f - 0.1f < width) {
                spawnFurniture(x);
            }

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
