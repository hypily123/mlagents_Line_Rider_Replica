using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GameObject player;
    public GameObject goal;
    private static float goal_x;
    private static float goal_y;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 playerPosition = player.GetComponent<Transform>().position;
        float player_x = playerPosition.x;
        float player_y = playerPosition.y;
        goal_x = Random.Range(player_x + 5, player_x + 25);
        goal_y = Random.Range(player_y - 2, player_y - 12);
        Vector3 goalPosition = new Vector3(goal_x, goal_y, 0);
        Quaternion goalRotation = Quaternion.identity;
        Instantiate(goal, goalPosition, goalRotation); 
    }

    public static float getGoal_x()
    {
        return goal_x;
    }
    public static float getGoal_y()
    {
        return goal_y;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
