using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockHandler : MonoBehaviour {

    public Image[] stockImages;
    public Text stockNumber;

    public Animator anim;

    public void SetStockImages(Sprite _hatImage) {
        for(int i = 0; i < stockImages.Length; i++) {
            stockImages[i].sprite = _hatImage;
        }
    }

    public void UpdateStock(int _hpLeft) {
        
        if(_hpLeft > stockImages.Length) {

            UpdateStockNumber(_hpLeft);

        } else {

            UpdateStockImages(_hpLeft);

        }

        AnimateStockLoss();

    }

    void UpdateStockImages(int _livesLeft) {
        //disable stock number
        stockNumber.enabled = false;

        //enable stock images == to _livesLeft
        for(int i = 0; i < stockImages.Length; i++) {
            
            if(i < _livesLeft) {
                stockImages[i].enabled = true;
            } else {
                stockImages[i].enabled = false;
            }

        }

    }

    void UpdateStockNumber(int _livesLeft) {
        //enable only first stock image and disable all others
        for (int i = 0; i < stockImages.Length; i++) {
            if (i == 0) {
                stockImages[i].enabled = true;
            } else {
                stockImages[i].enabled = false;
            }
        }

        //update number text
        stockNumber.enabled = true;
        stockNumber.text = "x  " + _livesLeft;
    }

    void AnimateStockLoss() {
        anim.SetTrigger("Loss");
    }

}
