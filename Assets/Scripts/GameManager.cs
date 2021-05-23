using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public enum GameStates
{
    playerPlaying,
    GameEnd,
    GamePause,
    ConditionChecking
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    private UIView uIView;
    [SerializeField]
    private LineRenderer lineRenderer;

    private PlayerData _currPlayer;
    private PlayerData CurrPlayer
    {
        get
        {
            return _currPlayer;
        }
        set
        {
            _currPlayer = value;
        }
    }

    [SerializeField]
    private List<PlayerData> _players = new List<PlayerData>();
    public List<PlayerData> Players
    {
        get
        {
            return _players;
        }
    }


    private List<Image> _slots=new List<Image>();
    public List<Image> Slots
    {
        get
        {
            return _slots;
        }
        set
        {
            _slots = value;
        }
    }

    private GameStates _currState;
    private GameStates CurrState
    {
        get
        {
            return _currState;
        }
        set
        {

            _currState = value;

        }
    }

    private int validGameChances = 3;

    private int currPlayerInd = 0;
    private int selectedSlotNo = -1;
    List<int> winningSlots = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        Init();
    }

    private void Init()
    {
        foreach (PlayerData player in Players)
        {
            player.Score = 0;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.gameObject.SetActive(false);
    }


    public void ChangeState(GameStates changeStateTo, int PlayerInd,List<int> winningSlots=null)
    {
        CurrState = changeStateTo;
        currPlayerInd = PlayerInd;
        PlayerData player = PlayerInd == -1 ? null : Players[PlayerInd];

        if (player == null)
        {
            Debug.Log("Null player at ind "+ currPlayerInd);
            
        }
        Debug.Log("entering state " + changeStateTo);
        
        switch (changeStateTo)
        {
            case GameStates.playerPlaying:
                StartCoroutine(PlayTurn(player));
                break;

            case GameStates.ConditionChecking:
                selectedSlotNo = -1;
                CheckConditions(player);
                break;

            case GameStates.GameEnd:
                StartCoroutine(EndGame(player,winningSlots));
                break;

            case GameStates.GamePause:
                PauseGame();
                break;
        }
    }

     IEnumerator PlayTurn(PlayerData _player)
    {
        if (CurrState != GameStates.playerPlaying)
            yield break;


        Debug.Log(_player.PlayerName);

        uIView.ChangePlayerTurn(currPlayerInd);
        //is it CPU
        if (_player.PlayerName == Players[0].PlayerName)
        {
           
            yield return new WaitForSeconds(2f);
            int randomValue = Random.Range(0, 10);
          //  if (randomValue % 3 != 0)
            {
                int bestScore = -1;
                //go for minimax algo,return ind
                for (int i = 0; i < Slots.Count; i++)
                {
                    //if slot empty
                    if (!Slots[i].enabled)
                    {
                        Slots[i].enabled = true;
                        Slots[i].sprite = _player.TurnSprite;
                        int score = MiniMax(Players[1], Slots, -1000, 1000);
                        Slots[i].enabled = false;
                        if (score > bestScore)
                        {
                            selectedSlotNo = i;
                            bestScore = score;
                        }
                    }
                }
                Debug.Log("selected slot " + selectedSlotNo + " fromminimax algo");
            }
            //else
            //{
            //    //randomly put value wherever slot is available;
            //    for(int i = 0;i<Slots.Count;i++)
            //    {
            //        if(Slots[i].sprite==null)
            //        {
            //            selectedSlotNo = i;
            //            break;
            //        }
            //    }
            //}

         
        }

        else
        {
            
            //if that slot is already fill
            if (selectedSlotNo!=-1 && Slots[selectedSlotNo].enabled)
            {
                selectedSlotNo = -1;
                Debug.Log(selectedSlotNo + " slot already full");
            }
          
            
            
        }

        if (selectedSlotNo != -1)
        {
            Debug.Log(" chaning slot " + selectedSlotNo + " to player's prite" + _player.PlayingOptions);
            Slots[selectedSlotNo].enabled = true;
            Slots[selectedSlotNo].sprite = _player.TurnSprite;
            Slots[selectedSlotNo].transform.parent.GetComponent<Button>().interactable = false;
            selectedSlotNo = -1;
           ChangeState(GameStates.ConditionChecking, currPlayerInd);

        }
    }

    int MiniMax(PlayerData player,List<Image> board, int alpha,int beta)
    {
        if (IsTie())
            return 0;

        if(HasWon(ref winningSlots))
        {
            //Debug.Log("Winning slots count " + winningSlots.Count);
            if(Slots[winningSlots[0]].sprite==Players[0].TurnSprite)
            {
                //CPU won
                return 1;
            }
            else
            {
                //player won
                return -1;
            }
        }

        int score;
        //if CPU
        if (Players[0] == player)
        {
            for(int i = 0; i < 9; i ++)
            {
                if (!board[i].enabled)
                {
                    board[i].enabled = true;
                    board[i].sprite = player.TurnSprite;
                    score = MiniMax(Players[1], board, alpha, beta);
                    board[i].enabled = false;

                    if (score > alpha)
                        alpha = score;

                    if (alpha > beta)
                        break;
                }
            }
            return alpha;
        }
        else
        {
             //if player
            for (int i = 0; i < 9; i++)
            {
                if (!board[i].enabled)
                {
                    board[i].enabled = true;
                    board[i].sprite = player.TurnSprite;
                    score = MiniMax(Players[0], board, alpha, beta);
                    board[i].enabled = false;

                    if (score < beta)
                        beta = score;

                    if (alpha > beta)
                        break;
                }
            }
            return beta;
        }
    }
    
     void CheckConditions(PlayerData player)
    {
        

        if (HasWon(ref winningSlots))
        {
            ChangeState(GameStates.GameEnd, currPlayerInd, winningSlots);
            return;
        }
        //if all slots are full
        if (IsTie())
        {
            ChangeState(GameStates.GameEnd, NextPlayer());
            return;
        }

        ChangeState(GameStates.playerPlaying, NextPlayer());

        

    }

    bool HasWon( ref List<int> _winningSlots)
    {

        //checl all the three options should be equal and not null
        //row 1
        if (CheckIfEqual(Slots[0], Slots[1], Slots[2]))
        {
            
           // print("1");
            if(_winningSlots!=null)
            _winningSlots=new List<int> { 0, 1, 2 };
            return true;

        }
        //row 2
         if (CheckIfEqual(Slots[3], Slots[4], Slots[5]))
        {
            //print("2");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 3, 4, 5 };
            return true;
        }
        //row 3
         if (CheckIfEqual(Slots[6], Slots[7], Slots[8]))
        {
           // print("3");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 6, 7, 8 };
            return true;
        }
        //col 1
         if (CheckIfEqual(Slots[0], Slots[3], Slots[6]))
        {
           
           // print("4");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 0, 3, 6 };
            return true;
        }
        //col 2
         if (CheckIfEqual(Slots[1], Slots[4], Slots[7]))
        {
            //print("5");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 1, 4, 7 };
            return true;
        }
        //col 3
         if (CheckIfEqual(Slots[2], Slots[5], Slots[8]))
        {
            //print("6");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 2, 5, 8 };
            return true;
        }
        //forward diag
         if (CheckIfEqual(Slots[0], Slots[4], Slots[8]))
        {
           // print("7");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 0, 4, 8 };
            return true;
        }
        //backward diag
         if (CheckIfEqual(Slots[2], Slots[4], Slots[6]))
        {
           // print("8");
            if (_winningSlots != null)
                _winningSlots = new List<int> { 2, 4, 6 };
            return true;
        }

        
        return false;

       
    }

    bool IsTie()
    {
        if (Slots[0].enabled && Slots[1].enabled && Slots[2].enabled &&
                 Slots[3].enabled && Slots[4].enabled && Slots[5].enabled &&
                 Slots[6].enabled && Slots[7].enabled && Slots[8].enabled)
        {

            return true;
        }
        else return false;
    }
    bool CheckIfEqual(Image one, Image two,Image three)
    {
        if (one.enabled && two.enabled && three.enabled && one.sprite == two.sprite  && two.sprite == three.sprite)
        {
            return true;
        }
        else return false;
    }

     IEnumerator EndGame(PlayerData winner,List<int> winningSlots)
    {
        if(winningSlots!=null)
        Debug.Log("winner is " + winner.PlayerName);
        validGameChances--;
       
        if (winningSlots!=null)
        {
            winner.Score += 1f;
            Debug.Log("winner score " + winner.Score);
            yield return new WaitForSeconds(1.5f);

            lineRenderer.gameObject.SetActive(true);
            lineRenderer.SetPosition(0, Slots[winningSlots[0]].transform.position -new Vector3(0f,0f,10f));
            lineRenderer.SetPosition(1, Slots[winningSlots[2]].transform.position - new Vector3(0f, 0f, 10f));

            //foreach(int i in winningSlots)
            //{
            //    Slots[i].transform.parent.GetChild(1).GetComponent<Image>().enabled = true;
            //}
            uIView.UpdateScore(winner);

        }

        else
        {
            foreach (PlayerData player in Players)
            {
                player.Score += 0.5f;
                Debug.Log("player score " + player.Score);
                //update score on UI
                uIView.UpdateScore(player);
            }

        }
        Debug.Log("GAMES LEFT" + validGameChances);
        uIView.UpdateGamesLeft(validGameChances);
        if (validGameChances == 0)
        {
            float maxScore = 0;
            yield return new WaitForSeconds(2f);
            PlayerData winningPlayer=null;
            foreach (PlayerData player in Players)
            {
                maxScore = Mathf.Max(player.Score, maxScore);
                if (maxScore == player.Score)
                {
                    winningPlayer = player;
                }
            }
            lineRenderer.gameObject.SetActive(false);
            uIView.GameEnded(winningPlayer.PlayerName);
        }
        
          else  Invoke(nameof(ResetGame), 2f);

        
    }

    public void PauseGame()
    {

    }

    public void OnSlotClicked(int slotNo)
    {
        selectedSlotNo = slotNo;
        Debug.Log("selected slot " + slotNo + " total slots:" + Slots.Count);
       

        Debug.Log(CurrState);
        if (CurrState == GameStates.playerPlaying && currPlayerInd!=0)
        {
            ChangeState(GameStates.playerPlaying, currPlayerInd);
        }
       
    }

    int NextPlayer() {
        Debug.Log("next player called");
        currPlayerInd = (currPlayerInd+1) %(Players.Count);
        CurrPlayer = Players[currPlayerInd];
        Debug.Log("changed player to: " + CurrPlayer.PlayerName);
        return currPlayerInd;
        
    }

    void ResetGame()
    {
        selectedSlotNo = -1;
        foreach(Image slot in Slots)
        {
            slot.sprite = null;
            slot.enabled = false;
            slot.transform.parent.GetComponent<Button>().interactable = true;
        }
       
        lineRenderer.gameObject.SetActive(false);
        ChangeState(GameStates.playerPlaying, currPlayerInd);


    }

    IEnumerator WaitFor(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
      
        if(callback!=null)
        {
            callback.Invoke();
        }
    }

    
}
