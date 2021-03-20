// https://www.notion.so/Random-Helpers-2817bd09b6a84909bee7b63a4478ce42
using System.Collections.Generic;
using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// A class that assists in generation of various randomized elements.
    /// </summary>
    public static class Rando {
        /// <summary>
        /// Returns a single element from an array of any type.
        /// </summary>
        public static T Element<T>(T[] from) {
            return from[Random.Range(0, from.Length)];
        }

        /// <summary>
        /// Returns a single element from a list of any type.
        /// </summary>
        public static T Element<T>(IList<T> from) {
            return from[Random.Range(0, from.Count)];
        }

        /// <summary>
        /// Shuffles an array.
        /// </summary>
        public static void Shuffle<T>(T[] from) {
            for (int i = from.Length - 1; i > 0; i--) {
                int r = Random.Range(0, i);
                Swap(ref from[i], ref from[r]);
            }
        }

        /// <summary>
        /// Shuffles a list.
        /// </summary>
        public static void Shuffle<T>(IList<T> from) {
            for (int i = from.Count - 1; i > 0; i--) {
                int r = Random.Range(0, i);
                T temp = from[i];
                from[i] = from[r];
                from[r] = temp;
            }
        }

        /// <summary>
        /// Utility to swap two elements
        /// </summary>
        private static void Swap<T>(ref T left, ref T right) {
            T temp = left;
            left = right;
            right = temp;
        }

        /// <summary>
        /// Provides a Vector3 within a range.
        /// </summary>
        public static Vector3 Vec3Range(Vector3 start, Vector3 end) {
            return new Vector3(Random.Range(start.x, end.x), Random.Range(start.y, end.y), Random.Range(start.z, end.z));
        }
    }
}