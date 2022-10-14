using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ServoCommand
{
    private string objectName;
    private string legPart;
    private float value;

    public string legOneObjectName = "leg1";
    public string legTwoObjectName = "leg2";
    public string legThreeObjectName = "leg3";
    public string legFourObjectName = "leg4";

    public string hipName = "hip";
    public string armName = "arm";
    public string legName = "leg";

    private float servoRange = 180.0f;

    public ServoCommand(string objectName, string legPart, float value)
    {
        this.objectName = objectName;
        this.legPart = legPart;
        this.value = value;
    }

    public string ToString()
    {
        return objectName + " " + legPart + " " + value;
    }

    public void Apply(CreatureControl cc)
    {
        PodControl[] arms = cc.GetArms();
        PodControl arm = (objectName == legOneObjectName ? arms[0] :
                          objectName == legTwoObjectName ? arms[1] :
                          objectName == legThreeObjectName ? arms[2] :
                          objectName == legFourObjectName ? arms[3] : null);
        if (arm == null)
        {
            Debug.LogError("Malformed Servo Command: Leg Name");
            return;
        }

        if (legPart == hipName)
        {
            arm.SetHipTarget(value / servoRange);
        }
        else if (legPart == armName)
        {
            arm.SetArmTarget(value / servoRange);
        }
        else if (legPart == legName)
        {
            arm.SetLegTarget(value / servoRange);
        }
        else
        {
            Debug.LogError("Malformed Servo Command: Leg Part Name");
        }
    }
}

public class PodCommand : ServoCommand
{
    private int podId;
    private float hipValue;
    private float armValue;
    private float legValue;

    public PodCommand(int podId, float hipValue, float armValue, float legValue) : base("","",0)
    {
        this.podId = podId;
        this.hipValue = hipValue;
        this.armValue = armValue;
        this.legValue = legValue;
    }

    public void Apply(CreatureControl cc)
    {
        if (podId == 1)
        {
            new ServoCommand(legOneObjectName, hipName, hipValue).Apply(cc);
            new ServoCommand(legOneObjectName, armName, armValue).Apply(cc);
            new ServoCommand(legOneObjectName, legName, legValue).Apply(cc);
        }
        else if (podId == 2)
        {
            new ServoCommand(legTwoObjectName, hipName, hipValue).Apply(cc);
            new ServoCommand(legTwoObjectName, armName, armValue).Apply(cc);
            new ServoCommand(legTwoObjectName, legName, legValue).Apply(cc);
        }
        else if (podId == 3)
        {
            new ServoCommand(legThreeObjectName, hipName, hipValue).Apply(cc);
            new ServoCommand(legThreeObjectName, armName, armValue).Apply(cc);
            new ServoCommand(legThreeObjectName, legName, legValue).Apply(cc);
        }
        else if (podId == 4)
        {
            new ServoCommand(legFourObjectName, hipName, hipValue).Apply(cc);
            new ServoCommand(legFourObjectName, armName, armValue).Apply(cc);
            new ServoCommand(legFourObjectName, legName, legValue).Apply(cc);
        }
        else
        {
            Debug.LogError("PodCommand.Apply Malformed PodID");
        }
    }

    
}

public class ArduninoCreatureSimulator : MonoBehaviour
{

    public TextAsset arduinoScript;
    private string scriptString;
    private CreatureControl creatureController;

    public float delayTime = 0.0f;
    public int commandLineCounter = 1;
    private List<string> program;
    public bool runningProgram = false;

    // Start is called before the first frame update
    void Start()
    {
        creatureController = GetComponent<CreatureControl>();
        scriptString = arduinoScript.ToString();
        program = ParseLines(scriptString);
    }

    // Update is called once per frame
    void Update()
    {
        if (!runningProgram) return;

        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            if (delayTime < 0) delayTime = 0;
            return;
        }

        if (commandLineCounter - 1 == program.Count) return;
        string nextCommand = program[commandLineCounter - 1];
        commandLineCounter++;


        if (nextCommand.StartsWith('#')) return; // Commenting out with '#'

        Debug.Log("Recieved Command: " + nextCommand);

