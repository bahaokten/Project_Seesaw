# Project Seesaw
Project SeeSaw aims to discover methodologies and patterns used in game balance practices.

I've created a modified version of Rock Paper Scissors called Super Rock Paper Scissors to conduct my research.

## Installation

Make sure you have Unity version 2020.1.9f1. (The project may work as intended but its not guaranteed.)

**Optional**

If you want to use Machine Learning AI make sure you have Python 3.7 or higher installed.

## How to run

* Build and run the project executable.

* Adjust player type and other settings and press start

* 200 games will automatically be played with the settings specificed by the user. You can adjust this number in the Menu scene under the `GlobalObjects` object `NumGamesToPlay` variable under `GlobalVars` script.

* 2 log files will be generated under `PERSISTENT_DATAPATH` named `log_MM-DD-YYYY-HH-MM-SS.csv` and `log_MM-DD-YYYY-HH-MM-SS.log`

* Examples of how I visualize the data can be found under `Research_RPC/STATS/` 

## How to Play Super Rock Paper Scissors? 

![m](https://github.com/bahaokten/Research_RPC/blob/master/GithubResources/RPSGIF1.gif)

#### **Round Structure**

You start the game with 5 coins and 0 score.

**Each player round consists of 3 phases:**
* Buy: In this phase you are allowed to buy as many cards as you want as long as you have the money for it.
* Action: In this phase you can either use a single card or upgrade one of your weapons' defense or attack attributes.
* Attack: In this phase you pick a weapon to attack your opponent. The attack initiates one both players finish the 3 phases.

#### **Winning Rounds and Score System**

You win the round if you correctly pick the weapon that counters your opponent's weapon. For example, if you picked scissor and your opponent picked rock, you win the round and gain +1 score.
However, if you both pick the same weapon, then the player with the highest attack attribute on their weapon wins.

The first person to reach a certain score wins the game.

#### **Coins System**

Coins are earned based on the damage you deal to your opponent on a winning round. Damage is calculated as `your opponent's weapon's defense attribute - your weapon's attack attribute + 1`. If the damage dealt is less than 1, it is rounded up to 1.

#### **Action Cards**
There are many action cards you can buy and play. 

**Some examples include:**

* Add 0.5 attack score to one of your weapons

* Add 0.5 defense score to one of your weapons

* Decrease one of opponent's weapon's defense score by 0.5 defense next turn

* Decrease one of opponent's weapon's defense score to 50% next turn

* Stun the enemy next turn if you win the interaction

* Cleanse any effects you may be under next turn

* If you win the next turn, gain 50% extra coins


You gain a +0.5 attack score card whenever you reach a score value multiple of 5.

#### **How it differs from Rock Paper Scissors**

In tradinitional rock paper scissors, the biggest factor in winning the game is having good luck. 
A good rock paper scissors will aim to have a perfectly randomized patters while trying to guess the other person's habits.
Super Rock Paper Scissors aims to amplify the guessing stage by altering the relative power dynamics within rock, paper, scissor.
For example, when a player invests coins to upgrade their rock, they will have a winning matchup against your rock. They might have a bigger incentive to play rock more often since their rock now has 2 winning matchups out of 3 possible. 

## Available AI players

These are all the AI players available to be picked in-game.

* **ScissorLover** Always attempts to upgrade scissor's attack attribute. Doesn't buy any cards. Has an 80% chance of choosing and 10% chance for each other weapon.

* **ScissorLover2** Has 33% chance each round to attempt buying a random card IF no upgrades can be bought. Always attempts to upgrade scissor attack and defense. Has a 100% chance of choosing scissor.

* **Greedy AI:** Greedy AI chooses the best combination of cards, upgrades, and weapons that yield the best potential win scenario for that round.

* **Mid Tracker AI:** An AI that tracks the opponents last N played weapons to predict pick habits, and tries to guess what the opponint is going to choose next round. Picks the opponent weapon prediction's losing matchup. Prioritizes Upgrading picked weapon's random attribute. If the current weapon cannot be upgraded, tries to buy and use a card.

* **Mid Tracker 2 AI:** Similar to Mid Tracker AI. Picks the weapon with winning matchup with bias to choose 1 specific weapon if winning ratios are too close. Prioritizes Upgrading picked weapon's random attribute. If current weapon cannot be upgraded, tries to buy and use a card

* **Random AI with Card, Upgrade, or hybrid focus:** These AI play random hands each with a slight bias.
  * **Random Hybrid AI:** Tries to buy a card 33% of the time. Tries to upgrade 50% of the time. Chooses each weapon with 33.3% chance
  * **Random Card Biased AI:** Tries to buy a card every round and use it. Never tries to upgrade. Chooses each weapon with 33.3% chance
  * **Random Upgrade Biased AI:** Never tries to buy a card. Tries to buy an upgrade 33% of the time if the player has less than 5 coins. Tries to buy an upgrade 100% of the time (prefers attack upgrade) if player has 5 or more coins. Chooses each weapon with 33.3% chance

* **Base MLAI and its implementations:** (REQUIRES PYTHON) MLAI (Machine Learning AI) uses a hybrid pretrained and real-time training to predict opponent's moves. 

## Log File Format

**The logger generates 2 files**

* **A text file keeping track of events and actions vs time**

![m](https://github.com/bahaokten/Research_RPC/blob/master/GithubResources/LogTxt1.PNG)

* **A CSV file keeping track of reports generated at the end of a game consisting of over 50+ datapoints of information. Each row represent 1 game.**

![m](https://github.com/bahaokten/Research_RPC/blob/master/GithubResources/Excel.PNG)
