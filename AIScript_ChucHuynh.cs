using UnityEngine;
using System.Collections;

public class AIScript_ChucHuynh : MonoBehaviour
{

    public CharacterScript mainScript;

    public float[] bombSpeeds; //returns the speed in which the bombs move
    public float[] buttonCooldowns; //returns the cooldown before the button can be pressed again
    public float playerSpeed; //returns the speed at which your a character moves
    public int[] beltDirections; //1 = moving away from character, -1 = moving towards character, 0 = stationary
    public float[] buttonLocations; //returns array of float values representing the position of each button on your side
    public float cLocation; //returns the z position of your character's
    public float oLocation; //returns the z position of your opponent's character
    public float[] bLocation; //returns array of float values that represent distance each bomb is from its corresponding button on your side
    private int targetBeltIndex;

    //basically it returns the location of the bomb and its button

    //mainScript.moveDown(); makes it so that the player moves down the belts
    //mainScript.moveUp(); makes it so that the player moves up the belts
    //mainScript.push(); makes it so that the player attempts to push the button to send the bombs back

    // Use this for initialization
    void Start ()
    {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();
        //playerSpeed = mainScript.getPlayerSpeeds();
    }

	// Update is called once per frame
	void Update ()
    {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();








        //Your AI code goes here

        //make it so that the AI goes to the belt furthest away from the player to drop
        //the bomb. On the way, return the bombs the opponent has sent down

        float minDistance = 1000;
        int minIndex = 0;   
        float curDistance; //the current distance from the targeted button's location and the character's location

        bombSpeeds = mainScript.getBombSpeeds();
        buttonLocations = mainScript.getButtonLocations();
        cLocation = mainScript.getCharacterLocation();
        oLocation = mainScript.getOpponentLocation();
        bLocation = mainScript.getBombDistances();        

        //for everytime the bomb is moving away from the player...
        for (int i = 0; i < beltDirections.Length; i++)
        {            
            curDistance = Mathf.Abs(buttonLocations[i] - mainScript.getCharacterLocation()); //the current distance between the button's location and the character's location
            float bTime = bLocation[i] / bombSpeeds[i]; //the time it takes for the bomb to explode on your side
            float cTime = Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[i]) / mainScript.getPlayerSpeed(); //the time it takes for you to get to your target location
            
            //if the button no longer has a cooldown, and a bomb is moving towards you or the bomb is stationary
            if (buttonCooldowns[i] <= 0 && (beltDirections[i] == -1))
            //if (buttonCooldowns[i] <= 0 && (beltDirections[i] == -1 || beltDirections[i] == 0))
                {
                //and if the distance between you and the target button is less than 1000
                if (curDistance < minDistance)
                {
                    //and if the time it takes for you to get to the location is less than the time it takes for the bomb to explode
                    //and if the button cooldown gets to 0 before the bomb can expldoe
                    if (cTime < bTime && bTime > buttonCooldowns[i])
                    {
                        //then set the minIndex equal to i. minIndex now is set as the location where that specific bomb is moving towards you or where the stationary bomb is
                        minIndex = i;
                        //and make the minDistance from 1000 equal to the distance between you and the target button
                        minDistance = curDistance;
                        //this in essence is now your "target". This is now the belt where you want to push the bomb back.
                    }
                }
            }
        }

        //targetBeltIndex = 0
        targetBeltIndex = minIndex; //make the targeted belt's location to where the minIndex's location is. The minIndex's location is (as above), the bomb moving towards you or the stationary bomb
        //use this to make it so that the character only pushes the button if it is within 1 radius from the button. This is to prevent the character from possibly missing
        float buttonRadius = 1;
        //the character can make it in time if the time it takes to get to the targetBeltIndex is less than the time it takes for the bomb to explode
        bool inTime = (Mathf.Abs(cLocation - buttonLocations[targetBeltIndex]) / mainScript.getPlayerSpeed()) + 0.35f < bLocation[targetBeltIndex] / bombSpeeds[targetBeltIndex];

        //if the button at the targeted belt location is below the character's location
        if (buttonLocations[targetBeltIndex] < cLocation)
        {
            //then make the character move down
            mainScript.moveDown();

            //push the button only if the character is within the range where it can press the button. This is to ensure that the character from missing.
            if ((buttonLocations[targetBeltIndex] - cLocation) < buttonRadius)
            {
                //and if the character can make it
                if (inTime)
                {
                    //then push the button to send the bomb back
                    mainScript.push();
                }

                //if the character can't make it in time, then go to a belt with a bomb coming towards it
                else if (buttonCooldowns[targetBeltIndex] <= 0 && (beltDirections[targetBeltIndex] == -1))
                {
                    mainScript.push();
                }
            }
        }
        else
        {
            //otherwise, if the button at the targeted belt location is above the character's location then make the characer move up
            mainScript.moveUp();
            //and make them attempt to push the closest button
            if ((buttonLocations[targetBeltIndex] - cLocation) < buttonRadius)
            {
                if (inTime)
                {
                    mainScript.push();
                }

                else if (buttonCooldowns[targetBeltIndex] <= 0 && (beltDirections[targetBeltIndex] == -1))
                {
                    mainScript.push();
                }
            }       
        }
    }
}
