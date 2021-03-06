﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerControler : PersonBrain {

    /** List of currently overlaping people */
    private List<PersonBrain> overlapping;

    /** Whether an 'move toward mouse' command was issued */
    private bool hasMouseTarget = false;
    /** Position toward which the player should move */
    private Vector2 mouseTarget;
    /** Transform of the target that will be bribed */
    private Transform personTarget;

    /** Whether the player just pressed the action button */
    public bool justPressedAction = false;
    /** Whether the action button was pressed on the last frame */
    public bool didPressAction = false;
    /** Whether the mouse button (or touch interface) was pressed last frame */
    public bool didMouse = false;
    /** Whether the player bribed anyone, this frame */
    public bool didBribeThisFrame = false;

    /** How much money the player has from bribing others */
    public int currentMoney = 0;

    private bool freezeMov = false;

    // Use this for initialization
    void Start () {
        this.rbody = this.GetComponent<Rigidbody2D>();
        this.mouseTarget = Vector2.zero;

        initInstance(enType.level_0, enColor.white, true);

        this.overlapping = new List<PersonBrain>();
        this.fixLayer = this.GetComponentInChildren<FixLayer>();
        this.fixLayer.fixLayer(this as PersonBrain, true);

        animator = GetComponentInChildren<Animator>();

        //this.GetComponent<SpriteRenderer>().sortingOrder = 20;
    }
	
	// Update is called once per frame
	void Update () {
        if (freezeMov) {

        }
        else if (Input.GetAxisRaw("Horizontal") < -0.3f) {
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
            if (this.didMouse) {
                // TODO Get the position of the transform beneath the mouse
            }
            else {
                // Simply get the mouse position to move
                mouseTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.personTarget = null;
            }
            hasMouseTarget = true;
        }
        else if (hasMouseTarget) {
            Vector3 target;

            if (this.personTarget != null) {
                target = this.personTarget.position;
            }
            else {
                target = mouseTarget;
            }

            if (this.transform.position.x < target.x - 0.5f) {
                this.rbody.velocity = new Vector3(this.horizontalSpeed, 0.0f, 0.0f);
            }
            else if (this.transform.position.x > target.x + 0.5f) {
                this.rbody.velocity = new Vector3(-this.horizontalSpeed, 0.0f, 0.0f);
            }
            else {
                hasMouseTarget = false;
                this.rbody.velocity = Vector3.zero;
            }
        }
        else {
            this.rbody.velocity = Vector2.zero;
        }
        // TODO Add touch?

        if (this.transform.position.x < PersonBrain.minHorPosition) {
            this.transform.position = new Vector3(PersonBrain.minHorPosition,
                    0.0f, 0.0f);
            if (this.rbody.velocity.x < 0) {
                this.rbody.velocity = Vector3.zero;
            }
        }
        else if (this.transform.position.x > PersonBrain.maxHorPosition) {
            this.transform.position = new Vector3(PersonBrain.maxHorPosition,
                    0.0f, 0.0f);
            if (this.rbody.velocity.x > 0) {
                this.rbody.velocity = Vector3.zero;
            }
        }

        this.justPressedAction = !this.didPressAction && Input.GetButtonDown("Action");
        this.didPressAction = Input.GetButtonDown("Action");
        this.didMouse = Input.GetMouseButtonDown(0);
        this.didBribeThisFrame = false;

        checkOverlap();
        
        if (this.dir != enDir.left && this.rbody.velocity.x < 0) {
            this.fixLayer.moveRight();
            this.dir = enDir.left;
        }
        else if (this.dir != enDir.right && this.rbody.velocity.x > 0) {
            this.fixLayer.moveLeft();
            this.dir = enDir.right;
        }

        if (this.rbody.velocity.x != 0f) {
            animator.SetFloat("MovBlend", 1);
        }
        else {
            animator.SetFloat("MovBlend", 0);
        }
    }

    new public void OnTriggerEnter2D(Collider2D other) {
        PersonBrain otherBrain;

        if (other != null && this.overlapping != null) {
            otherBrain = other.GetComponent<PersonBrain>();
            if (otherBrain != null) {
                this.overlapping.Add(otherBrain);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other) {
        PersonBrain otherBrain;

        if (other != null && this.overlapping != null) {
            otherBrain = other.GetComponent<PersonBrain>();
            if (otherBrain != null && this.overlapping.Contains(otherBrain)) {
                this.overlapping.Remove(otherBrain);
            }
        }
    }

    /**
     * Check if any person was overlapped
     */
    private void checkOverlap() {
        bool errorFlag;

        errorFlag = false;
        if (this.overlapping != null && overlapping.Count > 0) {
            foreach (PersonBrain other in overlapping) {
                // Check that the player has enough money to bribe the person
                if (this.currentMoney >= other.getPrice()) {
                    // TODO Make this more precise, in case it was a button
                    // press (i.e., check that the person is in front of the
                    // player etc)
                    if (other.GetComponent<Transform>() == this.personTarget ||
                            this.justPressedAction) {
                        // Remove a possible bribery target
                        this.personTarget = null;

                        this.currentMoney -= other.getPrice();
                        animator.SetTrigger("Bribe");
                        StartCoroutine("STOP");
                        other.doBribe();

                        return;
                    }
                }
                else if (this.justPressedAction &&
                        other.state != enState.bribed) {
                    errorFlag = true;
                }
            }
        }

        if (errorFlag) {
            // TODO Add a sign that you can't bribe that person
            Debug.Log("Can't bribe that person!");
        }
    }

    private System.Collections.IEnumerator STOP() {
        freezeMov = true;
        yield return new WaitForSeconds(2f);
        freezeMov = false;
    }

    override public void initInstance(enType type, enColor color, bool isPlayer) {
        base.initInstance(type, color, true);

        // The player position is always static, so set it
        this.transform.position = new Vector3(PersonBrain.minHorPosition, 0.0f, 0.0f);

        forceStop();

        this.gameObject.SetActive(true);

        this.isRunningAI = true;
    }

    public void clear(int initialMoney) {
        // Removed everything that was overlapping
        if (this.overlapping != null) {
            this.overlapping.Clear();
        }
        // Reset the position
        this.transform.position = Vector3.zero;
        // Reset the amount of money
        this.currentMoney = initialMoney;
    }

    public void advanceType() {
        this.type++;
        this.GetComponentInChildren<CharCostumeController>().ChangeLevel((int)this.type);
    }
}
