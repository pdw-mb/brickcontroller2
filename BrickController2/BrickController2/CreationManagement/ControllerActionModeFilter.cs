using SQLite;
using SQLiteNetExtensions.Attributes;
using BrickController2.Helpers;


namespace BrickController2.CreationManagement 
{
    public class ControllerActionModeFilter : NotifyPropertyChangedSource
    {

        bool? _state;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(ControllerAction))]
        public int ControllerActionId { get; set; }

        [ManyToOne]
        public ControllerAction ControllerAction { get; set; }

        [ForeignKey(typeof(ControllerMode))]
        public int ControllerModeId { get; set; }

        [ManyToOne]
        public ControllerMode ControllerMode { get; set; }
        
        public bool? State
        {
            get { return _state; }
            set { _state = value; RaisePropertyChanged(); }
        }

    }
}
