using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxNode //每一個節點的屬性:棋子顏色、位置、下面子節點(最多4個)、節點價值分數
{
    public int chess;
    public int[] pos;
    public List<MiniMaxNode> child;
    public float value;
}

public class AILevelThree : Player
{

    Dictionary<string, float> toScore = new Dictionary<string, float>();


    protected override void ChangeBtnColor()
    {
    }
    //評分表
    protected override void Start()
    {
        toScore.Add("aa___", 100);                      //眠二
        toScore.Add("a_a__", 100);
        toScore.Add("___aa", 100);
        toScore.Add("__a_a", 100);
        toScore.Add("a__a_", 100);
        toScore.Add("_a__a", 100);
        toScore.Add("a___a", 100);


        toScore.Add("__aa__", 500);                     //活二 
        toScore.Add("_a_a_", 500);
        toScore.Add("_a__a_", 500);

        toScore.Add("_aa__", 500);
        toScore.Add("__aa_", 500);


        toScore.Add("a_a_a", 1000);
        toScore.Add("aa__a", 1000);
        toScore.Add("_aa_a", 1000);
        toScore.Add("a_aa_", 1000);
        toScore.Add("_a_aa", 1000);
        toScore.Add("aa_a_", 1000);
        toScore.Add("aaa__", 1000);                     //眠三

        toScore.Add("_aa_a_", 9000);                    //跳活三
        toScore.Add("_a_aa_", 9000);

        toScore.Add("_aaa_", 10000);                    //活三       


        toScore.Add("a_aaa", 15000);                    //冲四
        toScore.Add("aaa_a", 15000);                    //冲四
        toScore.Add("_aaaa", 15000);                    //冲四
        toScore.Add("aaaa_", 15000);                    //冲四
        toScore.Add("aa_aa", 15000);                    //冲四        


        toScore.Add("_aaaa_", 1000000);                 //活四

        toScore.Add("aaaaa", float.MaxValue);           //连五


        if (chessColor != ChessType.Watch)
            Debug.Log(chessColor + "AILevelThree");
    }

