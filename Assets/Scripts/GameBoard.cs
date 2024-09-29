using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Unity.VisualScripting;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents the logic of a game board, which consists of a 2D grid of slots.
    /// </summary>
    public class GameBoard 
    {
        private Dictionary<string, Vector2> _validBoardSlots; // Holds valid board slots and their positions in a canvas.
        private readonly bool[,] _allBoardSlots; // 2D array representing all board slots where 1 represents a valid slot.
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
        public bool[,] AllBoardSlots
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
            _allBoardSlots = new bool[_boardSize, _boardSize];
        }

        /// <summary>
        /// Returns the 4 slots connected to the current slot (up, down, left, right).
        /// </summary>
        /// <param name="currentSlot">The key representing the current slot.</param>
        /// <returns>Array of connected slots, or null if the slot is invalid. For a non-existing connection, returns null in the proper index of the array.</returns>
        public string[] GetConnectedSlots(string currentSlot)
        {
            if (!ValidBoardSlots.ContainsKey(currentSlot))
            {
                Debug.LogError("Invalid parameter. You are checking connections for a slot that isn't valid.");
                return null;
            }

            string[] connectedSlots = new string[4];
            int[] centeredCurrentSlot = Utils.GetIndexesFromKey(Utils.TransformToCenteredCoords(currentSlot, BoardSize));
            int deltaDistance = Mathf.Max(Mathf.Abs(centeredCurrentSlot[0]), Mathf.Abs(centeredCurrentSlot[1]));

            for (int i = 0; i < 4; i++)
            {
                int[] temp = (int[])centeredCurrentSlot.Clone();

                // Adjust temp based on direction (0: right, 1: left, 2: up, 3: down)
                temp = centeredCurrentSlot.Contains(0)
                    ? UpdateCoordsForZeroSlot(centeredCurrentSlot, temp, i, deltaDistance)
                    : UpdateCoordsInAllDirections(temp, i, deltaDistance);

                string key = Utils.TransformToOffsetedCoords(Utils.ParseKeyFromIndexes(temp), BoardSize);
                if (ValidBoardSlots.ContainsKey(key))
                {
                    connectedSlots[i] = key;
                }
            }
            return connectedSlots;
        }

        /// <summary>
        /// Updates coordinates for non-zero slots (for general movement).
        /// Adjusts coordinates based on the direction and delta.
        /// </summary>
        /// <param name="coords">Current coordinates of the slot.</param>
        /// <param name="direction">Direction of movement (0: right, 1: left, 2: up, 3: down).</param>
        /// <param name="delta">The distance to move.</param>
        /// <returns> An array consisting of updated coordinates.

        private static int[] UpdateCoordsInAllDirections(int[] coords, int direction, int delta)
        {
            switch (direction)
            {
                case 0: coords[0] += delta; break; // right
                case 1: coords[0] -= delta; break; // left
                case 2: coords[1] += delta; break; // up
                case 3: coords[1] -= delta; break; // down
            }
            return coords;
        }

        /// <summary>
        /// Updates coordinates for slots with one axis at zero (centered on an axis).
        /// The movement depends on whether the x or y axis is zero.
        /// </summary>
        /// <returns> An array consisting of updated coordinates.
        private static int[] UpdateCoordsForZeroSlot(int[] current, int[] coords, int direction, int delta)
        { 
            if (current[1] == 0) // Check if the slot is centered on the x-axis
            {
                if (direction == 0) coords[0] += 1;   // right  
                else if (direction == 1) coords[0] -= 1; // left
                else if (direction == 2) coords[1] += delta; // up
                else if (direction == 3) coords[1] -= delta; // down
            }
            else // The slot is centered on the y-axis
            {
                if (direction == 0) coords[0] += delta;  // right
                else if (direction == 1) coords[0] -= delta; // left
                else if (direction == 2) coords[1] += 1;  // up
                else if (direction == 3) coords[1] -= 1;  // down
            }
            return coords;
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
                        bool slotValue = IsValidSlot(row, column, BoardSize);
                        if (slotValue)
                        {
                            string slotCode = row.ToString() + "," + column.ToString();

                            SpacingBetweenSlots = CalculateSpacingBetweenSlots(BoardSize, canvasSize);
                            Vector2 slotPosition = CalculateSlotPosition(row, column, BoardSize, canvasSize);
                            ValidBoardSlots.Add(slotCode, slotPosition);
                        }
                        _allBoardSlots[row, column] = slotValue;
                    }
                }
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
                return (row == column || row + column == boardSize - 1 || //include diagonals
                           (boardSize - 1) / 2 == row || (boardSize - 1) / 2 == column); //include crosses
            }
            return false;
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