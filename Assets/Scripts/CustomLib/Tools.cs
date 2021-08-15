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
        //based on the Fisher�Yates shuffle found on wikipedia, source: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

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
                    result = HexToRGB("#00b4d8");
                    break;
                case 6:
                    result = HexToRGB("#48cae4");
                    break;
                case 7:
                    result = HexToRGB("#023e8a");
                    break;
                case 8:
                    result = HexToRGB("#2d6a4f");
                    break;
                case 9:
                    result = HexToRGB("#40916c");
                    break;
                case 10:
                    result = HexToRGB("#52b788");
                    break;
                case 11:
                    result = HexToRGB("#74c69d");
                    break;
                case 12:
                    result = HexToRGB("#95d5b2");
                    break;
                case 13:
                    result = HexToRGB("#0077b6");
                    break;
                case 14:
                    result = HexToRGB("#a4133c");
                    break;
                case 15:
                    result = HexToRGB("#c9184a");
                    break;
                case 16:
                    result = HexToRGB("#ff4d6d");
                    break;
                case 17:
                    result = HexToRGB("#ff758f");
                    break;
                case 18:
                    result = HexToRGB("#ff8fa3");
                    break;
                case 19:
                    result = HexToRGB("#0096c7");
                    break;
                case 20:
                    result = HexToRGB("#ff7900");
                    break;
                case 21:
                    result = HexToRGB("#ff8500");
                    break;
                case 22:
                    result = HexToRGB("#ff9100");
                    break;
                case 23:
                    result = HexToRGB("#ff9e00");
                    break;
                case 24:
                    result = HexToRGB("#ffaa00");
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

    }
}
