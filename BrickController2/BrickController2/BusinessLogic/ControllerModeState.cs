using System;
using System.Collections.Generic;
using System.Text;
using BrickController2.Helpers;


namespace BrickController2.BusinessLogic
{
    public class ControllerModeState : NotifyPropertyChangedSource
    {
        public ControllerModeState(string name, bool state)
        {
            _state = state;
            Name = name;
        }
        public string Name { get; set; }

        private bool _state;
        public bool State { get { return _state; } set { _state = value; RaisePropertyChanged(); } }
    }
}
