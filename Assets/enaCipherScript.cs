using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Words;
using rnd = UnityEngine.Random;

public class enaCipherScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;
	public KMColorblindMode Colorblind;


	public TextMesh ramText;
	public Material[] biosBootupScreenStuff;
	public Material startupScreen, backgroundScreen;
	public GameObject biosStuff, startupStuff;
	public GameObject[] loadingBars;
	public MeshRenderer screen;
	public GameObject taskBar;
	public GameObject window;
	public GameObject submissionWindow;
	public GameObject blueScreenMessage;
	public GameObject shutdownScreen;
	public TextMesh timeText;
	public Material[] letterPatterns, numberPatterns;
	public MeshRenderer[] seqDisplays;
	public TextMesh submissionDisplayText;
	public Material bsodScreen;
	public TextMesh[] cbTexts;

	public KMSelectable startButton;
	public KMSelectable backSpace, submit;
	public KMSelectable[] keyboard;
	public KMSelectable reset;


	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private bool isActivated;

	private bool submission = false;

	private bool cbActive = false;

	private bool lessTime;

	private string getKey(string kw, string alphabet, bool kwFirst)
    {
		return (kwFirst ? (kw + alphabet) : alphabet.Except(kw).Concat(kw)).Distinct().Join("");
    }

	private string word = "";
	private string[] keywords = new string[2];
	private string encrypted = "";
	private bool[] jSub = new bool[6];
	private bool moduleSelected;

	private int[][] encryptedFlashSequence = new int[6][];
	private int[][] jSubFlashSequence = new int[6][];
	private int[][] arithmeticKWFlashSequence;
	private int[][] extinctionFlashSequence = new int[6][];
	private int[][] temptationKWFlashSequence;
	private int[] extPairs = new int[6];

	private string[] mainCB = { "BY", "WL", "WR", "A", "Y" };
	private string[] numCB = { "B", "Y" };

	private string sub = "";



	Data data = new Data();





	void wordGenerate()
    {
		word = data.PickWord(6);

		for (int a = 0; a < 2; a++)
        {
			keywords[a] = data.PickWord(3, 8);
        }

		arithmeticKWFlashSequence = new int[keywords[0].Length][];
		temptationKWFlashSequence = new int[keywords[1].Length][];

		for (int i = 0; i < 6; i++)
        {
			if (word[i] == 'J')
            {
				encrypted += "ABCDEFGHIKLMNOPQRSTUVWXYZ"[rnd.Range(0, 25)];
				jSub[i] = true;
            }
            else
            {
				encrypted += word[i];
            }
        }

		Debug.LogFormat("[ƎNA Cipher #{0}] The decrypted word is: {1}", moduleId, word);


		string temptKey = getKey(keywords[1].Replace('J', 'I'), "ABCDEFGHIKLMNOPQRSTUVWXYZ", ((Bomb.GetPortCount()) % 2 != 0 && (Bomb.GetPortPlateCount()) % 2 == 0));
		string arithKey = getKey(keywords[0], "ABCDEFGHIJKLMNOPQRSTUVWXYZ", (Bomb.GetSerialNumberLetters().Any(x => x == 'E' || x == 'N' || x == 'A')));

		generateExtPairSequence();
		encryptionStuff(temptKey, arithKey, encrypted);
    }

	void encryptionStuff(string key1, string key2, string word)
    {
		string currentMessage = word;

		Debug.LogFormat("[ƎNA Cipher #{0}] Temptation Stairway Key: {1}", moduleId, key1);
		Debug.LogFormat("[ƎNA Cipher #{0}] Beginning Temptation Stairway Encryption", moduleId);

		currentMessage = encryptTemptationStairway(key1, currentMessage);

		for (int i = 0; i < 6; i++)
        {
			if (jSub[i])
            {
				sub += currentMessage[i];
				currentMessage = currentMessage.Substring(0, i) + "J" + currentMessage.Substring(i + 1);
            }
            else
            {
				sub += "ABCDEFGHIKLMNOPQRSTUVWXYZ"[rnd.Range(0, 25)];
            }
        }

		Debug.LogFormat("[ƎNA Cipher #{0}] After encrypting with Temptation Stairway Cipher: {1}", moduleId, currentMessage);

		Debug.LogFormat("[ƎNA Cipher #{0}] Extinction Transposition Key: {1}", moduleId, extPairs.Join(""));
		Debug.LogFormat("[ƎNA Cipher #{0}] Beginning Extinction Transposition Encryption", moduleId);

		currentMessage = encryptExtinction(currentMessage);

		Debug.LogFormat("[ƎNA Cipher #{0}] After encrypting with Extinction Transposition: {1}", moduleId, currentMessage);

		Debug.LogFormat("[ƎNA Cipher #{0}] Arithmetic Sequence Key: {1}", moduleId, key2);
		Debug.LogFormat("[ƎNA Cipher #{0}] Beginning Arithmetic Sequence Encryption", moduleId);
		currentMessage = encryptArithmeticSequence(currentMessage, key2);
		
		Debug.LogFormat("[ƎNA Cipher #{0}] After encrypting with Arithmetic Sequence Cipher: {1}", moduleId, currentMessage);

		int[] reversed = extPairs;

		Array.Reverse(reversed);
		generatingEncryptedFlashSeq(currentMessage);
        generatingArithmeticKeywordFlashSeq(keywords[0]);
		generatingExtinctionFlashSeq(reversed.Join("").ToString());
		generatingTemptationKeywordFlashSeq(keywords[1]);
		generatingSubFlashSeq(sub);
    }

	string encryptTemptationStairway(string key, string word)
    {

		string output = "";




		for (int i = 0; i < word.Length; i+= 3)
        {
			string currentSet = word.Substring(i, 3);

			int[] row = currentSet.Select(x => key.IndexOf(x) / 5).ToArray();
			int[] col = currentSet.Select(x => key.IndexOf(x) % 5).ToArray();

			if (row.Distinct().Count() == 1 || col.Distinct().Count() == 1)
            {
				// Same letter? Stay put.
            }

			int cMin = col.Min();
			int cMax = col.Max();

			int cRange = cMax - cMin;

			col = col.Select(x => (cMin + cRange - x + cMin) % 5).ToArray();

			for (int j = 0; j < 3; j++)
            {
				output += key[(5 * row[j] + col[j]) % 26];
            }

        }

		

		return output;
    }

	void generateExtPairSequence()
    {
		List<int> scrambleNumbersA = new List<int>() { 1, 2, 3, 4, 5, 6 };
		List<int> scrambleNumbersB = new List<int>() { 1, 2, 3, 4, 5, 6 };
		List<int> finalNumbers = new List<int>();

		scrambleNumbersA.Shuffle();
		scrambleNumbersB.Shuffle();

		while (finalNumbers.Count != 6)
        {
			if (scrambleNumbersA[0] == scrambleNumbersB[0])
			{
				while (scrambleNumbersA[0] == scrambleNumbersB[0])
				{
					scrambleNumbersA.Shuffle();
					scrambleNumbersB.Shuffle();
				}
			}
			finalNumbers.Add(scrambleNumbersA[0]);
			finalNumbers.Add(scrambleNumbersB[0]);
			scrambleNumbersA.Shuffle();
			scrambleNumbersB.Shuffle();
		}

		for (int i = 0; i < 6; i++)
        {
			extPairs[i] = finalNumbers[i];
        }


    }

	string encryptExtinction (string word)
    {
		string output = "";

		int[] pairs = new int[6];


		for (int i = 0; i < 6; i++)
        {
			
			pairs[i] = extPairs[i] - 1;	

        }
		char[] letters = word.ToCharArray();

		for (int j = 0; j < 6; j += 2)
        {
			int indx1 = pairs[j];
			int indx2 = pairs[(j + 1) % 6];
            var temp = letters[indx1];

			letters[indx1] = letters[indx2];
			letters[indx2] = temp;

        }

		output = letters.Join("");
		

		return output;
    }

	string encryptArithmeticSequence (string word, string key)
    {
		string output = "";

		for (int i = 0; i < 6; i+= 2)
        {
			string currentSet = word.Substring(i, 2);

			int[] letterIndex = currentSet.Select(x => key.IndexOf(x)).ToArray();

			int e1 = letterIndex[0];
			int e2 = letterIndex[1];

			int offset = (e2 - e1 + 26) % 26;

			int d2 = (e2 + offset) % 26;
			int d1 = (d2 + offset) % 26;

			output += "" + key[d1] + key[d2];

        }


		return output;
    }


	void Awake()
    {

		moduleId = moduleIdCounter++;

		foreach (KMSelectable letter in keyboard)
        {
			letter.OnInteract += delegate () { keyPress(letter); return false; };
        }
		backSpace.OnInteract += delegate () { backSpacePress(); return false; };
		startButton.OnInteract += delegate () { startPress(); return false; };
		submit.OnInteract += delegate () { submitPress(); return false; };
		reset.OnInteract += delegate () { resetPress(); return false; };
		cbActive = Colorblind.ColorblindModeActive;

	}

	
	void Start()
    {
		Module.GetComponent<KMSelectable>().OnFocus += delegate { moduleSelected = true; };
		Module.GetComponent<KMSelectable>().OnDefocus += delegate { moduleSelected = false; };
		submissionDisplayText.text = "";
		for (int i = 0; i < 5; i++)
        {
			cbTexts[i].text = "";
        }
		StartCoroutine(startingScreen());
		wordGenerate();
    }

	void keyPress(KMSelectable letter)
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		letter.AddInteractionPunch(0.4f);
		if (moduleSolved)
        {
			return;
        }
		if (submissionDisplayText.text.Length < 6)
        {
			submissionDisplayText.text += letter.GetComponentInChildren<TextMesh>().text;
        }
    }

	void backSpacePress()
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		backSpace.AddInteractionPunch(0.4f);
		if (moduleSolved)
        {
			return;
        }
		submissionDisplayText.text = "";
    }

	void submitPress()
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		submit.AddInteractionPunch(0.4f);
		if (moduleSolved)
        {
			return;
        }
		
		if (submissionDisplayText.text == word)
        {
			StartCoroutine(solveAnimation());
			Debug.LogFormat("[ƎNA Cipher #{0}] You have spoken in the language of the Gods! Solved!", moduleId);
        }
        else
        {
			StartCoroutine(strikeAnimation());
			Debug.LogFormat("[ƎNA Cipher #{0}] The word you inputted isn't up to our community's typical standards! Strike!", moduleId);
        }
    }

	IEnumerator solveAnimation()
    {
		yield return null;
		moduleSolved = true;
		submissionDisplayText.text = "";
		string solveText = "YEAH!";
		int loop = 0;
		int pattern = 0;
		Audio.PlaySoundAtTransform("Solve", transform);

		if (Bomb.GetTime() < 30)
        {
			lessTime = true;
			Bomb.GetComponent<KMBombModule>().HandlePass();
        }

		for (int i = 0; i < solveText.Length; i++)
        {
			submissionDisplayText.text += solveText[i];
			if (submissionDisplayText.text.Length == 5)
            {
				submissionDisplayText.color = Color.green;
            }
			yield return new WaitForSeconds(0.26f);
        }
		yield return new WaitForSeconds(0.26f);
		submissionWindow.SetActive(false);
		yield return new WaitForSeconds(0.52f);
		window.SetActive(true);
		while (loop != 18)
        {
			if (pattern == letterPatterns.Length) pattern = 0;
			for (int i = 0; i < seqDisplays.Length; i++)
            {
				seqDisplays[i].material = letterPatterns[pattern];
				yield return new WaitForSeconds(0.06f);
            }
			loop++;
			pattern++;
        }
		for (int i = 0; i < seqDisplays.Length; i++)
        {
			seqDisplays[i].material = letterPatterns[i];
			yield return new WaitForSeconds(0.06f);
        }
		yield return new WaitForSeconds(1);
		window.SetActive(false);
		yield return new WaitForSeconds(0.8f);
		taskBar.SetActive(false);
		yield return new WaitForSeconds(1);
		screen.material = biosBootupScreenStuff[0];
		yield return new WaitForSeconds(1);
		screen.material = startupScreen;
		shutdownScreen.SetActive(true);
		yield return new WaitForSeconds(2);
		shutdownScreen.SetActive(false);
		screen.material = biosBootupScreenStuff[1];
		yield return new WaitForSeconds(0.8f);
		screen.material = biosBootupScreenStuff[0];
		if (!lessTime)
        {
			Module.GetComponent<KMBombModule>().HandlePass();
		}


    }

	IEnumerator strikeAnimation()
    {
		yield return null;
		Audio.PlaySoundAtTransform("Strike", transform);
		submissionDisplayText.text = "";
		submissionWindow.SetActive(false);
		yield return new WaitForSeconds(0.15f);
		taskBar.SetActive(false);
		screen.material = bsodScreen;
		blueScreenMessage.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		Module.GetComponent<KMBombModule>().HandleStrike();
		blueScreenMessage.SetActive(false);
		screen.material = backgroundScreen;
		window.SetActive(true);
		taskBar.SetActive(true);
		yield return new WaitForSeconds(1);
		submission = false;
		StartCoroutine(flashingEncryptedSequence());
		StartCoroutine(flashingArithmeticKWSequence());
		StartCoroutine(flashingExtinctionSequence());
		StartCoroutine(flashingTemptationKWSequence());
		StartCoroutine(flashingSubSequence());

	}

	void startPress()
    {
		Audio.PlaySoundAtTransform("Click", transform);
		startButton.AddInteractionPunch(0.4f);

		if (submission || !isActivated || moduleSolved)
        {
			return;
        }

		if (!submission && isActivated)
        {
			StopAllCoroutines();
			for (int i = 0; i < 5; i++)
            {
				seqDisplays[i].material = biosBootupScreenStuff[0];
				cbTexts[i].text = "";
            }
			window.SetActive(false);
			submissionWindow.SetActive(true);
			submission = true;
        }
    }

	void resetPress()
    {
		Audio.PlaySoundAtTransform("Click", transform);
		reset.AddInteractionPunch(0.4f);
		if (moduleSolved)
		{
			return;
		}
		if (submission) StartCoroutine(resetSequence());
    }

	IEnumerator resetSequence()
    {
		yield return null;
		submissionDisplayText.text = "";
		submissionWindow.SetActive(false);
		yield return new WaitForSeconds(1.5f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TitleMenuPressed, transform);
		window.SetActive(true);
		submission = false;
		yield return new WaitForSeconds(1);
		StartCoroutine(flashingEncryptedSequence());
		StartCoroutine(flashingArithmeticKWSequence());
		StartCoroutine(flashingExtinctionSequence());
		StartCoroutine(flashingTemptationKWSequence());
		StartCoroutine(flashingSubSequence());
		yield return null;
	}



	IEnumerator startingScreen()
    {
		int ram = 0;
		int loadingBar = 0;
		taskBar.SetActive(false);
		window.SetActive(false);
		submissionWindow.SetActive(false);

		yield return null;
		Audio.PlaySoundAtTransform("BiosStartup", transform);
		yield return new WaitForSeconds(4);
		screen.material = biosBootupScreenStuff[1];
		yield return new WaitForSeconds(0.5f);
		biosStuff.SetActive(true);
		while (ram != 256)
        {
			ram++;
			ramText.text = ram.ToString() + "MB OK";
			yield return new WaitForSeconds(0.033f);
        }
		yield return new WaitForSeconds(0.8f);
		biosStuff.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		screen.material = startupScreen;
		startupStuff.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		while (loadingBar != 8)
        {
			loadingBars[loadingBar].SetActive(true);
			loadingBar++;
			yield return new WaitForSeconds(0.2f);
        }
		yield return new WaitForSeconds(1);
		startupStuff.SetActive(false);
		screen.material = biosBootupScreenStuff[0];
		yield return new WaitForSeconds(1);
		screen.material = backgroundScreen;
		yield return new WaitForSeconds(1);
		Audio.PlaySoundAtTransform("OSBoot", transform);
		yield return new WaitForSeconds(1);
		taskBar.SetActive(true);
		yield return new WaitForSeconds(1);
		window.SetActive(true);
		isActivated = true;
		yield return new WaitForSeconds(1);
		StartCoroutine(flashingEncryptedSequence());
		StartCoroutine(flashingArithmeticKWSequence());
		StartCoroutine(flashingExtinctionSequence());
		StartCoroutine(flashingTemptationKWSequence());
		StartCoroutine(flashingSubSequence());
	}

	void generatingEncryptedFlashSeq(string word)
    {

        string[] sequences = new string[word.Length];
		char[] letters = word.ToCharArray();

		for (int i = 0; i < word.Length; i++)
        {
            char[][] num = new char[sequences.Length][];
			num[i] = new char[3];
			encryptedFlashSequence[i] = new int[3];
			sequences[i] = colorCodeData.generateLetterSequence(letters[i]);
			
			for (int j = 0; j < 3; j++)
            {
				num[i][j] = sequences[i].ToCharArray()[j];
				encryptedFlashSequence[i][j] = num[i][j] - '0';

			}
			
        }	
    }

	void generatingArithmeticKeywordFlashSeq(string kw)
    {
		string[] sequences = new string[kw.Length];
		char[] letters = kw.ToCharArray();

		for (int i = 0; i < kw.Length; i++)
        {
			char[][] num = new char[sequences.Length][];
            num[i] = new char[3];
			arithmeticKWFlashSequence[i] = new int[3];
			sequences[i] = colorCodeData.generateLetterSequence(letters[i]);

			for (int j = 0; j < 3; j++)
            {
				num[i][j] = sequences[i].ToCharArray()[j];
				arithmeticKWFlashSequence[i][j] = num[i][j] - '0';
            }
        }
    }

	void generatingExtinctionFlashSeq(string numbers)
    {
		string[] sequences = new string[numbers.Length];
        char[] digits = numbers.ToCharArray();

		for (int i = 0; i < numbers.Length; i++)
        {
			char[][] num = new char[sequences.Length][];
			num[i] = new char[3];
			extinctionFlashSequence[i] = new int[3];
			sequences[i] = colorCodeData.generateNumberSequence(digits[i]);

			for (int j = 0; j < 3; j++)
            {
				num[i][j] = sequences[i].ToCharArray()[j];
				extinctionFlashSequence[i][j] = num[i][j] - '0';
            }
        }
	}

	void generatingTemptationKeywordFlashSeq(string kw)
    {
		string[] sequences = new string[kw.Length];
		char[] letters = kw.ToCharArray();

		for (int i = 0; i < kw.Length; i++)
        {
			char[][] num = new char[sequences.Length][];
			num[i] = new char[3];
			temptationKWFlashSequence[i] = new int[3];
			sequences[i] = colorCodeData.generateLetterSequence(letters[i]);

			for (int j = 0; j < 3; j++)
            {
				num[i][j] = sequences[i].ToCharArray()[j];
				temptationKWFlashSequence[i][j] = num[i][j] - '0';
            }
        }
    }

	void generatingSubFlashSeq(string jSub)
    {
		string[] sequences = new string[jSub.Length];
		char[] letters = jSub.ToCharArray();

		for (int i = 0; i < jSub.Length; i++)
        {
			char[][] num = new char[sequences.Length][];
			num[i] = new char[3];
			jSubFlashSequence[i] = new int[3];
			sequences[i] = colorCodeData.generateLetterSequence(letters[i]);

			for (int j = 0; j < 3; j++)
            {
				num[i][j] = sequences[i].ToCharArray()[j];
				jSubFlashSequence[i][j] = num[i][j] - '0';
            }
        }
    }

	IEnumerator flashingEncryptedSequence()
    {
		yield return null;

		int letterPos = 0;
		int letterFlashPos = 0;

		while (!moduleSolved)
        {
			letterPos = 0;
			letterFlashPos = 0;
			while (letterPos != word.Length)
            {
				while (letterFlashPos < 3)
                {
					seqDisplays[0].material = letterPatterns[encryptedFlashSequence[letterPos][letterFlashPos]];
					cbTexts[0].text = cbActive ? mainCB[encryptedFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f);
					seqDisplays[0].material = biosBootupScreenStuff[0];
					cbTexts[0].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f);
                }
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f);
            }
			yield return new WaitForSeconds(1);
        }
    }

	IEnumerator flashingArithmeticKWSequence()
    {
		yield return null;

		int letterPos = 0;
		int letterFlashPos = 0;

		while (!moduleSolved)
		{
			letterPos = 0;
			letterFlashPos = 0;
			while (letterPos != keywords[0].Length)
			{
				while (letterFlashPos < 3)
				{
					seqDisplays[1].material = letterPatterns[arithmeticKWFlashSequence[letterPos][letterFlashPos]];
					cbTexts[1].text = cbActive ? mainCB[arithmeticKWFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f);
					seqDisplays[1].material = biosBootupScreenStuff[0];
					cbTexts[1].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f);
				}
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f);
			}
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator flashingExtinctionSequence()
    {
		yield return null;

		int numberPos = 0;
		int numberFlashPos = 0;

		while (!moduleSolved)
        {
			numberPos = 0;
			numberFlashPos = 0;
			while (numberPos != extPairs.Length)
            {
				while (numberFlashPos < 3)
                {
					seqDisplays[2].material = numberPatterns[extinctionFlashSequence[numberPos][numberFlashPos]];
					cbTexts[2].text = cbActive ? numCB[extinctionFlashSequence[numberPos][numberFlashPos]] : "";
					yield return new WaitForSeconds(0.3f);
					seqDisplays[2].material = biosBootupScreenStuff[0];
					cbTexts[2].text = "";
					numberFlashPos++;
					yield return new WaitForSeconds(0.3f);
                }
				numberPos++;
				numberFlashPos = 0;
				yield return new WaitForSeconds(0.7f);
            }
			yield return new WaitForSeconds(1);
        }
    }

	IEnumerator flashingTemptationKWSequence()
    {
		yield return null;

		int letterPos = 0;
		int letterFlashPos = 0;

		while (!moduleSolved)
		{
			letterPos = 0;
			letterFlashPos = 0;
			while (letterPos != keywords[1].Length)
			{
				while (letterFlashPos < 3)
				{
					seqDisplays[3].material = letterPatterns[temptationKWFlashSequence[letterPos][letterFlashPos]];
					cbTexts[3].text = cbActive ? mainCB[temptationKWFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f);
					seqDisplays[3].material = biosBootupScreenStuff[0];
					cbTexts[3].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f);
				}
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f);
			}
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator flashingSubSequence()
    {
		yield return null;
		int letterPos = 0;
		int letterFlashPos = 0;

        while (!moduleSolved)
        {
			letterPos = 0;
			letterFlashPos = 0;
			while (letterPos != sub.Length)
            {
				while (letterFlashPos < 3)
                {
					seqDisplays[4].material = letterPatterns[jSubFlashSequence[letterPos][letterFlashPos]];
					cbTexts[4].text = cbActive ? mainCB[jSubFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f);
					seqDisplays[4].material = biosBootupScreenStuff[0];
					cbTexts[4].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f);
                }
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f);
            }
			yield return new WaitForSeconds(1);
        }
    }


	
	
	void Update()
    {
		setupClock();

		if (moduleSelected && submission)
        {
			for (var ltr = 0; ltr < 26; ltr++)
            {
				if (Input.GetKeyDown(((char)('a' + ltr)).ToString()))
                {
					keyboard[getCharIndex((char)('A' + ltr))].OnInteract();
                }
			}
			if (Input.GetKeyDown(KeyCode.Return))
            {
				submit.OnInteract();
            }
			if (Input.GetKeyDown(KeyCode.Backspace))
            {
				backSpace.OnInteract();
            }
        }

    }

	void setupClock()
    {
		DateTime clock = DateTime.Now;
		timeText.text = clock.ToString("t");
    }

	// Twitch Plays

