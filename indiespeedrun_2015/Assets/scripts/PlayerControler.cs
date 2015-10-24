using UnityEngine;
using System.Collections;

public class PlayerControler : PersonBrain {

    /** Whether an 'move toward mouse' command was issued */
    private bool hasMouseTarget = false;
    /** Position toward which the player should move */
    private Vector2 mouseTarget;
    /** Transform of the target that will be bribed */
    private Transform personTarget;

    /** Whether the player just pressed the action button */
    private bool justPressedAction = false;
    /** Whether the action button was pressed on the last frame */
    private bool didPressAction = false;
    /** Whether the mouse button (or touch interface) was pressed last frame */
    private bool didMouse = false;
    /** Whether the player bribed anyone, this frame */
    private bool didBribeThisFrame = false;

    /** How much money the player has from bribing others */
    public int currentMoney = 0;

    // Use this for initialization
    void Start () {
        this.rbody = this.GetComponent<Rigidbody2D>();
        this.mouseTarget = Vector2.zero;

        initInstance(enType.level_0, enColor.black);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw("Horizontal") < -0.3f) {
            this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
            this.personTarget = null;
            this.hasMouseTarget = false;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0.3f) {
            this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
            this.personTarget = null;
            this.hasMouseTarget = false;
        }
        else if (Input.GetMouseButtonDown(0)) {
            // TODO Check if mouse is not overlapping a player and, in that
            // case, follow that player
            if (!this.didMouse) {
                // Get the position of the transform beneath the mouse
            }
            else {
                // Simply get the mouse position to move
                this.personTarget = null;
            }
            hasMouseTarget = true;
        }
        else if (hasMouseTarget) {
            if (this.personTarget != null) {
                
            }
        }
        else {
            this.rbody.velocity = Vector2.zero;
        }
        // TODO Add touch?
        
        this.justPressedAction = !this.didPressAction && Input.GetButtonDown("Action");
        this.didPressAction = Input.GetButtonDown("Action");
        this.didMouse = Input.GetMouseButtonDown(0);
        this.didBribeThisFrame = false;
    }

    new public void OnTriggerEnter2D(Collider2D other) {
        PersonBrain otherBrain;

        if (this.didBribeThisFrame) {
            return;
        }

        otherBrain = other.GetComponent<PersonBrain>();
        if (otherBrain) {
            // Check that the player has enough money to bribe the person
            if (this.currentMoney > otherBrain.getPrice()) {
                // TODO Make this more precise, in case it was a button press
                // (i.e., check that the person is in front of the player etc)
                if (other.GetComponent<Transform>() == this.personTarget ||
                        this.justPressedAction) {
                    // Remove a possible bribery target
                    this.personTarget = null;

                    this.currentMoney -= otherBrain.getPrice();
                    otherBrain.doBribe();

                    this.didBribeThisFrame = true;
                }
            }
            else if (this.justPressedAction) {
                // TODO Add a sign that you can't bribe that person
                Debug.Log("Can't bribe that person!");
            }
        }
    }

    override public void initInstance(enType type, enColor color) {
        base.initInstance(type, color);

        // The player position is always static, so set it
        this.transform.position = new Vector3(this.minHorPosition, 0.0f, 0.0f);

        forceStop();

        this.gameObject.SetActive(true);

        this.isRunningAI = true;
    }
}
