using UnityEngine;
using System.Collections;

// FUCK YEAH answers.unity3d.com
// http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html

public class FollowCamera : MonoBehaviour {

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;
    private float maxWidth;

    // Use this for initialization
    void Start() {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (target) {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 5f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
            if (targetPos.x < 4.5) {
                targetPos = new Vector3(4.5f, targetPos.y, targetPos.z);
            }
            if (targetPos.x > maxWidth) {
                targetPos = new Vector3(maxWidth, targetPos.y, targetPos.z);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);

        }
        else {
            LevelManager lvlManager;

            lvlManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            if (lvlManager) {
                target = lvlManager.player.gameObject;
                maxWidth = lvlManager.width;
            }
        }
    }
}
