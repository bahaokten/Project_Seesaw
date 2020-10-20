using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

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
    public Subscription<GameStarted> GameStartedSubscription;

    private GameData currGameData;
    private int currRow;
    private int currRound;

    // COLS: 0:playerLType | 1:playerRType | 2:winner | 3:numRounds | 4:LWins | 5:RWins | 6:Stalemates | 7:LLongestWinStreak | 8:RLongestWinStreak | (9)
    // 9:LTotalCards | 10:RTotalCards | 11:LTotalUpgrades | 12:RTotalUpgrades | (4)
    // 13:LWeaponUsage-LWeaponWin ... | 19:RWeaponUsage-RWeaponWin ... | 25:LWeaponUsage%-LWeaponWin% ... | 31:RWeaponUsage%-LWeaponWin% ... | (24)
    // 37:LWeaponUpgrades ... | 43:RWeaponUpgrades ...  | 49:LCards ... | ?:RCards ... | (12 + 2*numcards)
    public static readonly List<string> colNames = new List<string>()
    {
        "LPlayerType", "RPlayerType", "GameWinner", "TotalRounds", "NumLRoundWins",
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
            return;
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
        GameStartedSubscription = _EventBus.Subscribe<GameStarted>(_OnGameStarted);
    }

    void Start()
    {
        currRow = 1;

        logTxtPath = Application.persistentDataPath + "/log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_h-mm-ss") + ".txt";
        logResultsCsvPath = Application.persistentDataPath + "/log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_h-mm-ss") + ".csv";
        sheet = new ES3Spreadsheet();

        numStaticCols = 9 + 4 + 24 + 12;
        numCardCols = 2 * GameData.defaultCardDict.Count;
        totalCols = numStaticCols + numCardCols;

        SaveSheetTitle();
    }

    public void StartNewGame()
    {
        currGameData = new GameData(Time.time, GlobalVars.instance.LType, GlobalVars.instance.RType);
        currRound = 0;

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
        foreach (KeyValuePair<CardType, int> kv in GameData.defaultCardDict)
        {
            j++;
            sheet.SetCell(numStaticCols + j, 0, ( "L" + (CardType) kv.Key ).ToString());
        }
        foreach (KeyValuePair<CardType, int> kv in GameData.defaultCardDict)
        {
            j++;
            sheet.SetCell(numStaticCols + j, 0, ("R" + (CardType)kv.Key).ToString());
        }
    }

    public void SaveLogs()
    {
        print("SAVING");
        ES3.SaveRaw(rawStrOut, logTxtPath);
        sheet.Save(logResultsCsvPath);
    }

    void SetCurrentGameCells()
    {
        // COLS: 0:playerLType | 1:playerRType | 2:winner | 3:numRounds | 4:LWins | 5:RWins | 6:Stalemates | 7:LLongestWinStreak | 8:RLongestWinStreak | (9)
        // 9:LTotalCards | 10:RTotalCards | 11:LTotalUpgrades | 12:RTotalUpgrades | (4)
        // 13:LWeaponUsage-LWeaponWin ... | 19:RWeaponUsage-RWeaponWin ... | 25:LWeaponUsage%-LWeaponWin% ... | 31:RWeaponUsage%-LWeaponWin% ... | (24)
        // 37:LWeaponUpgrades ... | 43:RWeaponUpgrades ...  | 49:LCards ... | ?:RCards ... | (12 + 2*numcards)
        float totalRounds = currGameData.numWins[Player.L] + currGameData.numWins[Player.R] + currGameData.numWins[Player.NaN];
        //PlayerLType
        sheet.SetCell(0, currRow, Enum.GetName(typeof(PlayerType), currGameData.playerLType));
        //PlayerRType
        sheet.SetCell(1, currRow, Enum.GetName(typeof(PlayerType), currGameData.playerRType));
        //Winner
        sheet.SetCell(2, currRow, Enum.GetName(typeof(Player), currGameData.winner));
        //GameLength
        sheet.SetCell(3, currRow, totalRounds);
        //LWins
        sheet.SetCell(4, currRow, currGameData.numWins[Player.L]);
        //RWins
        sheet.SetCell(5, currRow, currGameData.numWins[Player.R]);
        //Stalemates
        sheet.SetCell(6, currRow, currGameData.numWins[Player.NaN]);
        //LLongestWinStreak
        sheet.SetCell(7, currRow, currGameData.longestWinStreak[Player.L]);
        //RLongestWinStreak
        sheet.SetCell(8, currRow, currGameData.longestWinStreak[Player.R]);
        //LTotalCards
        sheet.SetCell(9, currRow, currGameData.GetTotalCardsUsed(Player.L));
        //RTotalCards
        sheet.SetCell(10, currRow, currGameData.GetTotalCardsUsed(Player.R));
        //LTotalUpgrades
        sheet.SetCell(11, currRow, currGameData.GetTotalUpgrades(Player.L));
        //RTotalUpgrades
        sheet.SetCell(12, currRow, currGameData.GetTotalUpgrades(Player.R));
        //LWeaponUsage_Scissor
        sheet.SetCell(13, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Scissor]);
        //LWeaponWins_Scissor
        sheet.SetCell(14, currRow, currGameData.playerWeaponWins[Player.L][WeaponType.Scissor]);
        //LWeaponUsage_Paper
        sheet.SetCell(15, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Paper]);
        //LWeaponWins_Paper
        sheet.SetCell(16, currRow, currGameData.playerWeaponWins[Player.L][WeaponType.Paper]);
        //LWeaponUsage_Rock
        sheet.SetCell(17, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Rock]);
        //LWeaponWins_Rock
        sheet.SetCell(18, currRow, currGameData.playerWeaponWins[Player.L][WeaponType.Rock]);
        //RWeaponUsage_Scissor
        sheet.SetCell(19, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Scissor]);
        //RWeaponWins_Scissor
        sheet.SetCell(20, currRow, currGameData.playerWeaponWins[Player.R][WeaponType.Scissor]);
        //RWeaponUsage_Paper
        sheet.SetCell(21, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Paper]);
        //RWeaponWins_Paper
        sheet.SetCell(22, currRow, currGameData.playerWeaponWins[Player.R][WeaponType.Paper]);
        //RWeaponUsage_Rock
        sheet.SetCell(23, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Rock]);
        //RWeaponWins_Rock
        sheet.SetCell(24, currRow, currGameData.playerWeaponWins[Player.R][WeaponType.Rock]);
        //LWeaponUsage%_Scissor
        sheet.SetCell(25, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Scissor] / totalRounds);
        //LWeaponWins%_Scissor
        sheet.SetCell(26, currRow, (currGameData.playerWeaponUsage[Player.L][WeaponType.Scissor] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.L][WeaponType.Scissor] / currGameData.playerWeaponUsage[Player.L][WeaponType.Scissor]);
        //LWeaponUsage%_Paper
        sheet.SetCell(27, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Paper] / totalRounds);
        //LWeaponWins%_Paper
        sheet.SetCell(28, currRow, (currGameData.playerWeaponUsage[Player.L][WeaponType.Paper] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.L][WeaponType.Paper] / currGameData.playerWeaponUsage[Player.L][WeaponType.Paper]);
        //LWeaponUsage%_Rock
        sheet.SetCell(29, currRow, currGameData.playerWeaponUsage[Player.L][WeaponType.Rock] / totalRounds);
        //LWeaponWins%_Rock
        sheet.SetCell(30, currRow, (currGameData.playerWeaponUsage[Player.L][WeaponType.Rock] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.L][WeaponType.Rock] / currGameData.playerWeaponUsage[Player.L][WeaponType.Rock]);
        //RWeaponUsage%_Scissor
        sheet.SetCell(31, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Scissor] / totalRounds);
        //RWeaponWins%_Scissor
        sheet.SetCell(32, currRow, (currGameData.playerWeaponUsage[Player.R][WeaponType.Scissor] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.R][WeaponType.Scissor] / currGameData.playerWeaponUsage[Player.R][WeaponType.Scissor]);
        //RWeaponUsage%_Paper
        sheet.SetCell(33, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Paper] / totalRounds);
        //RWeaponWins%_Paper
        sheet.SetCell(34, currRow, (currGameData.playerWeaponUsage[Player.R][WeaponType.Paper] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.R][WeaponType.Paper] / currGameData.playerWeaponUsage[Player.R][WeaponType.Paper]);
        //RWeaponUsage%_Rock
        sheet.SetCell(35, currRow, currGameData.playerWeaponUsage[Player.R][WeaponType.Rock] / totalRounds);
        //RWeaponWins%_Rock
        sheet.SetCell(36, currRow, (currGameData.playerWeaponUsage[Player.R][WeaponType.Rock] == 0) ? 0 :
            currGameData.playerWeaponWins[Player.R][WeaponType.Rock] / currGameData.playerWeaponUsage[Player.R][WeaponType.Rock]);
        //LUpgrades_Scissor_Att
        sheet.SetCell(37, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Scissor][WeaponAttribute.Attack]);
        //LUpgrades_Scissor_Def
        sheet.SetCell(38, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Scissor][WeaponAttribute.Defense]);
        //LUpgrades_Paper_Att
        sheet.SetCell(39, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Paper][WeaponAttribute.Attack]);
        //LUpgrades_Paper_Def
        sheet.SetCell(40, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Paper][WeaponAttribute.Defense]);
        //LUpgrades_Rock_Att
        sheet.SetCell(41, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Rock][WeaponAttribute.Attack]);
        //LUpgrades_Rock_Def
        sheet.SetCell(42, currRow, currGameData.playerWeaponAttributeUpgrades[Player.L][WeaponType.Rock][WeaponAttribute.Defense]);
        //RUpgrades_Scissor_Att
        sheet.SetCell(43, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Scissor][WeaponAttribute.Attack]);
        //RUpgrades_Scissor_Def
        sheet.SetCell(44, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Scissor][WeaponAttribute.Defense]);
        //RUpgrades_Paper_Att
        sheet.SetCell(45, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Paper][WeaponAttribute.Attack]);
        //RUpgrades_Paper_Def
        sheet.SetCell(46, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Paper][WeaponAttribute.Defense]);
        //RUpgrades_Rock_Att
        sheet.SetCell(47, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Rock][WeaponAttribute.Attack]);
        //RUpgrades_Rock_Def
        sheet.SetCell(48, currRow, currGameData.playerWeaponAttributeUpgrades[Player.R][WeaponType.Rock][WeaponAttribute.Defense]);
        int j = -1;
        foreach (KeyValuePair<CardType, int> kv in currGameData.cardsUsed[Player.L])
        {
            j++;
            sheet.SetCell(numStaticCols + j, currRow, currGameData.cardsUsed[Player.L][(CardType)kv.Key]);
        }
        foreach (KeyValuePair<CardType, int> kv in currGameData.cardsUsed[Player.R])
        {
            j++;
            sheet.SetCell(numStaticCols + j, currRow, currGameData.cardsUsed[Player.R][(CardType)kv.Key]);
        }
    }

    private void LogLine(Player p, string line)
    {
        string player;
        if (p == Player.NaN)
        {
            player = ""; 
        } else
        {
            player = p.ToString();
        }

        rawStrOut += currRound + "|" + player + "|" + line + "\n";
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

        currGameData.cardsUsed[e.player.player][e.type] = currGameData.cardsUsed[e.player.player][e.type] + 1;
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
        if (e.newCurr == Player.L)
        {
            currRound++;
        }
        if (e.newCurr != Player.NaN)
        {
            LogLine(e.newCurr, "TURN STARTED");
        }
    }

    void _OnPlayerWonRound(PlayerWonRound e)
    {
        currGameData.numWins[e.winner_t] += 1;
        if (e.winner_t == Player.NaN)
        {
            LogLine(e.winner_t, "STALEMATE!");

        } else
        {
            LogLine(e.winner_t, "WON THE ROUND. EARNING " + e.coinsEarned + " COINS");
            LogLine(Player.NaN, "CURRENT SCORE L-R: "+ GameController.instance.GetPlayer(Player.L).score + "-" + GameController.instance.GetPlayer(Player.R).score + " COINS L-R: " + GameController.instance.GetPlayer(Player.L).coins + "-" + GameController.instance.GetPlayer(Player.R).coins);
            
            currGameData.currentWinStreak[e.winner_t] += 1;
            if (currGameData.currentWinStreak[e.winner_t] > currGameData.longestWinStreak[e.winner_t])
            {
                currGameData.longestWinStreak[e.winner_t] = currGameData.currentWinStreak[e.winner_t];
            }

            if (e.winner_t == Player.L)
            {
                currGameData.playerWeaponWins[Player.L][e.playerLWeapon] += 1;
            } else
            {
                currGameData.playerWeaponWins[Player.R][e.playerRWeapon] += 1;
            }
            

            Player opponent = GameController.instance.GetOpponentPlayerType(e.winner_t);
            currGameData.currentWinStreak[opponent] = 0;
        }
    }

    void _OnGameOver(GameOver e)
    {
        LogLine(e.winner, "WON!");

        currGameData.winner = e.winner;
        currGameData.gameLength = Time.time - currGameData.initTime;

        SetCurrentGameCells();
        currRow++;
    }

    void _OnGameStarted(GameStarted e)
    {
        StartNewGame();
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
        public Dictionary<Player, SortedDictionary<CardType, int>> cardsUsed;
        public Dictionary<Player, Dictionary<WeaponType, Dictionary<WeaponAttribute, int>>> playerWeaponAttributeUpgrades;
        public Dictionary<Player, Dictionary<WeaponType, int>> playerWeaponUsage;
        public Dictionary<Player, Dictionary<WeaponType, int>> playerWeaponWins;
        public Dictionary<Player, int> longestWinStreak;
        public Dictionary<Player, int> currentWinStreak;

        public static readonly SortedDictionary<CardType, int> defaultCardDict = new SortedDictionary<CardType, int>()
                    {
                        {CardType.SelfAttackIncreaseAdditiveCurrent1, 0},
                        {CardType.SelfDefenseIncreaseAdditiveCurrent1, 0}
                    };

        public int GetTotalCardsUsed(Player p)
        {
            int runningTotal = 0;
            foreach (KeyValuePair<CardType, int> kv in cardsUsed[p])
            {
                runningTotal += kv.Value;
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

        public GameData(float _initTime, PlayerType _playerLType, PlayerType _playerRType)
        {
            initTime = _initTime;
            playerLType = _playerLType;
            playerRType = _playerRType;

            numWins = new Dictionary<Player, int>()
            {
                { Player.L, 0 },
                { Player.R, 0 },
                { Player.NaN, 0 },
            };

            cardsUsed = new Dictionary<Player, SortedDictionary<CardType, int>>()
            {
                { Player.L, new SortedDictionary<CardType, int>(defaultCardDict)
                },
                { Player.R, new SortedDictionary<CardType, int>(defaultCardDict)
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
