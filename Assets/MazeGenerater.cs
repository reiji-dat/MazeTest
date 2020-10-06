using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeGenerater : MonoBehaviour
{
    [SerializeField]
    GameObject block;//壁のオブジェクトを取得

    bool[,] wall;//壁の配列を定義

    int[] markX;//マーカーXの配列を定義
    int[] markZ;//マーカーZの配列を定義

    [SerializeField]
    int mazeWidth = 30;//縦の長さを定義(内側の奇数分の長さ)
    [SerializeField]
    int mazeHeight = 30;//横の長さを定義(内側の奇数分の長さ)

    /// <summary>
    /// 壁を設置
    /// </summary>
    void plaseWall()
    {
        //上下左右の枠を作る
        for (int z = 0; z < mazeHeight * 2 + 1; z = z + 1)
            Instantiate(block, new Vector3(0, 0, z), Quaternion.identity);
        for (int x = 1; x < mazeWidth * 2 + 1; x = x + 1)
            Instantiate(block, new Vector3(x, 0, 0), Quaternion.identity);
        for (int z = 1; z < mazeHeight * 2 + 1; z = z + 1)
            Instantiate(block, new Vector3(mazeWidth * 2, 0, z), Quaternion.identity);
        for (int x = 1; x < mazeWidth * 2; x = x + 1)
            Instantiate(block, new Vector3(x, 0, mazeHeight * 2), Quaternion.identity);
    }

    /// <summary>
    /// 迷路の初期化
    /// </summary>
    void initMase()
    {
        //もし3×3の場合は[0~4,0~4]の配列になっている
        wall = new bool[mazeWidth * 2 - 1, mazeHeight * 2 - 1];//迷路のすべてのブロック数の配列を定義
        for (int z = 0; z < mazeHeight * 2 - 1; z = z + 1)
            for (int x = 0; x < mazeWidth * 2 - 1; x = x + 1)
                wall[x, z] = true;//すべてtrueに割り当てる

        //マーカーを必要分の配列を作る
        //今回は縦×横の数にした(確率上最大だが最大まで使うことはないだろう)
        markX = new int[mazeWidth * mazeHeight];
        markZ = new int[mazeWidth * mazeHeight];

        //初期地点をランダムで決める
        markX[0] = Random.Range(0, mazeWidth);//30の場合(0~29)
        markZ[0] = Random.Range(0, mazeHeight);
        //ランダムで決めた場所をfalseにする。
        wall[markX[0] * 2, markZ[0] * 2] = false; 
    }

    /// <summary>
    /// 穴を掘る(迷路自動生成アルゴリズムの穴掘り法を採用)
    /// </summary>
    void digMaze()
    {
        int markNow = 0;//穴掘りの初期値0とする。

        //マーカーが0未満で終了
        while (markNow >= 0)
        {
            bool digged = false;//掘ったかどうか。掘っていない。

            //重複なしの乱数
            int start = 0;
            int end = 3;
            List<int> points = new List<int>();
            for (int i = start; i <= end; i++)
            {
                points.Add(i); //開始地点から1ずつリストに追加
            }
            //リストが0になるまで繰り返す
            while (points.Count > 0)
            {
                int index = Random.Range(0, points.Count);//0からリスト数までの乱数をインデックスする

                int rnd = points[index];//インデックスの数字をrndに読み取る(以降rndを使えば値が使える)

                switch (rnd)
                {
                    case 0://北(上)
                        //もし、マーカーが最大以上の場合"じゃない"か、上のマスがfalse"じゃない"場合
                        if (!(markZ[markNow] * 2 >= (mazeHeight - 1) * 2 || !wall[markX[markNow] * 2, markZ[markNow] * 2 + 2]))
                        {
                            //1つ上と2つ上のマスを消す
                            wall[markX[markNow] * 2, markZ[markNow] * 2 + 1] = false;
                            wall[markX[markNow] * 2, markZ[markNow] * 2 + 2] = false;

                            markNow++;//マーカーを1つ足す
                            //マーカーに移動先の情報を入れる
                            markX[markNow] = markX[markNow - 1];
                            markZ[markNow] = markZ[markNow - 1] + 1;
                            digged = true;//掘った
                        }
                        break;
                    case 1://東(右)
                        //以下向きが変わるだけで処理は同じ
                        if (!(markX[markNow] * 2 >= (mazeWidth - 1) * 2 || !wall[markX[markNow] * 2 + 2, markZ[markNow] * 2]))
                        {
                            wall[markX[markNow] * 2 + 1, markZ[markNow] * 2] = false;
                            wall[markX[markNow] * 2 + 2, markZ[markNow] * 2] = false;
                            markNow++;
                            markX[markNow] = markX[markNow - 1] + 1;
                            markZ[markNow] = markZ[markNow - 1];
                            digged = true;
                        }
                        break;
                    case 2://南(下)
                        if (!(markZ[markNow] * 2 <= 0 || !wall[markX[markNow] * 2, markZ[markNow] * 2 - 2]))
                        {
                            wall[markX[markNow] * 2, markZ[markNow] * 2 - 1] = false;
                            wall[markX[markNow] * 2, markZ[markNow] * 2 - 2] = false;
                            markNow++;
                            markX[markNow] = markX[markNow - 1];
                            markZ[markNow] = markZ[markNow - 1] - 1;
                            digged = true;
                        }
                        break;
                    case 3://西(左)
                        if (!(markX[markNow] * 2 <= 0 || !wall[markX[markNow] * 2 - 2, markZ[markNow] * 2]))
                        {
                            wall[markX[markNow] * 2 - 1, markZ[markNow] * 2] = false;
                            wall[markX[markNow] * 2 - 2, markZ[markNow] * 2] = false;
                            markNow++;
                            markX[markNow] = markX[markNow - 1] - 1;
                            markZ[markNow] = markZ[markNow - 1];
                            digged = true;
                        }
                        break;
                }
                if (digged) break;//掘ったら重複なしの乱数を強制終了
                points.RemoveAt(index);//選ばれた数字を削除する  
            }
            if (!digged) markNow--;//掘れなかったら1つまえのマーカーで先ほどの処理を再度繰り返す
            //掘れても先ほどの処理を繰り返す
        }
    }

    /// <summary>
    /// 迷路を設置
    /// </summary>
    void plaseMaze()
    {
        for (int z = 0; z < mazeHeight * 2 - 1; z = z + 1)
            for (int x = 0; x < mazeWidth * 2 - 1; x = x + 1)
                if (wall[x, z]) 
                    Instantiate(block, new Vector3(x + 1, 0, z + 1), Quaternion.identity);//trueのみ生成
    }

    void Awake()
    {
        plaseWall();
        initMase();
        digMaze();
        plaseMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("SampleScene");//リセット(迷路再生成)
    }
}