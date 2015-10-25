using UnityEngine;
using System.Collections;

public class EndGameScene : MonoBehaviour {

	public Camera camera;
	public GameObject mainChar;
	public GameObject boss;
	public GameObject[] persons;

	public float charsVel;
	public float camInterpVel = 1;

	public Vector3[] camPos; 

	float timeCount;
	float animInterp;
	float iniInterp;
	float SKIN = 0.001f;


	// Use this for initialization
	void Start () {


		timeCount = 0;

		StartCoroutine("EndCutscene");
	}
	
	void Update(){
		timeCount += Time.deltaTime;
		Debug.Log(timeCount);
	}

	IEnumerator EndCutscene(){
		//Player entra andando com seus minions
		mainChar.GetComponent<Animator>().SetFloat("MovBlend", 1);
		foreach(GameObject p in persons)
			p.GetComponent<Animator>().SetFloat("MovBlend", 1);

		animInterp = 0;
		iniInterp = 3.5f;
		float charWalkMultiplier = 1;
		bool bossDied = false;
		
		while(timeCount <= 17){
			
			//Mover e parar char
			mainChar.GetComponent<Animator>().SetFloat("MovBlend", charWalkMultiplier);
			mainChar.transform.Translate(-charsVel * charWalkMultiplier,0,0);
			if(mainChar.transform.position.x<=2){
				if(charWalkMultiplier > 0){
					charWalkMultiplier*=0.95f;
					if(charWalkMultiplier < SKIN)
						charWalkMultiplier = 0;
				}
			}

			//Mover minions
			foreach(GameObject p in persons)
				p.transform.Translate(-charsVel,0,0);

			//Derrubar boss
			if(!bossDied && timeCount > 10.5f){
				bossDied = true;
				boss.GetComponent<Animator>().SetTrigger("BossFall");
			}else if(timeCount > 12.5f){
				boss.transform.Translate(-charsVel,0,0);				
			}
			

			//Mover camera
			if(timeCount > iniInterp){
				camera.transform.position = Vector3.Lerp(camPos[0], camPos[1], animInterp);
				//Interpolação de Hermite (Easy In Out)
				if(animInterp < 1f - SKIN){
					animInterp = Hermite(0f,1f,(timeCount-iniInterp) * camInterpVel);
				}else
					animInterp = 1;
				
				/*//Interpolação Linear
				if(animInterp < 1){
					animInterp += .01f;
					animInterp *= 1+camInterpVel;
				}else
					animInterp = 1;
				*/
			}
			yield return null; //wait for a frame
		}


		animInterp = 0;
		iniInterp = 19.5f;
		bool charFullWalk = false;
		charWalkMultiplier = SKIN;
		while(true){
			
			//Fazer char andar
			mainChar.GetComponent<Animator>().SetFloat("MovBlend", charWalkMultiplier);
			mainChar.transform.Translate(-charsVel * charWalkMultiplier,0,0);
			if(!charFullWalk){
				charWalkMultiplier/=0.95f;	
				if(charWalkMultiplier >=1){
					charWalkMultiplier=1;
					charFullWalk = true;
				}
			}else if(mainChar.transform.position.x <= -10 && charWalkMultiplier>0){
					charWalkMultiplier*=0.95f;
					if(charWalkMultiplier < SKIN)
						charWalkMultiplier = 0;
			}



			//Mover camera
			if(timeCount > iniInterp){
				camera.transform.position = Vector3.Lerp(camPos[1], camPos[2], animInterp);
				//Interpolação de Hermite (Easy In Out)
				if(animInterp < 1f - SKIN){
					animInterp = Hermite(0f,1f,(timeCount-iniInterp) * camInterpVel);
				}else
					animInterp = 1;
			}
			yield return null; //wait for a frame			
		}



		//CUIDADO: NÃO FAÇA UM WHILE SEM AO MENOS TER UM YIELD DENTRO
		//yield return null; //wait for a frame
    	//yield return new WaitForSeconds(3.14f);
	}





	//Interpolação de Hermite (Easy In Out)
	public float Hermite(float start, float end, float value){
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }
 
}

