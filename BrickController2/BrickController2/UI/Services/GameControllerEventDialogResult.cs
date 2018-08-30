﻿using BrickController2.HardwareServices;

namespace BrickController2.UI.Services
{
    public class GameControllerEventDialogResult
    {
        public GameControllerEventDialogResult(bool isOk, GameControllerEventType eventType, string eventCode)
        {
            IsOk = isOk;
            EventType = eventType;
            EventCode = eventCode;
        }

        public bool IsOk { get; }
        public GameControllerEventType EventType { get; }
        public string EventCode { get; }
    }
}
