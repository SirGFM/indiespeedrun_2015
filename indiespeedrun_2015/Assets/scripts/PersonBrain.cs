﻿using UnityEngine;
using System.Collections;

public class PersonBrain : MonoBehaviour {

    /** Definitions of persons' types */
    public enum enType {
        level_0 = 0,
        level_1,
        level_2,
        max
    };

    /** Definitions of persons' states relative to the player */
    public enum enState {
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
    /** Leftmost position the person can go to */
    public float minHorPosition = -10.0f;
    /** Rightmost position the person can go to */
    public float maxHorPosition = 10.0f;
    /** The horizontal speed */
    public float horizontalSpeed = 3.5f;

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
    /** The rigid body */
    private Rigidbody2D rbody;

    // Use this for initialization
    void Start() {
        this.rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // TODO Set the animation according with the state

        // Forcibly reverse the velocity, if going out of bounds
        if (this.transform.position.x <= this.minHorPosition) {
            this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
        }
        else if (this.transform.position.x >= this.maxHorPosition) {
            this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
        }

        // Reactivate the AI, if it has finished running
        if (!isRunningAI) {
            this.runningCoroutine = StartCoroutine(doAI());
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        PersonBrain otherBrain;

        otherBrain = other.GetComponent<PersonBrain>();
        if (otherBrain) {
            if (otherBrain.color == this.color) {
                Debug.Log("YAY! A friend!");
            }
            // TODO Check for parent/child color etc
        }
    }
    public void OnTriggerExit2D(Collider2D other) {
        PersonBrain otherBrain;

        otherBrain = other.GetComponent<PersonBrain>();
        if (otherBrain) {
            // TODO Check for parent/child color etc
        }
    }

    private IEnumerator doAI() {
        this.isRunningAI = true;
        switch (aiState) {
            case enAIState.idle: {
                float time;
                
                this.rbody.velocity = Vector2.zero;

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

                // Set speed
                if (this.transform.position.x <= this.minHorPosition) {
                    this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                }
                else if (this.transform.position.x >= this.maxHorPosition) {
                    this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                }
                else if (Random.Range(0, 1) == 0) {
                    this.rbody.velocity = new Vector2(-this.horizontalSpeed, 0.0f);
                }
                else {
                    this.rbody.velocity = new Vector2(this.horizontalSpeed, 0.0f);
                }

                Debug.Log("Walking for " + time.ToString() + "s");
                yield return new WaitForSeconds(time);
                Debug.Log("Exiting walk!");

                this.rbody.velocity = Vector2.zero;
    
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
     * Removes the AI loop and force this to be inactive
     */
    public void forceStop() {
        // Enable the next coroutine to start
        this.isRunningAI = false;
        // Switch the state to idle
        this.aiState = enAIState.idle;
        // Stop the current coroutine
        if (this.runningCoroutine != null) {
            StopCoroutine(this.runningCoroutine);
        }
        this.runningCoroutine = null;
        this.gameObject.SetActive(false);
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

        // Randomize the position
        this.transform.position = new Vector3(Random.Range(
                this.minHorPosition + 0.5f, this.maxHorPosition - 0.5f), 0, 0);

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
