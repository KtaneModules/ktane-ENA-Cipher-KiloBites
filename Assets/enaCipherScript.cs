using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Words;
using rnd = UnityEngine.Random;
using UnityEngine.Video;
using KeepCoding;

public class enaCipherScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;
	public KMColorblindMode Colorblind;


	public TextMesh ramText;
	public Material[] biosBootupScreenStuff;
	public Material startupScreen, backgroundScreen;
	public Material[] dreamStartupScreens;
	public GameObject biosStuff;
	public GameObject[] startupStuff;
	public GameObject[] loadingBars;
	public GameObject specialWindow;
	public GameObject dreamLoadingCircle;
	public MeshRenderer screen, border;
	public GameObject taskBar;
	public GameObject window;
	public GameObject submissionWindow;
	public GameObject blueScreenMessage;
	public GameObject[] shutdownScreen;
	public GameObject speedStuff;
	public TextMesh timeText;
	public Material[] letterPatterns, numberPatterns;
	public Material[] dreamLetterPatterns, dreamNumberPatterns;
	public Material[] startIcons;
	public Material[] borderTextures;
	public MeshRenderer[] seqDisplays;
	public MeshRenderer startIcon;
	public TextMesh submissionDisplayText;
	public Material bsodScreen;
	public TextMesh[] cbTexts;
	public VideoPlayer specialSolveOut;
	public VideoClip specialClip;

	public Material speedLitLED;
	public MeshRenderer[] speedLEDS;

	public KMSelectable startButton;
	public KMSelectable backSpace, submit;
	public KMSelectable[] keyboard;
	public KMSelectable reset;
	public KMSelectable[] speedButtons;


	private int moduleId;
	private static int moduleIdCounter = 1;
	private bool moduleSolved;

	private int _enaCipherId;
	private static int _enaCipherIdCounter = 1;

	private bool isActivated;
	private bool submission = false;
	private bool cbActive = false;
	private bool lessTime;

	private bool specialSolve()
	{
		return DateTime.Now.Month == 2 && DateTime.Now.Day == 23;
	}


	private bool disableAllButtons;
	private bool dreamBBQMode = false;
	private bool spinning;

	private static VideoClip assignedClip;

	private string getKey(string kw, string alphabet, bool kwFirst)
    {
		return (kwFirst ? (kw + alphabet) : alphabet.Except(kw).Concat(kw)).Distinct().Join("");
    }

	private string word = "";
	private string[] keywords = new string[2];
	private string encrypted = "";
	private bool[] jSub = new bool[6];
	private string submissionText;
	private bool moduleSelected;

	private int[][] encryptedFlashSequence = new int[6][];
	private int[][] jSubFlashSequence = new int[6][];
	private int[][] arithmeticKWFlashSequence;
	private int[][] extinctionFlashSequence = new int[6][];
	private int[][] temptationKWFlashSequence;
	private int[] extPairs = new int[6];
	int[] reversed;


    private int speedIndex = 2;
	private int speed = 1;

	private int dreamIx;

	private string[] mainCB = { "BY", "", "", "", "Y" };
	private string[] numCB = { "B", "Y" };
	private string[] mainDreamCB = { "WR", "", "G", "KT", "KY" };
	private string[] numDreamCB = { "", "R" };

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

		if (dreamBBQMode)
		{
			Debug.LogFormat("[ƎNA Cipher #{0}] Dream BBQ Mode has been activated!", moduleId);
		}

		if (specialSolve())
		{
			Debug.LogFormat("[ƎNA Cipher #{0}] The bomb is activated on February 23rd. Happy ENA Day!", moduleId);
		}

		Debug.LogFormat("[ƎNA Cipher #{0}] The decrypted word is: {1}", moduleId, word);


		string temptKey = getKey(keywords[1].Replace('J', 'I'), "ABCDEFGHIKLMNOPQRSTUVWXYZ", ((Bomb.GetPortCount()) % 2 != 0 && (Bomb.GetPortPlateCount()) % 2 == 0));
		string arithKey = getKey(keywords[0], "ABCDEFGHIJKLMNOPQRSTUVWXYZ", (Bomb.GetSerialNumberLetters().Any(x => "ENA".Contains(x))));

		generateExtPairSequence();
		encryptionStuff(temptKey, arithKey, encrypted);
    }

	void encryptionStuff(string key1, string key2, string word)
    {

		string currentMessage;
		currentMessage = word;

		Debug.LogFormat("[ƎNA Cipher #{0}] Temptation Stairway Keyword: {1}", moduleId, keywords[1]);
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

		Debug.LogFormat("[ƎNA Cipher #{0}] Arithmetic Sequence Keyword: {1}", moduleId, keywords[0]);
		Debug.LogFormat("[ƎNA Cipher #{0}] Arithmetic Sequence Key: {1}", moduleId, key2);
		Debug.LogFormat("[ƎNA Cipher #{0}] Beginning Arithmetic Sequence Encryption", moduleId);
		currentMessage = encryptArithmeticSequence(currentMessage, key2);
		
		Debug.LogFormat("[ƎNA Cipher #{0}] After encrypting with Arithmetic Sequence Cipher: {1}", moduleId, currentMessage);

		reversed = extPairs;

		Array.Reverse(reversed);

		encrypted = currentMessage;

		generatingEncryptedFlashSeq(encrypted);
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

	class ENACipherSettings
	{
		public bool DreamBBQMode = false;
	}

	private ENACipherSettings ENASettings = new ENACipherSettings();

	void Awake()
    {

		ModConfig<ENACipherSettings> config = new ModConfig<ENACipherSettings>("ENACipherSettings");
		ENASettings = config.Read();
		config.Write(ENASettings);
		dreamBBQMode = ENASettings.DreamBBQMode;

		moduleId = moduleIdCounter++;
		_enaCipherId = _enaCipherIdCounter++;

		foreach (KMSelectable letter in keyboard)
        {
			letter.OnInteract += delegate () { keyPress(letter); return false; };
        }

		foreach (KMSelectable arrow in speedButtons)
		{
			arrow.OnInteract += delegate () { speedButtonPress(arrow); return false; };
		}

		backSpace.OnInteract += delegate () { backSpacePress(); return false; };
		startButton.OnInteract += delegate () { startPress(); return false; };
		submit.OnInteract += delegate () { submitPress(); return false; };
		reset.OnInteract += delegate () { resetPress(); return false; };
		cbActive = Colorblind.ColorblindModeActive;
        Module.GetComponent<KMSelectable>().OnFocus += delegate { moduleSelected = true; };
        Module.GetComponent<KMSelectable>().OnDefocus += delegate { moduleSelected = false; };
    }

	private void OnDestroy()
    {
		_enaCipherIdCounter = 1;
    }

	void Start()
    {
		submissionDisplayText.text = "";
		for (int i = 0; i < 5; i++)
        {
			cbTexts[i].text = "";
        }

		border.material = dreamBBQMode ? borderTextures[1] : borderTextures[0];

		dreamIx = rnd.Range(0, 2);

		if (!assignedClip && !Application.isEditor)
		{
			assignedClip = PathManager.GetAssets<VideoClip>("ena").Single();
            specialClip = assignedClip;
            specialSolveOut.clip = specialClip;
        }
		else
		{
			specialSolveOut.clip = specialClip;
		}



        specialSolveOut.Prepare();

        StartCoroutine(startingScreen());
        wordGenerate();
    }


	void speedButtonPress(KMSelectable arrow)
	{
		Audio.PlaySoundAtTransform("Click", transform);
		arrow.AddInteractionPunch(0.4f);

		if (moduleSolved || !isActivated || disableAllButtons)
		{
			return;
		}

		for (int i = 0; i < 2; i++)
		{
			if (arrow == speedButtons[i])
			{
				switch (i)
				{
					case 0:
						speedIndex--;
						break;
					case 1:
						speedIndex++;
						break;
				}

				if (speedIndex < 0)
				{
					speedIndex = 0;
				}
				else if (speedIndex > 4)
				{
					speedIndex = 4;
				}
			}
		}
		changeSpeed();
	}

	void changeSpeed()
	{

		StopAllCoroutines();

		for (int i = 0; i < 5; i++)
		{
			speedLEDS[i].material = speedIndex == i ? speedLitLED : biosBootupScreenStuff[0];
			seqDisplays[i].material = biosBootupScreenStuff[0];
		}

		switch (speedIndex)
		{
			case 0:
				speed = -2;
				break;
			case 1:
				speed = -1;
				break;
			case 2:
				speed = 1;
				break;
			case 3:
				speed = 2;
				break;
			case 4:
				speed = 3;
				break;
		}

        StartCoroutine(flashingEncryptedSequence());
        StartCoroutine(flashingArithmeticKWSequence());
        StartCoroutine(flashingExtinctionSequence());
        StartCoroutine(flashingTemptationKWSequence());
        StartCoroutine(flashingSubSequence());
    }

	void keyPress(KMSelectable letter)
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		letter.AddInteractionPunch(0.4f);
		if (moduleSolved || disableAllButtons)
        {
			return;
        }
		if (submissionDisplayText.text.Length < 6)
        {
			submissionText += letter.GetComponentInChildren<TextMesh>().text;
			submissionDisplayText.text = submissionText;
        }
    }

	void backSpacePress()
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		backSpace.AddInteractionPunch(0.4f);
		if (moduleSolved || disableAllButtons)
        {
			return;
        }
		if (submissionDisplayText.text.Length > 0)
		{
			submissionText = submissionText.Remove(submissionText.Length - 1);
			submissionDisplayText.text = submissionText;
		}
    }

	void submitPress()
    {
		Audio.PlaySoundAtTransform("KeyPress", transform);
		submit.AddInteractionPunch(0.4f);
		if (moduleSolved || disableAllButtons)
        {
			return;
        }
		
		if (submissionText == word)
        {
			var normal = "You have spoken in the language of the Gods! Solved!";
			var special = "TURRŌN? TURRŌN!? TURRŌN!";

			StartCoroutine(specialSolve() ? specialSolveAnimation() : solveAnimation());

			Debug.LogFormat("[ƎNA Cipher #{0}] {1}", moduleId, specialSolve() ? special : normal);
        }
        else
        {
			StartCoroutine(strikeAnimation());
			Debug.LogFormat("[ƎNA Cipher #{0}] The word you inputted isn't up to our community's typical standards! Strike!", moduleId);
        }
    }

	IEnumerator specialSolveAnimation()
	{
		yield return null;

		var part = 0;

		disableAllButtons = true;

		Audio.PlaySoundAtTransform("SpecialSolve", transform);
		if (Bomb.GetTime() < 30)
		{
			lessTime = true;
			moduleSolved = true;
			Bomb.GetComponent<KMBombModule>().HandlePass();
		}

		submissionWindow.SetActive(false);
		window.SetActive(true);
		speedStuff.SetActive(false);
		
		while (part != 8)
		{
			for (var i = 0; i < 5; i++)
			{
				seqDisplays[i].material = letterPatterns[rnd.Range(0,5)];
			}
            part++;
			yield return new WaitForSeconds(0.6f);
		}
		for (var i = 0; i < 5; i++)
		{
			seqDisplays[i].material = biosBootupScreenStuff[0];
		}
		window.SetActive(false);
		specialSolveOut.Play();
		specialWindow.SetActive(true);
		yield return new WaitForSeconds(6.634f);
		specialWindow.SetActive(false);
		specialSolveOut.Stop();
        yield return new WaitForSeconds(0.8f);
        taskBar.SetActive(false);
        yield return new WaitForSeconds(1);
        screen.material = biosBootupScreenStuff[0];
        yield return new WaitForSeconds(1);
        screen.material = startupScreen;
        shutdownScreen[dreamBBQMode ? 1 : 0].SetActive(true);
        yield return new WaitForSeconds(2);
        shutdownScreen[dreamBBQMode ? 1 : 0].SetActive(false);
        screen.material = biosBootupScreenStuff[1];
        yield return new WaitForSeconds(0.8f);
        screen.material = biosBootupScreenStuff[0];
        if (!lessTime)
        {
            Module.GetComponent<KMBombModule>().HandlePass();
			moduleSolved = true;
        }

    }

	IEnumerator solveAnimation()
    {
		yield return null;
		submission = false;
		disableAllButtons = true;
		submissionDisplayText.text = "";
		string solveText = "YEAH!";
		int loop = 0;
		int pattern = 0;
		Audio.PlaySoundAtTransform("Solve", transform);

		if (Bomb.GetTime() < 30)
        {
			lessTime = true;
			moduleSolved = true;
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
		speedStuff.SetActive(false);
		while (loop != 18)
        {
			if (pattern == letterPatterns.Length) pattern = 0;
			for (int i = 0; i < seqDisplays.Length; i++)
            {
				seqDisplays[i].material = dreamBBQMode ? dreamLetterPatterns[pattern] : letterPatterns[pattern];
				yield return new WaitForSeconds(0.06f);
            }
			loop++;
			pattern++;
        }
		for (int i = 0; i < seqDisplays.Length; i++)
        {
			seqDisplays[i].material = dreamBBQMode ? dreamLetterPatterns[i] : letterPatterns[i];
			yield return new WaitForSeconds(0.06f);
        }
		yield return new WaitForSeconds(1);
		window.SetActive(false);
		yield return new WaitForSeconds(0.8f);
		taskBar.SetActive(false);
		yield return new WaitForSeconds(1);
		screen.material = biosBootupScreenStuff[0];
		yield return new WaitForSeconds(1);
		screen.material = dreamBBQMode ? dreamStartupScreens[dreamIx] : startupScreen;
		shutdownScreen[dreamBBQMode ? 1 : 0].SetActive(true);
		yield return new WaitForSeconds(2);
		shutdownScreen[dreamBBQMode ? 1 : 0].SetActive(false);
		screen.material = biosBootupScreenStuff[1];
		yield return new WaitForSeconds(0.8f);
		screen.material = biosBootupScreenStuff[0];
		if (!lessTime)
        {
			moduleSolved = true;
			Module.GetComponent<KMBombModule>().HandlePass();
		}


    }

	IEnumerator strikeAnimation()
    {
		yield return null;
		submission = false;
		disableAllButtons = true;
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
		disableAllButtons = false;
		yield return new WaitForSeconds(1);
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

		if (submission || !isActivated || moduleSolved || disableAllButtons)
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
		submission = false;
		submissionDisplayText.text = "";
		submissionWindow.SetActive(false);
		yield return new WaitForSeconds(1.5f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TitleMenuPressed, transform);
		window.SetActive(true);
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
		specialWindow.SetActive(false);

		yield return null;
		if (_enaCipherId == 1)
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
		screen.material = dreamBBQMode ? dreamStartupScreens[dreamIx] : startupScreen;
		startupStuff[dreamBBQMode ? 1 : 0].SetActive(true);
		yield return new WaitForSeconds(0.1f);
		if (!dreamBBQMode)
		{
            while (loadingBar != 8)
            {
                loadingBars[loadingBar].SetActive(true);
                loadingBar++;
                yield return new WaitForSeconds(0.2f);
            }
        }
		else
		{
			spinning = true;
			yield return new WaitForSeconds(1.6f);
		}
		yield return new WaitForSeconds(1);
		spinning = false;
		startupStuff[dreamBBQMode ? 1 : 0].SetActive(false);
		screen.material = biosBootupScreenStuff[0];
		yield return new WaitForSeconds(1);
		screen.material = backgroundScreen;
		yield return new WaitForSeconds(1);
		if (_enaCipherId == 1)
			Audio.PlaySoundAtTransform("OSBoot", transform);
		yield return new WaitForSeconds(1);
		startIcon.material = dreamBBQMode ? startIcons[1] : startIcons[0];
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
					seqDisplays[0].material = dreamBBQMode ? dreamLetterPatterns[encryptedFlashSequence[letterPos][letterFlashPos]] : letterPatterns[encryptedFlashSequence[letterPos][letterFlashPos]];
					cbTexts[0].text = cbActive && dreamBBQMode ? mainDreamCB[encryptedFlashSequence[letterPos][letterFlashPos]] : cbActive ? mainCB[encryptedFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
					seqDisplays[0].material = biosBootupScreenStuff[0];
					cbTexts[0].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
                }
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f / (1 + (0.1f * speed)));
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
					seqDisplays[1].material = dreamBBQMode ? dreamLetterPatterns[arithmeticKWFlashSequence[letterPos][letterFlashPos]] : letterPatterns[arithmeticKWFlashSequence[letterPos][letterFlashPos]];
					cbTexts[1].text = cbActive && dreamBBQMode ? mainDreamCB[arithmeticKWFlashSequence[letterPos][letterFlashPos]] : cbActive? mainCB[arithmeticKWFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
					seqDisplays[1].material = biosBootupScreenStuff[0];
					cbTexts[1].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
				}
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f / (1 + (0.1f * speed)));
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
					seqDisplays[2].material = dreamBBQMode ? dreamNumberPatterns[extinctionFlashSequence[numberPos][numberFlashPos]] : numberPatterns[extinctionFlashSequence[numberPos][numberFlashPos]];
					cbTexts[2].text = cbActive && dreamBBQMode ? numDreamCB[extinctionFlashSequence[numberPos][numberFlashPos]] : cbActive ? numCB[extinctionFlashSequence[numberPos][numberFlashPos]] : "";
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
					seqDisplays[2].material = biosBootupScreenStuff[0];
					cbTexts[2].text = "";
					numberFlashPos++;
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
                }
				numberPos++;
				numberFlashPos = 0;
				yield return new WaitForSeconds(0.7f / (1 + (0.1f * speed)));
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
					seqDisplays[3].material = dreamBBQMode ? dreamLetterPatterns[temptationKWFlashSequence[letterPos][letterFlashPos]] : letterPatterns[temptationKWFlashSequence[letterPos][letterFlashPos]];
					cbTexts[3].text = cbActive && dreamBBQMode ? mainDreamCB[temptationKWFlashSequence[letterPos][letterFlashPos]] : cbActive ? mainCB[temptationKWFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
					seqDisplays[3].material = biosBootupScreenStuff[0];
					cbTexts[3].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
				}
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f / (1 + (0.1f * speed)));
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
					seqDisplays[4].material =  dreamBBQMode ? dreamLetterPatterns[jSubFlashSequence[letterPos][letterFlashPos]] : letterPatterns[jSubFlashSequence[letterPos][letterFlashPos]];
					cbTexts[4].text = cbActive && dreamBBQMode ? mainDreamCB[jSubFlashSequence[letterPos][letterFlashPos]] : cbActive ? mainCB[jSubFlashSequence[letterPos][letterFlashPos]] : "";
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
					seqDisplays[4].material = biosBootupScreenStuff[0];
					cbTexts[4].text = "";
					letterFlashPos++;
					yield return new WaitForSeconds(0.3f / (1 + (0.1f * speed)));
                }
				letterPos++;
				letterFlashPos = 0;
				yield return new WaitForSeconds(0.7f / (1 + (0.1f * speed)));
            }
			yield return new WaitForSeconds(1);
        }
    }


	
	
	void Update()
    {
		setupClock();

		if (spinning)
		{
			dreamLoadingCircle.transform.Rotate(-Vector3.forward * 120 * Time.deltaTime);
		}

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
	private readonly string TwitchHelpMessage = "!{0} submit [insert answer here] to input your decrypted word. || !{0} cb to enable colorblind mode. || !{0} speed -/+ to increase or decrease speed of the flashing patterns. || !{0} dreambbq to change skin variant to Dream BBQ. Please\n" + 
		"note that once you've done this, you cannot revert back to the original skin!";
#pragma warning restore 414

	private int getCharIndex(char c)
    {
		return "QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c);
    }

	private string getModuleCode()
	{
		Transform closest = null;
		float closestDistance = float.MaxValue;

		foreach (Transform children in transform.parent)
		{
			var distance = (transform.position - children.position).magnitude;
			if (children.gameObject.name == "TwitchModule(Clone)" && (closest == null || distance < closestDistance))
			{
				closest = children;
				closestDistance = distance;
			}
		}

		return closest != null ? closest.Find("MultiDeckerUI").Find("IDText").GetComponent<UnityEngine.UI.Text>().text : null;
	}

	IEnumerator ProcessTwitchCommand (string command)
    {
		yield return null;
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		if (disableAllButtons)
		{
			yield return "sendtochaterror You cannot interact with the module at the moment!";
			yield break;
		}

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

		if (split[0].EqualsIgnoreCase("SPEED"))
		{
			if (!isActivated)
			{
				yield return "sendtochaterror OS is still booting. Please stand by before adjusting the speed.";
				yield break;
			}

			if (split.Length != 2)
			{
				yield break;
            }

			if (split[1].EqualsIgnoreCase("-"))
			{
				speedButtons[0].OnInteract();
				yield break;
			}
			else if (split[1].EqualsIgnoreCase("+"))
			{
				speedButtons[1].OnInteract();
				yield break;
			}
			else
			{
				yield return "sendtochaterror Please specify either + or - to increase or decrease the speed!";
			}
			yield break;
		}

        if (split[0].EqualsIgnoreCase("DREAMBBQ"))
        {
			if (!isActivated)
			{
				yield return "sendtochaterror The module is not activated yet! Please wait before you make the transition to Dream BBQ mode!";
				yield break;
			}

            else if (dreamBBQMode)
            {
                yield return "sendtochaterror You have already changed the look of the module!";
                yield break;
            }

            yield return string.Format("sendtochat Dream BBQ mode is now activated on Module {0} (ƎNA Cipher).", getModuleCode());
            dreamBBQMode = !dreamBBQMode;
            yield return new WaitForSeconds(1);
            yield return "sendtochat Please stand by...";
            StopAllCoroutines();
            for (int i = 0; i < 5; i++)
            {
                seqDisplays[i].material = biosBootupScreenStuff[0];
				cbTexts[i].text = "";
            }
            isActivated = false;
            taskBar.SetActive(false);
            window.SetActive(false);
            startIcon.material = startIcons[1];
            screen.material = biosBootupScreenStuff[0];
            Audio.PlaySoundAtTransform("Shutdown", transform);
            yield return new WaitForSeconds(3);
            border.material = borderTextures[1];
            Audio.PlaySoundAtTransform("ShortStartup", transform);
            yield return new WaitForSeconds(2);
            screen.material = biosBootupScreenStuff[1];
            yield return new WaitForSeconds(0.8f);
            screen.material = backgroundScreen;
            taskBar.SetActive(true);
            window.SetActive(true);
            yield return new WaitForSeconds(1);
            isActivated = true;
            StartCoroutine(flashingEncryptedSequence());
            StartCoroutine(flashingArithmeticKWSequence());
            StartCoroutine(flashingExtinctionSequence());
            StartCoroutine(flashingTemptationKWSequence());
            StartCoroutine(flashingSubSequence());
            yield break;
        }


        if (split.Length != 2 || !split[0].EqualsIgnoreCase("SUBMIT"))
        {
			yield break;
        }

		if (!isActivated)
        {
			yield return "sendtochaterror OS is still booting. Please stand by before submitting anything.";
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
			while (submissionDisplayText.text.Length != 0)
			{
				backSpace.OnInteract();
				yield return new WaitForSeconds(0.1f);
			}
        }
		int start = submission ? submissionDisplayText.text.Length : 0;

		if (!submission)
		{
			startButton.OnInteract();
		}
		for (int i = start; i < 6; i++)
        {
			keyboard[getCharIndex(word[i])].OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
		submit.OnInteract();
		
		while (!moduleSolved)
		{
			yield return true;
		}

		yield return null;
    }


}





