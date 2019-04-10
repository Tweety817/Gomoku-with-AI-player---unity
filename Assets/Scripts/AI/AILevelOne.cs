using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelOne : Player {
    //建立字典，用字串(代表棋子的排列類型)來查找分數
    protected Dictionary<string, float> toScore = new Dictionary<string, float>();
    protected float[,] score = new float[15, 15]; //這個2為陣列最多為15*15，型別為float，用來算分數

    protected override  void Start()
    {   //連續兩個顏色的棋子，底線為空(無棋子)     
        toScore.Add("aa_", 50);
        toScore.Add("_aa", 50);
        toScore.Add("_aa_", 100);
        //連續三個顏色的棋子   
        toScore.Add("_aaa_", 1000);
        toScore.Add("aaa_", 500);
        toScore.Add("_aaa", 500);
        //連續四個顏色的棋子  
        toScore.Add("_aaaa_", 10000);
        toScore.Add("aaaa_", 5000);
        toScore.Add("_aaaa", 5000);
        //連續五個顏色的棋子  
        toScore.Add("aaaaa", float.MaxValue);
        toScore.Add("aaaaa_", float.MaxValue);
        toScore.Add("_aaaaa", float.MaxValue);
        toScore.Add("_aaaaa_", float.MaxValue);
        if (chessColor != ChessType.Watch)
            Debug.Log(chessColor + "AILevelOne");
    }

    public virtual void CheckOneLine(int[] pos, int[] offset,int chess)//參數多了代表棋子顏色的數字
    {   //這段是把ChessBoard腳本的CheckOneLine()複製過來，加以修改
        //先讓str = "a"，代表下的一顆子；原本是int linkNum = 1;
        string str = "a";
        //掃右邊
        for (int i = offset[0], j = offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i += offset[0], j += offset[1])
        {
            if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] ==chess)//如果棋盤位置上的棋子顏色跟參數一樣
            {
                str += "a";//每次掃到一個跟自身顏色一樣的棋子，就+一個a

            }
            else if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == 0)//如果棋盤位置上的棋子顏色為空，代表沒有棋子，給底線
            {
                str += "_";
                break;
            }
            else
            {
                break;
            }
        }
        //掃左邊
        for (int i = -offset[0], j = -offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i -= offset[0], j -= offset[1])
        {
            if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == chess)
            {
                str = "a" + str;//把字往左邊+

            }
            else if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == 0)
            {
                str = "_" +str;
                break;
            }
            else
            {
                break;
            }
        }

        //如果toScore字典內含有上面函式內找出的字串（用來判斷棋子的排列型別），就加上對應的分數
        if (toScore.ContainsKey(str))
        {
            score[pos[0], pos[1]] += toScore[str];
            //score[pos[0], pos[1]]代表，score陣列中，這個位置的陣列項目所返回的分數(分數是float型別)
            //toScore[str]代表分數。給予str，讓字典找出對應的value(value是float型別)
        }

    }

    public void SetScore(int[] pos)
    {
        score[pos[0], pos[1]] = 0;//評分前先把它清空
        //因為不知道AI跟真人玩家誰黑誰白，所以兩種都要評分數，讓AI決定怎麼下
        //先評黑子分數 參數多了1
        CheckOneLine(pos, new int[2] { 1, 0 },1);
        CheckOneLine(pos, new int[2] { 1, 1 },1);
        CheckOneLine(pos, new int[2] { 1, -1 },1);
        CheckOneLine(pos, new int[2] { 0, 1 },1);
        //先評白子分數 參數多了2
        CheckOneLine(pos, new int[2] { 1, 0 }, 2);
        CheckOneLine(pos, new int[2] { 1, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, -1 },2);
        CheckOneLine(pos, new int[2] { 0, 1 }, 2);
    }

    public override void PlayeChess()
    {   //這邊也是複製過來修改的
        if(ChessBoard.Instacne.chessStack.Count == 0)//棋子被下後會入棧，從棧裡要是找不到任何棋子就代表AI是先發(黑子)
        {   //讓AI下在棋盤中心點
            if (ChessBoard.Instacne.PlayChess(new int[2] { 7,7 }))
                ChessBoard.Instacne.timer = 0;
            return;
        }

        float maxScore = 0;//maxScore是AI下棋拿到的最大分數，預設為0
        int[] maxPos = new int[2] { 0, 0 };//maxPos預設為0，是代表下棋拿到最大分數的位置
        //遍歷每個棋盤位置
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if(ChessBoard.Instacne.grid[i,j] == 0)//如果棋盤上這位置沒有被下過棋子
                //就暫定下在那個位置，並且判斷分數、設置分數
                //遍歷去找最高分數的位置(找到maxScore，就找到maxPos)，找到就下在那
                {
                    SetScore(new int[2] { i,j});
                    if(score[i,j]>= maxScore)
                    {
                        maxPos[0] = i;
                        maxPos[1] = j;
                        maxScore = score[i, j];
                    }
                }
            }
        }
        //確定找到最高分的位置，就下AI棋子在那
        if (ChessBoard.Instacne.PlayChess(maxPos))
            ChessBoard.Instacne.timer = 0;

    }

    protected override void ChangeBtnColor()
    {
    }
}
