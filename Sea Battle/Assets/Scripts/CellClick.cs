using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaBattle;
using UnityEngine;

namespace Assets.Scripts
{
    public class CellClick : MonoBehaviour
    {
        public delegate void MouseClickEventHandler(object sender, MouseActionEventArgs e);
        public event MouseClickEventHandler MouseClickEvent;

        public GridPoint position;
        // Use this for initialization
        private void Start()
        {
            MouseClickEvent += gameObject.GetComponentInParent<Grid>().OnCellClick;
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnMouseDown()
        {
            if (!enabled) return;
            if (MouseClickEvent != null)
            {
                MouseClickEvent(this, new MouseActionEventArgs(position));
                enabled = false;
            }
        }
    }
}
