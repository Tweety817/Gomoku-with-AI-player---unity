using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public ChessType chessColor = ChessType.Black;
   public  bool isDoibleMode = false;
    Button btn;

    protected virtual void Start()
    {
        btn = GameObject.Find("RetractBtn").GetComponent<Button>();
        //print(PlayerPrefs.GetInt("Double"));
        if (PlayerPrefs.GetInt("Double") == 10)
            isDoibleMode = true;
    }

    protected virtual  void FixedUpdate()
    {
        if(chessColor ==ChessBoard.Instacne.turn && ChessBoard.Instacne.timer > 0.3f)
        //輪到自己顏色的回合跟超過0.3秒才可以下棋子
            PlayeChess();
        if(!isDoibleMode)
            ChangeBtnColor();
    }

    public  virtual void PlayeChess() //玩家放下棋子，會呼叫此函式，此函式會找ChessBoard單例去執行函式，讓棋子生成
    {
        if (Input.GetMouseButtonDown(0)&& !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);//把點擊位置轉換成世界座標
            //相機在棋盤的正中心，所以世界座標的(0,0)，對螢幕座標(棋盤)來說剛好是(7,7)
            //所以世界座標轉換成棋盤位置要X、Y都要+7
            //print((int)(pos.x + 7.5f)+ " " + (int)(pos.y + 7.5f));
            //為了四捨五入才+0.5 
            //讓ChessBoard單例透過prefab去生成棋子，參數是新的位置
            if(ChessBoard.Instacne.PlayChess(new int[2] { (int)(pos.x + 7.5f) , (int)(pos.y + 7.5f) }))
                ChessBoard.Instacne.timer = 0;//下完棋，計時器歸零
        }
    }


    protected virtual void ChangeBtnColor()
    {
        if (chessColor == ChessType.Watch)
            return;
        if (ChessBoard.Instacne.turn == chessColor)
            btn.interactable = true;
        else
            btn.interactable = false;
    }
}
