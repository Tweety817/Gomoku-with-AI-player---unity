using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour {

    static ChessBoard _instacne;

    public ChessType turn = ChessType.Black;//代表輪到誰，是哪個顏色的旗子
    public int[,] grid; //棋盤上的線
    public GameObject[] prefabs;//棋子的預製資源 0=黑 1=白
    public float timer = 0;
    public bool gameStart = false;
    Transform parent;//讓棋子生成時，成為棋盤的下層物件
    public Text winner;
    public   Stack<Transform> chessStack = new Stack<Transform>();

    public static ChessBoard Instacne //單例
    {
        get
        {
            return _instacne;
        }

        
    }

    private void Awake()
    {
        if(Instacne == null)
        {
            _instacne = this;
        }
    }

    private void Start()
    {
        parent = GameObject.Find("Parent").transform;
        grid = new int[15, 15];//產生一個15*15的陣列，內容都是整數，代表棋盤的網格
        //用來確定某位置上的棋子是黑還是白
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime; //計時用
    }

    public bool PlayChess(int[] pos)
    {
        if (!gameStart) return false;
        pos[0] = Mathf.Clamp(pos[0], 0, 14);//約束下棋的位置，最小0，最大14
        pos[1] = Mathf.Clamp(pos[1], 0, 14);//下棋座標是螢幕座標，pos[0]=X軸座標，pos[1]=Y軸座標

        if (grid[pos[0], pos[1]] != 0) return false;

        if(turn == ChessType.Black)
        {   //pos是世界座標，相機中心點是(0,0)，但對棋盤來說是(7,7)
            //所以棋子放在棋盤(螢幕座標)上，座標都要-7
            //總之，棋盤(螢幕座標)轉換成世界座標要-7
            GameObject go=  Instantiate(prefabs[0], new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
            chessStack.Push(go.transform);
            go.transform.SetParent(parent);//讓棋子生成時，成為棋盤的下層物件
            grid[pos[0], pos[1]] = 1;//紀錄下棋的點，1=黑
            //為何設1=黑、2=白?
            //因為這樣搭配比較好記憶，配合下方枚舉的順序
            // public enum ChessType
            // {
            //     Watch , 這個是0
            //     Black,  這個是1
            //     White   這個是2
            // }
                        
            //判断胜负
            if (CheckWinner(pos))
            {
                GameEnd();
            }
            turn = ChessType.White;
            
        }
        else if(turn == ChessType.White)
        {
            GameObject go = Instantiate(prefabs[1], new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
            chessStack.Push(go.transform);
            go.transform.SetParent(parent);
            grid[pos[0], pos[1]] = 2;//紀錄下棋的點，2=白
            //判断胜负
            if (CheckWinner(pos))
            {
                GameEnd();
            }
            turn = ChessType.Black;
        }

        return true;
    }

    void GameEnd()
    {
        winner.transform.parent .parent.gameObject.SetActive(true);
        switch (turn)
        {
            case ChessType.Watch:
                break;
            case ChessType.Black:
                winner.text = "黑棋胜！";
                break;
            case ChessType.White:
                winner.text = "白棋胜！";
                break;
            default:
                break;
        }       
        gameStart = false;
        Debug.Log(turn + "赢了");//turn是ChessType枚舉，不用toString也可以轉成字串
    }
   
    public bool CheckWinner(int [] pos)
    {   //利用確認棋子連線的函式，往直、橫、左斜、右斜這四種方向，用偏移量查看是否棋子同一種顏色
        if (CheckOneLine(pos, new int[2] { 1, 0 })) return true;
        if (CheckOneLine(pos, new int[2] { 0, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, -1})) return true;
        return false;
    }

    public bool CheckOneLine(int[] pos,int[] offset)//確認棋子連線
    {
        int linkNum = 1;//代表下了一顆子
        //先掃右邊，i代表X軸，j代表Y軸，(i,j)
        //pos是下棋的位置，offset是偏移量
        for (int i = offset[0],j = offset[1];(pos[0] + i >= 0 && pos[0] + i <15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i += offset[0],j += offset[1])//+= offset[]，因為要往同個方向繼續查找棋子
        {
            if(grid[pos[0] + i, pos[1] + j] == (int)turn)//如果棋盤上這個位置+上偏移量的所屬顏色，是這回合輪到的顏色
            {
                linkNum++;
            }
            else
            {
                break;
            }
        }
        //掃左邊，+個負號方向變相反方向
        for (int i = -offset[0], j = -offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i -= offset[0], j -= offset[1])
        {
            if (grid[pos[0] + i, pos[1] + j] == (int)turn)
            {
                linkNum++;
            }
            else
            {
                break;
            }
        }

        if (linkNum > 4) return true;//五顆棋子連一起就贏了

        return false;
    }

    public void RetractChess()
    {
        if (chessStack.Count > 1)
        {
            Transform pos = chessStack.Pop();
            grid[(int)(pos.position.x + 7), (int)(pos.position.y + 7)] = 0;
            Destroy(pos.gameObject);
             pos = chessStack.Pop();
            grid[(int)(pos.position.x + 7), (int)(pos.position.y + 7)] = 0;
            Destroy(pos.gameObject);
        }
    }

}

public enum ChessType
{
    Watch ,
    Black,
    White
}
