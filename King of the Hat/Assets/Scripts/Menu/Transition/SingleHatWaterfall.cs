using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleHatWaterfall : MonoBehaviour {

    float rotationSpeed;
    float moveSpeed;
    Vector2 direction;

    public void Init(Vector2 _direction, float _moveSpeed, float _rotationSpeed, Sprite _sprite) {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-360, 360)));
        
        direction = _direction;
        moveSpeed = _moveSpeed;
        rotationSpeed = _rotationSpeed;

        GetComponent<Image>().sprite = _sprite;
        GetComponent<Image>().SetNativeSize();
    }

    void Update () {

        transform.Translate(moveSpeed * direction * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(0, 0, (-rotationSpeed * Time.deltaTime)));

        if (transform.position.x > Screen.width + 20)
            Destroy(gameObject);

	}

}
