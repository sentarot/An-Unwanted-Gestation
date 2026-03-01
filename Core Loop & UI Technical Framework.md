# **TECHNICAL GDD: Core Loop, AI Architecture, and UI**

## **1\. System Architecture Overview**

The game is an asymmetrical simulation/RPG. The architecture is split into two distinct, antagonistic systems running concurrently:

1. **The Host AI (Autonomous Agent):** A schedule-driven NPC that attempts to go about her daily life, maintain her stats, and actively fight the condition.  
2. **The Player Controller (The Payload):** An interface that generates resources passively and actively purchases upgrades to disrupt the Host's AI, alter her stats, and drive the condition to term.

## **2\. Session Initialization (Setup Phase)**

At the start of a session, the player configures the match by selecting two primary data objects. This determines the base variables and active skill trees for the run.

### **2.1 Host Selection (The Target)**

The player selects a Host\_Profile object. This defines the AI's starting stats, daily schedule, and specific vulnerabilities.

**Example Host Data Structures:**

{  
  "Host\_ID": "The\_Cheerleader",  
  "Base\_Stats": {  
    "Physical\_Resistance": 70,    // High agility, fights off Discomfort  
    "Mental\_Defense": 40,         // Susceptible to panic  
    "Financial\_Resources": 20,    // Low ability to afford elite doctors  
    "Social\_Standing": 90         // Needs to maintain this to function  
  },  
  "Schedule": \["Attend\_School", "Cheer\_Practice", "Socialize"\],  
  "Vulnerability": "Humiliation"  // Humiliation damage has a 1.5x multiplier  
}

{  
  "Host\_ID": "The\_Trophy\_Wife",  
  "Base\_Stats": {  
    "Physical\_Resistance": 30,    // Low tolerance for physical burden  
    "Mental\_Defense": 80,         // Stubborn, high denial  
    "Financial\_Resources": 100,   // Can rapidly fill the "Intervention" meter  
    "Social\_Standing": 80  
  },  
  "Schedule": \["Spa\_Day", "Charity\_Gala", "Shopping"\],  
  "Vulnerability": "Discomfort"   // Discomfort severely limits her mobility  
}

### **2.2 Gestation Type Selection (The Payload)**

The player selects an Affliction\_Class, which determines the available skill tree arrays and the ultimate win condition.

* Macrosomic\_Natural: Focuses on physical encumbrance and Discomfort mechanics.  
* Witch\_Curse: Focuses on visible grotesque mutations and Humiliation mechanics.  
* Alien\_Brood: Focuses on Endocrine manipulation (mind-control/symbiosis).

## **3\. The Core Simulation Loop (Tick Logic)**

The game operates on a daily/hourly tick system. Every tick, the GameManager resolves the Player's modifiers against the Host's AI goals.

### **3.1 Player State & Resources**

* **Biomass (Float):** The spendable currency for the skill tree.  
  * *Tick Generation:* Base\_Gen \+ (Host.Humiliation \* Mult) \+ (Host.Discomfort \* Mult)  
  * *Design Note:* The player earns currency faster when the Host's daily schedule fails or she suffers stat debuffs.  
* **Gestation (Float 0.0 \- 100.0):** The primary Win Condition progress bar. Speed is dictated by active Player upgrades.

### **3.2 Host AI State Machine (The "Victim" Loop)**

The Host AI evaluates its situation every tick and attempts to execute a DailyTask or an InterventionTask.

**Variables:**

* **Humiliation (0.0 \- 100.0):** Psychological damage. High values block Social\_Tasks (e.g., attending Galas).  
* **Discomfort (0.0 \- 100.0):** Physical damage. High values block Physical\_Tasks (e.g., Cheer Practice) and eventually cause Is\_Bedridden \= true.  
* **Intervention\_Meter (0.0 \- 100.0):** The Lose Condition. If the Host fills this, the payload is extracted/cured, and the player loses.

**AI Evaluation Pseudocode:**

void UpdateHostAI() {  
    // 1\. Check for hard CC (Crowd Control) from Player Upgrades  
    if (PlayerUpgrades.Contains("Symbiotic\_Euphoria")) {  
        CancelInterventionAttempts(); // Mind-control prevents seeking help  
    }

    // 2\. Evaluate Status  
    if (Discomfort \> 85.0f) {  
        SetState(HostState.Bedridden);  
        Mobility \= 0;  
    } else if (Humiliation \> 75.0f) {  
        SetState(HostState.Isolated); // Refuses to leave the house  
    } else {  
        SetState(HostState.Active);  
    }

    // 3\. Execute Action  
    if (CurrentState \== HostState.Active) {  
        if (Gestation \> 30.0f && \!IsMindControlled) {  
            // Panic sets in. Host uses financial resources to fill Intervention meter.  
            AttemptTask(TaskType.Seek\_Doctor);   
        } else {  
            // Host attempts normal life to maintain Social Standing.  
            AttemptTask(Schedule.GetNextTask());  
        }  
    }  
}

## **4\. UI/UX Architecture**

The frontend is designed as a "Parasite Dashboard"—clinical, voyeuristic, and analytical.

### **4.1 Primary Viewport: The Host Monitor**

* **Visuals:** A 2D layered sprite or 3D character model of the specific Host (e.g., the Cheerleader in uniform).  
* **Dynamic Updating:** \* Listens to Gestation: Scales the abdominal rig/sprite, alters posture (swayback).  
  * Listens to PlayerUpgrades: Applies visual toggles (e.g., sweat particles, ruined clothes, visible movement under the skin, unnatural glowing veins).

### **4.2 HUD Overlay (The Dashboard)**

* **Top Bar:** Displays Biomass pool, Gestation progress (Win bar), and Intervention progress (Lose bar).  
* **Left Panel (Host Schedule & Vitals):** \* Shows the Host's current attempted task (e.g., *"Attempting: Cheer Practice"*).  
  * Shows success/failure of the task based on player interference.  
  * Live readouts of Humiliation, Discomfort, and Social\_Standing.  
* **Right Panel (Evolution Skill Tree):** \* Divided into: Somatic (Physical burden), Endocrine (Hormones/Mind control), and Morphological (Visual/Structural changes).  
  * Nodes are unlocked using Biomass.

### **4.3 The Dynamic Event Log**

* **Function:** A scrolling console log at the bottom of the screen translating the math into narrative body horror and social degradation.  
* **Logic:** Uses string interpolation based on the Host Archetype, active tasks, and current variables.  
* *Example Output:* "The Trophy Wife attempts to attend the charity gala, but severe Polyhydramnios causes her to stumble heavily. Humiliation increased by 15%."