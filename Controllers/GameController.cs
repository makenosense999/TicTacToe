using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TicTacToe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        [HttpPost]
        public IActionResult Move([FromBody] Move move)
        {
            var gameBoard = new GameBoard();
            if (!System.IO.File.Exists("data.json"))
            {
                gameBoard.Save();
            }
            else
            {
                gameBoard = GameBoard.Load();
            }

            var board = gameBoard.Board;
            var currentPlayer = gameBoard.CurrentPlayer;

            if (move.Row < 0 || move.Row > 2 || move.Column < 0 || move.Column > 2)
            {
                return BadRequest("Invalid move. Row and column values must be between 0 and 2.");
            }

            if (board[move.Row][move.Column] != null)
            {
                return BadRequest("Invalid move. That spot is already taken.");
            }

            board[move.Row][move.Column] = new Step
            {
                Player = currentPlayer,
            };

            bool hasWon = false;

            //ROWS
            for (int i = 0; i < 3; i++)
            {
                Step firstStep = board[i][0];

                if (firstStep == null)
                {
                    break;
                }

                var isRowWon = false;

                for (int j = 1; j < 3; j++)
                {
                    Step step = board[i][j];

                    if (step == null)
                    {
                        isRowWon = false;

                        break;
                    }

                    if (step.Player != firstStep.Player) 
                    {
                        isRowWon = false;

                        break;
                    }

                    isRowWon = true;
                }

                if (isRowWon)
                {
                    gameBoard.ResetBoard();
                    gameBoard.Save();

                    return Ok($"Player {currentPlayer} has won!");
                }
            }

            //COLUMNS
            for (int i = 0; i < 3; i++)
            {
                Step firstStep = board[0][i];

                if (firstStep == null)
                {
                    break;
                }

                for (int j = 1; j < 3; j++)
                {
                    Step step = board[j][i];

                    if (step == null)
                    {
                        hasWon = false;

                        break;
                    }

                    if (step.Player != firstStep.Player)
                    {
                        hasWon = false;

                        break;
                    }

                    hasWon = true;
                }

                if (hasWon)
                {
                    gameBoard.ResetBoard();
                    gameBoard.Save();

                    return Ok($"Player {currentPlayer} has won!");
                }
            }

            //DIAGONALS
            for (int i = 0; i < 2; i++)
            {
                Step firstStep = board[i][i];
                Step nextStep = board[i + 1][i + 1];

                if (firstStep == null)
                {
                    hasWon = false;
                 
                    break;
                }

                if (nextStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (firstStep.Player != nextStep.Player)
                {
                    hasWon = false;

                    break;
                }

                hasWon = true;
            }
            
            if (hasWon)
            {
                gameBoard.ResetBoard();
                gameBoard.Save();

                return Ok($"Player {currentPlayer} has won!");
            }


            for (int c = 2, r = 0; c > 0; c--, r++)
            {
                Step firstStep = board[r][c];
                Step nextStep = board[r + 1][c - 1];

                if (firstStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (nextStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (firstStep.Player != nextStep.Player)
                {
                    hasWon = false;

                    break;
                }

                hasWon = true;
            }

            if (hasWon)
            {
                gameBoard.ResetBoard();
                gameBoard.Save();

                return Ok($"Player {currentPlayer} has won!");
            }

            bool isTie = true;
            foreach (List<Step> rows in board)
            {
                foreach (Step step in rows)
                {
                    if (step == null)
                    {
                        isTie = false;
                        break;
                    }
                }
                if (!isTie)
                {
                    break;
                }
            }
            if (isTie)
            {
                gameBoard.ResetBoard();
                gameBoard.Save();

                return Ok("The game ended in a tie.");
            }

            gameBoard.SwitchPlayer();
            gameBoard.Save();

            return Ok("Move successful");
        }
    }

    class Step
    {
        public string Player { get; set; }
    }

    class GameBoard
    {
        public GameBoard()
        {
            ResetBoard();
        }


        public List<List<Step>> Board { get; set; }
        public string CurrentPlayer { get; set; } 


        public void ResetBoard()
        {
            Board = new List<List<Step>> {
                new List<Step> { null, null, null },
                new List<Step> { null, null, null },
                new List<Step> { null, null, null },
            };
            CurrentPlayer = "X";
        }

        public void SwitchPlayer()
        {
            if (CurrentPlayer == "X")
            {
                CurrentPlayer = "O";
            }
            else
            {
                CurrentPlayer = "X";
            }
        }

        public void Save()
        {
            var newJsonData = JsonSerializer.Serialize(this);
            File.WriteAllText("data.json", newJsonData);
        }

        public static GameBoard Load()
        {
            string json = File.ReadAllText("data.json");
            
            return JsonSerializer.Deserialize<GameBoard>(json);
        }
    }
}

