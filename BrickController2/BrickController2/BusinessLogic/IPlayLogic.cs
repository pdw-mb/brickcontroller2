﻿using BrickController2.CreationManagement;
using BrickController2.PlatformServices.GameController;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BrickController2.BusinessLogic
{
    public interface IPlayLogic
    {
        ControllerProfile ActiveProfile { get; set; }

        ObservableCollection<ControllerModeState> ControllerModeStates { get; set; }

        CreationValidationResult ValidateCreation(Creation creation);
        bool ValidateControllerAction(ControllerAction controllerAction);

        void StartPlay();
        void StopPlay();

        void ProcessGameControllerEvent(GameControllerEventArgs e);
    }
}
