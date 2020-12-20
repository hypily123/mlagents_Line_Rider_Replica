using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    private float original_player_x = -13f;
    private float original_player_y = 7f;

    private float curr_agent_x;
    private float curr_agent_y;

    private bool run_or_not;
    private CircleCollider2D goal_body;
    private CircleCollider2D bike_body;

    public GameObject flag;
    public GameObject player;
    public GameObject linePrefab;
    public Rigidbody2D rb;

    public GameObject goal;
    public int max_step = 130;
    public float current_step = 0.0f;
    public float min_distance = 99f;
    public float total_distance;
    public float timer;
    public float temp_reward;

    Line activeLine;

    void Start()
    {
    }
    void Update ()
    {
        if (run_or_not)
        {
            timer += Time.deltaTime;
        }
    }

    // New learn episode
    public override void OnEpisodeBegin()
    {
        GameObject[] rootGOs = gameObject.scene.GetRootGameObjects();
        for (int i = 0; i < rootGOs.Length; i++)
        {
            GameObject go = rootGOs[i];
            if (go.name.Equals("Flag(Clone)"))
                Destroy(go);
            if (go.name.Equals("Line_Normal(Clone)"))
                Destroy(go);
        }
        float goal_x = Random.Range(original_player_x + 5, original_player_x + 25);
        float goal_y = Random.Range(original_player_y - 2, original_player_y - 12);
        Vector3 goalPosition = new Vector3(goal_x, goal_y, 0);
        Vector3 playerPosition = new Vector3(original_player_x, original_player_y, 0);
        Quaternion rotation = Quaternion.identity;
        // Reset the flag
        goal = Instantiate(flag, goalPosition, rotation);
        // Reset the bike position
        player.transform.position = playerPosition;
        player.transform.rotation = rotation;
        rb = player.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        // Reset the agent position
        this.transform.position = new Vector3(original_player_x - 0.5f, original_player_y - 0.5f, 0);

        curr_agent_x = original_player_x;
        curr_agent_y = original_player_y;

        // Initial the line drawer
        GameObject lineGO = Instantiate(linePrefab);
        activeLine = lineGO.GetComponent<Line>();

        // Run_or_not suggest if the player is running or waiting for drawing
        run_or_not = false;
        current_step = 0.0f;
        timer = 0.0f;
        total_distance = Vector3.Distance(player.transform.position, goal.transform.position);
        min_distance = Vector3.Distance(player.transform.position, goal.transform.position);
        temp_reward = 0;
    }

    // Collect observation results
    public override void CollectObservations(VectorSensor sensor)
    {
        // Record the position of the flag
        sensor.AddObservation(goal.transform.position.x);
        sensor.AddObservation(goal.transform.position.y);
        // Record the position of the agent
        sensor.AddObservation(this.transform.position.x);
        sensor.AddObservation(this.transform.position.y);
    }

    // Give reward to the action
    public override void OnActionReceived(float[] vectorAction)
    {
        /*
        This game has two part:
        1. Draw a line to the flag
        2. Press space and the bike will start to go.
        Check if the bike is able to touch the flag

        run_or_not paramter is to suggest if it is the first part or the second one
        false: draw a line
        true: start to run

        vectorAction is a two-float array
        They will be the start and the end point of a line
        */

        if (!run_or_not)
        {
            Draw(vectorAction);
        }
        else
        {
            Run();
        }
    }

    private void Draw(float[] vectorAction)
    {
        if (activeLine != null)
        {
            // Draws lines as the wish of agent.
            curr_agent_x = curr_agent_x + vectorAction[0];
            curr_agent_y = curr_agent_y + vectorAction[1];
            this.transform.position = new Vector3(curr_agent_x, curr_agent_y, 0);
            Vector2 mousePos = new Vector2(curr_agent_x, curr_agent_y);
            activeLine.UpdateLine(mousePos);
            // Add 1 to total steps.
            current_step += 1f;
        }
        if (current_step >= max_step)
        {
            // If reach the maximum step allowed
            // run the game and set a reward base on the distance of the end of line to the goal, the range of reward is (0,1) * coefficient
            run_or_not = true;
            temp_reward = (1 - Vector3.Distance(goal.transform.position, new Vector3(curr_agent_x, curr_agent_y, 0)) / total_distance) * 0.8f;
        }
        else if (Vector3.Distance(this.transform.position, goal.transform.position) <= 1)
        {
            // If the agent touch the flag, give some extra reward and run game immediately
            //Debug.Log("Gotcha");
            temp_reward = 1f;
            run_or_not = true;
        }
        else if (this.transform.position.x >= 14 || this.transform.position.y <= -7 ||
          this.transform.position.x <= -14 || this.transform.position.y >= 7)
        {
            // If the line goes outside the frame, the episode ends
            //Debug.Log("Failed");
            //Debug.Log(-150 + current_step * 0.1f);
            // Give the agent some reward based on how far it drew before went out of window, the total steps bonus will become to penalty after the agent "knows" do not draw to outside.
            SetReward(-150f + current_step * 0.1f);
            EndEpisode();
        }
    }

    private void Run()
    {
        goal_body = goal.GetComponent<CircleCollider2D>();
        bike_body = player.GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        min_distance = Mathf.Min(min_distance, Vector3.Distance(bike_body.transform.position, goal_body.transform.position));

        // The reward is consisted by 4 parts: basic reward, minimum distance of bike to goal, steps penalty, distance of end of line to the goal.
        if (min_distance <= 1)
        {
            //When bike reachs the goal
            Debug.Log("goal");
            Debug.Log(100f - current_step * 0.4f + temp_reward * 100f);
            /* Reward =
                    1. 100 (base)
                    2. + 0 (we know the bike has reached goal, so this part has already included in base)
                    3. - current_step * 0.4f (steps penalty, it is doubled because we want the agent take more consideration of reducing steps while reaching goals)
                    4. + temp_reward * 100f (distance of end of line to the goal)
             */
            SetReward(100f - current_step * 0.4f + temp_reward * 100f);
            EndEpisode();
        }
        else if(timer >= 3 && Mathf.Abs(rb.velocity.y) < 0.05f && Mathf.Abs(rb.velocity.x) < 0.05f)
        {
            
            //When bike does not reach the goal and stop at somewhere
            //Debug.Log("stuck");
            Debug.Log(0f + (1 - min_distance / total_distance) * 30 - current_step * 0.2f + temp_reward * 100f);
            /* Reward =
                    1. 0 (base)
                    2. + (1 - min_distance / total_distance) * 30f (minimum distance of bike to goal)
                    3. - current_step * 0.2f (steps penalty)
                    4. + temp_reward * 100f (distance of end of line to the goal)
             */
            SetReward(0f + (1 - min_distance / total_distance) * 30 - current_step * 0.2f + temp_reward * 100f);
            EndEpisode();
            
        }
        // when bike goes out of window
        else if (player.transform.position.x < -14 || player.transform.position.y < -8
            || player.transform.position.x > 14)
        {
            //Debug.Log("out");
            Debug.Log(-30f + (1 - min_distance / total_distance) * 60f - current_step * 0.2f + temp_reward * 100f);
            /* Reward =
                    1. -30 (base)
                    2. + (1 - min_distance / total_distance) * 60f (minimum distance of bike to goal)
                    3. - current_step * 0.2f (steps penalty)
                    4. + temp_reward * 100f (distance of end of line to the goal)
                There are two situations of the bike flys out of the window.
                    1. The agents just do not care about it because focuing on connenting the line to the end can result in a better reward.
                    2. The agents does not connect the line to the goal but let the bike do a stunt fly show to the goal
                We definitely want to disencourage the first situation, but gives second situation a fair reward.
                Thus, we reduced the base reward and gives double distance reward of bike to goal.
             */
            SetReward(-30f + (1 - min_distance / total_distance) * 60f - current_step * 0.2f + temp_reward * 100f);
            EndEpisode();
        }
    }

    private void FixedUpdate()
    {
        RequestDecision();
    }
}
