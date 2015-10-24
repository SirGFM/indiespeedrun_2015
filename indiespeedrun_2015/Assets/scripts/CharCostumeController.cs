using UnityEngine;
using System.Collections;

public class CharCostumeController : MonoBehaviour {

	public int level;

	public Sprite[] corpos;
	public Sprite[] biceps;
	public Sprite[] antebraco;

	// Use this for initialization
	void Start () {
		ChangeLevel(level);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeLevel(int level){
		foreach(SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()){
			if(s.gameObject.name == "sprCorpo")
				s.sprite = corpos[level];
			if(s.gameObject.name == "sprBiceps")
				s.sprite = biceps[level];
			if(s.gameObject.name == "sprAntebraco")
				s.sprite = antebraco[level];
		}
	}
}
