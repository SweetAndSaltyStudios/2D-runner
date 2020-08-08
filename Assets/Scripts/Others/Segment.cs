using UnityEngine;

public class Segment : MonoBehaviour
{
    private Transform[] obstacleContainers;
    private Transform[] collectables;

    private const float MAX_Y = 3;
    private const float MIN_Y = -2.7f;

    private readonly float segmentWidth = 27.3f;

    private void Awake()
    {
        var obstacles = transform.Find("Obstacles");
        var childCount = obstacles.childCount;

        obstacleContainers = new Transform[childCount];
        collectables = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            obstacleContainers[i] = obstacles.GetChild(i);

            // Child "2" must be a collectable
            collectables[i] = obstacleContainers[i].transform.GetChild(2);
        }
    }

    public void Move(Vector2 position, bool isActive = true, int obstacles = 7)
    {
        transform.position = position + Vector2.right * segmentWidth;

        ReplaceObstacles(isActive, obstacles);
    }

    private void ReplaceObstacles(bool isActive = true, int obstacles = 7)
    {
        var index = 0;

        for (int i = 0; i < obstacleContainers.Length; i++)
        {
            if (index < obstacles)
            {
                obstacleContainers[i].gameObject.SetActive(isActive);
            }

            obstacleContainers[i].position = new Vector2(obstacleContainers[i].position.x, Random.Range(MIN_Y, MAX_Y));
            collectables[i].gameObject.SetActive(true);

            index++;
        }
    }
}
