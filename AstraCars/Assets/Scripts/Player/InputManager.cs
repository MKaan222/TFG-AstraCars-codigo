using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    // Movimiento horizontal
    public static float GetHorizontal(PlayerData player)
    {
        //return Input.GetAxis(player.IsPlayer1() ? "Horizontal" : "Horizontal2");
        return Input.GetAxis(player.IsPlayer1() ? "Dancepad1Horizontal" : "Horizontal2");

    }

    // Movimiento vertical
    public static float GetVertical(PlayerData player)
    {
        //return Input.GetAxis(player.IsPlayer1() ? "Vertical" : "Vertical2");
        return Input.GetAxis(player.IsPlayer1() ? "Dancepad1Vertical" : "Vertical2");

    }

    // Respawn
    public static bool GetRespawn(PlayerData player)
    {
        if (player.IsPlayer1())
            return Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Dancepad1Vertical") > 0.5f;
        else
            return Input.GetKeyDown(KeyCode.UpArrow);
    }

    public static bool GetPause()
    {
        return Input.GetKeyDown(KeyCode.Escape)
            || Input.GetButtonDown("Pause")
            || Input.GetAxisRaw("Dancepad1Vertical") < -0.5f;
    }

    public static bool GetRestart()
    {
        return Input.GetKeyDown(KeyCode.R)
            || Input.GetButtonDown("Restart")
            || Input.GetAxisRaw("Dancepad1Horizontal") < -0.5f;
    }

    public static bool GetExit()
    {
        return Input.GetKeyDown(KeyCode.Q)
            || Input.GetButtonDown("Exit")
            || Input.GetAxisRaw("Dancepad1Horizontal") > 0.5f;
    }
    public static bool GetMenuLeft()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetAxisRaw("Dancepad1Horizontal") < -0.5f;
    }

    public static bool GetMenuRight()
    {
        return Input.GetKeyDown(KeyCode.RightArrow)
            || Input.GetKeyDown(KeyCode.D)
            || Input.GetAxisRaw("Dancepad1Horizontal") > 0.5f;
    }

    public static bool GetMenuUp()
    {
        return Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetKeyDown(KeyCode.W)
            || Input.GetAxisRaw("Dancepad1Vertical") > 0.5f;
    }

    public static bool GetMenuDown()
    {
        return Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetAxisRaw("Dancepad1Vertical") < -0.5f;
    }

    public static bool GetMenuAccept()
    {
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
    }

    public static bool GetMenuCancel()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace);
    }


}
