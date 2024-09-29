using Board;
using UnityEngine;

namespace PlayerData
{
    /// <summary>
    /// Represents a player in the game, managing the player's name and pieces.
    /// </summary>
    public class Player
    {
        private string _playerName;
        private int _numberOfPieces;
        private int _numberOfAvailablePieces;

        public string PlayerName
        {
            get { return _playerName; }
            private set { _playerName = value; }
        }
        public int NumberOfPieces
        {
            get { return _numberOfPieces; }
            set { _numberOfPieces = value; }
        }
        public int NumberOfAvailablePieces
        {
            get { return _numberOfAvailablePieces; }
            set { _numberOfAvailablePieces = value; }
        }
        public Player(Ownership playerName, int numberOfPieces)
        {
            PlayerName = playerName.ToString();
            NumberOfPieces = numberOfPieces;
            NumberOfAvailablePieces = numberOfPieces;
        }

        /// <summary>
        /// Decreases the player's available pieces by 1 when they place a piece on the board.
        /// Logs an error if the player has no pieces left.
        /// </summary>
        public void PlacePiece()
        {
            if (NumberOfAvailablePieces == 0)
            {
                Debug.LogError($"{PlayerName} has no pieces left.");
                return;
            }
            NumberOfAvailablePieces--;
        }

        /// <summary>
        /// Resets the player's available pieces to the total number of pieces.
        /// </summary>
        public void ReloadPieces() => NumberOfAvailablePieces = NumberOfPieces;
    }
}
