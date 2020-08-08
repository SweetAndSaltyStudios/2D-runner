using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GAME_STATE 
{
    START,
    RUN,
    END
}

public class GameManager : Singelton<GameManager>
{ 
    public GameObject BirdHeroPrefab, SegmentPrefab;

    private Segment firstSegment, secondSegment;

    public GAME_STATE CurrentGameState 
    {
        get;
        private set;
    }

    public bool IsLevelCreated 
    {
        get;
        private set;
    }

    public bool NeedToMoveSegments
    {
        get
        {
            return CameraEngine.Instance.WorldRightEdgePosition >= secondSegment.transform.position.x;
        }
    }

    public int CurrentSceneIndex
    {
        get
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void Start() 
    {
        ChangeGameState(GAME_STATE.START);
    }

    public void ChangeGameState(GAME_STATE newGameState) 
    {
        CurrentGameState = newGameState;

        switch (CurrentGameState) 
        {
            case GAME_STATE.START:

                GameMaster.Instance.LoadScore();

                StartGame();

                break;

            case GAME_STATE.RUN:

                RunGame();

                break;

            case GAME_STATE.END:

                EndGame();

                break;

            default:

                break;
        }
    }

    private GameObject CreatePrefabInstance(GameObject prefab, Vector2 position = new Vector2())
    {
        var newInsance = Instantiate(prefab, position, Quaternion.identity);
        newInsance.name = prefab.name;
        return newInsance;
    }

    public bool CreateLevel() 
    {
        firstSegment = CreatePrefabInstance(SegmentPrefab).GetComponent<Segment>();
        firstSegment.Move(Vector2.zero, false, 2);

        secondSegment = CreatePrefabInstance(SegmentPrefab).GetComponent<Segment>();
        secondSegment.Move(new Vector2(27.3f, 0));

        CreatePrefabInstance(BirdHeroPrefab, new Vector2(CameraEngine.Instance.transform.position.x - 4, 0));

        return IsLevelCreated = true;
    }

    private void StartGame()
    {
        if (IsLevelCreated == false)
            StartCoroutine(IStart());
    }

    private void RunGame() 
    {
        if (IsLevelCreated)
            StartCoroutine(IRun());
    }

    private void EndGame()
    {
        if (IsLevelCreated)
            StartCoroutine(IEnd(CurrentSceneIndex, 2f));
    }

    private IEnumerator IStart() 
   {
        yield return new WaitUntil(() => CreateLevel());

        ChangeGameState(GAME_STATE.RUN);
    }

    private IEnumerator IRun() 
    {
        Segment tempSegment;      

        while (CurrentGameState.Equals(GAME_STATE.RUN))
        {
            if(NeedToMoveSegments)
            {
                firstSegment.Move(secondSegment.transform.position);

                tempSegment = firstSegment;
                firstSegment = secondSegment;
                secondSegment = tempSegment;
            }

            yield return null;
        }
    }

    private IEnumerator IEnd(int sceneIndex, float loadDelay) 
    {
        GameMaster.Instance.SaveScore();

        yield return new WaitForSeconds(loadDelay);

        SceneManager.LoadScene(sceneIndex);
    }
}
