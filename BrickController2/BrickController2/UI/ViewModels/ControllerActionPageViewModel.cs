using BrickController2.CreationManagement;
using BrickController2.DeviceManagement;
using BrickController2.UI.Commands;
using BrickController2.UI.Services.Dialog;
using BrickController2.UI.Services.Navigation;
using BrickController2.UI.Services.Preferences;
using BrickController2.UI.Services.Translation;
using BrickController2.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrickController2.UI.ViewModels
{
    public class ControllerActionPageViewModel : PageViewModelBase
    {
        private readonly ICreationManager _creationManager;
        private readonly IDeviceManager _deviceManager;
        private readonly IDialogService _dialogService;
        private readonly IPreferencesService _preferences;

        private CancellationTokenSource _disappearingTokenSource;

        private Device _selectedDevice;

        public ControllerActionPageViewModel(
            INavigationService navigationService,
            ITranslationService translationService,
            ICreationManager creationManager,
            IDeviceManager deviceManager,
            IDialogService dialogService,
            IPreferencesService preferences,
            NavigationParameters parameters)
            : base(navigationService, translationService)
        {
            _creationManager = creationManager;
            _deviceManager = deviceManager;
            _dialogService = dialogService;
            _preferences = preferences;

            ControllerAction = parameters.Get<ControllerAction>("controlleraction", null);
            ControllerEvent = parameters.Get<ControllerEvent>("controllerevent", null) ?? ControllerAction?.ControllerEvent;

            var device = _deviceManager.GetDeviceById(ControllerAction?.DeviceId);
            if (ControllerAction != null && device != null)
            {
                SelectedDevice = device;
                Action.Channel = ControllerAction.Channel;
                Action.IsInvert = ControllerAction.IsInvert;
                Action.ChannelOutputType = ControllerAction.ChannelOutputType;
                Action.MaxServoAngle = ControllerAction.MaxServoAngle;
                Action.ButtonType = ControllerAction.ButtonType;
                Action.AxisType = ControllerAction.AxisType;
                Action.AxisCharacteristic = ControllerAction.AxisCharacteristic;
                Action.MaxOutputPercent = ControllerAction.MaxOutputPercent;
                Action.AxisActiveZonePercent = ControllerAction.AxisActiveZonePercent;
                Action.AxisDeadZonePercent = ControllerAction.AxisDeadZonePercent;
                Action.ServoBaseAngle = ControllerAction.ServoBaseAngle;
                Action.StepperAngle = ControllerAction.StepperAngle;
                Action.SequenceName = ControllerAction.SequenceName;
                Action.ControllerModeName = ControllerAction.ControllerModeName;
                Action.ControllerActionModeFilters = new Dictionary<string,ControllerActionModeFilterType>(ControllerAction.ControllerActionModeFilters);
            }
            else
            {
                var lastSelectedDeviceId = _preferences.Get<string>("LastSelectedDeviceId", null, "com.scn.BrickController2.ControllerActionPage");
                SelectedDevice = _deviceManager.GetDeviceById(lastSelectedDeviceId) ?? _deviceManager.Devices.FirstOrDefault();
                Action.Channel = 0;
                Action.IsInvert = false;
                Action.ChannelOutputType = ChannelOutputType.NormalMotor;
                Action.MaxServoAngle = 90;
                Action.ButtonType = ControllerButtonType.Normal;
                Action.AxisType = ControllerAxisType.Normal;
                Action.AxisCharacteristic = ControllerAxisCharacteristic.Linear;
                Action.MaxOutputPercent = 100;
                Action.AxisActiveZonePercent = 100;
                Action.AxisDeadZonePercent = 0;
                Action.ServoBaseAngle = 0;
                Action.StepperAngle = 90;
                Action.SequenceName = string.Empty;
                Action.ControllerActionModeFilters = new Dictionary<string, ControllerActionModeFilterType>();


            }
            PopulateControllerActionModeFiltersViewModel();

            SaveControllerActionCommand = new SafeCommand(async () => await SaveControllerActionAsync(), () => SelectedDevice != null && !_dialogService.IsDialogOpen);
            SelectDeviceCommand = new SafeCommand(async () => await SelectDeviceAsync());
            OpenDeviceDetailsCommand = new SafeCommand(async () => await OpenDeviceDetailsAsync(), () => SelectedDevice != null);
            SelectChannelOutputTypeCommand = new SafeCommand(async () => await SelectChannelOutputTypeAsync(), () => SelectedDevice != null);
            OpenChannelSetupCommand = new SafeCommand(async () => await OpenChannelSetupAsync(), () => SelectedDevice != null);
            SelectButtonTypeCommand = new SafeCommand(async () => await SelectButtonTypeAsync());
            SelectSequenceCommand = new SafeCommand(async () => await SelectSequenceAsync());
            SelectControllerModeCommand = new SafeCommand(async () => await SelectControllerModeAsync());
            OpenSequenceEditorCommand = new SafeCommand(async () => await OpenSequenceEditorAsync());
            SelectAxisTypeCommand = new SafeCommand(async () => await SelectAxisTypeAsync());
            SelectAxisCharacteristicCommand = new SafeCommand(async () => await SelectAxisCharacteristicAsync());
            SelectModeFilterCommand = new SafeCommand<ControllerActionModeFilterViewModel>(async (mode) => await SelectModeFilterAsync(mode));
        }

        public ObservableCollection<Device> Devices => _deviceManager.Devices;
        public ObservableCollection<string> Sequences => new ObservableCollection<string>(_creationManager.Sequences.Select(s => s.Name).ToArray());

        public ObservableCollection<string> ControllerModes => new ObservableCollection<string>(ControllerEvent.ControllerProfile.ControllerModes.Select(m => m.Name).ToArray());
        public ControllerEvent ControllerEvent { get; }
        public ControllerAction ControllerAction { get; }

        public ObservableCollection<ControllerActionModeFilterViewModel> ControllerActionModeFiltersViewModel { get; } = new ObservableCollection<ControllerActionModeFilterViewModel>();


        private void PopulateControllerActionModeFiltersViewModel()
        {
            ControllerActionModeFiltersViewModel.Clear();
            foreach (var controllerMode in ControllerEvent.ControllerProfile.ControllerModes)
            {
                ControllerActionModeFilterType state;
                if (Action.ControllerActionModeFilters.TryGetValue(controllerMode.Name, out state))
                {
                    ControllerActionModeFiltersViewModel.Add(new ControllerActionModeFilterViewModel(controllerMode, state));
                }
                else
                {
                    ControllerActionModeFiltersViewModel.Add(new ControllerActionModeFilterViewModel(controllerMode, ControllerActionModeFilterType.Ignore));
                }
            }
        }


        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                Action.DeviceId = value.Id;

                if (_selectedDevice.NumberOfChannels <= Action.Channel)
                {
                    Action.Channel = 0;
                }

                RaisePropertyChanged();
            }
        }

        public ControllerAction Action { get; } = new ControllerAction();

        public ICommand SaveControllerActionCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        public ICommand SelectChannelOutputTypeCommand { get; }
        public ICommand OpenDeviceDetailsCommand { get; }
        public ICommand OpenChannelSetupCommand { get; }
        public ICommand SelectButtonTypeCommand { get; }
        public ICommand SelectSequenceCommand { get; }
        public ICommand SelectControllerModeCommand { get; }
        public ICommand OpenSequenceEditorCommand { get; }
        public ICommand SelectAxisTypeCommand { get; }
        public ICommand SelectAxisCharacteristicCommand { get; }
        public ICommand SelectModeFilterCommand { get; }

        public bool IsModeSelectButtonType { get { return Action.ButtonType == ControllerButtonType.SetMode || Action.ButtonType == ControllerButtonType.ToggleMode; } }

        public override void OnAppearing()
        {
            _disappearingTokenSource?.Cancel();
            _disappearingTokenSource = new CancellationTokenSource();
        }

        public override void OnDisappearing()
        {
            _preferences.Set<string>("LastSelectedDeviceId", _selectedDevice.Id, "com.scn.BrickController2.ControllerActionPage");

            _disappearingTokenSource?.Cancel();
        }

        private async Task SaveControllerActionAsync()
        {
            if (SelectedDevice == null)
            {
                await _dialogService.ShowMessageBoxAsync(
                    Translate("Warning"),
                    Translate("SelectDeviceBeforeSaving"),
                    Translate("Ok"),
                    _disappearingTokenSource.Token);
                return;
            }

            await _dialogService.ShowProgressDialogAsync(
                false,
                async (progressDialog, token) =>
                {
                    if (ControllerAction != null)
                    {
                        await _creationManager.UpdateControllerActionAsync(
                            ControllerAction,
                            Action.DeviceId,
                            Action.Channel,
                            Action.IsInvert,
                            Action.ButtonType,
                            Action.AxisType,
                            Action.AxisCharacteristic,
                            Action.MaxOutputPercent,
                            Action.AxisActiveZonePercent,
                            Action.AxisDeadZonePercent,
                            Action.ChannelOutputType,
                            Action.MaxServoAngle,
                            Action.ServoBaseAngle,
                            Action.StepperAngle,
                            Action.SequenceName,
                            Action.ControllerModeName,
                            Action.ControllerActionModeFilters);
                    }
                    else
                    {
                        await _creationManager.AddControllerActionAsync(
                            ControllerEvent,
                            Action.DeviceId,
                            Action.Channel,
                            Action.IsInvert,
                            Action.ButtonType,
                            Action.AxisType,
                            Action.AxisCharacteristic,
                            Action.MaxOutputPercent,
                            Action.AxisActiveZonePercent,
                            Action.AxisDeadZonePercent,
                            Action.ChannelOutputType,
                            Action.MaxServoAngle,
                            Action.ServoBaseAngle,
                            Action.StepperAngle,
                            Action.SequenceName,
                            Action.ControllerModeName,
                            Action.ControllerActionModeFilters);
                    }
                },
                Translate("Saving"));

            await NavigationService.NavigateBackAsync();
        }

        private async Task SelectDeviceAsync()
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Devices,
                Translate("SelectDevice"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                SelectedDevice = result.SelectedItem;
            }
        }

        private async Task OpenDeviceDetailsAsync()
        {
            if (SelectedDevice == null)
            {
                return;
            }

            await NavigationService.NavigateToAsync<DevicePageViewModel>(new NavigationParameters(("device", SelectedDevice)));
        }

        private async Task SelectChannelOutputTypeAsync()
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Enum.GetNames(typeof(ChannelOutputType)),
                Translate("ChannelType"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                Action.ChannelOutputType = (ChannelOutputType)Enum.Parse(typeof(ChannelOutputType), result.SelectedItem);
            }
        }

        private async Task OpenChannelSetupAsync()
        {
            if (SelectedDevice == null)
            {
                return;
            }

            await NavigationService.NavigateToAsync<ChannelSetupPageViewModel>(new NavigationParameters(("device", SelectedDevice), ("controlleraction", Action)));
        }

        private async Task SelectButtonTypeAsync()
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Enum.GetNames(typeof(ControllerButtonType)),
                Translate("ButtonType"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                Action.ButtonType = (ControllerButtonType)Enum.Parse(typeof(ControllerButtonType), result.SelectedItem);
                RaisePropertyChanged(nameof(IsModeSelectButtonType));
            }
        }

        private async Task SelectSequenceAsync()
        {
            if (Sequences.Any())
            {
                var result = await _dialogService.ShowSelectionDialogAsync(
                    Sequences,
                    Translate("SelectSequence"),
                    Translate("Cancel"),
                    _disappearingTokenSource.Token);

                if (result.IsOk)
                {
                    Action.SequenceName = result.SelectedItem;
                }
            }
            else
            {
                await _dialogService.ShowMessageBoxAsync(
                    Translate("Warning"),
                    Translate("NoSequences"),
                    Translate("Ok"),
                    _disappearingTokenSource.Token);
            }
        }

        private async Task OpenSequenceEditorAsync()
        {
            var selectedSequence = _creationManager.Sequences.FirstOrDefault(s => s.Name == Action.SequenceName);

            if (selectedSequence != null)
            {
                await NavigationService.NavigateToAsync<SequenceEditorPageViewModel>(new NavigationParameters(("sequence", selectedSequence)));
            }
            else
            {
                await _dialogService.ShowMessageBoxAsync(
                    Translate("Warning"),
                    Translate("MissingSequence"),
                    Translate("Ok"),
                    _disappearingTokenSource.Token);
            }
        }

        private async Task SelectControllerModeAsync()
        {
            if (ControllerModes.Any())
            {
                var result = await _dialogService.ShowSelectionDialogAsync(
                    ControllerModes,
                    Translate("SelectMode"),
                    Translate("Cancel"),
                    _disappearingTokenSource.Token);

                if (result.IsOk)
                {
                    Action.ControllerModeName = result.SelectedItem;
                }
            }
            else
            {
                await _dialogService.ShowMessageBoxAsync(
                    Translate("Warning"),
                    Translate("NoModes"),
                    Translate("Ok"),
                    _disappearingTokenSource.Token);
            }
        }

        private async Task SelectAxisTypeAsync()
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Enum.GetNames(typeof(ControllerAxisType)),
                Translate("AxisType"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                Action.AxisType = (ControllerAxisType)Enum.Parse(typeof(ControllerAxisType), result.SelectedItem);
            }
        }

        private async Task SelectAxisCharacteristicAsync()
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Enum.GetNames(typeof(ControllerAxisCharacteristic)),
                Translate("AxisCharacteristic"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                Action.AxisCharacteristic = (ControllerAxisCharacteristic)Enum.Parse(typeof(ControllerAxisCharacteristic), result.SelectedItem);
            }
        }

        private async Task SelectModeFilterAsync(ControllerActionModeFilterViewModel camfvm)
        {
            var result = await _dialogService.ShowSelectionDialogAsync(
                Enum.GetNames(typeof(ControllerActionModeFilterType)),
                Translate("ModeFilterState"),
                Translate("Cancel"),
                _disappearingTokenSource.Token);

            if (result.IsOk)
            {
                Action.ControllerActionModeFilters[camfvm.ControllerMode.Name] =  (ControllerActionModeFilterType)Enum.Parse(typeof(ControllerActionModeFilterType), result.SelectedItem);
                PopulateControllerActionModeFiltersViewModel();
            }
        }

        public class ControllerActionModeFilterViewModel : NotifyPropertyChangedSource
        {

            public ControllerActionModeFilterViewModel(ControllerMode controllerMode, ControllerActionModeFilterType state)
            {
                ControllerMode = controllerMode;
                _state = state;
            }

            private ControllerActionModeFilterType _state;
            public ControllerMode ControllerMode { get; }
            public ControllerActionModeFilterType State { 
                get { 
                    return _state;  
                } 
                set { 
                    _state = value; 
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(StateName));
                } 
            }

            public string StateName
            {
                get
                {
                    return Enum.GetName(typeof(ControllerActionModeFilterType), State);
                }
            }
        }
    }
}
