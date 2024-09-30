namespace Message
{
    /// <summary>
    /// Static class that handles all messages present in the game.
    /// </summary>
    public static class Messages
    {
        // Player turn messages
        public static readonly string TurnInfo = "{0}'s turn";
        public static readonly string ResetTurnInfo = "{0}";

        // Game state messages
        public static readonly string GameOver = "Game Over! {0} has won!";
        public static readonly string Draw = "It's a draw! No more valid moves!";

        // Action messages
        public static readonly string PiecePlaced = "{0} placed a piece on {1}.";
        public static readonly string PieceMoved = "{0} moved a piece to {1}.";

        // Mill messages
        public static readonly string MillFormed = "{0} - Mill!";
        public static readonly string Mill = "{0} has formed a mill!";

        // Error or invalid action messages
        public static readonly string InvalidMove = "Invalid move! Try again.";
        public static readonly string SlotOccupied = "$Cannot move there. Slot {SlotKey} is occupied!";

        //Instruction messages
        public static readonly string PlacePiece = "Place a piece on available slot";
        public static readonly string MovePiece = "Move a piece to the connected slot";
        public static readonly string Fly = "Move a piece to an available slot";
        public static readonly string RemovePiece = "Remove other player's piece";



    }
}
