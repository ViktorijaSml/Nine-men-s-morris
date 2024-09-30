using Board;
using PlayerData;
using Tools;
using TMPro;
using UnityEngine;
using Message;

namespace GamePlay
{
    public enum GameState
    {
        PlacingPieces,
        MovingPieces,
        Flying,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public GameBoard _gameBoard;
        public GameState _currentState;
        public Player _player1, _player2;
        private Player _currentPlayer;
        private TextMeshProUGUI _player1Text, _player2Text, _turnInfo, _player1Info, _player2Info;

        public Player CurrentPlayer
        {
            get { return _currentPlayer;}
            private set { _currentPlayer = value;}
        }
        public GameState CurrentState
        {
            get { return _currentState; }
            private set { _currentState = value;}
        }
        private int TotalPieces; 
        void Start()
        {
            InitializeVariables();
            InitializeGame();
        }

        /// <summary>
        /// Update is called once per frame.
        /// Manages the different game states and transitions.
        /// </summary>
        void Update()
        {
            switch (CurrentState)
            {
                case GameState.PlacingPieces:
                    HandlePiecePlacement();
                    break;
                case GameState.MovingPieces:
                    HandlePieceMovement();
                    break;
                case GameState.Flying:
                    HandleFlying();
                    break;
                case GameState.GameOver:
                    // Game Over logic
                    break;
            }
        }
        /// <summary>
        /// Handles the logic for placing pieces on the board.
        /// </summary>
        private void HandlePiecePlacement()
        {
            if (CurrentPlayer.NumberOfAvailablePieces > 0 && Input.GetMouseButtonDown(0))
            {
               PlacePiece();
                SetText(Messages.PlacePiece, _turnInfo);
            }
            if (_player1.NumberOfAvailablePieces == 0 && _player2.NumberOfAvailablePieces == 0)
            {
                _player1.ReloadPieces();
                _player2.ReloadPieces();
                CurrentState = GameState.MovingPieces;
            }
        }

        /// <summary>
        /// Handles the logic for placing a piece on the game board. 
        /// Casts a ray from the mouse position to detect a game slot. If the selected slot is available, 
        /// the current player places a piece there. Checks if a mill has been formed, and if so, allows the player to remove an opponent's piece. 
        /// Switches the turn to the next player afterwards.
        /// </summary>
        private void PlacePiece()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                string slotKey = hit.collider.gameObject.name;
                Slot selectedSlot = Utils.GetSlotObject(slotKey);

