using UnityEngine;
using System.Collections;

public class init_screen : MonoBehaviour {

    private bool didFinish;

    public Transform credits;
    public SpriteRenderer creditsSpr;

    // Use this for initialization
    void Start() {
        creditsSpr = credits.GetComponent<SpriteRenderer>();
        creditsSpr.color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update() {

    }

    public void startGame() {
        // TODO Check screen index
        Application.LoadLevel(1);
    }

    public void displayCredits() {
        StartCoroutine(onCredits());
    }

    public IEnumerator onCredits() {
        didFinish = false;

        StartCoroutine(creditFadeIn());
        while (!didFinish) {
            yield return null;
        }

        while (!Input.anyKeyDown) {
            yield return null;
        }

        didFinish = false;
        StartCoroutine(creditFadeOut());
        while (!didFinish) {
            yield return null;
        }
    }

    public IEnumerator creditFadeIn() {
        while (this.creditsSpr.color.a != 1f) {
            float a;

            a = this.creditsSpr.color.a + Time.deltaTime;
            if (a > 1f)
                a = 1f;

            this.creditsSpr.color = new Color(1f, 1f, 1f, a);

            yield return null;
        }

        didFinish = true;
    }

    public IEnumerator creditFadeOut() {
        while (this.creditsSpr.color.a != 0f) {
            float a;

            a = this.creditsSpr.color.a - Time.deltaTime;
            if (a < 0f)
                a = 0f;

            this.creditsSpr.color = new Color(1f, 1f, 1f, a);

            yield return null;
        }

        didFinish = true;
    }

}