        if (nextCommand.Contains("delay"))
        {
            float millisec = float.Parse(nextCommand.Substring(nextCommand.IndexOf("(") + 1, nextCommand.IndexOf(")") - nextCommand.IndexOf("(") - 1));
            delayTime = millisec / 1000.0f;
            return;
        }
        else if (nextCommand.Contains("goto"))
        {
            int index = int.Parse(nextCommand.Substring(nextCommand.IndexOf("(") + 1, nextCommand.IndexOf(")") - nextCommand.IndexOf("(") - 1));
            commandLineCounter = index;
            return;
        }
        else if (nextCommand.Contains("controlPod"))
        {
            PodCommand command = ProcessPodCommand(nextCommand);
            command.Apply(creatureController);
        }
        else if (nextCommand != "")
        {
            ServoCommand command = ProcessServoCommand(nextCommand);
            command.Apply(creatureController);
        }

        if (commandLineCounter - 1 == program.Count)
        {
            Debug.Log("Reached end of program.");
        }
    }

    List<string> ParseLines(string script)
    {
        List<string> commands = new List<string>();
        for (string line = GetNextLine(ref script); line != ""; line = GetNextLine(ref script))
        {
            commands.Add(line);
        }
        return commands;
    }

    string GetNextLine(ref string script)
    {
        int newlineIndex = script.IndexOf('\n');
        if (newlineIndex == -1)
        {
            string temp = script;
            script = "";
            return temp;
        }
        string line = script.Substring(0, newlineIndex);
        script = script.Remove(0, newlineIndex + 1);
        return line;
    }

    PodCommand ProcessPodCommand(string command)
    {
        //controlPod(arm,h,a,l)
        command = command.Remove(0, command.IndexOf('(') + 1);
        
        int armID = int.Parse(command.Substring(0,command.IndexOf(',')));
        command = command.Remove(0, command.IndexOf(',') + 1);

        float hipValue = float.Parse(command.Substring(0,command.IndexOf(',')));
        command = command.Remove(0, command.IndexOf(',') + 1);

        float armValue = float.Parse(command.Substring(0,command.IndexOf(',')));
        command = command.Remove(0, command.IndexOf(',') + 1);

        float legValue = float.Parse(command.Substring(0,command.IndexOf(')')));
        command = command.Remove(0, command.IndexOf(')') + 1);

        Debug.Log(armID + " " + hipValue + " " + armValue + " " +  legValue);
        return new PodCommand(armID, hipValue, armValue, legValue);
    }

    ServoCommand ProcessServoCommand(string command)
    {

        string objectName = command.Substring(0,command.IndexOf('.'));
        command = command.Remove(0, objectName.Length + 1);
        string legPart = command.Substring(0,command.IndexOf('('));
        command = command.Remove(0, legPart.Length + 1);
        string value = command.Substring(0,command.IndexOf(')'));
        command = command.Remove(0, value.Length + 1);

        return new ServoCommand(objectName, legPart, float.Parse(value));
    }
}

[CustomEditor(typeof(ArduninoCreatureSimulator))]
[CanEditMultipleObjects]
class ArduninoCreatureSimulatorEditor : Editor
{

    SerializedProperty arduinoScript;
    SerializedProperty runningProgram;
    SerializedProperty commandLineCounter;
    
    void OnEnable()
    {
        arduinoScript = serializedObject.FindProperty("arduinoScript");
        runningProgram = serializedObject.FindProperty("runningProgram");
        commandLineCounter = serializedObject.FindProperty("commandLineCounter");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(arduinoScript);
        EditorGUILayout.PropertyField(commandLineCounter);

        ArduninoCreatureSimulator sim = (ArduninoCreatureSimulator) target;

        if (GUILayout.Button("Run Program"))
        {
            sim.runningProgram = true;
        }

        if (GUILayout.Button("Pause Program"))
        {
            sim.runningProgram = false;
        }

        if (GUILayout.Button("Restart Program"))
        {
            sim.commandLineCounter = 1;
            sim.runningProgram = true;
        }

        serializedObject.ApplyModifiedProperties();
    }


    // public override void OnInspectorGUI()
    // {
    //     if (GUILayout.Button("Run Program"))
    //     {
    //         runningProgram = true;
    //         runningProgram.ApplyModifiedProperties();
    //     }
    // }
}
