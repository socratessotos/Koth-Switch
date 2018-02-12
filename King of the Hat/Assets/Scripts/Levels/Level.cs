using System.Collections;
using UnityEngine;

//Base class for level
public class Level : MonoBehaviour {

    public enum LevelType { GRASS, LAVA, SNOW, VOID, FACTORY, SKY, SWAMP }

    public LevelType type;

    public Transform[] startingSpawnPoints;

	[HideInInspector]
	public SmashCam smashCam;
	public float minX = -37, maxX = 37, minY = -21, maxY = 21;
	public bool isFixed = false;
    public float borderWidth = 13;
    public float minSize = 7;
	public float maxSize = 27;

	public GameObject[] respawnableObjects;

    public void Awake() {
        smashCam = Camera.main.GetComponent<SmashCam>();

    }

    void Start() {
    }

    public void CreateCamera() {
        
        for(int i = 0; i < Camera.main.transform.childCount; i++) {
            DestroyImmediate(Camera.main.transform.GetChild(i).gameObject);
        }

        GameObject BG;

        switch (type) {
            case LevelType.GRASS:
                BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Cam"), Vector3.zero, Quaternion.identity);
                GameObject pollen = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Pollen Effect"), Vector3.zero, Quaternion.identity);
                pollen.transform.SetParent(Camera.main.transform, false);
                break;
            case LevelType.SNOW:
                BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Cam_Ice"), Vector3.zero, Quaternion.identity);
                GameObject snow = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Snow Effect"), Vector3.zero, Quaternion.identity);
                snow.transform.SetParent(Camera.main.transform, false);
                break;
            case LevelType.LAVA:
                BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Castle"), Vector3.zero, Quaternion.identity);
                break;
			case LevelType.VOID:
				BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Void"), Vector3.zero, Quaternion.identity);
				break;
			case LevelType.FACTORY:
				BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Storage"), Vector3.zero, Quaternion.identity);
				break;
			case LevelType.SKY:
				BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Sky"), Vector3.zero, Quaternion.identity);
				GameObject wind = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Dust Effect"), Vector3.zero, Quaternion.identity);
				wind.transform.SetParent(Camera.main.transform, false);
				break;
			case LevelType.SWAMP:
				BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Swamp"), Vector3.zero, Quaternion.identity);
				break;
            default:
                //print("error with level type; setting to BG_Cam");
                BG = MonoBehaviour.Instantiate(Resources.Load<GameObject>("BG_Cam"), Vector3.zero, Quaternion.identity);
                break;
        }

        BG.transform.parent = transform;
        Camera.main.clearFlags = CameraClearFlags.Depth;

		UpdateSmashCamBounds ();
    }

    public void UpdateSmashCamBounds () {

		smashCam.minX = this.minX;
		smashCam.minY = this.minY;
		smashCam.maxX = this.maxX;
		smashCam.maxY = this.maxY;
		smashCam.isFixed = this.isFixed;
        smashCam.borderWidth = this.borderWidth;
		smashCam.maxSize = this.maxSize;
        smashCam.minSize = this.minSize;

	}

	public void RespawnObjects () {

		for (int i = 0; i < respawnableObjects.Length; i++) {
            
            Respawnable canRespawn = respawnableObjects[i].GetComponent<Respawnable>();

            if(canRespawn) {
                canRespawn.Reset();
            }

		}

	}

}
