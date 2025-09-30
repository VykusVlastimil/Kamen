using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        bool exitProgram = false;

        while (!exitProgram)
        {
            Console.Clear();
            DisplayMainMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PlayRockPaperScissors();
                    break;
                case "2":
                    PlayHangman();
                    break;
                case "3":
                    PlayTicTacToe();
                    break;
                case "4":
                    exitProgram = true;
                    Console.WriteLine("Thanks for playing! Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("=== WELCOME TO THE GAME COLLECTION ===");
        Console.WriteLine("1. Rock Paper Scissors");
        Console.WriteLine("2. Hangman");
        Console.WriteLine("3. Tic Tac Toe");
        Console.WriteLine("4. Exit");
        Console.Write("Choose a game (1-4): ");
    }

    // ==================== ROCK PAPER SCISSORS ====================
    static void PlayRockPaperScissors()
    {
        bool playAgain = true;

        while (playAgain)
        {
            Console.Clear();
            Console.WriteLine("=== ROCK PAPER SCISSORS ===");

            Random random = new Random();
            string playerChoice;
            string computerChoice;

            // Get player choice with validation
            while (true)
            {
                Console.Write("Enter ROCK, PAPER, or SCISSORS: ");
                playerChoice = Console.ReadLine().ToUpper();

                if (playerChoice == "ROCK" || playerChoice == "PAPER" || playerChoice == "SCISSORS")
                    break;
                else
                    Console.WriteLine("Invalid choice! Please try again.");
            }

            // Computer choice
            switch (random.Next(1, 4))
            {
                case 1: computerChoice = "ROCK"; break;
                case 2: computerChoice = "PAPER"; break;
                case 3: computerChoice = "SCISSORS"; break;
                default: computerChoice = "ROCK"; break;
            }

            Console.WriteLine($"\nPlayer: {playerChoice}");
            Console.WriteLine($"Computer: {computerChoice}");
            Console.WriteLine("");

            // Determine winner
            switch (playerChoice)
            {
                case "ROCK":
                    if (computerChoice == "ROCK")
                        Console.WriteLine("It's a draw!");
                    else if (computerChoice == "PAPER")
                        Console.WriteLine("You lose!");
                    else
                        Console.WriteLine("You win!");
                    break;
                case "PAPER":
                    if (computerChoice == "ROCK")
                        Console.WriteLine("You win!");
                    else if (computerChoice == "PAPER")
                        Console.WriteLine("It's a draw!");
                    else
                        Console.WriteLine("You lose!");
                    break;
                case "SCISSORS":
                    if (computerChoice == "ROCK")
                        Console.WriteLine("You lose!");
                    else if (computerChoice == "PAPER")
                        Console.WriteLine("You win!");
                    else
                        Console.WriteLine("It's a draw!");
                    break;
            }

            playAgain = AskToPlayAgain("Rock Paper Scissors");
        }
    }

    // ==================== HANGMAN ====================
    static void PlayHangman()
    {
        bool playAgain = true;

        while (playAgain)
        {
            Console.Clear();
            Console.WriteLine("=== HANGMAN ===");

            // ========== WORD LIST - ADD YOUR WORDS HERE ==========
            List<string> wordList = new List<string>
            {
                "PROGRAMMING", "COMPUTER", "KEYBOARD", "DEVELOPER",
                "SOFTWARE", "ALGORITHM", "DATABASE", "INTERNET",
                "KEYBOARD", "MONITOR", "NETWORK", "SECURITY"
            };
            // =====================================================

            Random random = new Random();
            string wordToGuess = wordList[random.Next(wordList.Count)];
            char[] guessedLetters = new char[wordToGuess.Length];
            List<char> incorrectGuesses = new List<char>();
            int attemptsLeft = 6;

            // Initialize guessed letters with underscores
            for (int i = 0; i < guessedLetters.Length; i++)
            {
                guessedLetters[i] = '_';
            }

            bool wordGuessed = false;

            while (attemptsLeft > 0 && !wordGuessed)
            {
                Console.WriteLine($"\nAttempts left: {attemptsLeft}");
                Console.WriteLine($"Word: {string.Join(" ", guessedLetters)}");
                Console.WriteLine($"Incorrect guesses: {string.Join(", ", incorrectGuesses)}");
                Console.Write("Guess a letter: ");

                string input = Console.ReadLine().ToUpper();

                if (input.Length != 1 || !char.IsLetter(input[0]))
                {
                    Console.WriteLine("Please enter a single letter!");
                    continue;
                }

                char guess = input[0];

                if (guessedLetters.Contains(guess) || incorrectGuesses.Contains(guess))
                {
                    Console.WriteLine("You already guessed that letter!");
                    continue;
                }

                if (wordToGuess.Contains(guess))
                {
                    for (int i = 0; i < wordToGuess.Length; i++)
                    {
                        if (wordToGuess[i] == guess)
                        {
                            guessedLetters[i] = guess;
                        }
                    }

                    if (!guessedLetters.Contains('_'))
                    {
                        wordGuessed = true;
                    }
                }
                else
                {
                    incorrectGuesses.Add(guess);
                    attemptsLeft--;
                }
            }

            Console.Clear();
            Console.WriteLine("=== HANGMAN RESULTS ===");
            Console.WriteLine($"The word was: {wordToGuess}");

            if (wordGuessed)
            {
                Console.WriteLine("Congratulations! You won!");
            }
            else
            {
                Console.WriteLine("Game over! You lost!");
            }

            playAgain = AskToPlayAgain("Hangman");
        }
    }

    // ==================== TIC TAC TOE ====================
    static void PlayTicTacToe()
    {
        bool playAgain = true;

        while (playAgain)
        {
            Console.Clear();
            Console.WriteLine("=== TIC TAC TOE ===");

            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char currentPlayer = 'X';
            bool gameWon = false;
            int moves = 0;

            while (moves < 9 && !gameWon)
            {
                DisplayTicTacToeBoard(board);
                Console.WriteLine($"Player {currentPlayer}'s turn.");
                Console.Write("Choose a position (1-9): ");

                string input = Console.ReadLine();

                if (int.TryParse(input, out int position) && position >= 1 && position <= 9)
                {
                    if (board[position - 1] != 'X' && board[position - 1] != 'O')
                    {
                        board[position - 1] = currentPlayer;
                        moves++;

                        gameWon = CheckTicTacToeWin(board, currentPlayer);

                        if (!gameWon)
                        {
                            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
                        }
                    }
                    else
                    {
                        Console.WriteLine("That position is already taken! Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Please enter a number between 1-9. Press any key to continue...");
                    Console.ReadKey();
                }

                Console.Clear();
                Console.WriteLine("=== TIC TAC TOE ===");
            }

            DisplayTicTacToeBoard(board);

            if (gameWon)
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
            }
            else
            {
                Console.WriteLine("It's a draw!");
            }

            playAgain = AskToPlayAgain("Tic Tac Toe");
        }
    }

    static void DisplayTicTacToeBoard(char[] board)
    {
        Console.WriteLine("\n " + board[0] + " | " + board[1] + " | " + board[2]);
        Console.WriteLine("-----------");
        Console.WriteLine(" " + board[3] + " | " + board[4] + " | " + board[5]);
        Console.WriteLine("-----------");
        Console.WriteLine(" " + board[6] + " | " + board[7] + " | " + board[8]);
        Console.WriteLine();
    }

    static bool CheckTicTacToeWin(char[] board, char player)
    {
        // Check rows
        for (int i = 0; i < 9; i += 3)
        {
            if (board[i] == player && board[i + 1] == player && board[i + 2] == player)
                return true;
        }

        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[i] == player && board[i + 3] == player && board[i + 6] == player)
                return true;
        }

        // Check diagonals
        if (board[0] == player && board[4] == player && board[8] == player)
            return true;
        if (board[2] == player && board[4] == player && board[6] == player)
            return true;

        return false;
    }

    // ==================== HELPER METHODS ====================
    static bool AskToPlayAgain(string gameName)
    {
        while (true)
        {
            Console.WriteLine($"\nWhat would you like to do?");
            Console.WriteLine($"1. Play {gameName} again");
            Console.WriteLine($"2. Return to main menu");
            Console.WriteLine($"3. Exit program");
            Console.Write("Choose an option (1-3): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    return true; // Play same game again
                case "2":
                    return false; // Return to main menu
                case "3":
                    Console.WriteLine("Thanks for playing! Goodbye!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.");
                    break;
            }
        }
    }
}