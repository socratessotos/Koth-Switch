using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParallaxLayer {

	ONE,
	TWO,
	THREE,
	FOUR,
	FIVE
	
}

public static class ParallaxManager {

	public static float ParallaxLayerToMoveSpeed (ParallaxLayer parallaxLayer) {

		switch (parallaxLayer) {

		case ParallaxLayer.ONE:
			return 5f;

		case ParallaxLayer.TWO:
			return 3.5f;

		case ParallaxLayer.THREE:
			return 2f;

		case ParallaxLayer.FOUR:
			return 1f;

		case ParallaxLayer.FIVE:
			return 0.5f;

			default:
			return 0f;

		}
		
	}

}
