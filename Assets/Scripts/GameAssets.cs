using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// This script allows you to reference assets that are in Unity by creating a new class
public class GameAssets : MonoBehaviour {

    private static GameAssets _i;

    public static GameAssets i {
        get {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    public Transform pfChatBubble;

}
