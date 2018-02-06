using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
	[Header("Inspector Values")]
	public bool someBool = true;
	public float someFloat = 1f;
	public int someInt = 30;

	[ReadOnly("debug")]
	public bool debugBool = false;
	[ReadOnly("debug")]
	public float debugFloat = 0f;
	[ReadOnly("debug")]
	public int debugInt = 0;

	private void Update() {
		debugInt++;
		if(debugInt == someInt) {
			debugBool = !debugBool;
			debugInt = 0;
		}

		debugFloat += someFloat * Time.deltaTime;

		if(someBool && Mathf.Sin(debugFloat) > .95) {
			debugInt = 0;
		}
	}
}
