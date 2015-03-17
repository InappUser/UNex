using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScore : MonoBehaviour {
	public static int enemiesTotal = 0;//this gets its real value from the spawnign class, wherein it is incremented whenever a static is spawned
	public Text enemiesText;

	private GameManager gm;
	private GameObject player1Pnl;
	private int thisStaticsKilled = 0;
	private int thisAlivesKilled = 0;
	private decimal playerScore = 0;
	private bool playerNameSet=false;

	void Awake () {
		gm = GetComponent<GameManager> ();
		player1Pnl = GameObject.Find("PnlSBPlayer1");

	}

	void FixedUpdate () {
		enemiesText.text = "Enemies Left: " + (PlayerScore.enemiesTotal-GameManager.enemyStaticsDead);//GameManagers enemycount has the current amount of enemies in the game
		SetScorboardVals ();
	}
	void SetScorboardVals(){
		playerScore = thisStaticsKilled - (thisAlivesKilled/2);//using playerscore as a temporary var for part of the equation - when player kills alives this should be reflected negatively in score
		playerScore = decimal.Divide (playerScore, gm.returnPlayerTime ())*100;//using decimal divide to that decimal places are included in the result
		player1Pnl.transform.FindChild ("TxtPlayerScore1").GetComponent<Text> ().text = playerScore.ToString("0.##");//player's kills divided byy amount of time, multiplied by 100
		player1Pnl.transform.FindChild ("TxtPlayerEkills1").GetComponent<Text> ().text = thisStaticsKilled.ToString();
		if(!playerNameSet || enemiesTotal<1){//ensuring that the name is only set once and that the player has spawned. enemies total is used in order to ensure the latter: if enemies have spawned, then the player has spawned
			player1Pnl.transform.FindChild ("TxtPlayerName1").GetComponent<Text> ().text = PhotonNetwork.player.name;
			playerNameSet = true;
		}
	}

	public void AddStaticsKiled(){
		thisStaticsKilled++;
	}
	public void AddAlivesKiled(){
		thisAlivesKilled++;
	}
}
