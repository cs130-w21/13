using System;

/// <summary>
/// Class that gets the string of codes from the panel and turns in into the
/// string whiuch can be understood by the robot
/// </summary>
public class codeprocessor
{
    ///To hold the code  string from the panel
    private string code = "";

    /// <summary>
    /// to hold the result of the translation
    /// </summary>
    private string result = "";

    /// <summary>
    /// funciton to go through the funciton and transalte the string from the panel
    /// to a string that the robot can understand
    /// </summary>
    private void codeProcessor()
    {
        int loopCounter = 0;        //loop counter
        int loopSP = -1;            //loop starting position
        string tempCounter = "";
        for (int i = 0; i < code.Length; i++)
        {
            // get the information of the loop
            if (code[i] == 'l')
            {
                i++;
                for (; code[i] != '{'; i++)
                {
                    tempCounter += code[i];
                }
                loopSP = i;
                i++;
                Console.Out.WriteLine(tempCounter);
                int.TryParse(tempCounter, out loopCounter);
            }

            //To check if the loop is finished or there are more repetitions of the
            //loop code needed
            if (code[i] == '}')
            {
                if (loopCounter > 1)
                {
                    i = loopSP;
                    loopCounter--;
                }
                else
                {
                    loopSP = -1;
                }
            }
            //all other characters in the string are supposed to be correctly formatted :) so they just get sent out
            else
            {
                result += code[i];
            }
        }
    }

    /// <summary>
    /// get the result of the code translation
    /// </summary>
    /// <returns> returns a string which hold the result of the translation </returns>
    public string getResult()
    {
        codeProcessor();
        return result;
    }

    /// <summary>
    /// set the code to the new code string and empty the previouse results
    /// </summary>
    /// <param name="pCode"></param> Code from the panel which might have a loop in it
    public void setCode(string pCode)
    {
        code = pCode;
        result = "";
    }
}


//A little tihng to test the code above
// also to use the codeProcessor you make the object, give it the code string, and ask for the result.
/*
public class test
{
    public static void Main()
    {
        codeprocessor code1 = new codeprocessor();

        code1.setCode("RLLRLFFBBl3{RLLFB}F");
        string result = code1.getResult();

        Console.Out.WriteLine(result);

    }
}*/