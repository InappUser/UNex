using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScore : MonoBehaviour {
	public static int enemieStaticsTotal = 1;//this gets its real value from the spawnign class, wherein it is incremented whenever a static is spawned
	public Text enemiesText;

	private GameManager gm;
	private GameObject player1Pnl;
	private int thisStaticsKilled = 0;
	private int thisAlivesKilled = 0;
	private decimal playerScore = 0;
	private bool playerNameSet=false;

	private float timerSec=1f;
	private short timerMin=0;
	
	void Awake () {
		gm = GetComponent<GameManager> ();
		player1Pnl = GameObject.Find("PnlSBPlayer1");

	}

	void FixedUpdate () {
		enemiesText.text = "Enemies Left: " + (PlayerScore.enemieStaticsTotal-GameManager.enemyStaticsDead);//GameManagers enemycount has the current amount of enemies in the game
		SetScorboardVals ();
		KeepTime ();
	}
	void SetScorboardVals(){
		playerScore = thisStaticsKilled - (thisAlivesKilled/2);//using playerscore as a temporary var for part of the equation - when player kills alives this should be reflected negatively in score
		//Debug.Log("player score is "+playerScore+" which is statics killed:"+thisStaticsKilled+" plus alives");
		playerScore = decimal.Divide (playerScore, (int)((timerMin*60)+timerSec))*100;//using decimal divide to that decimal places are included in the result
		//Debug.Log ("time "+(int)((timerMin*60)+timerSec)*100);
		player1Pnl.transform.FindChild ("TxtPlayerScore1").GetComponent<Text> ().text = playerScore.ToString("0.##");//player's kills divided byy amount of time, multiplied by 100
		player1Pnl.transform.FindChild ("TxtPlayerEkills1").GetComponent<Text> ().text = thisStaticsKilled.ToString();
		if(!playerNameSet || enemieStaticsTotal<1){//ensuring that the name is only set once and that the player has spawned. enemies total is used in order to ensure the latter: if enemies have spawned, then the player has spawned
			player1Pnl.transform.FindChild ("TxtPlayerName1").GetComponent<Text> ().text = PhotonNetwork.player.name;
			playerNameSet = true;
		}
	}

	void KeepTime()
	{
		if(enemieStaticsTotal >0){//ensuring that the timer stops once enemies have been killed
			timerSec += Time.deltaTime;
			if(timerSec>=60f){
				timerSec = 0f;
				timerMin++;
			}
			player1Pnl.transform.FindChild ("TxtPlayerTime1").GetComponent<Text> ().text = timerMin.ToString ("00") + ":" + timerSec.ToString ("00");
		}
		
	}
	public void AddStaticsKiled(){
		thisStaticsKilled++;
	}
	public void AddAlivesKiled(){
		thisAlivesKilled++;
	}
}
