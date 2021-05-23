using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChanceDecider : MonoBehaviour
{
    [SerializeField]
    private GameObject playerNamePrefab;
    [SerializeField]
    private Transform spinningScrollRectContent;
    [SerializeField]
    private GameEventListener OnChanceDecided;


    private List<PlayerData> players= new List<PlayerData>();
    private Queue<KeyValuePair<int,GameObject>> playerPrefabs = new Queue<KeyValuePair<int, GameObject>>();
    

    // Start is called before the first frame update
    void Start()
    {
        players = GameManager.instance.Players;
        InitializeSpinner();
        Debug.Log("players count " + GameManager.instance.Players.Count);
        
    }

    void InitializeSpinner()
    {
        for(int i =0;i<players.Count;i++)
        {
            PlayerData player = players[i];
            //instantiate playernameprefab of each player
            GameObject prefab = Instantiate(playerNamePrefab);

            //initialize it with player's name
            prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.PlayerName;
            prefab.name = player.PlayerName;

            //set parent and anchor position as scrollrect's content
            prefab.GetComponent<RectTransform>().SetParent(spinningScrollRectContent);
            //prefab.GetComponent<RectTransform>().position = Vector3.zero;
            prefab.GetComponent<RectTransform>().anchoredPosition = spinningScrollRectContent.GetComponent<RectTransform>().anchoredPosition;
            prefab.transform.localScale = Vector3.one;
            playerPrefabs.Enqueue(new KeyValuePair<int, GameObject>(i,prefab));
        }
    }

    public void Spin(Button btn)
    {
        btn.interactable = false;
        StartCoroutine(StartSpinning(spinningScrollRectContent));
    }

    IEnumerator StartSpinning(Transform parent)
    {
        
        //chose random float between 3f to 4f
        int spinningTime = Random.Range(10,20);
        
        //start while unitll time is zero
        while (spinningTime > 0)
        {
            //deque a player
            KeyValuePair <int,GameObject> player = playerPrefabs.Dequeue();

            //set it's sibling index as last
            player.Value.transform.SetAsLastSibling();

            //wait for 0.1f
            yield return new WaitForSeconds(0.1f);
            spinningTime -= 1;

            //enque it again
            playerPrefabs.Enqueue(player);
            
        }
        Debug.Log("spinning time " + spinningTime);


        yield return new WaitForSecondsRealtime(2f);
        OnChanceDecided.OnEventRaised();
        playerPrefabs.Dequeue();
        KeyValuePair<int, GameObject> _player = playerPrefabs.Dequeue();
        Debug.Log("First chance is " + _player.Value.name);
        GameManager.instance.ChangeState(GameStates.playerPlaying,_player.Key);
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
