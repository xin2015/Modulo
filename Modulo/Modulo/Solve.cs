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
            string[] road = new string[question.pieceArray.Length];
            int pieceIndex = question.pieceArray.Length - 1;
            if (Test(question.mapArray, question.mapArray.Length, question.mapArray[0].Length, question.modu, question.modu - 1, question.pieceArray.OrderByDescending(o => o.num).ThenBy(o => o.value).ToArray(), pieceIndex, 0, question.addCountLimit, road))
            {
                result = string.Join(string.Empty, road);
            }
            return result;
        }

        private static bool Test(int[][] map, int mapx, int mapy, int modu, int moduLimit, Piece[] pieces, int pieceIndex, int addCount, int addCountLimit, string[] road)
        {
            Piece currentPiece = pieces[pieceIndex];
            bool[][] piece = currentPiece.piece;
            if (pieceIndex > 0)
            {
                pieceIndex--;
                for (int i = mapx - currentPiece.x; i >= 0; i--)
                {
                    for (int j = mapy - currentPiece.y; j >= 0; j--)
                    {
                        int copyCount = addCount;
                        for (int r = currentPiece.x - 1; r >= 0; r--)
                        {
                            int x = i + r;
                            for (int s = currentPiece.y - 1; s >= 0; s--)
                            {
                                if (piece[r][s])
                                {
                                    int y = j + s;
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
                            if (Test(map, mapx, mapy, modu, moduLimit, pieces, pieceIndex, copyCount, addCountLimit, road))
                            {
                                road[currentPiece.id] = string.Format("{0}{1}", i, j);
                                return true;
                            }
                        }
                        for (int r = currentPiece.x - 1; r >= 0; r--)
                        {
                            int x = i + r;
                            for (int s = currentPiece.y - 1; s >= 0; s--)
                            {
                                if (piece[r][s])
                                {
                                    int y = j + s;
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
                for (int i = mapx - currentPiece.x; i >= 0; i--)
                {
                    for (int j = mapy - currentPiece.y; j >= 0; j--)
                    {
                        int copyCount = addCount;
                        for (int r = currentPiece.x - 1; r >= 0; r--)
                        {
                            int x = i + r;
                            for (int s = currentPiece.y - 1; s >= 0; s--)
                            {
                                if (piece[r][s])
                                {
                                    int y = j + s;
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
                            road[currentPiece.id] = string.Format("{0}{1}", i, j);
                            return true;
                        }
                        for (int r = currentPiece.x - 1; r >= 0; r--)
                        {
                            int x = i + r;
                            for (int s = currentPiece.y - 1; s >= 0; s--)
                            {
                                if (piece[r][s])
                                {
                                    int y = j + s;
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
            return false;
        }
    }

    public class Question
    {
        public int level { get; set; }
        public int modu { get; set; }
        public string[] map { get; set; }
        public int[][] mapArray { get; set; }
        public string[] pieces { get; set; }
        public Piece[] pieceArray { get; set; }
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
            pieceArray = new Piece[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                string[] pieceStr = pieces[i].Split(',');
                Piece piece = new Piece();
                piece.id = i;
                piece.x = pieceStr.Length;
                piece.y = pieceStr[0].Length;
                piece.num = (mapx - piece.x + 1) * (mapy - piece.y + 1);
                piece.piece = new bool[piece.x][];
                for (int j = 0; j < piece.x; j++)
                {
                    piece.piece[j] = new bool[piece.y];
                    for (int k = 0; k < piece.y; k++)
                    {
                        if (pieceStr[j][k] == 'X')
                        {
                            piece.piece[j][k] = true;
                            piece.value++;
                        }
                    }
                }
                pieceArray[i] = piece;
            }
            piecesValue = pieceArray.Sum(o => o.value);
            addCountLimit = ((modu - 1) * piecesValue - mapValue) / modu;
        }
    }

    public class Piece
    {
        public bool[][] piece { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int value { get; set; }
        public int id { get; set; }
        public int num { get; set; }
    }
}
