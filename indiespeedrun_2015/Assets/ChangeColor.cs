using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour{

    public Color color;

    // Use this for initialization
    void Start() {
        // TODO Check why this doesn't work
        //GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    void Update() {

    }
}