#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} submit [insert answer here] to input your decrypted word. || cb to enable colorblind mode.";
#pragma warning restore 414

	private int getCharIndex(char c)
    {
		return "QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c);
    }

	IEnumerator ProcessTwitchCommand (string command)
    {
		yield return null;
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		if (split[0].EqualsIgnoreCase("CB"))
		{
			if (!isActivated)
			{
				yield return "sendtochaterror OS is still booting. Please stand by before enabling colorblind mode.";
				yield break;
			}
			cbActive = !cbActive;
			yield break;
		}

		if (split.Length != 2 || !split[0].EqualsIgnoreCase("SUBMIT"))
        {
			yield break;
        }

		

		int[] buttons = split[1].Select(getCharIndex).ToArray();
		if (buttons.Any(x => x < 0))
        {
			yield break;
        }
		startButton.OnInteract();
		yield return new WaitForSeconds(0.1f);
		foreach (char let in split[1])
        {
			keyboard[getCharIndex(let)].OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
		yield return new WaitForSeconds(0.1f);
		submit.OnInteract();
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		while (!isActivated)
        {
			yield return true;
        }
		if (submission && !word.StartsWith(submissionDisplayText.text))
        {
			backSpace.OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
		int start = submission ? submissionDisplayText.text.Length : 0;
		startButton.OnInteract();
		for (int i = start; i < 6; i++)
        {
			keyboard[getCharIndex(word[i])].OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
		submit.OnInteract();
		yield return null;
    }


}





