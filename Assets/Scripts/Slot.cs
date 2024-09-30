using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    /// <summary>
    /// Enum representing the ownership state of a slot.
    /// </summary>
    public enum Ownership
    {
        Player1,
        Player2,
        Nobody
    }
    public enum PieceVisibility
    {
        Show, 
        Hide
    }
    /// <summary>
    /// Represents a slot on the game board, containing information about its owner and unique key.
    /// </summary>
    public class Slot : MonoBehaviour
    {
        private Ownership _occupiedBy;
        private string _slotKey;
        public Ownership OccupiedBy
        {
            get { return _occupiedBy; }
            set { _occupiedBy = value; } 
        }
        public string SlotKey
        {
            get
            { return _slotKey;}
            private set { _slotKey = value; }
        }

        public Slot(string slotKey)
        {
            SlotKey = slotKey;
            OccupiedBy = Ownership.Nobody;
        }

        /// <summary>
        /// Resets the slot, marking it as unoccupied.
        /// </summary>
        public void RemovePiece()
        {
            OccupiedBy = Ownership.Nobody;
            TogglePieceVisibility(PieceVisibility.Hide);
        }

        /// <summary>
        /// Checks if the slot is available for a player to move to (i.e., unoccupied).
        /// </summary>
        /// <returns>True if the slot is unoccupied, false otherwise.</returns>
        public bool IsSlotAvailable() => OccupiedBy != Ownership.Nobody;

        /// <summary>
        /// Attempts to move a player to this slot.
        /// </summary>
        /// <param name="player">The player attempting to move to this slot.</param>
        public void MoveHere(Ownership player)
        {
            OccupiedBy = player;
            TogglePieceVisibility(PieceVisibility.Show);
            Debug.Log($"Moved {player} to slot {SlotKey}");
        }

        /// <summary>
        /// Sets the color of the game piece to the specified player's color, 
        /// but makes the piece fully transparent by setting the alpha value to 0.
        /// </summary>
        /// <param name="playerColor">The color to set for the player's piece.</param>
        public void SetPieceColor(Color playerColor)
        {
            Image piece = transform.GetChild(0).GetComponent<Image>();
            Utils.ComponentNullCheck(piece);
            playerColor.a = 0;
            piece.color = playerColor;
        }

        /// <summary>
        /// Toggles the visibility of the piece by adjusting its alpha value. 
        /// Makes the piece fully visible or fully transparent based on the provided visibility option.
        /// </summary>
        /// <param name="visibility">The visibility state (Show or Hide) to toggle the piece.</param>

        private void TogglePieceVisibility(PieceVisibility visibility)
        {
            Image piece = transform.GetChild(0).GetComponent<Image>();
            Utils.ComponentNullCheck(piece);
            Color color = piece.color;

            if (visibility == PieceVisibility.Show)
            {
                color = piece.color;
                color.a = 1;
            }
            else
            {
                color.a = 0;
            }
        }

    }
}
