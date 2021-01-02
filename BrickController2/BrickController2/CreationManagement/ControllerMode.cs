using BrickController2.Helpers;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace BrickController2.CreationManagement
{
    public class ControllerMode : NotifyPropertyChangedSource
    {
        private string _name;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(ControllerProfile))]
        public int ControllerProfileId { get; set; }

        [ManyToOne]
        public ControllerProfile ControllerProfile { get; set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
