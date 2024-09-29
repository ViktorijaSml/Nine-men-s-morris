using UnityEngine;

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

    /// <summary>
    /// Represents a slot on the game board, containing information about its owner and unique key.
    /// </summary>
    public class Slot 
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
        public void RemovePiece() => OccupiedBy = Ownership.Nobody;

        /// <summary>
        /// Checks if the slot is available for a player to move to (i.e., unoccupied).
        /// </summary>
        /// <returns>True if the slot is unoccupied, false otherwise.</returns>
        public bool IsSlotAvailable() => OccupiedBy != Ownership.Nobody;

        /// <summary>
        /// Attempts to move a player to this slot.
        /// If the slot is already occupied, the move fails.
        /// </summary>
        /// <param name="player">The player attempting to move to this slot.</param>
        /// <returns>True if the move is successful, false if the slot is already occupied.</returns>
        public bool MoveTo(Ownership player)
        {
            if (!IsSlotAvailable())
            {
                Debug.Log($"Cannot move there. Slot {SlotKey} is occupied!");
                return false;
            }
            OccupiedBy = player;
            Debug.Log($"Moved {player} to slot {SlotKey}");
            return true;
        }
    }
}
