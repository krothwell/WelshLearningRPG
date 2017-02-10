﻿using System;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;
using DbUtilities;

public class DbSetup {
	private enum tbls 	{
						PlayerGames,	
						EnglishVocab,
						WelshVocab,
						VocabGrammar,
						VocabTranslations,
						DiscoveredVocab,
						DiscoveredGrammar,
						VocabRuleList,
						Proficiencies,
						AcquiredVocabSkills,
						AcquiredGrammarSkills,
						SentenceIdentifications,
						SentenceTranslations,
						SentenceGrammar,
						SentenceRulesList,
						DiscoveredSentences,
                        Dialogues,
                        Characters,
                        CharacterDialogues,
                        DialogueNodes,
                        PlayerChoices,
                        Quests
						};

	private enum tblSqlStrs {header,body, pk};
	private int numberOfTbls = Enum.GetNames(typeof(tbls)).Length;
	public static string[,] tblSqlArray;
	string sql;								

	private static string conn;

    public DbSetup () {
        conn = DbCommands.GetConnectionString();
        tblSqlArray = new string[
                                    numberOfTbls,
                                    Enum.GetNames(typeof(tblSqlStrs)).Length
                                    ]; //size of array is the # tables by the sql string parts to build the tables


        SetTblSqlArray();
    }

