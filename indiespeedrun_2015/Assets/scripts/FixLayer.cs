using UnityEngine;
using System.Collections;

public class FixLayer : MonoBehaviour {

    public SpriteRenderer head, neck = null, rfarm, rarm, lfarm, larm, body,
            rfoot, rleg, rcalf, lfoot, lleg, lcalf;
    
    public void fixLayer(PersonBrain self, bool isPlayer) {
        int init;    

        switch (self.type) {
            case PersonBrain.enType.boss:     init = 10; break;
            case PersonBrain.enType.follower: init = 20; break;
            case PersonBrain.enType.level_0:  init = 30; break;
            case PersonBrain.enType.level_1:  init = 40; break;
            case PersonBrain.enType.level_2:  init = 50; break;
            default: init = -20; break;
        }

        if (isPlayer) {
            init = 60;
        }

        this.head.sortingOrder = init - 2;
        if (this.neck != null) {
            this.neck.sortingOrder = init - 2;
        }
        this.rfarm.sortingOrder = init + 0;
        this.rarm.sortingOrder  = init + 0;
        this.lfarm.sortingOrder = init - 2;
        this.larm.sortingOrder  = init - 2;
        this.body.sortingOrder  = init - 1;
        this.rfoot.sortingOrder = init - 2;
        this.rleg.sortingOrder  = init - 2;
        this.rcalf.sortingOrder = init - 2;
        this.lfoot.sortingOrder = init - 3;
        this.lleg.sortingOrder  = init - 3;
        this.lcalf.sortingOrder = init - 3;
    }

    public SpriteRenderer getBody() {
        return this.body;
    }

    public void setType(int type) {
        this.GetComponent<CharCostumeController>().ChangeLevel(type);
    }

    public void moveLeft() {
        float scale;

        scale = Mathf.Abs(this.transform.localScale.x);
        this.transform.localScale = new Vector3(scale, scale);
    }

    public void moveRight() {
        float scale;

        scale = Mathf.Abs(this.transform.localScale.x);
        this.transform.localScale = new Vector3(-scale, scale);
    }
}
