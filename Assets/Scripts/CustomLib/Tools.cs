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
            id = id % 24;
            switch (id)
            {
                case 0: //initial state
                    result = Color.white;
                    break;
                case 1:
                    result = HexToRGB("#03045e"); //Dark blue
                    break;
                case 2:
                    result = HexToRGB("#1b4332"); //Dark green
                    break;
                case 3:
                    result = HexToRGB("#800f2f"); //Dark red
                    break;
                case 4:
                    result = HexToRGB("#ff6d00"); //Dark orange
                    break;
                case 5:
                    result = HexToRGB("#0609c6"); //B
                    break;
                case 6:
                    result = HexToRGB("#2c6d52"); //G
                    break;
                case 7:
                    result = HexToRGB("#cd184b"); //R
                    break;
                case 8:
                    result = HexToRGB("#b34d00"); //O
                    break;
                case 9:
                    result = HexToRGB("#2024f8"); //B
                    break;
                case 10:
                    result = HexToRGB("#49b689");//G
                    break;
                case 11:
                    result = HexToRGB("#ea4876"); //R
                    break;
                case 12:
                    result = HexToRGB("#ffa866"); //O
                    break;
                case 13:
                    result = HexToRGB("#9c9efc"); //B
                    break;
                case 14:
                    result = HexToRGB("#92d3b8"); //G
                    break;
                case 15:
                    result = HexToRGB("#cc0000"); //R
                    break;
                case 16:
                    result = HexToRGB("#ff9900"); //O
                    break;
                case 17:
                    result = HexToRGB("#0066ff"); //B
                    break;
                case 18:
                    result = HexToRGB("#00cc00"); //G
                    break;
                case 19:
                    result = HexToRGB("#ff6666"); //R
                    break;
                case 20:
                    result = HexToRGB("#ffcc80"); //O
                    break;
                case 21:
                    result = HexToRGB("#003d99"); //B
                    break;
                case 22:
                    result = HexToRGB("#4dff4d"); //G
                    break;
                case 23:
                    result = HexToRGB("#990000"); //R
                    break;
                case 24:
                    result = HexToRGB("#b36b00"); //O
                    break;
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

        public static float RemapFloats(float value, float oldMin, float oldMax, float newLow, float newMax)
        {
            return newLow + (value - oldMin) * (newMax - newLow) / (oldMax - oldMin);
        }

    }
}
