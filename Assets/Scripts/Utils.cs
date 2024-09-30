using Board;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// The Utils class provides utility methods for managing and interacting with game components,
    /// including slot retrieval, coordinate transformation, and component validation. 
    /// It offers custom methods that help manage the game.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Retrieves a Slot object from the Canvas by its key. The key corresponds to the slot's name.
        /// If the slotKey is null, returns null.
        /// </summary>
        /// <param name="slotKey">The key/name of the slot object.</param>
        /// <returns>The Slot component corresponding to the given key, or null if not found.</returns>
        public static Slot GetSlotObject(string slotKey)
        {
            if (slotKey == null)
                return null;
            GameObject parentObj = GameObject.FindGameObjectWithTag("Canvas");
            ComponentNullCheck(parentObj);
            return parentObj.transform.Find($"Slot({slotKey})").GetComponent<Slot>();
        }
        /// <summary>
        /// Checks if a given component is null and logs an error message if it is.
        /// </summary>
        /// <typeparam name="T">The type of the component being checked.</typeparam>
        /// <param name="component">The component to check for null.</param>
        public static void ComponentNullCheck<T>(T component)
        {
            if (component == null)
            {
                Debug.LogError($"Could not find {typeof(T).ToString()}");
            }
        }

        /// <summary>
        /// Extracts and returns the numerical indexes from a given string key formatted as "x,y".
        /// If the key is invalid (null, empty, or improperly formatted), an error is logged.
        /// </summary>
        /// <param name="key">The string key to extract indexes from.</param>
        /// <returns>An array of integers where indexes[0] is the "x" value and indexes[1] is the "y" value,
        /// or null if the key is invalid.</returns>
        public static int[] GetIndexesFromKey(string key)
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

        /// <summary>
        /// Converts an array of two integers (representing slot coordinates) into a string key formatted as "x,y".
        /// </summary>
        /// <param name="indexes">An array of two integers representing the x and y coordinates of a slot.</param>
        /// <returns>A string formatted as "x,y" where x is the first element and y is the second. Returns null if the input is invalid.</returns>
        public static string ParseKeyFromIndexes(int[] indexes) 
        {
            if (indexes == null || indexes.Length !=2)
            {
                Debug.LogError("Invalid paramater for ParseKeyFromIndexes. It should be an array consisting of 2 int elements.");
                return null;
            }
            return $"{indexes[0]},{indexes[1]}";
        }

        /// <summary>
        /// Converts a slot's key to centered coordinates, where the center of the board is (0,0).
        /// </summary>
        public static string TransformToCenteredCoords(string slot, int boardSize)
        {
            int[] indexes = Utils.GetIndexesFromKey(slot);
            if (indexes == null)
            {
                Debug.LogError("Invalid slot. Cannot transform the coords of the slot.");
                return null;
            }

            int center = (boardSize - 1) / 2;
            return Utils.ParseKeyFromIndexes(new int[2] { indexes[0] - center, indexes[1] - center });
        }

        /// <summary>
        /// Converts a slot's key from centered coordinates back to offset coordinates.
        /// </summary>
        public static string TransformToOffsetedCoords(string centeredSlot, int boardSize)
        {
            int[] indexes = Utils.GetIndexesFromKey(centeredSlot);
            if (indexes == null)
            {
                Debug.LogError("Invalid slot. Cannot transform the coords of the slot.");
                return null;
            }

            int center = (boardSize - 1) / 2;
            return Utils.ParseKeyFromIndexes(new int[2] { indexes[0] + center, indexes[1] + center });
        }
    }
}