	void SetTblSqlArray() {
		tblSqlArray[(int)tbls.PlayerGames, (int)tblSqlStrs.header] 				= 	"PlayerGames";
		tblSqlArray[(int)tbls.PlayerGames, (int)tblSqlStrs.body] 				= 	"SaveIDs INTEGER NOT NULL, "
																				+	"SaveRefs VARCHAR(50), "
																				+	"PlayerNames VARCHAR(50), "
                                                                                +   "PortraitImages VARCHAR(200), "
																				+   "Dates VARCHAR(50), "
                                                                                +   "LocationName VARCHAR(25), "
                                                                                +   "LocationX REAL NOT NULL, "
                                                                                +   "LocationY REAL NOT NULL, ";
        tblSqlArray[(int)tbls.PlayerGames, (int)tblSqlStrs.pk] 					=	"SaveIDs"; 

		tblSqlArray[(int)tbls.EnglishVocab, (int)tblSqlStrs.header]				= 	"EnglishVocab";
		tblSqlArray[(int)tbls.EnglishVocab, (int)tblSqlStrs.body]				= 	"EnglishText VARCHAR(140) NOT NULL, ";
		tblSqlArray[(int)tbls.EnglishVocab, (int)tblSqlStrs.pk]					=	"EnglishText"; 

		tblSqlArray[(int)tbls.WelshVocab, (int)tblSqlStrs.header] 				= 	"WelshVocab";
		tblSqlArray[(int)tbls.WelshVocab, (int)tblSqlStrs.body] 				= 	"WelshText VARCHAR(140) NOT NULL, ";
		tblSqlArray[(int)tbls.WelshVocab, (int)tblSqlStrs.pk] 					= 	"WelshText";

		tblSqlArray[(int)tbls.VocabGrammar, (int)tblSqlStrs.header] 			= 	"VocabGrammar";
		tblSqlArray[(int)tbls.VocabGrammar, (int)tblSqlStrs.body]				= 	"RuleIDs INTEGER NOT NULL, "
																				+	"ShortDescriptions VARCHAR(100), "
                                                                                +   "LongDescriptions VARCHAR(500), ";
        tblSqlArray[(int)tbls.VocabGrammar, (int)tblSqlStrs.pk]					= 	"RuleIDs";

		tblSqlArray[(int)tbls.VocabTranslations, (int)tblSqlStrs.header]		= 	"VocabTranslations";
		tblSqlArray[(int)tbls.VocabTranslations, (int)tblSqlStrs.body]			= 	"EnglishText VARCHAR(140) NOT NULL, "
																				+	"WelshText VARCHAR(140) NOT NULL, "
																				+	"FOREIGN KEY (EnglishText) "
																					+ "REFERENCES EnglishVocab(EnglishText) ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (WelshText) "
																					+ "REFERENCES WelshVocab(WelshText) ON DELETE CASCADE ON UPDATE CASCADE, ";
		tblSqlArray[(int)tbls.VocabTranslations, (int)tblSqlStrs.pk]			= 	"EnglishText, WelshText";

		tblSqlArray[(int)tbls.DiscoveredVocab, (int)tblSqlStrs.header]			= 	"DiscoveredVocab";
		tblSqlArray[(int)tbls.DiscoveredVocab, (int)tblSqlStrs.body]			= 	"EnglishText VARCHAR(140) NOT NULL, "
																				+	"WelshText VARCHAR(140) NOT NULL, "
																				+	"SaveIDs INT NOT NULL, "
																				+	"CorrectTallies INT NOT NULL, "
																				+	"FOREIGN KEY (EnglishText, WelshText) "
																					+ "REFERENCES VocabTranslations(EnglishText,WelshText) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (SaveIDs) REFERENCES PlayerGames(SaveIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.DiscoveredVocab, (int)tblSqlStrs.pk]				= 	"EnglishText, WelshText, SaveIDs";

		tblSqlArray[(int)tbls.DiscoveredGrammar, (int)tblSqlStrs.header]		= 	"DiscoveredVocabGrammar";
		tblSqlArray[(int)tbls.DiscoveredGrammar, (int)tblSqlStrs.body]			= 	"RuleIDs INT NOT NULL, "
																				+	"SaveIDs INT NOT NULL, "
																				+	"CorrectTallies INT NOT NULL, "
																				+	"FOREIGN KEY (RuleIDs) REFERENCES VocabGrammar(RuleIDs) ON DELETE CASCADE, "
																				+	"FOREIGN KEY (SaveIDs) REFERENCES PlayerGames(SaveIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.DiscoveredGrammar, (int)tblSqlStrs.pk]			= 	"RuleIDs, SaveIDs";

		tblSqlArray[(int)tbls.VocabRuleList, (int)tblSqlStrs.header]			= 	"VocabRuleList";
		tblSqlArray[(int)tbls.VocabRuleList, (int)tblSqlStrs.body]				= 	"EnglishText VARCHAR(140) NOT NULL, "
																				+	"WelshText VARCHAR(140) NOT NULL, "
																				+	"RuleIDs INT NOT NULL, "
																				+	"FOREIGN KEY (EnglishText, WelshText) "
																					+ "REFERENCES VocabTranslations(EnglishText, WelshText) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (RuleIDs) REFERENCES VocabGrammar(RuleIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.VocabRuleList, (int)tblSqlStrs.pk]				= 	"EnglishText, WelshText, RuleIDs";

		tblSqlArray[(int)tbls.Proficiencies, (int)tblSqlStrs.header]			= 	"Proficiencies";
		tblSqlArray[(int)tbls.Proficiencies, (int)tblSqlStrs.body]				= 	"ProficiencyNames VARCHAR(20) NOT NULL, "
																				+	"Thresholds INT NOT NULL, ";
		tblSqlArray[(int)tbls.Proficiencies, (int)tblSqlStrs.pk]				= 	"ProficiencyNames";

		tblSqlArray[(int)tbls.AcquiredVocabSkills, (int)tblSqlStrs.header]		= 	"AcquiredVocabSkills";
		tblSqlArray[(int)tbls.AcquiredVocabSkills, (int)tblSqlStrs.body]		= 	"EnglishText VARCHAR(140) NOT NULL, "
																				+ 	"WelshText VARCHAR(140) NOT NULL, "
																				+	"ProficiencyNames VARCHAR(20) NOT NULL, "
																				+	"SaveIDs INT NOT NULL, "
																				+	"FOREIGN KEY (EnglishText, WelshText) "
																					+ "REFERENCES VocabTranslations(EnglishText,WelshText) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (ProficiencyNames) REFERENCES Proficiencies(ProficiencyNames) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (SaveIDs) REFERENCES PlayerGames(SaveIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.AcquiredVocabSkills, (int)tblSqlStrs.pk]			= 	"EnglishText, WelshText, ProficiencyNames, SaveIDs";

		tblSqlArray[(int)tbls.AcquiredGrammarSkills, (int)tblSqlStrs.header]	= 	"AcquiredGrammarSkills";
		tblSqlArray[(int)tbls.AcquiredGrammarSkills, (int)tblSqlStrs.body]		= 	"RuleIDs INT NOT NULL, "
																				+	"ProficiencyNames VARCHAR(20) NOT NULL, "
																				+	"SaveIDs INT NOT NULL, "
																				+	"FOREIGN KEY (RuleIDs) REFERENCES VocabGrammar(RuleIDs) ON DELETE CASCADE, "
																				+	"FOREIGN KEY (ProficiencyNames) REFERENCES Proficiencies(ProficiencyNames) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (SaveIDs) REFERENCES PlayerGames(SaveIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.AcquiredGrammarSkills, (int)tblSqlStrs.pk]		= 	"RuleIDs, ProficiencyNames, SaveIDs";

		tblSqlArray[(int)tbls.SentenceIdentifications, (int)tblSqlStrs.header]	= 	"SentenceIdentifications";
		tblSqlArray[(int)tbls.SentenceIdentifications, (int)tblSqlStrs.body]	= 	"SentenceIDs INT NOT NULL, ";
		tblSqlArray[(int)tbls.SentenceIdentifications, (int)tblSqlStrs.pk]		= 	"SentenceIDs";

		tblSqlArray[(int)tbls.SentenceTranslations, (int)tblSqlStrs.header]		= 	"SentenceTranslations";
		tblSqlArray[(int)tbls.SentenceTranslations, (int)tblSqlStrs.body]		= 	"EnglishText VARCHAR(140) NOT NULL, "
																				+ 	"WelshText VARCHAR(140) NOT NULL, "
																				+	"SentenceIDs INT NOT NULL, "
																				+	"OrderIDs INT NOT NULL, "
																				+	"FOREIGN KEY (EnglishText, WelshText) "
																					+ "REFERENCES VocabTranslations(EnglishText, WelshText) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, "
																				+	"FOREIGN KEY (SentenceIDs) "
																					+ "REFERENCES SentenceIdentifications(SentenceIDs) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, ";
		tblSqlArray[(int)tbls.SentenceTranslations, (int)tblSqlStrs.pk]			= 	"EnglishText, WelshText, SentenceIDs, OrderIDs";

		tblSqlArray[(int)tbls.SentenceGrammar, (int)tblSqlStrs.header]			= 	"SentenceGrammar";
		tblSqlArray[(int)tbls.SentenceGrammar, (int)tblSqlStrs.body]			= 	"RuleIDs INT NOT NULL, "
																				+	"Descriptions VARCHAR(200), ";
		tblSqlArray[(int)tbls.SentenceGrammar, (int)tblSqlStrs.pk]				= 	"RuleIDs";

		tblSqlArray[(int)tbls.SentenceRulesList, (int)tblSqlStrs.header]		= 	"SentenceRulesList";
		tblSqlArray[(int)tbls.SentenceRulesList, (int)tblSqlStrs.body]			= 	"RuleIDs INT NOT NULL, "
																				+	"SentenceIDs INT NOT NULL, "
																				+	"EnglishText VARCHAR(140) NOT NULL, "
																				+ 	"WelshText VARCHAR(140) NOT NULL, "
																				+	"OrderIDs INT NOT NULL, "
																				+	"FOREIGN KEY (RuleIDs) REFERENCES SentenceGrammar(RuleIDs) ON DELETE CASCADE, "
																				+	"FOREIGN KEY (EnglishText, WelshText, SentenceIDs, OrderIDs) "
																					+ "REFERENCES SentenceTranslations(EnglishText, WelshText, SentenceIDs, OrderIDs) "
																					+ "ON DELETE CASCADE ON UPDATE CASCADE, ";
		tblSqlArray[(int)tbls.SentenceRulesList, (int)tblSqlStrs.pk]			= 	"RuleIDs, SentenceIDs, EnglishText, WelshText, OrderIDs";//fk referencing composite pk must use all values of pk

		tblSqlArray[(int)tbls.DiscoveredSentences, (int)tblSqlStrs.header]		= 	"DiscoveredSentences";
		tblSqlArray[(int)tbls.DiscoveredSentences, (int)tblSqlStrs.body]		= 	"SentenceIDs INT NOT NULL, "
																				+	"SaveIDs INT NOT NULL, "
																				+	"FOREIGN KEY (SentenceIDs) "
																					+ "REFERENCES SentenceIdentifications (SentenceIDs) ON DELETE CASCADE, "
																				+	"FOREIGN KEY (SaveIDs) REFERENCES PlayerGames(SaveIDs) ON DELETE CASCADE, ";
		tblSqlArray[(int)tbls.DiscoveredSentences, (int)tblSqlStrs.pk]			= 	"SentenceIDs, SaveIDs";

        //DIALOGUES
        tblSqlArray[(int)tbls.Dialogues, (int)tblSqlStrs.header]                = "Dialogues";
        tblSqlArray[(int)tbls.Dialogues, (int)tblSqlStrs.body]                  = "DialogueIDs INT NOT NULL, "
                                                                                + "DialogueDescriptions VARCHAR(200) NOT NULL, "
                                                                                + "Active INT NOT NULL, ";
        tblSqlArray[(int)tbls.Dialogues, (int)tblSqlStrs.pk]                    = "DialogueIDs";

        tblSqlArray[(int)tbls.Characters, (int)tblSqlStrs.header]               = "Characters";
        tblSqlArray[(int)tbls.Characters, (int)tblSqlStrs.body]                 = "CharacterNames VARCHAR(100) NOT NULL, "
                                                                                + "Scenes VARCHAR(100) NULL,";
        tblSqlArray[(int)tbls.Characters, (int)tblSqlStrs.pk]                   = "CharacterNames, Scenes";

        tblSqlArray[(int)tbls.CharacterDialogues, (int)tblSqlStrs.header]       = "CharacterDialogues";
        tblSqlArray[(int)tbls.CharacterDialogues, (int)tblSqlStrs.body]         = "CharacterNames VARCHAR(100) NULL, "
                                                                                + "Scenes VARCHAR(100) NULL,"
                                                                                + "DialogueIDs INT NOT NULL, "
                                                                                + "FOREIGN KEY (CharacterNames, Scenes) REFERENCES Characters(CharacterNames, Scenes) ON DELETE CASCADE, "
                                                                                + "FOREIGN KEY (DialogueIDs) REFERENCES Dialogues(DialogueIDs) ON DELETE CASCADE, ";
        tblSqlArray[(int)tbls.CharacterDialogues, (int)tblSqlStrs.pk]           = "CharacterNames, Scenes, DialogueIDs";

        tblSqlArray[(int)tbls.DialogueNodes, (int)tblSqlStrs.header]            = "DialogueNodes";
        tblSqlArray[(int)tbls.DialogueNodes, (int)tblSqlStrs.body]              = "NodeIDs INT NULL, "
                                                                                + "NodeText VARCHAR(500) NOT NULL, "
                                                                                + "DialogueIDs INT, "
                                                                                + "CharacterSpeaking VARCHAR(100) NULL, "
                                                                                + "Scenes VARCHAR(100) NULL, "
                                                                                + "EndDialogueOption INT, "
                                                                                + "FOREIGN KEY (CharacterSpeaking, Scenes) REFERENCES Characters(CharacterNames, Scenes) ON DELETE CASCADE, "
                                                                                + "FOREIGN KEY (DialogueIDs) REFERENCES Dialogues(DialogueIDs) ON DELETE CASCADE, ";
        tblSqlArray[(int)tbls.DialogueNodes, (int)tblSqlStrs.pk]                = "NodeIDs";

        tblSqlArray[(int)tbls.PlayerChoices, (int)tblSqlStrs.header]            = "PlayerChoices";
        tblSqlArray[(int)tbls.PlayerChoices, (int)tblSqlStrs.body]              = "ChoiceIDs INT NOT NULL, "
                                                                                + "ChoiceText VARCHAR(500) NOT NULL, "
                                                                                + "NodeIDs INT NULL, "
                                                                                + "NextNodes INT, "
                                                                                + "FOREIGN KEY (NodeIDs) REFERENCES DialogueNodes(NodeIDs) ON DELETE CASCADE, "
                                                                                + "FOREIGN KEY (NextNodes) REFERENCES DialogueNodes(NodeIDs) ON DELETE CASCADE, ";
        tblSqlArray[(int)tbls.PlayerChoices, (int)tblSqlStrs.pk]                = "ChoiceIDs";

        tblSqlArray[(int)tbls.Quests, (int)tblSqlStrs.header]                   = "Quests";
        tblSqlArray[(int)tbls.Quests, (int)tblSqlStrs.body]                     = "QuestNames VARCHAR(100) NOT NULL, "
                                                                                + "QuestDescriptions VARCHAR(500) NULL, ";
        tblSqlArray[(int)tbls.Quests, (int)tblSqlStrs.pk]                       = "QuestNames";

    }

