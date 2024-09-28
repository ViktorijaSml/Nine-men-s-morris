using Radishmouse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    /// <summary>
    /// The BoardManager class manages the drawing and layout of a game board on a canvas. 
    /// It uses Unity's UI system to instantiate slot objects, draw lines between them, 
    /// and render visual elements based on a given GameBoard instance.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        private RectTransform canvasTransform;
        private GameObject slotPrefab;
        private UILineRenderer lineRenderer;
        public static BoardManager instance;
        private enum Direction
        {
            Up, Down, Right, Left
        }
        public BoardManager() 
        {
            instance = this;
        }

        /// <summary>
        /// Draws the game board on the canvas based on the provided GameBoard configuration.
        /// It sets up board slots and draws lines connecting them according to specific patterns.
        /// </summary>
        /// <param name="gameBoard">The GameBoard object containing information about the board's layout and size.</param>
        public void DrawBoard(GameBoard gameBoard)
        {
            canvasTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
            ComponentNullCheck(canvasTransform);

            slotPrefab = Resources.Load<GameObject>("Prefabs/Slot");
            ComponentNullCheck(slotPrefab);

            lineRenderer = canvasTransform.GetComponentInChildren<UILineRenderer>();
            ComponentNullCheck(lineRenderer);

            // Initialize Game board variable on the specified canvas
            gameBoard.InitializeBoard(canvasTransform);
            lineRenderer.LineThickness = CalculateLineThickness(gameBoard.NumberOfRings);

            // Draw all the slots
            foreach (var slot in gameBoard.ValidBoardSlots) 
            {
                DrawSlot(slot.Key, slot.Value, gameBoard.SpacingBetweenSlots, slotPrefab, canvasTransform);
            }
            Debug.Log($"Drew {gameBoard.ValidBoardSlots.Count} slots.");

            //Draw the lines between slots
            string firstPosition = $"0,{(gameBoard.BoardSize - 1) / 2}"; // start drawing in the middle slot on the bottom of the most outer square
            lineRenderer.StartDrawing(gameBoard.ValidBoardSlots[firstPosition]);
            for (int i = 0; i < gameBoard.NumberOfRings; i++)
            {
                DrawSquare(lineRenderer, gameBoard.ValidBoardSlots, gameBoard.NumberOfRings-i); // draw a square
                if (i != gameBoard.NumberOfRings-1) 
                {
                    DrawLineInDirection(lineRenderer, Direction.Up, gameBoard.ValidBoardSlots, 1); // after drawing a square, go up a slot for the next square
                }
            }
            Debug.Log($"Drew {gameBoard.NumberOfRings} rings.");

            if (gameBoard.NumberOfRings != 1)
            {
                DrawTShape(lineRenderer, gameBoard.ValidBoardSlots, gameBoard.NumberOfRings); // finish drawing the cross
            }
            lineRenderer.CompleteDrawing();
            Debug.Log("Completed drawing the board.");
        }

        /// <summary>
        /// Instantiates and positions a slot on the board based on the provided parameters. Size of the slot depends on the spacing between them.
        /// </summary>
        /// <param name="slotCode">The unique key identifying the slot (e.g., "0,1").</param>
        /// <param name="slotPosition">The position of the slot on the canvas.</param>
        /// <param name="spacing">Spacing between slots, used to size the slot.</param>
        /// <param name="slotPrefab">Prefab object to instantiate the slot.</param>
        /// <param name="canvasTransform">The RectTransform of the canvas.</param>
        public static void DrawSlot(string slotCode, Vector2 slotPosition, float spacing, GameObject slotPrefab, RectTransform canvasTransform)
        {
            GameObject slot = Instantiate(slotPrefab, canvasTransform);

            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            slotRectTransform.position = slotPosition;
            slotRectTransform.anchoredPosition = slotPosition;
            slotRectTransform.sizeDelta = new Vector2(spacing * 0.5f, spacing * 0.5f);

            slot.name = slotPrefab.name + '(' + slotCode + ')';
            slot.GetComponent<Image>().color = Color.black;
        }

        /// <summary>
        /// Draws a square on the board by connecting board slots with lines in a square pattern, starting from the middle on the bottom side.
        /// </summary>
        /// <param name="lineRenderer">The UILineRenderer used to draw the lines.</param>
        /// <param name="boardSlots">A dictionary of board slots, where the key is a string (coordinates) and value is the position.</param>
        /// <param name="deltaDistance">The distance (in number of slots) to move in each direction.</param>
        private static void DrawSquare(UILineRenderer lineRenderer, Dictionary<string, Vector2> boardSlots, int deltaDistance)
        {
            if (lineRenderer.CurrentState != UILineRenderer.DrawingState.Drawing)
            {
                throw new ArgumentException("Cannot get current position if there is no drawing active.");
            }

            DrawLineInDirection(lineRenderer, Direction.Right, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Up, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Up, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Down, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Down, boardSlots, deltaDistance);
            DrawLineInDirection(lineRenderer, Direction.Right, boardSlots, deltaDistance);

            Debug.Log("Completed drawing the square.");
        }

        /// <summary>
        /// Draws a T pattern over the board slots for a board with multiple rings.
        /// </summary>
        /// <param name="lineRenderer">The UILineRenderer used to draw the lines.</param>
        /// <param name="boardSlots">A dictionary of board slots, where the key is a string (coordinates in the matrix) and value is the position.</param>
        /// <param name="numberOfRings">The number of rings in the game board.</param>
        private static void DrawTShape(UILineRenderer lineRenderer, Dictionary<string, Vector2> boardSlots, int numberOfRings)
        {
            DrawLineInDirection(lineRenderer, Direction.Right, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Up, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Right, boardSlots, numberOfRings-1);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, numberOfRings-1);
            DrawLineInDirection(lineRenderer, Direction.Up, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Up, boardSlots, numberOfRings - 1);
            DrawLineInDirection(lineRenderer, Direction.Down, boardSlots, numberOfRings - 1);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Down, boardSlots, 1);
            DrawLineInDirection(lineRenderer, Direction.Left, boardSlots, numberOfRings - 1);
            DrawLineInDirection(lineRenderer, Direction.Right, boardSlots, numberOfRings - 1);

            Debug.Log("Completed drawing the cross.");
        }

        /// <summary>
        /// Draws a line in a specified direction from the current slot position.
        /// </summary>
        /// <param name="lineRenderer">The UILineRenderer used to draw the line.</param>
        /// <param name="direction">The direction in which to move and draw the line (Up, Down, Left, Right).</param>
        /// <param name="boardSlots">A dictionary of board slots, where the key is a string (coordinates) and value is the position.</param>
        /// <param name="deltaDistance">The number of points to move in the specified direction.</param>
        private static void DrawLineInDirection(UILineRenderer lineRenderer, Direction direction, Dictionary<string, Vector2> boardSlots, int deltaDistance)
        {
            if (lineRenderer.CurrentState != UILineRenderer.DrawingState.Drawing)
            {
               throw new ArgumentException("Cannot get current position if there is no drawing active.");
            }

            int[] indexes = new int[2];
            try
            {
                string current2DIndex = boardSlots.FirstOrDefault(x => x.Value.Equals(lineRenderer.CurrentPosition)).Key;
                indexes = GetIndexesFromKey(current2DIndex);
            }
            catch (Exception e) 
            { 
                Debug.LogError(e.Message);
            }
            
            switch (direction)
            {
                case Direction.Up:
                    indexes[0] += deltaDistance;
                    break;
                case Direction.Down:
                    indexes[0] -= deltaDistance;
                    break;
                case Direction.Left:
                    indexes[1] -= deltaDistance;
                    break;
                case Direction.Right:
                    indexes[1] += deltaDistance;
                    break;
                default:
                    Debug.LogError("Invalid direction to draw line. Use 'Up', 'Down', 'Left', or 'Right'.");
                    return;
            }

            string nextPointKey = $"{indexes[0]},{indexes[1]}";
            lineRenderer.DrawLine(boardSlots[nextPointKey]);
        }

        /// <summary>
        /// Calculates the thickness of a line based on the number of rings in the game board.
        /// The thickness is determined using the quadratic polynomial equation:
        /// y = 1.25 * numberOfRings^2 - 15 * numberOfRings + 51.75.
        /// </summary>
        /// <param name="numberOfRings">The number of rings used to calculate the line thickness.</param>
        /// <returns>A float representing the calculated line thickness.</returns>
        private static float CalculateLineThickness(int numberOfRings) => 68.4f/(0.8f + numberOfRings);

        /// <summary>
        /// Checks if a given component is null and logs an error message if it is.
        /// </summary>
        /// <typeparam name="T">The type of the component being checked.</typeparam>
        /// <param name="component">The component to check for null.</param>
        private static void ComponentNullCheck<T> (T component)
        {
            if (component == null)
            {
                Debug.LogError($"Could not find {nameof(component)}");
            }
        }

        /// <summary>
        /// Extracts and returns the numerical indexes from a given string key formatted as "x,y".
        /// If the key is invalid (null, empty, or improperly formatted), an error is logged.
        /// </summary>
        /// <param name="key">The string key to extract indexes from.</param>
        /// <returns>An array of integers where indexes[0] is the "x" value and indexes[1] is the "y" value,
        /// or null if the key is invalid.</returns>
        private static int[] GetIndexesFromKey(string key)
        {
            //indexes[0] is left, indexes[1] is right number 
            if (string.IsNullOrEmpty(key) || !key.Contains(','))
            {
                Debug.LogError($"Given key is invalid. Key: {key}");
                return null;
            }
            string[] indexes = key.Split(',');
            return new int[] { int.Parse(indexes[0]), int.Parse(indexes[1]) };
        }   
    }
}
