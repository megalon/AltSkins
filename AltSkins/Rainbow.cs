using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AltSkins
{
    public class Rainbow : MonoBehaviour
    {
        public float Speed = 0.25f;
        public float Offset = 0f;
        Material mat;
        void Start()
        {
            //Console.WriteLine("GAMING");
            mat = this.GetComponent<Renderer>().sharedMaterial;
        }

        void Update()
        {
            //mat.SetColor("_Color", new Color(1, 0, 0));
            Color color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed + Offset, 1), 1, 1));
            mat.SetColor("_Color", color);
            mat.SetColor("_Emission", Color.Lerp(color, new Color(0,0,0), 0.5f));
            mat.SetColor("_SpecularColor", color);
        }
    }
}
