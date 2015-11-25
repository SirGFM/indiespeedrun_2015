using UnityEngine;
using System.Collections;

public class init_screen : MonoBehaviour {

    private bool didFinish;

    public AudioClip firstSong;

	public Transform inicio_01;
	private SpriteRenderer inicioSpr_01;
	public Transform inicio_02;
	private SpriteRenderer inicioSpr_02;
	public Transform inicio_03;
	private SpriteRenderer inicioSpr_03;
	public Transform inicio_04;
	private SpriteRenderer inicioSpr_04;
	public Transform inicio_05;
	private SpriteRenderer inicioSpr_05;
	public Transform inicio_06;
	private SpriteRenderer inicioSpr_06;
	public Transform inicio_07;
	private SpriteRenderer inicioSpr_07;
	public Transform inicio_08;
	private SpriteRenderer inicioSpr_08;
	public Transform inicio_09;
	private SpriteRenderer inicioSpr_09;

	public Transform title;
	private SpriteRenderer titleSpr;

    public Transform credits;
    private SpriteRenderer creditsSpr;

	public Transform playBt;
	private SpriteRenderer playBtSpr;
	public Transform creditsBt;
	private SpriteRenderer creditsBtSpr;

	private bool creditsRunning = false;
	private bool finishedIntro = false;
	private bool forceExit = false;

	private Coroutine introCoroutine = null;

    // Use this for initialization
    void Start() {
        GameObject go;

        creditsSpr = credits.GetComponent<SpriteRenderer>();
        creditsSpr.color = new Color(1f, 1f, 1f, 0f);

		titleSpr = title.GetComponent<SpriteRenderer>();
		titleSpr.color = new Color(1f, 1f, 1f, 0f);

		inicioSpr_01 = inicio_01.GetComponent<SpriteRenderer>();
		inicioSpr_01.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_02 = inicio_02.GetComponent<SpriteRenderer>();
		inicioSpr_02.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_03 = inicio_03.GetComponent<SpriteRenderer>();
		inicioSpr_03.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_04 = inicio_04.GetComponent<SpriteRenderer>();
		inicioSpr_04.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_05 = inicio_05.GetComponent<SpriteRenderer>();
		inicioSpr_05.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_06 = inicio_06.GetComponent<SpriteRenderer>();
		inicioSpr_06.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_07 = inicio_07.GetComponent<SpriteRenderer>();
		inicioSpr_07.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_08 = inicio_08.GetComponent<SpriteRenderer>();
		inicioSpr_08.color = new Color(1f, 1f, 1f, 0f);
		inicioSpr_09 = inicio_09.GetComponent<SpriteRenderer>();
		inicioSpr_09.color = new Color(1f, 1f, 1f, 0f);
		
		playBtSpr = playBt.GetComponent<SpriteRenderer>();
		playBtSpr.color = new Color(1f, 1f, 1f, 0f);
		playBt.gameObject.SetActive(false);

		creditsBtSpr = creditsBt.GetComponent<SpriteRenderer>();
		creditsBtSpr.color = new Color(1f, 1f, 1f, 0f);
		creditsBt.gameObject.SetActive(false);

        go = GameObject.Find("MainAudioController");
        if (go != null) {
            SoundController sc;

            sc = go.GetComponent<SoundController>();
            if (sc != null) {
                AudioSource audSrc;

                audSrc = go.GetComponent<AudioSource>();
                if (audSrc) {
                    sc.PlayLoop(firstSong, audSrc);
                }
            }
        }

		finishedIntro = false;

		this.introCoroutine = StartCoroutine(onIntro());
		StartCoroutine(forceExitIntro());
    }

    // Update is called once per frame
    void Update() {

    }

    public void startGame() {
        // TODO Check screen index
        Application.LoadLevel(1);
    }

	public IEnumerator onIntro() {
		finishedIntro = false;

		StartCoroutine(spriteFadeIn(this.inicioSpr_01, 1.0f));
		yield return new WaitForSeconds(0.75f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_02, 2.5f));
		yield return new WaitForSeconds(3.25f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_03, 1.5f));
		yield return new WaitForSeconds(0.75f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_04, 3.0f));
		yield return new WaitForSeconds(5.25f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_05, 1.0f));
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_06, 5.0f));
		yield return new WaitForSeconds(4.0f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_07, 5.0f));
		yield return new WaitForSeconds(5.5f);
		StartCoroutine(spriteFadeIn(this.inicioSpr_08, 5.0f));
		yield return new WaitForSeconds(7.0f);

		StartCoroutine(spriteFadeOut(this.inicioSpr_01, 0.75f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_02, 0.8f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_03, 0.85f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_04, 0.9f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_05, 0.95f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_06, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_07, 1.05f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_08, 1.1f));
		
		yield return new WaitForSeconds(2.0f);
		
		StartCoroutine(spriteFadeIn(this.inicioSpr_09, 3.0f));
		yield return new WaitForSeconds(4.0f);

		// Force the 'forceExitIntro' coroutine to run
		this.introCoroutine = null;
	}

	public IEnumerator forceExitIntro() {
		while (!Input.anyKeyDown && this.introCoroutine != null) {
			yield return null;
		}

		if (this.introCoroutine != null) {
			StopCoroutine(this.introCoroutine);
		}

		this.forceExit = true;
		yield return null;
		this.forceExit = false;

		StartCoroutine(spriteFadeOut(this.inicioSpr_01, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_02, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_03, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_04, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_05, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_06, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_07, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_08, 1.0f));
		StartCoroutine(spriteFadeOut(this.inicioSpr_09, 1.0f));

		yield return new WaitForSeconds(1.1f);
		
		playBt.gameObject.SetActive(true);
		creditsBt.gameObject.SetActive(true);

		StartCoroutine(spriteFadeIn(this.titleSpr, 2.5f));
		StartCoroutine(spriteFadeIn(this.playBtSpr, 2.5f));
		StartCoroutine(spriteFadeIn(this.creditsBtSpr, 2.5f));
		yield return new WaitForSeconds(3.0f);
		
		finishedIntro = true;
	}

    public void displayCredits() {
        if (!creditsRunning && finishedIntro)
            StartCoroutine(onCredits());
    }

	public IEnumerator onCredits() {
		creditsRunning = true;
		
		didFinish = false;
		StartCoroutine(spriteFadeIn(this.creditsSpr));
        while (!didFinish) {
            yield return null;
        }

        while (!Input.anyKeyDown) {
            yield return null;
        }

        didFinish = false;
		StartCoroutine(spriteFadeOut(this.creditsSpr));
        while (!didFinish) {
            yield return null;
        }
        creditsRunning = false;
    }

    public IEnumerator spriteFadeIn(SpriteRenderer spr, float time = 1.0f) {
		didFinish = false;

        while (spr.color.a != 1f) {
            float a;

			a = spr.color.a + Time.deltaTime / time;
            if (a > 1f)
                a = 1f;

			spr.color = new Color(1f, 1f, 1f, a);

			if (this.forceExit) {
				spr.color = new Color(1f, 1f, 1f, 1f);
				break;
			}

            yield return null;
        }

        didFinish = true;
    }

    public IEnumerator spriteFadeOut(SpriteRenderer spr, float time = 1.0f) {
		didFinish = false;

		while (spr.color.a != 0f) {
            float a;

			a = spr.color.a - Time.deltaTime / time;
            if (a < 0f)
                a = 0f;

			spr.color = new Color(1f, 1f, 1f, a);

			if (this.forceExit) {
				spr.color = new Color(1f, 1f, 1f, 0f);
				break;
			}

            yield return null;
        }

        didFinish = true;
    }

}
