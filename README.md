# Undergraduate Thesis Draft  
**Advantages of Communication between Autonomous and Semi-Autonomous Vehicles**  
**Hutton Amison-Addy**  
**87152025**  
**Ashesi University**  
**April 7, 2024**  

---

## Abstract  

The urge for shorter travel times has led people to find better, real-time routes to minimize travel time [4], which has also become another motivator for the implementation of autonomous vehicles for their efficiency. Gathering and exchanging data could enable vehicles to expand their environmental awareness for improved journey-related decisions.

**How does communication among autonomous vehicles impact travel time and safety?** This study aims to investigate the effects of inter-vehicle communication on journey time and overall traffic flow. This study uses a simulation of vehicles with and without the ability to exchange information while navigating road network environments based on real-world locations. The journey time of the vehicles is recorded and analyzed for assessment.

Findings and conclusions will be discussed in the completed report.

---

### Research Question  

With the rise of autonomous and varying forms of AI-assisted driving, how does communication between autonomous vehicles improve travel time and safety?

### Aim  

The research aims to investigate the advantages that autonomous and semi-autonomous vehicles capable of communicating with each other have over autonomous vehicles not capable of communicating with each other in areas such as journey length. The primary factors to be observed are how vehicles avoid collisions and reduce the impact, coordinate on highways and intersects, and how long it takes to move from start to destination.

### Objectives  

- Design a communication and decision-making framework based on existing research.  
- Simulate the framework in environments mirroring real road networks.  
- Measure performance using journey time, safety incidents, and traffic density.  

### Significance  

Enabling vehicles to share environmental data can enhance safety through improved road awareness and potentially reduce environmental impact by shortening journey times. At the end of the research, the study will provide insight into how these vehicles perform in terms of journey time, traffic flow, and safety factors such as collision avoidance. The research also has the potential to advise future implementation of Communicating Autonomous Vehicles.

---
## Requirements 

- Unity engine Version 2020.3.19f1

--
### Setup 
- Download and install the Unity Hub from the official website: https://unity3d.com/get-unity/download
- Open the Unity Hub required to install the recommended version
- Go to the install tab on the left
- Click on install editor and search for the specific version for this project
- Install the specified version
--
### Project Setup 
- Clone the repository to your local machine. Avoid cloning to a OneDrive directory as it may cause issues with the file paths
- Open the Unity Hub
- Click on the add project button 
- Click on Add project from disk
- Navigate to the cloned repository and select the project folder
- Click on Add Project
- The project should be visible in your Unity Hub
--
### Running the Project 
This project is a simulation and therefore runs and stores data for analysis

- Open the project by double-clicking it
- Once the Unity editor opens, you can select between various environments made for the simulation.
    - Go to the `Scenes` or `locations` folder in the assets tab and double click any of the scenes you wish to simulate on
- Once open, click on the play button in the top middle section of the Unity editor
- Wait until none of the vehicles are spawned and the tab on the right shows no vehicle in the pane.
- Click on the play button to end the simulation.
- The data is saved in the `SimulationData` folder in the navigation pane



---
## Credit
- [Mattheieu Chirbani, Unity Traffic Ssystem](https://github.com/mchrbn)  
- Dr. Ayorkor Korsah