    void ReplaceTable(string tblName) {
        DropTable("copied", false);
        IDbConnection _dbc = new SqliteConnection(conn);
        _dbc.Open(); //Open connection to the database.
        IDbCommand _dbcm = _dbc.CreateCommand();
        _dbcm.CommandText = "PRAGMA foreign_keys=ON;";
        _dbcm.ExecuteNonQuery();
        string sql = "CREATE TABLE copied AS SELECT * FROM " + tblName;
        _dbcm.CommandText = sql;
        _dbcm.ExecuteNonQuery();
        DropTable(tblName, false);
        CreateTables();

        CopyTable(tblName, "copied");

        //PrintTable(tblName);

        _dbcm.Dispose();
        _dbcm = null;
        _dbc.Close();
        _dbc = null;
    }

    void CopyTable(string tblNameTo, string tblNameFrom) {
        IDbConnection _dbc = new SqliteConnection(conn);
        _dbc.Open(); //Open connection to the database.
        IDbCommand _dbcm = _dbc.CreateCommand();
        _dbcm.CommandText = "PRAGMA foreign_keys=ON;";
        _dbcm.ExecuteNonQuery();

        sql = "SELECT * FROM " + tblNameFrom;
        string copiedFieldNames = "";
        _dbcm.CommandText = sql;
        IDataReader _dbr = _dbcm.ExecuteReader();
        for (int i = 0; i < _dbr.FieldCount; i++) {
            if (i == _dbr.FieldCount - 1) {
                copiedFieldNames += (_dbr.GetName(i));
            }
            else { copiedFieldNames += (_dbr.GetName(i)) + ","; }
        }

        _dbr.Dispose();
        _dbr = null;

        sql = "INSERT INTO " + tblNameTo + "(" + copiedFieldNames + ") SELECT " + copiedFieldNames + " FROM " + tblNameFrom;

        _dbcm.CommandText = sql;
        _dbcm.ExecuteNonQuery();

        //PrintTable(tblNameTo);

        _dbcm.Dispose();
        _dbcm = null;
        _dbc.Close();
        _dbc = null;
    }

