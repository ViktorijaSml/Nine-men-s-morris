using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents the logic of a game board, which consists of a 2D grid of slots.
    /// </summary>
    public class GameBoard 
    {
        private Dictionary<string, Vector2> _validBoardSlots; // Holds valid board slots and their positions in a canvas.
        private readonly int[,] _allBoardSlots; // 2D array representing all board slots where 1 represents a valid slot.
        [SerializeField] private readonly int _numberOfRings; // Number of rings (squares) on the board.
        [SerializeField] private readonly int _boardSize; // Total size of the board, used for 2D array. Similar to the Length propery of an array.
        private float _spacing; // Spacing between slots.

        public int NumberOfRings { 
            get 
            {
                return _numberOfRings;
            } 
        }
        public int BoardSize
        {
            get
            {
                return _boardSize;
            }
        }
        public int[,] AllBoardSlots
        {
            get
            {
                return _allBoardSlots;
            }
        }
        public Dictionary<string, Vector2> ValidBoardSlots {
            get
            {
                return _validBoardSlots;
            }
            private set
            {
                _validBoardSlots = value;
            }
        }
        public float SpacingBetweenSlots {
            get
            {
                return _spacing;
            }
            private set
            {
                _spacing = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the GameBoard class with a specified number of rings (squares).
        /// </summary>
        /// <param name="numberOfRings">The number of rings on the board. Must be at least 1.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when numberOfRings is less than 1.</exception>
        public GameBoard(int numberOfRings = 2) 
        {
            if (numberOfRings < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfRings), "Number of rings must be at least 1.");
            }
            _numberOfRings = numberOfRings;
            _validBoardSlots = new Dictionary<string, Vector2>();
            _boardSize = numberOfRings * 2 + 1;
            _allBoardSlots = new int[_boardSize, _boardSize];
        }

        /// <summary>
        /// Initializes the board by determining valid slots and their positions based on the canvas size.
        /// </summary>
        /// <param name="canvas">The RectTransform of the canvas the board will be displayed on.</param>
        public void InitializeBoard(RectTransform canvas)
        {
            Vector2 canvasSize = canvas.sizeDelta;
            try
            {
                // calculates all necessary properties of the board by checking each slot in the AllBoardSlots 
                for (int row = 0; row < BoardSize; row++)
                {
                    for (int column = 0; column < BoardSize; column++)
                    {
                        if (IsValidSlot(row, column, BoardSize))
                        {
                            _allBoardSlots[row, column] = 1;

                            string slotCode = row.ToString() + "," + column.ToString();

                            SpacingBetweenSlots = CalculateSpacingBetweenSlots(BoardSize, canvasSize);
                            Vector2 slotPosition = CalculateSlotPosition(row, column, BoardSize, canvasSize);
                            ValidBoardSlots.Add(slotCode, slotPosition);
                        }
                        else
                        {
                            _allBoardSlots[row, column] = 0;
                        }
                    }
                }
                this.ValidBoardSlots = _validBoardSlots;
            }
            catch (Exception e) 
            {
                    Debug.LogError("Failed to initialize board. Reason: " + e.ToString());
            }
            Debug.Log($"Succesfully initialized board with {NumberOfRings} rings.");
        }

        /// <summary>
        /// Calculates the position on the canvas of a specific slot on the board.
        /// </summary>
        /// <param name="row">The row index of the slot.</param>
        /// <param name="column">The column index of the slot.</param>
        /// <param name="boardSize">The size of the board.</param>
        /// <param name="canvasSize">The size of the canvas.</param>
        /// <returns>A Vector2 representing the position of the slot.</returns>
        private static Vector2 CalculateSlotPosition(int row, int column, int boardSize, Vector2 canvasSize)
        {
            canvasSize = ClampCanvasSize(canvasSize, 1080f, 1920f);
            try
            {
                CheckRowAndColumnValidity(row, column, boardSize);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            float spacing = CalculateSpacingBetweenSlots(boardSize, canvasSize);
            float xOffset = (column - (boardSize - 1) / 2) * spacing + canvasSize.x / 2;
            float yOffset = (row - (boardSize - 1) / 2) * spacing + canvasSize.y / 2;

            return new Vector2(xOffset, yOffset);
         
        }

        /// <summary>
        /// Calculates the spacing between slots based on the board size and canvas size.
        /// </summary>
        /// <param name="boardSize">The size of the board.</param>
        /// <param name="canvasSize">The size of the canvas.</param>
        /// <returns>The calculated spacing as a float.</returns>
        private static float CalculateSpacingBetweenSlots(int boardSize, Vector2 canvasSize)
        {
            canvasSize = ClampCanvasSize(canvasSize, 1080f, 1920f);
            return   Mathf.Min(canvasSize.x, canvasSize.y) / (boardSize + 1);
        }

        /// <summary>
        /// Clamps the canvas size to ensure it falls within reasonable width and height limits.
        /// </summary>
        /// <param name="canvasSize">The current size of the canvas.</param>
        /// <param name="maxHeight">The maximum height allowed for the canvas.</param>
        /// <param name="maxWidth">The maximum width allowed for the canvas.</param>
        /// <returns>A Vector2 containing the clamped canvas size.</returns>
        private static Vector2 ClampCanvasSize(Vector2 canvasSize, float maxHeight, float maxWidth) => 
            new Vector2(Mathf.Clamp(canvasSize.x, 100f, maxWidth), Mathf.Clamp(canvasSize.y, 100f, maxHeight));

        /// <summary>
        /// Determines whether a specific slot is valid based on its row and column indices.
        /// </summary>
        /// <param name="row">The row index of the slot.</param>
        /// <param name="column">The column index of the slot.</param>
        /// <param name="boardSize">The size of the board.</param>
        /// <returns>True if the slot is valid; otherwise, false.</returns>
        private static bool IsValidSlot(int row, int column, int boardSize)
        {
            try
            {
                CheckRowAndColumnValidity(row, column, boardSize);
            }
            catch (Exception e) 
            {
                Debug.LogError(e.Message);
            }

            if (!(row == (boardSize - 1) / 2 && row == column)) //exclude the slot in the middle
            {
                return (row == column || row + column == boardSize - 1 || //include the diagonal from upper right to bottom left corner
                            (boardSize - 1) / 2 == row || (boardSize - 1) / 2 == column); //include the diagonal from upper left to bottom right corner
            }
            else return false;
        }

        /// <summary>
        /// Checks the validity of row and column indices against the board size.
        /// </summary>
        /// <param name="row">The row index to check.</param>
        /// <param name="column">The column index to check.</param>
        /// <param name="boardSize">The size of the board.</param>
        /// <exception cref="ArgumentException">Thrown when row or column indices are out of bounds.</exception>
        private static void CheckRowAndColumnValidity(int row, int column, int boardSize)
        {
            if (!((row >= 0 && row < boardSize) && (column >= 0 && column < boardSize)))
            {
                throw new ArgumentException("Row or column doesn't match the size of the board.");
            }
        }
    }
}