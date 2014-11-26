using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScore : MonoBehaviour {
	public static int EnemiesLeft;
	Text text;

	void Awake () {
		text = GetComponent<Text> ();
	}

	void Start()
	{

	}
	
	// Update is called once per frame
	void Update () {
		EnemiesLeft= GameObject.FindGameObjectsWithTag ("Enemy").Length;
		text.text = "Enemies Left: " + EnemiesLeft;
	}
}
