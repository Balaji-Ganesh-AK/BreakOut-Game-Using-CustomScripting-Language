using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using MegaScrypt;
using MegaScryptLib;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class BreakOutGameController : MonoBehaviour
{

    Machine machine;

    [SerializeField] private TextAsset programSource;
    [SerializeField] private string program;

    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private GameObject rectPrefab;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound;


    [SerializeField] private List<MegaScryptGameObject> megaScryptGameObjects =  new List<MegaScryptGameObject>();

    [SerializeField] private Canvas canvas;
    [SerializeField]private GameObject displayText;

    private List<GameObject> ListTextToDisplay =new List<GameObject>();


    private static BreakOutGameController _instance_;
    private List<object> parameter;

    public static BreakOutGameController Instance()
    {
        return _instance_;
    }

    // Start is called before the first frame update
        void Start()
        {
            
        _instance_ = this;
        SetupMachine();
        LoadProgram();
        megaScryptGameObjects = new List<MegaScryptGameObject>();

        parameter = new List<object>();
        machine.TryInvoke("Start");
    }

    void SetupMachine()
    {




        machine = new Machine();

        machine.Declare(Log);
        machine.Declare(SpawnCircle);
        machine.Declare(SpawnRect);
        machine.Declare(Min);
        machine.Declare(Max);
        machine.Declare(GetKey);
        machine.Declare(PlaySfx);
        machine.Declare(DestroyObject);
        machine.Declare(DrawText);

        machine.Declare(new NativeFunction("Random", (List<object> parameters) =>
        {
            return Random.Range(
                Convert.ToSingle(parameters[0]),
                Convert.ToSingle(parameters[1])
            );
        }));
        parameter = new List<object>();

    }

    void LoadProgram()
    {
        if (programSource != null)
            program = programSource.text;

        machine.Execute(program);
    }

    // Update is called once per frame
    void Update()
    {
        parameter.Clear();

        foreach (var VARIABLE in ListTextToDisplay)
        {
            Destroy(VARIABLE.gameObject);
        }

        parameter.Add(Time.deltaTime);
        machine.TryInvoke("Update", parameter);

    }

    #region Bindings


    object Log(List<object> parameter)
    {
        string s = "";
        for (int i = 0; i < parameter.Count; i++)
        {
            if (i > 0)
                s += ",";
            if (parameter[i] == null)
                s += "null";
            else
            {
                s += parameter[i].ToString();
            }

        }
        Debug.Log(s);
        return null;
    }

    private object SpawnCircle(List<object> parameters)
    {
        int radius = (int) parameters[0];
        int x = (int) parameters[1];
        int y = (int) parameters[2];
        int z = 0;
        object prototype = parameters[3];
        string objectColor = (string) parameters[4];
    


        GameObject cirlceInstance = Instantiate(circlePrefab);
        cirlceInstance.transform.position = new Vector3(x,y,z);
        cirlceInstance.transform.localScale = new Vector3(radius,radius,1);



        if (ColorUtility.TryParseHtmlString(objectColor, out Color color))
        {
            cirlceInstance.GetComponent<MeshRenderer>().material.color = color;
        }

        MegaScryptGameObject msObj = new MegaScryptGameObject(cirlceInstance);
        msObj.Declare("prototype", prototype);
        megaScryptGameObjects.Add(msObj);
        return msObj;
    }

    private object SpawnRect(List<object> parameters)
    {
        int width = (int)parameters[0];
        int height = (int)parameters[1];
        int x = (int)parameters[2];
        int y = (int)parameters[3];
        int z = 0;
        object prototype = parameters[4];
        string objectColor = (string)parameters[5];

        GameObject rectInstance = Instantiate(rectPrefab);
        rectInstance.transform.position = new Vector3(x, y, z);
        rectInstance.transform.localScale = new Vector3(width, height, 1);

        if (ColorUtility.TryParseHtmlString(objectColor, out Color color))
        {
            rectInstance.GetComponent<MeshRenderer>().material.color = color;
        }

        MegaScryptGameObject obj = new MegaScryptGameObject(rectInstance);
        foreach (var VARIABLE in prototype as MegaScrypt.Object)
        {
            obj.Declare(VARIABLE.Key, VARIABLE.Value);
        }
       // obj.Declare("prototype", prototype);

        megaScryptGameObjects.Add(obj);
        return obj;
    }

    private object Max(List<object> parameters)
    {
        return Mathf.Max(
            Convert.ToSingle(parameters[0]),
            Convert.ToSingle(parameters[1])
        );
    }
    private object Min(List<object> parameters)
    {
        return Mathf.Min(
            Convert.ToSingle(parameters[0]),
            Convert.ToSingle(parameters[1])
        );
    }

    private object GetKey(List<object> parameters)
    {
        Enum.TryParse((string)parameters[0], out KeyCode key);
        return Input.GetKey(key);
    }



    private object DestroyObject(List<object> parameters)
    {
        MegaScryptGameObject megObj = (MegaScryptGameObject)parameters[0];

        GameObject targetObject = megObj.Target;

        for (int i = 0; i < megaScryptGameObjects.Count; i++)
        {
            if (targetObject == megaScryptGameObjects[i].Target)
            {
                Destroy(targetObject);
                megaScryptGameObjects.RemoveAt(i);
                break;
            }
        }

        return null;
    }

    private object PlaySfx(List<object> parameters)
    {
        string clipName = (string)parameters[0];

        sound.name = clipName;
        audioSource.clip = sound;
        audioSource.Play();
       

        return null;
    }


    private object DrawText(List<object> parameters)
    {
        string text = parameters[0] as string;
        int x = (int)parameters[1];
        int y =(int) parameters[2];

        GameObject textToAdd = Instantiate(displayText,canvas.transform);
        textToAdd.GetComponent<Text>().text = text;
        textToAdd.transform.localPosition = new Vector3(x,y,0);
        ListTextToDisplay.Add(textToAdd);
        

        
        return null;
    }


    public void CollisionTrigger(GameObject a, GameObject b)
    {
        MegaScryptGameObject gameObjectA = null;
        MegaScryptGameObject gameObjectB = null;


        for (int i = 0; i < megaScryptGameObjects.Count; i++)
        {
            if (megaScryptGameObjects[i].Target.name == a.name)
            {
                gameObjectA = megaScryptGameObjects[i];
            }
            else if (megaScryptGameObjects[i].Target.name == b.name)
            {
                gameObjectB = megaScryptGameObjects[i];
            }
        }


        if (gameObjectA!=null && gameObjectB!= null)
        {
            Debug.Log("A==> " + gameObjectA.Target + "B==> " + gameObjectB.Target);
            parameter.Clear();
            parameter.Add(gameObjectA);
            parameter.Add(gameObjectB);
            parameter.Add(Time.deltaTime);

            machine.TryInvoke("OnCollision", parameter);
        }
        else
        {
            Debug.Log("Not found");
        }
    }

    #endregion
}






