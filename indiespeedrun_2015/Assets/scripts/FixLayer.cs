﻿using UnityEngine;
using System.Collections;

public class FixLayer : MonoBehaviour {

    public SpriteRenderer head, neck = null, rfarm, rarm, lfarm, larm, body,
            rfoot, rleg, rcalf, lfoot, lleg, lcalf;
    
    public void fixLayer(PersonBrain self) {
        int init;    

        switch (self.type) {
            case PersonBrain.enType.boss:     init = 10; break;
            case PersonBrain.enType.follower: init = 20; break;
            case PersonBrain.enType.level_0:  init = 30; break;
            case PersonBrain.enType.level_1:  init = 40; break;
            case PersonBrain.enType.level_2:  init = 50; break;
            default: init = -20; break;
        }

        this.head.sortingOrder = init - 2;
        if (this.neck != null) {
            // TODO Check the neck's layer
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

    public void moveLeft() {
        this.transform.localScale = new Vector3(0.8f, 0.8f);
    }

    public void moveRight() {
        this.transform.localScale = new Vector3(-0.8f, 0.8f);
    }
}