    void CreateTable(int sqlArrayRow) {
		IDbConnection _dbc = new SqliteConnection(conn);
		_dbc.Open(); //Open connection to the database.
		IDbCommand _dbcm = _dbc.CreateCommand();
		_dbcm.CommandText = "PRAGMA foreign_keys=ON;";
		_dbcm.ExecuteNonQuery();
		_dbcm.CommandText	= "CREATE TABLE IF NOT EXISTS " 	 
							+ tblSqlArray[sqlArrayRow, (int)tblSqlStrs.header] 	
							+ " (" 							
							+ tblSqlArray[sqlArrayRow, (int)tblSqlStrs.body] 
							+ "PRIMARY KEY (" + tblSqlArray[sqlArrayRow, (int)tblSqlStrs.pk] + ")"	
							+ ");";
		_dbcm.ExecuteNonQuery();
		_dbcm.Dispose();
		_dbcm = null;
		_dbc.Close();
		_dbc = null;
	}

    void DropTable(string tblName, bool cascade = true) {
        IDbConnection _dbc = new SqliteConnection(conn);
        _dbc.Open(); //Open connection to the database.
        IDbCommand _dbcm = _dbc.CreateCommand();
        if (cascade) {
            _dbcm.CommandText = "PRAGMA foreign_keys=ON;";
            _dbcm.ExecuteNonQuery();
        }
        _dbcm.CommandText = "DROP TABLE IF EXISTS " + tblName + ";";
        _dbcm.ExecuteNonQuery();
        _dbcm.Dispose();
        _dbcm = null;
        _dbc.Close();
        _dbc = null;
    }


    public void CreateTables() {
		for (int i = 0; i < numberOfTbls; i++) {
			CreateTable(i);
		}
	}

	

}