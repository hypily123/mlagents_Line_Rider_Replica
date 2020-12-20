**Summary:**

This project is using ml-agent in Unity to learn a Line Rider Replica like game. We add a goal point to our game. If the bike touches the flag, aka the goal, the agent wins. As the reinforcement learning algorithm, we use Proximal Policy Optimization (PPO).

Line Rider Replica: [https://github.com/Brackeys/Line-Rider-Replica](https://github.com/Brackeys/Line-Rider-Replica)

ml-agents: [https://github.com/Unity-Technologies/ml-agents](https://github.com/Unity-Technologies/ml-agents)

**Instruction:**
1. Unity ml-agents needs both Unity and Python. Install Unity and Python3.
2. Be aware that the version of the python package and the version of the unity package should be the same. Install ml agents package into python. Open the terminal and type in 
`pip3 install mlagents==0.16.1`
3. Download the “ml-agents-master” folder in the repo and cd to the path
4. Cd to the “ml-agents” folder
5. Open the unity and open the project. In the terminal, enter
    `mlagents-learn config/config.yaml`
   If it succeeds, you should see the information like this:

![rUcarD.png](https://s3.ax1x.com/2020/12/20/rUcarD.png)

Then press the start button in Unity. It will start to learn.

6.If the learning progress ends, the model will be saved in the “models” folder. Our trained model is in the folder and named as “ppo”. Feel free to use it as the model to run the game.

**Training Flow:**

For each episode:



1. Reset the bike position and generate a random flag position
2. The agent, which simulates as a mouse, starts to draw a line:
    1. if the agent is out of frame, the episode ends
    2. if the agent meets the requirement, the bike starts to run
3. If the bike drops out of the frame, the episode ends

**Project Introduction:**

There are two most important files in the project:



1. config.yaml:<br>
The config file to the training. It can change all hyperparameters of the training.
2. PlayerAgents.cs:<br>
The script that contains the main training logic. For example, how the reward is calculated, how one episode runs, how the goal generates. For more details, please read the comments in the file.<br>
    *Important functions:*
```
public override void OnEpisodeBegin()
```
&emsp; &emsp; Initialize the training condition at the start of each episode.
```
public override void CollectObservations(VectorSensor sensor)
```
&emsp; &emsp; Collect the observation data. Use sensor.AddObservation() to add what is needed to observe. The fewer observations, the better.
```
public override void OnActionReceived(float[] vectorAction)
```
&emsp; &emsp; This is the core function of the agent. It decides how the agent could get rewards.according to each action. Input “vectorAction” is a random float array, which is considered as the actions of the agent. Use setReward() to give the agent a reward. Use EndEpisode() to jump out of the episode. This function will run every frame, which means it will run 60 times per second.

About rewards:
<br>
&emsp; &emsp; As mentioned in training flow, we have two steps in each episode: draw and run. Hence we have two types of rewards. The point which is drawing, we call it “agent”. The closer the agent to the goal point, the higher reward it will get. Then the bike runs. The closer the bike to the goal point, the higher reward it will get. The agent’s rewards have a lower weight than the bike’s.

**Our Result:**


[![rUhhZT.png](https://s3.ax1x.com/2020/12/20/rUhhZT.png)](https://imgchr.com/i/rUhhZT)
[![rUhoi4.png](https://s3.ax1x.com/2020/12/20/rUhoi4.png)](https://imgchr.com/i/rUhoi4)
