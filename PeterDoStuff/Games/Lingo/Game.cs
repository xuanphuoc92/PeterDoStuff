using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Games.Lingo
{
    public class Game
    {
        public string SecretWord { get; private set; }
        public List<Guess> Guesses { get; private set; }
        public string CurrentGuess { get; private set; }
        public bool IsGameOver { get; private set; }
        public Dictionary<char, GuessState> AlphabetStates { get; private set; }

        public const int MAX_GUESS = 6;
        public const int WORD_LENGTH = 5;

        public Game(string secretWord)
        {
            Guesses = new List<Guess>();
            CurrentGuess = "";
            IsGameOver = false;
            AlphabetStates = new Dictionary<char, GuessState>();
            SecretWord = secretWord;
        }

        public Game() : this(WordPool.GetRandomGuessWord())
        {
        }

        public Game Input(string word)
        {
            foreach (char c in word)
                Input(c);
            return this;
        }

        public Game Input(char c)
        {
            if (IsGameOver || CurrentGuess.Length == WORD_LENGTH)
                return this;

            CurrentGuess += char.ToLower(c);
            return this;
        }

        public void Backspace()
        {
            if (CurrentGuess.Length > 0)
                CurrentGuess = CurrentGuess.Substring(0, CurrentGuess.Length - 1);
        }

        public GuessResult EnterGuess()
        {
            if (CurrentGuess.Length < WORD_LENGTH || WordPool.IsValid(CurrentGuess) == false)
                return GuessResult.Invalid;

            Guess guess = new Guess(CurrentGuess);
            CurrentGuess = "";
            for (int cIndex = 0; cIndex < SecretWord.Length; cIndex++)
            {
                char secretChar = SecretWord[cIndex];
                char guessChar = guess.Word[cIndex];
                GuessState guessState;
                if (secretChar == guessChar)
                    guessState = GuessState.Match;
                else if (SecretWord.Contains(guessChar))
                    guessState = GuessState.Misplace;
                else
                    guessState = GuessState.Miss;
                AlphabetStates[guessChar] = guessState;
                guess.States.Add(guessState);
            }

            Guesses.Add(guess);

            IsGameOver = SecretWord == guess.Word || Guesses.Count == MAX_GUESS;

            return SecretWord == guess.Word ? GuessResult.Correct : GuessResult.Incorrect;
        }
    }

    public class Guess
    {
        public string Word { get; private set; }
        public List<GuessState> States { get; private set; } = new List<GuessState>();

        public Guess(string word)
        {
            Word = word;
        }
    }

    public enum GuessState
    {
        Miss, Misplace, Match
    }

    public enum GuessResult
    {
        Invalid, Incorrect, Correct, GameOver
    }
}
