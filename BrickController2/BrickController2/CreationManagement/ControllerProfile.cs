using BrickController2.Helpers;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace BrickController2.CreationManagement
{
    public class ControllerProfile : NotifyPropertyChangedSource
    {
        private string _name;
        private ObservableCollection<ControllerEvent> _controllerEvents = new ObservableCollection<ControllerEvent>();
        private ObservableCollection<ControllerMode> _controllerModes = new ObservableCollection<ControllerMode>();


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Creation))]
        public int CreationId { get; set; }

        [ManyToOne]
        public Creation Creation { get; set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public ObservableCollection<ControllerEvent> ControllerEvents
        {
            get { return _controllerEvents; }
            set { _controllerEvents = value; RaisePropertyChanged(); }
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public ObservableCollection<ControllerMode> ControllerModes
        {
            get { return _controllerModes; }
            set { _controllerModes = value; RaisePropertyChanged(); }
        }

        public override string ToString()
        {
            return Name;
        }

        public async Task<bool> IsModeNameAvailableAsync(string name)
        {
            return await Task<bool>.Run(() => { return true; });
        }



    }
}
