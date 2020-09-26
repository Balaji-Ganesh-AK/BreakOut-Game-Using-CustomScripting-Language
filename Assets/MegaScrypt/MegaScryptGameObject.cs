using System;
using UnityEngine;

public class MegaScryptGameObject : MegaScrypt.Object
{
    private GameObject target;
    public GameObject Target => target;

    public MegaScryptGameObject(GameObject ITarget)
    {
        this.target = ITarget;
        Bind();
    }

    private void Bind()
    {
        Declare("name", () => target.name, (object name) =>
        {
            target.name = (string)name;
        });

        Declare("x",
        () => target.transform.position.x,
        (object x) =>
        {

            Vector3 pos = target.transform.position;
            pos.x = System.Convert.ToSingle(x);
            target.transform.position = pos;
        });

        Declare("y",
            () => target.transform.position.y,
            (object y) =>
            {

                Vector3 pos = target.transform.position;
                pos.y = System.Convert.ToSingle(y);
                target.transform.position = pos;
            });

        Declare("z",
            () => target.transform.position.z,
            (object z) =>
            {

                Vector3 pos = target.transform.position;
                pos.z = System.Convert.ToSingle(z);
                target.transform.position = pos;
            });

        Declare("color",
        () => target.GetComponent<MeshRenderer>().material.color,
        (object color) =>
        {
            string colorString = (string)color;
            MeshRenderer renderer = target.GetComponent<MeshRenderer>();

            if (ColorUtility.TryParseHtmlString(colorString, out Color color1))
            {
                renderer.material.color = color1;
            }
        });

        Declare("width",
            () => target.transform.localScale.x);
        Declare("height",
            () => target.transform.localScale.y);

    }
          
    }