    public float CheckOneLine(int[,] grid, int[] pos, int[] offset, int chess)
    //這邊grid參數不能直接用ChessBoard.Instacne.grid，因為ChessBoard.Instacne.grid是棋盤(單例)的grid
    //這邊grid參數是每個節點內自己假設的grid
    {
        float score = 0;
        bool lfirst = true, lstop = false, rstop = false;
        int AllNum = 1;
        string str = "a";
        int ri = offset[0], rj = offset[1];
        int li = -offset[0], lj = -offset[1];
        while (AllNum < 7 && (!lstop || !rstop))
        {
            if (lfirst)
            {
                //左边
                if ((pos[0] + li >= 0 && pos[0] + li < 15) &&
            pos[1] + lj >= 0 && pos[1] + lj < 15 && !lstop)
                {
                    if (grid[pos[0] + li, pos[1] + lj] == chess)
                    {
                        AllNum++;
                        str = "a" + str;

                    }
                    else if (grid[pos[0] + li, pos[1] + lj] == 0)
                    {
                        AllNum++;
                        str = "_" + str;
                        if (!rstop) lfirst = false;
                    }
                    else
                    {
                        lstop = true;
                        if (!rstop) lfirst = false;
                    }
                    li -= offset[0]; lj -= offset[1];
                }
                else
                {
                    lstop = true;
                    if (!rstop) lfirst = false;
                }
            }
            else
            {
                if ((pos[0] + ri >= 0 && pos[0] + ri < 15) &&
          pos[1] + rj >= 0 && pos[1] + rj < 15 && !lfirst && !rstop)
                {
                    if (grid[pos[0] + ri, pos[1] + rj] == chess)
                    {
                        AllNum++;
                        str += "a";

                    }
                    else if (grid[pos[0] + ri, pos[1] + rj] == 0)
                    {
                        AllNum++;
                        str += "_";
                        if (!lstop) lfirst = true;
                    }
                    else
                    {
                        rstop = true;
                        if (!lstop) lfirst = true;
                    }
                    ri += offset[0]; rj += offset[1];
                }
                else
                {
                    rstop = true;
                    if (!lstop) lfirst = true;
                }
            }
        }

        string cmpStr = "";
        foreach (var keyInfo in toScore)
        {
            if (str.Contains(keyInfo.Key))
            {
                if (cmpStr != "")
                {
                    if (toScore[keyInfo.Key] > toScore[cmpStr])
                    {
                        cmpStr = keyInfo.Key;
                    }
                }
                else
                {
                    cmpStr = keyInfo.Key;
                }
            }
        }

        if (cmpStr != "")
        {
            score += toScore[cmpStr];
        }
        return score;
    }
    //評分函式
    public float GetScore(int[,] grid, int[] pos)
    {
        float score = 0;

        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 1);

        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 2);

        return score;
    }
    

    //返回節點 使用極大極小法
    List<MiniMaxNode> GetList(int[,] grid, int chess, bool mySelf)
    //這函式要返回list，list內容是node(也就是每一種下棋狀況的節點)
    {
        List<MiniMaxNode> nodeList = new List<MiniMaxNode>(); //新增一個list
        MiniMaxNode node; //聲明節點，之後才定義

//先找節點~
        //去遍歷棋盤上每個位置
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                int[] pos = new int[2] { i, j };
                if (grid[pos[0], pos[1]] != 0) continue;//如果這位置有放棋子的話

                node = new MiniMaxNode();//新增node，接下來開始定義
                node.pos = pos;//每個節點有一些資訊，包含:棋子位置、棋子顏色、節點所價值的分數
                node.chess = chess;
                //如果是自己方的棋子
                if (mySelf)
                    node.value = GetScore(grid, pos);//去計算此節點的分數
                //是敵方的，加上負號
                else
                    node.value = -GetScore(grid, pos);

//找到節點，加入list，如果超過4個就比較誰該加進來
                    //每個節點下方還會有4個節點，因為一個棋子最多有四個方位可以放棋子
                    //所以如果節點數量小於4，就還可以再加入節點
                    //找到node(節點)，要把節點加入節點list之中
                if (nodeList.Count < 4)
                {
                    nodeList.Add(node);
                }
                else //如果大於4，比較誰該放進來，因為list最多只能放四個節點
                {
                    foreach (var item in nodeList)//遍歷清單內的節點
                    {
                        if (mySelf)//對於自己方，找極大值的節點加入進來
                        {
                            if (node.value > item.value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                        else//對於敵方，找極小值的節點加入進來
                        {
                            if (node.value < item.value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
        }

        return nodeList;
    }

    //找好節點後，創建樹
    public void CreateTree(MiniMaxNode node, int[,] grid, int deep, bool mySelf)
    {
//這條件用來判斷是否可以不用創建樹了
        if (deep == 0 || node.value == float.MaxValue)
        {
            return;
        }
//如果要創建樹
//先把棋子下進來
        grid[node.pos[0], node.pos[1]] = node.chess;//定義棋盤位置上的棋子顏色
//賦予節點子節點，就會變成樹
        node.child = GetList(grid, node.chess, !mySelf);//注意這邊參數是!mySelf，因為下一輪的4個節點是輪到對手下棋，所以要找不是自己的
        foreach (var item in node.child)
        {
//每個子節點又去創建樹，形成孫子節點
            CreateTree(item, (int[,])grid.Clone(), deep - 1, !mySelf);
            //注意這邊參數!mySelf，因為子節點都跟自己顏色相反
            //grid這邊使用.Clone()，把母節點的grid複製保存下來
            //所以無論母節點的選擇如何不同，子節點會保存下來
        }
    }

//Beta是可能解決方案 的最小上限	
//Alpha是可能解決方案 的最大下限
//剪枝:找節點中最大最小值，把最小的那個分支剪掉
    public float AlphaBeta(MiniMaxNode node, int deep, bool mySelf, float alpha, float beta)
    {

        if (deep == 0 || node.value == float.MaxValue || node.value == float.MinValue)
        {
            return node.value;
        }
        //是自己的話，遍歷子節點找最大值
        if (mySelf)
        {
            foreach (var child in node.child)
            {   //找最大值，和自己(alpha)、下一層節點的值比
                //Mathf.Max(A,B); 意思是A、B取誰最大
                alpha = Mathf.Max(alpha, AlphaBeta(child, deep - 1, !mySelf, alpha, beta));

                //alpha剪枝

                if (alpha >= beta)
                {
                    return alpha;
                }

            }
            return alpha;
        }
        //不是自己的話，遍歷子節點取最小值
        else
        {
            foreach (var child in node.child)
            {
                beta = Mathf.Min(beta, AlphaBeta(child, deep - 1, !mySelf, alpha, beta));

                //beta剪枝
                if (alpha >= beta)
                {
                    return beta;
                }

            }
            return beta;
        }
    }



    //寫好剪枝後，寫下棋函式
    public override void PlayeChess()
    {   //如果AI是黑棋的話，讓AI先下棋盤中心點
        if (ChessBoard.Instacne.chessStack.Count == 0)
        {
            if (ChessBoard.Instacne.PlayChess(new int[2] { 7, 7 }))
                ChessBoard.Instacne.timer = 0;
            return;
        }

        MiniMaxNode node = null;//先給一個空節點，先幫節點佔位置的意思，之後存放節點進來
        //遍歷節點清單中的每個節點，節點清單偷過函式返回而得到
        foreach (var item in GetList(ChessBoard.Instacne.grid, (int)chessColor, true))
        {   //針對每個節點去生成樹
            CreateTree(item, (int[,])ChessBoard.Instacne.grid.Clone(),3,false);
            //然後尋找Alpha、Beta值
            float a = float.MinValue;
            float b = float.MaxValue;
            //每個節點的價值原本是空的(null)，透過AlphaBeta()找到值、給予節點價值
            item.value += AlphaBeta(item,3,false,a,b);
            //如果node賦值過，給予最大值
            if(node != null)
            {//挑最大值的節點去下棋
                if (node.value < item.value)
                    node = item;
            }
            else//如果node沒賦值過，給予值
            {
                node = item;
            }
        }
        ChessBoard.Instacne.PlayChess(node.pos);
    }
}
