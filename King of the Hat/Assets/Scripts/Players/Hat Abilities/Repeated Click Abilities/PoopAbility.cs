using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopAbility : RepeatedClickAbilities {

    public GameObject prefab;

    public int maxPoops = 1;
    GameObject[] poops;

    public override void Start() {
        base.Start();

        CreatePoops();
    }

    public override void OnNextClick() {
        base.OnNextClick();

        if (!hat.isBeingThrown && onlyIfDangerous) return;

        if (currentClickCount <= numberOfExtraClicksAllowed)
            Poop();
    }

    int poopCounter = 0;
    public void Poop() {
    
        for(int i = 0; i < poops.Length; i++) {

            if (!poops[i].activeSelf) {

                poops[i].transform.position = transform.position + Vector3.up * 0.5f + (Vector3) hat.velocity.normalized * 2;

                poops[i].SetActive(true);
                break;
            } else {
                poopCounter++;
            }

        }    

        for(int i = 0; i < poops.Length; i++) {
            
            if(i == poopCounter % poops.Length) {
                poops[i].transform.position = transform.position + Vector3.up * 0.5f + (Vector3)hat.velocity.normalized * 2;
                break;
            }

        }

    }

    void CreatePoops() {

        poops = new GameObject[maxPoops];

        for(int i = 0; i < maxPoops; i++) {

            GameObject newPoop = Instantiate(prefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            newPoop.SetActive(false);
            newPoop.transform.SetParent(GameController.instance.game.level.transform);

            poops[i] = newPoop;
        }
        
    }

}
