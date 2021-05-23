using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private Transform gridParent;
    [SerializeField]
    private TextMeshProUGUI CPUScore;
    [SerializeField]
    private TextMeshProUGUI PlayerScore;
    [SerializeField]
    private GameObject endGamePanel;
    [SerializeField]
    private TextMeshProUGUI playerWinText;
    [SerializeField]
    private TextMeshProUGUI playerLabel;
    [SerializeField]
    private TextMeshProUGUI CPULabel;
    [SerializeField]
    public TextMeshProUGUI validGamesLeft;
    [SerializeField]
    private GameObject winEffect_PS;




    private List<Image> slots = new List<Image>();
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        for(int i=0;i<gridParent.childCount;i++)
        {
            int ind = i;
            Transform child = gridParent.GetChild(i);
            child.GetComponent<Button>().onClick.AddListener(()=> { gameManager.OnSlotClicked(ind); });
            //Debug.Log(child.name + " " + i);
            slots.Add(child.GetChild(0).GetComponent<Image>());
        }
        gameManager.Slots = slots;
        CPULabel.text = gameManager.Players[0].PlayerName;
        playerLabel.text = gameManager.Players[1].PlayerName;
    }

    public void UpdateScore(PlayerData player)
    {
        if (gameManager.Players[1] == player)
        {
            GameObject scoreUpdateEffect = Instantiate(winEffect_PS, PlayerScore.transform);
            scoreUpdateEffect.GetComponent<ParticleSystem>().Play();
            scoreUpdateEffect.transform.localPosition = Vector3.zero;
           
           // Destroy(scoreUpdateEffect, 10f);


            PlayerScore.text = player.Score.ToString();
        }
        else if (gameManager.Players[0] == player)
        {
            GameObject scoreUpdateEffect =Instantiate(winEffect_PS,CPUScore.transform);
            scoreUpdateEffect.GetComponent<ParticleSystem>().Play();
            scoreUpdateEffect.transform.localPosition = Vector3.zero;
            //Destroy(scoreUpdateEffect, 10f);
            CPUScore.text = player.Score.ToString();
        }
    }
  

    public void GameEnded(string WinnerName)
    {
        endGamePanel.SetActive(true);
        if (WinnerName != null)
        {
            playerWinText.text = "Wuhooo !! \n " + WinnerName + " won the game !!";
        }
        else
        {
            playerWinText.text = "Oh,no ! \n It's a tie, let's try again !!";
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangePlayerTurn(int playerInd)
    {
        if(playerInd==0)
        {
            CPULabel.transform.parent.GetComponent<Image>().color = Color.green;
            playerLabel.transform.parent.GetComponent<Image>().color = Color.white;

        }

        if (playerInd==1)
        {
            playerLabel.transform.parent.GetComponent<Image>().color = Color.green;
            CPULabel.transform.parent.GetComponent<Image>().color = Color.white;
        }


    }

    public void UpdateGamesLeft(int val)
    {
        validGamesLeft.text = "Valid Games left: " + val;
    }

}
