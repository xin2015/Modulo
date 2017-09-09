using DotNet4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Modulo
{
    public class Modulo
    {
        Question question;
        int level;
        int modu;
        int moduLimit;
        int x;
        int y;
        int[][] map;
        Piece[] pieces;
        int pieceValueSum;
        int mapValue;
        int addCountLimit;

        public Modulo()
        {

        }

        public void Auto()
        {
            try
            {
                HttpHelper hh = new HttpHelper();
                HttpItem hi = new HttpItem()
                {
                    Cookie = "laravel_session=eyJpdiI6IndMMzNOeGpERVllY0lkWVlzTDBLQVE9PSIsInZhbHVlIjoiWnZ6bW1tZTFBZUM0dlF3MHlWcXNSeGxYUWJEa3dOOE1EYjFFMjFiTFU5ZnpiT29maVFqQmRKRVc2bjVsT3VmS1FMbTZ1MjNGTlppUHVpMFU1N1dcL3FBPT0iLCJtYWMiOiI5NTc1M2E3NDdmMTU4Yzc0NjVjMGUzNjUzNjBlMDAzY2NkMmQ2NjE2ZTFjOGJiNmY1MTdiOThhNTA0YzQ1MjIyIn0"
                };
                HttpResult hr;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                while (true)
                {
                    hi.URL = "http://www.qlcoder.com/train/automodu";
                    hr = hh.GetHtml(hi);
                    string html = hr.Html;
                    html = html.Substring(html.IndexOf("{\"level\""));
                    html = html.Substring(0, html.IndexOf("<br>"));
                    question = jss.Deserialize<Question>(html);
                    Console.WriteLine("level:{0} start, {1}", question.level, DateTime.Now.ToString("HH:mm:ss"));
                    hi.URL = "http://www.qlcoder.com/train/moducheck?solution=" + Do();
                    Console.WriteLine("level:{0} end, {1}", question.level, DateTime.Now.ToString("HH:mm:ss"));
                    hr = hh.GetHtml(hi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        string Do()
        {
            level = question.level;
            modu = question.modu;
            moduLimit = modu - 1;
            x = question.map.Length;
            y = question.map[0].Length;
            map = new int[x][];
            for (int i = 0; i < x; i++)
            {
                map[i] = new int[y];
                for (int j = 0; j < y; j++)
                {
                    map[i][j] = int.Parse(question.map[i][j].ToString());
                    mapValue += map[i][j];
                }
            }
            pieces = new Piece[question.pieces.Length];
            for (int i = 0; i < question.pieces.Length; i++)
            {
                string[] pieceStr = question.pieces[i].Split(',');
                Piece piece = new Piece();
                piece.id = i;
                piece.x = pieceStr.Length;
                piece.y = pieceStr[0].Length;
                piece.num = (x - piece.x + 1) * (y - piece.y + 1);
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
                pieces[i] = piece;
            }
            pieces = pieces.OrderByDescending(o => o.num).ThenBy(o => o.value).ToArray();
            pieceValueSum = pieces.Sum(o => o.value);
            addCountLimit = ((modu - 1) * pieceValueSum - mapValue) / modu;
            string[] road = new string[pieces.Length];
            int pieceIndex = pieces.Length - 1;
            if (Do(pieceIndex, 0, road))
            {
                return string.Join(string.Empty, road);
            }
            return "";
        }

        bool Do(int pieceIndex, int addCount, string[] road)
        {
            Piece currentPiece = pieces[pieceIndex];
            bool[][] piece = currentPiece.piece;
            if (pieceIndex == 0)
            {
                for (int i = x - currentPiece.x; i >= 0; i--)
                {
                    for (int j = y - currentPiece.y; j >= 0; j--)
                    {
                        int copyAddCount = addCount;
                        for (int r = 0; r < currentPiece.x; r++)
                        {
                            int a = i + r;
                            for (int s = 0; s < currentPiece.y; s++)
                            {
                                if (piece[r][s])
                                {
                                    int b = j + s;
                                    if (map[a][b] == moduLimit)
                                    {
                                        map[a][b] = 0;
                                    }
                                    else
                                    {
                                        map[a][b]++;
                                        copyAddCount++;
                                    }
                                }
                            }
                        }
                        if (copyAddCount == addCountLimit)
                        {
                            road[currentPiece.id] = string.Format("{0}{1}", i, j);
                            return true;
                        }
                        for (int r = 0; r < currentPiece.x; r++)
                        {
                            int a = i + r;
                            for (int s = 0; s < currentPiece.y; s++)
                            {
                                if (piece[r][s])
                                {
                                    int b = j + s;
                                    if (map[a][b] == 0)
                                    {
                                        map[a][b] = moduLimit;
                                    }
                                    else
                                    {
                                        map[a][b]--;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                pieceIndex--;
                for (int i = x - currentPiece.x; i >= 0; i--)
                {
                    for (int j = y - currentPiece.y; j >= 0; j--)
                    {
                        int copyAddCount = addCount;
                        for (int r = 0; r < currentPiece.x; r++)
                        {
                            int a = i + r;
                            for (int s = 0; s < currentPiece.y; s++)
                            {
                                if (piece[r][s])
                                {
                                    int b = j + s;
                                    if (map[a][b] == moduLimit)
                                    {
                                        map[a][b] = 0;
                                    }
                                    else
                                    {
                                        map[a][b]++;
                                        copyAddCount++;
                                    }
                                }
                            }
                        }
                        if (copyAddCount <= addCountLimit)
                        {
                            if (Do(pieceIndex, copyAddCount, road))
                            {
                                road[currentPiece.id] = string.Format("{0}{1}", i, j);
                                return true;
                            }
                        }
                        for (int r = 0; r < currentPiece.x; r++)
                        {
                            int a = i + r;
                            for (int s = 0; s < currentPiece.y; s++)
                            {
                                if (piece[r][s])
                                {
                                    int b = j + s;
                                    if (map[a][b] == 0)
                                    {
                                        map[a][b] = moduLimit;
                                    }
                                    else
                                    {
                                        map[a][b]--;
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
        public string[] pieces { get; set; }
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