                if (selectedSlot.IsSlotAvailable())
                {
                    selectedSlot.MoveHere(CurrentPlayer.PlayerName == "Player1" ? Ownership.Player1 : Ownership.Player2);
                    CurrentPlayer.PlacePiece();

                    if (CheckForMill(slotKey, CurrentPlayer))
                    {
                        Debug.Log(string.Format(Messages.MillFormed, CurrentPlayer));
                        RemovePiece();
                    }
                    SwitchPlayer();
                    return;
                }
                //play sound that indicates invalid move
            }
        }

        private void RemovePiece()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            SetText(Messages.RemovePiece, _turnInfo);

            if (hit.collider != null)
            {
                string slotKey = hit.collider.gameObject.name;
                Slot selectedSlot = Utils.GetSlotObject(slotKey);

                if (!selectedSlot.IsSlotAvailable() && !selectedSlot.OccupiedBy.ToString().Equals(CurrentPlayer.PlayerName))
                {
                    selectedSlot.RemovePiece();
                    return;
                }
                //play sound that indicates invalid move
            }
        }
        /// <summary>
        /// Checks if placing/moving to the current slot forms a mill.
        /// </summary>
        /// <param name="slotKey">The slot key.</param>
        /// <param name="player">The player who just placed/moved a piece.</param>
        /// <returns>True if a mill is formed, otherwise false.</returns>
        bool CheckForMill(string slotKey, Player player)
        {
            string[] slots = _gameBoard.GetConnectedSlots(slotKey);
            if (slots == null) return false;

            Ownership playerOwnership = player.PlayerName == "Player1" ? Ownership.Player1 : Ownership.Player2;

            Slot[] connectedSlots = new Slot[4];
            int count = 0;
            for (int i = 0; i<4; i++)
            {
                connectedSlots[i] = Utils.GetSlotObject(slots[i]);
                if (connectedSlots[i] != null && connectedSlots[i].OccupiedBy.Equals(playerOwnership))
                {
                    count++;
                    if (count == 2)
                    {
                        return true;
                    }
                    if (i == 1)
                    {
                        count = 0;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Handles the logic for moving pieces during the MovingPieces phase.
        /// </summary>
        void HandlePieceMovement()
        {
            SetText(Messages.MovePiece, _turnInfo);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    string slotKey = hit.collider.gameObject.name;
                    Slot selectedSlot = Utils.GetSlotObject(slotKey);

                    // Check if the player is moving their own piece
                    if (selectedSlot.OccupiedBy == (CurrentPlayer.PlayerName == "Player1" ? Ownership.Player1 : Ownership.Player2))
                    {
                        // Player selected their piece to move
                      //  HandleSlotSelectionForMovement(selectedSlot);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the logic for moving pieces when a player has only 3 pieces and can "fly".
        /// </summary>
        void HandleFlying()
        {
            SetText(Messages.Fly, _turnInfo);

            // Flying phase logic: allow the player to move to any empty slot
        }
        /// <summary>
        /// Initializes the game by setting up players, board, and starting the game in the placing phase.
        /// </summary>
        private void InitializeGame(int numberOfRings = 3)
        {
            _gameBoard = new GameBoard(numberOfRings); 
            TotalPieces = numberOfRings*3;
            BoardManager.instance.DrawBoard(_gameBoard);

            _player1 = new Player(Ownership.Player1, TotalPieces);
            _player2 = new Player(Ownership.Player2, TotalPieces);
            CurrentPlayer = _player1;

            CurrentState = GameState.PlacingPieces;
        }

        /// <summary>
        /// Initializes UI components by finding and assigning references to TextMeshProUGUI objects 
        /// for turn information, player 1, and player 2. Also performs null checks on the components.
        /// </summary>
        private void InitializeVariables()
        {
            _turnInfo = GameObject.FindGameObjectWithTag("TurnInfo").transform.Find("TextInfo").GetComponent<TextMeshProUGUI>();
            Utils.ComponentNullCheck( _turnInfo );

            Transform player1Obj = GameObject.FindGameObjectWithTag("PlayerOne").transform;
            Transform player2Obj = GameObject.FindGameObjectWithTag("PlayerTwo").transform;

            _player1Text = player1Obj.Find("TextPlayer1").GetComponent<TextMeshProUGUI>();
            Utils.ComponentNullCheck(_player1Text);

            _player2Text = player2Obj.Find("TextPlayer2").GetComponent<TextMeshProUGUI>();
            Utils.ComponentNullCheck(_player2Text);

            _player1Info = player2Obj.Find("TextInfo").GetComponent<TextMeshProUGUI>();
            Utils.ComponentNullCheck(_player1Info);

            _player2Info = player2Obj.Find("TextInfo").GetComponent<TextMeshProUGUI>();
            Utils.ComponentNullCheck(_player2Info);
        }

        /// <summary>
        /// Switches the current player between player 1 and player 2. Updates the UI text 
        /// to reflect whose turn it is by calling the SetText method.
        /// </summary>
        private void SwitchPlayer()
        {
            if (CurrentPlayer == _player1)
            {
                CurrentPlayer = _player1;
                SetText(Messages.TurnInfo, _player1Info, CurrentPlayer);
            }
            else
            {
                CurrentPlayer = _player2;
                SetText(Messages.TurnInfo, _player2Info, CurrentPlayer);
            }
        }

        /// <summary>
        /// Updates a TextMeshProUGUI component with a specified message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="textObj">The UI text object to update.</param>
        private static void SetText(string message, TextMeshProUGUI textObj)
        {
            string text = string.Format(message);
            textObj.text = text;
        }

        /// <summary>
        /// Updates a TextMeshProUGUI component with a message that includes player-specific information.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="textObj">The UI text object to update.</param>
        /// <param name="player">The player whose information is included in the message.</param>

        private static void SetText(string message, TextMeshProUGUI textObj, Player player)
        {
            string text = string.Format(message, player);
            textObj.text = text;
        }
    }
}

