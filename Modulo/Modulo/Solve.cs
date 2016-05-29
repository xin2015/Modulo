using DotNet4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Modulo
{
    public class Solve
    {
        public static void Auto()
        {
            try
            {
                HttpHelper hh = new HttpHelper();
                HttpItem hi = new HttpItem()
                {
                    Cookie = "laravel_session=eyJpdiI6Ijc1K3I3cFFQWWNjT2tPZTBLTytKeXc9PSIsInZhbHVlIjoibFJBMnJhd1d3b204c3dFWHNUZUFcLzIyeGhlcHNLVCtBbXlwNFIxaTRXQ09pR2srK2hEMmVvRGJlQjFyUUZxOE0ybXl2VWdxdE1vWUN4MTc5eGRBdHFBPT0iLCJtYWMiOiJmMzE5N2Y2MzFmYzg5YTE4MmFhNTcxODM4M2NjZmMyNzc3MGE0MDM4YmU3MGE3MWQwZjIxNmZhNWJlMjdkZGZlIn0%3D"
                };
                HttpResult hr;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                for (int i = 9; i < 64; i++)
                {
                    hi.URL = "http://www.qlcoder.com/train/automodu";
                    hr = hh.GetHtml(hi);
                    string html = hr.Html;
                    html = html.Substring(html.IndexOf("{\"level\""));
                    html = html.Substring(0, html.IndexOf("<br>"));
                    Question question = jss.Deserialize<Question>(html);
                    question.Init();
                    Console.WriteLine("level:{0} start, {1}", question.level, DateTime.Now.ToString("HH:mm:ss"));
                    hi.URL = "http://www.qlcoder.com/train/moducheck?solution=" + Do(question);
                    hr = hh.GetHtml(hi);
                    Console.WriteLine("level:{0} end, {1}", question.level, DateTime.Now.ToString("HH:mm:ss"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string Do(Question question)
        {
            string result = string.Empty;
            Stack<string> road = new Stack<string>();
            question.piecesArray = question.piecesArray.Reverse().ToArray();
            int pieceIndex = question.piecesArray.Length - 1;
            if (Test(question.mapArray, question.mapArray.Length, question.mapArray[0].Length, question.modu, question.modu - 1, question.piecesArray, pieceIndex, question.piecesArray[pieceIndex].Length, question.piecesArray[pieceIndex][0].Length, 0, question.addCountLimit, road))
            {
                while (road.Any())
                {
                    result += road.Pop();
                }
            }
            return result;
        }

        private static bool Test(int[][] map, int mapx, int mapy, int modu, int moduLimit, int[][][] pieces, int pieceIndex, int piecex, int piecey, int addCount, int addCountLimit, Stack<string> road)
        {
            int[][] piece = pieces[pieceIndex];
            if (pieceIndex > 0)
            {
                pieceIndex--;
                int piecexNew = pieces[pieceIndex].Length, pieceyNew = pieces[pieceIndex][0].Length;
                for (int i = mapx - piecex; i >= 0; i--)
                {
                    for (int j = mapy - piecey; j >= 0; j--)
                    {
                        int copyCount = addCount;
                        for (int r = piecex - 1; r >= 0; r--)
                        {
                            for (int s = piecey - 1; s >= 0; s--)
                            {
                                if (piece[r][s] == 1)
                                {
                                    int x = i + r, y = j + s;
                                    if (map[x][y] == moduLimit)
                                    {
                                        map[x][y] = 0;
                                    }
                                    else
                                    {
                                        map[x][y]++;
                                        copyCount++;
                                    }
                                }
                            }
                        }
                        if (copyCount <= addCountLimit)
                        {
                            if (Test(map, mapx, mapy, modu, moduLimit, pieces, pieceIndex, piecexNew, pieceyNew, copyCount, addCountLimit, road))
                            {
                                road.Push(string.Format("{0}{1}", i, j));
                                return true;
                            }
                        }
                        for (int r = piecex - 1; r >= 0; r--)
                        {
                            for (int s = piecey - 1; s >= 0; s--)
                            {
                                if (piece[r][s] == 1)
                                {
                                    int x = i + r, y = j + s;
                                    if (map[x][y] == 0)
                                    {
                                        map[x][y] = moduLimit;
                                    }
                                    else
                                    {
                                        map[x][y]--;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = mapx - piecex; i >= 0; i--)
                {
                    for (int j = mapy - piecey; j >= 0; j--)
                    {
                        for (int r = piecex - 1; r >= 0; r--)
                        {
                            for (int s = piecey - 1; s >= 0; s--)
                            {
                                if (piece[r][s] == 1)
                                {
                                    if (map[i + r][j + s] == moduLimit)
                                    {
                                        map[i + r][j + s] = 0;
                                    }
                                    else
                                    {
                                        map[i + r][j + s]++;
                                        addCount++;
                                    }
                                }
                            }
                        }
                        if (addCount == addCountLimit)
                        {
                            if (Over(map, mapx, mapy))
                            {
                                road.Push(string.Format("{0}{1}", i, j));
                                return true;
                            }
                        }
                        for (int r = piecex - 1; r >= 0; r--)
                        {
                            for (int s = piecey - 1; s >= 0; s--)
                            {
                                if (piece[r][s] == 1)
                                {
                                    if (map[i + r][j + s] == 0)
                                    {
                                        map[i + r][j + s] = moduLimit;
                                    }
                                    else
                                    {
                                        map[i + r][j + s]--;
                                        addCount--;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool Over(int[][] map, int mapx, int mapy)
        {
            for (int i = 0; i < mapx; i++)
            {
                for (int j = 0; j < mapy; j++)
                {
                    if (map[i][j] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class Question
    {
        public int level { get; set; }
        public int modu { get; set; }
        public string[] map { get; set; }
        public int[][] mapArray { get; set; }
        public string[] pieces { get; set; }
        public int[][][] piecesArray { get; set; }
        public int piecesValue { get; set; }
        public int mapValue { get; set; }
        public int addCountLimit { get; set; }

        public void Init()
        {
            mapArray = new int[map.Length][];
            int mapx = map.Length, mapy = map[0].Length;
            for (int i = 0; i < mapx; i++)
            {
                mapArray[i] = new int[mapy];
                for (int j = 0; j < mapy; j++)
                {
                    mapArray[i][j] = int.Parse(map[i][j].ToString());
                    mapValue += mapArray[i][j];
                }
            }
            piecesArray = new int[pieces.Length][][];
            for (int i = 0; i < pieces.Length; i++)
            {
                string[] piece = pieces[i].Split(',');
                int piecex = piece.Length, piecey = piece[0].Length;
                piecesArray[i] = new int[piecex][];
                for (int j = 0; j < piecex; j++)
                {
                    piecesArray[i][j] = new int[piecey];
                    for (int k = 0; k < piecey; k++)
                    {
                        if (piece[j][k] == 'X')
                        {
                            piecesArray[i][j][k] = 1;
                            piecesValue++;
                        }
                    }
                }
            }
            addCountLimit = (3 * piecesValue - mapValue) / modu;
        }
    }
}
