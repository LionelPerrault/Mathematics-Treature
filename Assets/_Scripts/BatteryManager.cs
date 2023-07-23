using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BatteryManager : MonoBehaviour {
    public float battery_percentage       = default_batt_percentage;  /// The battery charge level
    static float default_batt_percentage  = 100f;                     /// Default, initial, start percentage
    static float time_per_percent         = 18f;                      /// The time in seconds it takes for the battery level to decrease by 1
    static float percent_per_wrong_answer = 60f/time_per_percent;     /// Percent loss of battery for an incorrect answer

    public bool countdownStarted = false;  /// Indicates whether the battery has started discharging (with the start of the first puzzle)
    public bool isInSceneSwitch  = false;  /// if we need to switch between single and multiple Answer Scenes we need to prevent accessing Scene relevant Objects
    float nextTimeStamp;                   /// Time at which the next discharge of the battery will take place

    [SerializeReference] Sprite[] batt_sprites = new Sprite[4];

    TextWriter textWriter;
    SpriteRenderer? batt_sprite;
    TMP_Text? batt_text;

    void Update() {
        if ( !countdownStarted )          return;  //=> break out if coutdown hasnt yet started
        if ( Time.time <= nextTimeStamp ) return;  //=> break out if next TimeStamp hasn't been reached yet

        nextTimeStamp = Time.time + time_per_percent;  // set new future time stamp
        battery_percentage--;                          // reduce battery percentage
        updateBattery();                               // update new battery percentage/state to other dependencies
    }

    /// invoke the whole Objects workflow => starting it 
    public void setup(TextWriter TW) {
        searchForGameObj();

        textWriter = TW;
        countdownStarted   = true;
        battery_percentage = default_batt_percentage;
        nextTimeStamp      = Time.time + time_per_percent;
    }

    /// stop the whole Objects workflow
    public void stop() {
        countdownStarted = false;
        batt_sprite = null;
        batt_text   = null;
    }

    /// tell the Script to prepare for a Scene Change
    public void initiateSceneChange() {
        isInSceneSwitch = true;
        batt_sprite = null;
        batt_text   = null;
    }

    /// tell the Script that the Scene Change is over
    public void endSceneChange() {
        isInSceneSwitch = false;
        searchForGameObj();
        updateBattery();
    }

    /// method to reduce the percentage, because of a wrong answer
    public void wrongAnswer() {
        battery_percentage -= percent_per_wrong_answer;
        updateBattery();
    }

    /// search in the current Scene for the Sprite and Text holders 
    void searchForGameObj() {
        batt_sprite = GameObject.FindGameObjectWithTag("BattSprite").GetComponent<SpriteRenderer>();
        batt_text   = GameObject.FindGameObjectWithTag("BattText").GetComponent<TMP_Text>();
    }

    /// intiate a Game Over due to 
    void gameOver() {
        stop();
        battery_percentage = default_batt_percentage;
        textWriter.changeSceneToIndex( TextWriter.SCENE.GAME_OVER ); //* initiate the Game Over Scene
    }

    /// update new battery percentage/state to other dependencies
    void updateBattery() {
        if (isInSceneSwitch) return; //=> prevent accessing Scene relevant Objects in Scene Change

        if (battery_percentage <= 0) {
            gameOver();
            return;
        }

        batt_text.text  = Mathf.RoundToInt(battery_percentage).ToString() + " %";
        batt_text.color = Color.Lerp(Color.red, Color.green, 0.01f*battery_percentage);  // interpolate between RED and GREEN depending on the batt percentage
        
        //: why this formular works: https://www.desmos.com/calculator/9vsolgfbw4
        batt_sprite.sprite = batt_sprites[ Mathf.FloorToInt( 4 - .04f*battery_percentage ) ];
    }
}
