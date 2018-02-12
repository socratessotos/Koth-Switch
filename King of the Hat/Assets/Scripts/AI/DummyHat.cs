using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHat : Hat {

    public override void CreateFirePrefab() {

        GameObject f = Instantiate(firePrefab, transform.position, Quaternion.identity) as GameObject;
        f.transform.parent = transform;
        f.transform.position = transform.position + Vector3.up * 0.5f;
        f.transform.localScale = Vector3.one;

        f.layer = LayerMask.NameToLayer("Enemy");

        f.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_OutlineColor", EnemyColorManager.instance.enemies[0].enemyColor);

        dangerFire = f.transform.GetChild(0).GetComponent<ParticleSystem>();
        dangerFireFront = f.transform.GetChild(1).GetComponent<ParticleSystem>();

        var col = dangerFire.colorOverLifetime;
        //var colFront = dangerFireFront.colorOverLifetime;

        Gradient grad = EnemyColorManager.instance.enemies[0].hatFireColor;
        //Gradient gradFront = GameController.instance.playerInputs[owner.playerIndex].playerColorFront;

        col.color = grad;
        //colFront.color = gradFront;
    }

    public override void ToggleHatGlow(bool _isDangerous) {
        if (spriteRenderer == null) return;

        Color32 _hatGlowColor = EnemyColorManager.instance.enemies[0].hatGlowColor;
        Color32 _hatColor = (_isDangerous) ? _hatGlowColor : new Color32(0, 0, 0, 0);

        spriteRenderer.material.SetColor("_OverrideColor", _hatColor);

    }

    public override void Throw(Vector2 directionalInput, float chargeTime) {
        
        base.Throw(directionalInput, chargeTime);

        owner.GetComponent<Dummy>().isWearingHat = false;
        owner.GetComponent<Dummy>().DetermineNewThrowTime();

        owner.GetComponent<Dummy>().SetStateToGetHatBack();

    }

    public override void LandOnHead() {

        base.LandOnHead();

        owner.GetComponent<Dummy>().isWearingHat = true;
        owner.GetComponent<Dummy>().DetermineNewThrowTime();

    }

}