using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIFollow : MonoBehaviour {


	void Update () {
        if (ChessBoard.Instacne.chessStack.Count > 0)
            transform.position = ChessBoard.Instacne.chessStack.Peek().position;
	}

    public void OnRelayBtn()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnBtn()
    {
        SceneManager.LoadScene(0);

    }
}
