using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger instance;

    public static string logTxtPath;
    public static string logResultsCsvPath;

    private ES3Spreadsheet sheet;
    private string rawStrOut;

    public Subscription<EndTurnPhase> EndTurnPhaseSubscription;
    public Subscription<TurnPhaseChanged> TurnPhaseChangedSubscription;
    public Subscription<CardPurchased> CardPurchasedSubscription;
    public Subscription<CardUsed> CardUsedSubscription;
    public Subscription<CardDestroyed> CardDestroyedSubscription;
    public Subscription<WeaponUpgraded> WeaponUpgradedSubscription;
    public Subscription<AttackWeaponPicked> AttackWeaponPickedSubscription;
    public Subscription<GameStateOver> GameStateOverSubscription;
    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;
    public Subscription<PlayerWonRound> PlayerWonRoundSubscription;
    public Subscription<GameOver> GameOverSubscription;

    private GameData currGameData;
    private int currGameIndex; 

    // COLS: playerLType | playerRType | winner | gameLength | LWins | RWins | Stalemates | LLongestWinStreak | RLongestWinStreak | (9)
    // LTotalCards | RTotalCards | LTotalUpgrades | RTotalUpgrades | (4)
    // LWeaponUsage-LWeaponWin ... | RWeaponUsage-RWeaponWin ... | LWeaponUsage%-LWeaponWin% ... | RWeaponUsage%-LWeaponWin% ... | (24)
    // LWeaponUpgrades ... | RWeaponUpgrades ...  | LCards ... | RCards ... | (12 + 2*numcards)
    public static readonly List<string> colNames = new List<string>()
    {
        "LPlayerType", "RPlayerType", "GameWinner", "GameLength_(s)", "NumLRoundWins",
        "NumRRoundWins", "NumStaleMates", "LLongestWinStreak", "RLongestWinStreak",
        "LTotalCards", "RTotalCards", "LTotalUpgrades", "RTotalUpgrades",
        "LWeaponUsage_Scissor", "LWeaponWins_Scissor", "LWeaponUsage_Paper", "LWeaponWins_Paper", "LWeaponUsage_Rock", "LWeaponWins_Rock",
        "RWeaponUsage_Scissor", "RWeaponWins_Scissor", "RWeaponUsage_Paper", "RWeaponWins_Paper", "RWeaponUsage_Rock", "RWeaponWins_Rock",
        "LWeaponUsage%_Scissor", "LWeaponWins%_Scissor", "LWeaponUsage%_Paper", "LWeaponWins%_Paper", "LWeaponUsage%_Rock", "LWeaponWins%_Rock",
        "RWeaponUsage%_Scissor", "RWeaponWins%_Scissor", "RWeaponUsage%_Paper", "RWeaponWins%_Paper", "RWeaponUsage%_Rock", "RWeaponWins%_Rock",
        "LUpgrade_Scissor_Attack", "LUpgrade_Scissor_Defense", "LUpgrade_Paper_Attack", "LUpgrade_Paper_Defense", "LUpgrade_Rock_Attack", "LUpgrade_Rock_Defense",
        "RUpgrade_Scissor_Attack", "RUpgrade_Scissor_Defense", "RUpgrade_Paper_Attack", "RUpgrade_Paper_Defense", "RUpgrade_Rock_Attack", "RUpgrade_Rock_Defense",
    };

    private int numStaticCols;
    private int numCardCols;
    private int totalCols;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        EndTurnPhaseSubscription = _EventBus.Subscribe<EndTurnPhase>(_OnEndTurnPhase);
        TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        CardDestroyedSubscription = _EventBus.Subscribe<CardDestroyed>(_OnCardDestroyed);
        WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
        GameStateOverSubscription = _EventBus.Subscribe<GameStateOver>(_OnGameStateOver);
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
        PlayerWonRoundSubscription = _EventBus.Subscribe<PlayerWonRound>(_OnPlayerWonRound);
        GameOverSubscription = _EventBus.Subscribe<GameOver>(_OnGameOver);
    }

    void Start()
    {
        currGameIndex = 1;

        numStaticCols = 9 + 4 + 24 + 12;
        numCardCols = 2 * currGameData.cardsUsed[Player.L].Count;
        totalCols = numStaticCols + numCardCols;

        logTxtPath = Application.persistentDataPath + "/log_" + DateTime.Today.ToString("d") + ".txt";
        logResultsCsvPath = Application.persistentDataPath + "/log_" + DateTime.Today.ToString("d") + ".csv";
        sheet = new ES3Spreadsheet();

        SaveSheetTitle();
    }

    public void StartNewGame()
    {
        currGameData = new GameData(Time.time, GlobalVars.instance.LType, GlobalVars.instance.RType);

        LogLine(Player.NaN, "====================================");
        LogLine(Player.NaN, "NEW GAME STARTED");
        LogLine(Player.L, GlobalVars.instance.LType.ToString());
        LogLine(Player.R, GlobalVars.instance.RType.ToString());
        LogLine(Player.NaN, "====================================");
    }

    void SaveSheetTitle()
    {
        for (int i = 0; i < numStaticCols; i++)
        {
            sheet.SetCell(i, 0, colNames[i]);
        }
        int j = -1;
        foreach (DictionaryEntry de in currGameData.cardsUsed[Player.L])
        {
            j++;
            sheet.SetCell(numStaticCols + j, 0, ( (CardType) de.Key ).ToString());
        }
    }

    void SaveLogs()
    {
        ES3.SaveRaw(rawStrOut, logTxtPath);

        //TODO: Change gamelength to num rounds played
        // COLS: 0:playerLType | 1:playerRType | 2:winner | 3:gameLength | 4:LWins | 5:RWins | 6:Stalemates | 7:LLongestWinStreak | 8:RLongestWinStreak | (9)
        // 9:LTotalCards | 10:RTotalCards | 11:LTotalUpgrades | 12:RTotalUpgrades | (4)
        // 13:LWeaponUsage-LWeaponWin ... | 19:RWeaponUsage-RWeaponWin ... | 25:LWeaponUsage%-LWeaponWin% ... | 31:RWeaponUsage%-LWeaponWin% ... | (24)
        // 37:LWeaponUpgrades ... | 43:RWeaponUpgrades ...  | 49:LCards ... | ?:RCards ... | (12 + 2*numcards)

        //PlayerLType
        sheet.SetCell(0, 1, currGameData.playerLType);
        //PlayerRType
        sheet.SetCell(1, 1, currGameData.playerRType);
        //Winner
        sheet.SetCell(2, 1, currGameData.winner);
        //GameLength
        sheet.SetCell(3, 1, currGameData.gameLength);
        //LWins
        sheet.SetCell(4, 1, currGameData.numWins[Player.L]);
        //RWins
        sheet.SetCell(5, 1, currGameData.numWins[Player.R]);
        //Stalemates
        sheet.SetCell(6, 1, currGameData.numWins[Player.NaN]);
        //LLongestWinStreak
        sheet.SetCell(7, 1, currGameData.longestWinStreak[Player.L]);
        //RLongestWinStreak
        sheet.SetCell(8, 1, currGameData.longestWinStreak[Player.R]);
        //LTotalCards
        sheet.SetCell(9, 1, currGameData.GetTotalCardsUsed(Player.L));
        //RTotalCards
        sheet.SetCell(10, 1, currGameData.GetTotalCardsUsed(Player.L));
        //LTotalUpgrades
        sheet.SetCell(11, 1, currGameData.GetTotalUpgrades(Player.L));
        //RTotalUpgrades
        sheet.SetCell(12, 1, currGameData.GetTotalUpgrades(Player.R));
        //LWeaponUsage_Scissor
        sheet.SetCell(13, 1, currGameData.playerWeaponUsage[Player.L][WeaponType.Scissor]);
        //LWeaponWins_Scissor
        sheet.SetCell(14, 1, currGameData.playerWeaponWins[Player.L][WeaponType.Scissor]);
        //LWeaponUsage_Paper
        sheet.SetCell(15, 1, currGameData.playerWeaponUsage[Player.L][WeaponType.Paper]);
        //LWeaponWins_Paper
        sheet.SetCell(16, 1, currGameData.playerWeaponWins[Player.L][WeaponType.Paper]);
        //LWeaponUsage_Rock
        sheet.SetCell(17, 1, currGameData.playerWeaponUsage[Player.L][WeaponType.Rock]);
        //LWeaponWins_Rock
        sheet.SetCell(18, 1, currGameData.playerWeaponWins[Player.L][WeaponType.Rock]);
        //RWeaponUsage_Scissor
        sheet.SetCell(19, 1, currGameData.playerWeaponUsage[Player.R][WeaponType.Scissor]);
        //RWeaponWins_Scissor
        sheet.SetCell(20, 1, currGameData.playerWeaponWins[Player.R][WeaponType.Scissor]);
        //RWeaponUsage_Paper
        sheet.SetCell(21, 1, currGameData.playerWeaponUsage[Player.R][WeaponType.Paper]);
        //RWeaponWins_Paper
        sheet.SetCell(22, 1, currGameData.playerWeaponWins[Player.R][WeaponType.Paper]);
        //RWeaponUsage_Rock
        sheet.SetCell(23, 1, currGameData.playerWeaponUsage[Player.R][WeaponType.Rock]);
        //RWeaponWins_Rock
        sheet.SetCell(24, 1, currGameData.playerWeaponWins[Player.R][WeaponType.Rock]);
        //
        //sheet.SetCell(, 1, );

        sheet.Save("logResultsCsvPath");
    }

    private void LogLine(Player p, string line)
    {
        string player;
        if (p == Player.NaN)
        {
            player = "GAME"; 
        } else
        {
            player = p.ToString();
        }

        rawStrOut += Time.time + "|" + p.ToString() + "|" + line + "\n";
    }

    // ================= ALL LISTENERS =======================

    void _OnEndTurnPhase(EndTurnPhase e)
    {

    }

    void _OnTurnPhaseChanged(TurnPhaseChanged e)
    {
    }

    void _OnCardPurchased(CardPurchased e)
    {
    }

    void _OnCardUsed(CardUsed e)
    {
        LogLine(e.player.player, "CARD USED: " + e.type);

        currGameData.cardsUsed[e.player.player][e.type] = (int)currGameData.cardsUsed[e.player.player][e.type] + 1;
    }

    void _OnCardDestroyed(CardDestroyed e)
    {
    }

    void _OnWeaponUpgraded(WeaponUpgraded e)
    {
        LogLine(e.player.player, "WEAPON UPGRADED: " + e.type + " ATTR: " + e.attr);

        currGameData.playerWeaponAttributeUpgrades[e.player.player][e.type][e.attr] += 1;
    }

    void _OnAttackWeaponPicked(AttackWeaponPicked e)
    {
        LogLine(e.player.player, "WEAPON PICKED: " + e.type);

        currGameData.playerWeaponUsage[e.player.player][e.type] += 1;
    }

    void _OnGameStateOver(GameStateOver e)
    {
    }

    void _OnCurrentPlayerChanged(CurrentPlayerChanged e)
    {
        if (e.newCurr != Player.NaN)
        {
            LogLine(e.newCurr, "TURN STARTED");
        }
    }

    void _OnPlayerWonRound(PlayerWonRound e)
    {
        currGameData.numWins[e.player_t] += 1;
        if (e.player_t == Player.NaN)
        {
            LogLine(e.player_t, "STALEMATE!");

        } else
        {
            LogLine(e.player_t, "WON THE ROUND. COINS: " + e.coinsEarned);
            currGameData.currentWinStreak[e.player_t] += 1;
            if (currGameData.currentWinStreak[e.player_t] > currGameData.longestWinStreak[e.player_t])
            {
                currGameData.longestWinStreak[e.player_t] = currGameData.currentWinStreak[e.player_t];
            }

            Player opponent = GameController.instance.GetOpponentPlayerType(e.player_t);
            currGameData.currentWinStreak[opponent] = 0;
        }
    }

    void _OnGameOver(GameOver e)
    {
        LogLine(e.winner, "WON!");

        currGameData.winner = e.winner;
        currGameData.gameLength = Time.time - currGameData.initTime;
    }

    // ============== All Recorded SpreadSheet Data ================
    public class GameData
    {
        public float initTime;
        public PlayerType playerLType;
        public PlayerType playerRType;

        public Player winner;
        public float gameLength;
        public Dictionary<Player, int> numWins; //NaN is num stalemates
        public Dictionary<Player, OrderedDictionary> cardsUsed;
        public Dictionary<Player, Dictionary<WeaponType, Dictionary<WeaponAttribute, int>>> playerWeaponAttributeUpgrades;
        public Dictionary<Player, Dictionary<WeaponType, int>> playerWeaponUsage;
        public Dictionary<Player, Dictionary<WeaponType, int>> playerWeaponWins;
        public Dictionary<Player, int> longestWinStreak;
        public Dictionary<Player, int> currentWinStreak;

        public int GetTotalCardsUsed(Player p)
        {
            int runningTotal = 0;
            foreach (DictionaryEntry de in cardsUsed[p])
            {
                runningTotal += (int)de.Value;
            }
            return runningTotal;
        }

        public int GetTotalUpgrades(Player p)
        {
            int runningTotal = 0;
            foreach (KeyValuePair<WeaponType, Dictionary<WeaponAttribute, int>> kvType in playerWeaponAttributeUpgrades[p])
            {
                foreach (KeyValuePair<WeaponAttribute, int> kvAttr in kvType.Value)
                {
                    runningTotal += kvAttr.Value;
                }
            }
            return runningTotal;
        }

        public GameData(float _initTime, PlayerType _playerLType, PlayerType playerRType)
        {
            initTime = _initTime;
            playerLType = _playerLType;
            playerRType = _playerLType;

            numWins = new Dictionary<Player, int>()
            {
                { Player.L, 0 },
                { Player.R, 0 },
                { Player.NaN, 0 },
            };

            cardsUsed = new Dictionary<Player, OrderedDictionary>()
            {
                { Player.L, new OrderedDictionary()
                    {
                        {CardType.SelfAttackIncreaseAdditiveCurrent1, 0},
                        {CardType.SelfDefenseIncreaseAdditiveCurrent1, 0}
                    }
                },
                { Player.R, new OrderedDictionary()
                    {
                        {CardType.SelfAttackIncreaseAdditiveCurrent1, 0},
                        {CardType.SelfDefenseIncreaseAdditiveCurrent1, 0}
                    }
                }
            };

            playerWeaponAttributeUpgrades = new Dictionary<Player, Dictionary<WeaponType, Dictionary<WeaponAttribute, int>>>()
            {

                { Player.L, new Dictionary<WeaponType, Dictionary<WeaponAttribute,int>>()
                    {
                        {WeaponType.Scissor, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        },
                        {WeaponType.Paper, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        },
                        {WeaponType.Rock, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        }
                    }
                },
                { Player.R, new Dictionary<WeaponType, Dictionary<WeaponAttribute,int>>()
                    {
                        {WeaponType.Scissor, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        },
                        {WeaponType.Paper, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        },
                        {WeaponType.Rock, new Dictionary<WeaponAttribute, int>()
                            {
                                {WeaponAttribute.Attack, 0},
                                {WeaponAttribute.Defense, 0}
                            }
                        }
                    }
                }
            };

            playerWeaponUsage = new Dictionary<Player, Dictionary<WeaponType, int>>()
            {
                { Player.L, new Dictionary<WeaponType, int>()
                    {
                        {WeaponType.Scissor, 0},
                        {WeaponType.Paper, 0},
                        {WeaponType.Rock, 0}
                    }
                },
                { Player.R, new Dictionary<WeaponType, int>()
                    {
                        {WeaponType.Scissor, 0},
                        {WeaponType.Paper, 0},
                        {WeaponType.Rock, 0}
                    }
                }
            };

            playerWeaponWins = new Dictionary<Player, Dictionary<WeaponType, int>>()
            {
                { Player.L, new Dictionary<WeaponType, int>()
                    {
                        {WeaponType.Scissor, 0},
                        {WeaponType.Paper, 0},
                        {WeaponType.Rock, 0}
                    }
                },
                { Player.R, new Dictionary<WeaponType, int>()
                    {
                        {WeaponType.Scissor, 0},
                        {WeaponType.Paper, 0},
                        {WeaponType.Rock, 0}
                    }
                }
            };

            longestWinStreak = new Dictionary<Player, int>()
            {
                { Player.L, 0 },
                { Player.R, 0 },
            };

            currentWinStreak = new Dictionary<Player, int>()
            {
                { Player.L, 0 },
                { Player.R, 0 },
            };
        }
    }



}
