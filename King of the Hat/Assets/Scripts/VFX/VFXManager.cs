using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour {

	public static VFXManager instance;

    private int i;

    public VFXLibrary library;

	// Use this for initialization
	void Awake () {

			instance = this;
		
	}

	public void Update () {

        

    }

    public void EmitAtPosition(string systemName, ParticleSystem.EmitParams emitParams, int numberOfParticles, Vector3 position, int _teamIndex, int _effectIndex, bool retainShape = false, Transform relativeTo = null) {

        ParticleSystem system = library.GetParticleSystemFromName(systemName);

        if (system == null) return;

        emitParams.position = position;
        emitParams.applyShapeToPosition = retainShape;

        if (relativeTo != null) {

            var main = system.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
            main.customSimulationSpace = relativeTo;

        }

        ParticleSystem.ColorOverLifetimeModule c = system.colorOverLifetime;

        if (_teamIndex == -1) {
            c.color = EnemyColorManager.instance.enemies[0].hatFireColor;
        } else {
            switch (_effectIndex) {
                case 0:
                    c.color = PlayerColorManager.instance.players[_teamIndex - 1].hatFireColor;
                    break;
                case 1:
                    c.color = PlayerColorManager.instance.players[_teamIndex - 1].hatLoseLifeColor;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        

        system.Emit(emitParams, numberOfParticles);

    }

    public void EmitAtPosition(ParticleSystem system, int numberOfParticles, Vector3 position, bool retainShape = false, Transform relativeTo = null) {

		if (system == null) return;
		
		// Any parameters we assign in emitParams will override the current system's when we call Emit.
		// Here we will override the position and velocity. All other parameters will use the behavior defined in the Inspector.
		var emitParams = new ParticleSystem.EmitParams();
		emitParams.position = position;
		emitParams.applyShapeToPosition = retainShape;

		if (relativeTo != null) {
			
			var main = system.main;
			main.simulationSpace = ParticleSystemSimulationSpace.Custom;
			main.customSimulationSpace = relativeTo;

		}

        system.Emit(emitParams, numberOfParticles);

    }

    public void EmitAtPosition(ParticleSystem system, ParticleSystem.EmitParams emitParams, int numberOfParticles, Vector3 position, bool retainShape = false) {

        if (system == null) return;

        // Any parameters we assign in emitParams will override the current system's when we call Emit.
        // Here we will override the position and velocity. All other parameters will use the behavior defined in the Inspector.
        emitParams.position = position;
        emitParams.applyShapeToPosition = retainShape;
        system.Emit(emitParams, numberOfParticles);

    }

    public void EmitAtPosition(string systemName, int numberOfParticles, Vector3 position, bool retainShape = false, Transform relativeTo = null) {
		EmitAtPosition(library.GetParticleSystemFromName(systemName), numberOfParticles, position, retainShape, relativeTo);
    }

    public void EmitAtPosition(string systemName, ParticleSystem.EmitParams emitParams, int numberOfParticles, Vector3 position, bool retainShape = false) {
        EmitAtPosition(library.GetParticleSystemFromName(systemName), emitParams, numberOfParticles, position, retainShape);
    }

    public void EmitAtPosition(string systemName, ParticleSystem.EmitParams emitParams, int numberOfParticles, Vector3 position, int _teamIndex,  int _effectIndex, bool retainShape = false) {

        ParticleSystem p = library.GetParticleSystemFromName(systemName);
        ParticleSystem.ColorOverLifetimeModule c = p.colorOverLifetime;

        if(_teamIndex == -1) {
            c.color = EnemyColorManager.instance.enemies[0].hatFireColor;
        } else {
            switch (_effectIndex) {
                case 0:
                    c.color = PlayerColorManager.instance.players[_teamIndex - 1].hatFireColor;
                    break;
                case 1:
                    c.color = PlayerColorManager.instance.players[_teamIndex - 1].hatLoseLifeColor;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }

        

        EmitAtPosition(library.GetParticleSystemFromName(systemName), emitParams, numberOfParticles, position, retainShape);
    
    }

    /*
    public void EmitAtPosition(string systemName, ParticleSystem.EmitParams emitParams, int numberOfParticles, Vector3 position, int _playerIndex, int _effectIndex, bool retainShape = false, Transform relativeTo = null) {

        ParticleSystem p = library.GetParticleSystemFromName(systemName);
        ParticleSystem.ColorOverLifetimeModule c = p.colorOverLifetime;

        switch (_effectIndex) {
            case 0:
                c.color = PlayerColorManager.instance.players[_playerIndex].hatFireColor;
                break;
            case 1:
                c.color = PlayerColorManager.instance.players[_playerIndex].hatLoseLifeColor;
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

        EmitAtPosition(systemName, emitParams, numberOfParticles, position, retainShape, relativeTo);

    }
    */
}