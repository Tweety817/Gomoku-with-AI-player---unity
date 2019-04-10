using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;


public class NetPlayer : NetworkBehaviour {
    [SyncVar]
    public ChessType chessColor = ChessType.Black;
    public bool isDoibleMode = false;
    Button btn;

    void Start()
    {
        if (isLocalPlayer)
        {

            CmdSetPlayer();
            if (chessColor == ChessType.Watch)
                return;
            btn = GameObject.Find("RetractBtn").GetComponent<Button>();
            btn.onClick.AddListener(RetractBtn);
        }
        Debug.Log(Network.player.ipAddress);
    }

      void FixedUpdate()
    {
        
        if (chessColor == NetChessBoard.Instacne.turn && NetChessBoard.Instacne.timer > 0.3f && isLocalPlayer &&
            NetChessBoard.Instacne.PlayerNumber >1)
            PlayeChess();

        if (chessColor != ChessType.Watch && isLocalPlayer && !NetChessBoard.Instacne.gameStart)
            NetChessBoard.Instacne.GameEnd();

        if (chessColor != ChessType.Watch && isLocalPlayer)
            ChangeBtnColor();
 
    }

    public  void PlayeChess()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() )
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //print((int)(pos.x + 7.5f)+ " " + (int)(pos.y + 7.5f));
            CmdChess(pos);
        }
    }

    [Command]
    public void CmdChess(Vector2 pos)
    {
        if (NetChessBoard.Instacne.PlayChess(new int[2] { (int)(pos.x + 7.5f), (int)(pos.y + 7.5f) }))
            NetChessBoard.Instacne.timer = 0;
    }


     void ChangeBtnColor()
    {
        if (chessColor == ChessType.Watch)
            return;
        if (NetChessBoard.Instacne.turn == chessColor)
            btn.interactable = true;
        else
            btn.interactable = false;
    }

    [Command]
    public void CmdSetPlayer()
    {
        NetChessBoard.Instacne.PlayerNumber++;
        if (NetChessBoard.Instacne.PlayerNumber == 1)
            chessColor = ChessType.Black;
        else if (NetChessBoard.Instacne.PlayerNumber == 2)
            chessColor = ChessType.White;
        else
            chessColor = ChessType.Watch;
    }

    public void RetractBtn()
    {
        CmdRetractBtn();
    }

    [Command]
    public void CmdRetractBtn()
    {
        NetChessBoard.Instacne.RetractChess();
    }
}
