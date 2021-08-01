using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Custom
{
    public static class Tools
    {
        //Instantiate a random number generator
        private static System.Random rnd = new System.Random();

        //source: https://stackoverflow.com/questions/273313/randomize-a-listt
        //based on the Fisher–Yates shuffle found on wikipedia, source: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Color GetColor(int id)
        {
            Color result;
            switch (id)
            {
                case 0: //initial state
                    result = Color.white;
                    break;
                //AGENT 1
                case 10:
                    result = HexToRGB("#03045e"); //Dark blue
                    break;
                case 11:
                    result = HexToRGB("#023e8a");
                    break;
                case 12:
                    result = HexToRGB("#0077b6");
                    break;
                case 13:
                    result = HexToRGB("#0096c7");
                    break;
                case 14:
                    result = HexToRGB("#00b4d8");
                    break;
                case 15:
                    result = HexToRGB("#48cae4");
                    break;

                //AGENT 2
                case 20:
                    result = HexToRGB("#1b4332"); //Dark green
                    break;
                case 21:
                    result = HexToRGB("#2d6a4f");
                    break;
                case 22:
                    result = HexToRGB("#40916c");
                    break;
                case 23:
                    result = HexToRGB("#52b788");
                    break;
                case 24:
                    result = HexToRGB("#74c69d");
                    break;
                case 25:
                    result = HexToRGB("#95d5b2");
                    break;

                //AGENT 3
                case 30:
                    result = HexToRGB("#800f2f"); //Dark red
                    break;
                case 31:
                    result = HexToRGB("#a4133c");
                    break;
                case 32:
                    result = HexToRGB("#c9184a");
                    break;
                case 33:
                    result = HexToRGB("#ff4d6d");
                    break;
                case 34:
                    result = HexToRGB("#ff758f");
                    break;
                case 35:
                    result = HexToRGB("#ff8fa3");
                    break;

                //AGENT 4
                case 40:
                    result = HexToRGB("#ff6d00"); //Dark orange
                    break;
                case 41:
                    result = HexToRGB("#ff7900");
                    break;
                case 42:
                    result = HexToRGB("#ff8500");
                    break;
                case 43:
                    result = HexToRGB("#ff9100");
                    break;
                case 44:
                    result = HexToRGB("#ff9e00");
                    break;
                case 45:
                    result = HexToRGB("#ffaa00");
                    break;

                //NOT AVAILABLE
                default:
                    result = Color.white;
                    break;
            }
            return result;
        }

        public static Color HexToRGB(string colName)
        {
            Color newColor = Color.white;
            if (ColorUtility.TryParseHtmlString(colName, out newColor))
            { }
            return newColor;
        }

        // From CodeMonkey library
        // Create a Sprite in the World, no parent
        public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
        {
            return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
        }

        // From CodeMonkey library
        // Create a Sprite in the World
        public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            return gameObject;
        }

    }
}
