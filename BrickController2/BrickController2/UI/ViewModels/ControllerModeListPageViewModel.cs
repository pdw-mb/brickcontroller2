using BrickController2.CreationManagement;
using BrickController2.UI.Commands;
using BrickController2.UI.Services.Dialog;
using BrickController2.UI.Services.Navigation;
using BrickController2.UI.Services.Translation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;


using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrickController2.UI.ViewModels
{
    public class ControllerModeListPageViewModel : PageViewModelBase
    {
        private readonly ICreationManager _creationManager;
        private readonly IDialogService _dialogService;
        private readonly ControllerProfile _controllerProfile;

        private CancellationTokenSource _disappearingTokenSource;

        public ControllerModeListPageViewModel(
            INavigationService navigationService,
            ITranslationService translationService,
            ICreationManager creationManager,
            IDialogService dialogService,
            NavigationParameters parameters)
            : base(navigationService, translationService)
        {
            _creationManager = creationManager;
            _dialogService = dialogService;

            _controllerProfile = parameters.Get<ControllerProfile>("controllerprofile");

            AddControllerModeCommand = new SafeCommand(async () => await AddControllerModeAsync());
            //SequenceTappedCommand = new SafeCommand<Sequence>(async sequence => await NavigationService.NavigateToAsync<SequenceEditorPageViewModel>(new NavigationParameters(("sequence", sequence))));
            //DeleteSequenceCommand = new SafeCommand<Sequence>(async (sequence) => await DeleteSequenceAsync(sequence));
        }
        public override void OnAppearing()
        {
            _disappearingTokenSource?.Cancel();
            _disappearingTokenSource = new CancellationTokenSource();
        }

        public ObservableCollection<ControllerMode> ControllerModes => _controllerProfile.ControllerModes;

        public ICommand AddControllerModeCommand { get; }
        public ICommand ControllerModeTappedCommand { get; }
        public ICommand DeleteControllerModeCommand { get; }

        private async Task AddControllerModeAsync()
        {
            try
            {
                var result = await _dialogService.ShowInputDialogAsync(
                    null,
                    Translate("ModeName"),
                    Translate("Create"),
                    Translate("Cancel"),
                    KeyboardType.Text,
                    (modeName) => !string.IsNullOrEmpty(modeName),
                    _disappearingTokenSource.Token);

                if (result.IsOk)
                {
                    if (string.IsNullOrWhiteSpace(result.Result))
                    {
                        await _dialogService.ShowMessageBoxAsync(
                            Translate("Warning"),
                            Translate("ModeNameCanNotBeEmpty"),
                            Translate("Ok"),
                            _disappearingTokenSource.Token);

                        return;
                    }
                    else if (!(await _controllerProfile.IsModeNameAvailableAsync(result.Result)))
                    {
                        await _dialogService.ShowMessageBoxAsync(
                            Translate("Warning"),
                            Translate("ModeNameIsUsed"),
                            Translate("Ok"),
                            _disappearingTokenSource.Token);

                        return;
                    }

                    ControllerMode mode = null;
                    await _dialogService.ShowProgressDialogAsync(
                        false,
                        async (progressDialog, token) =>
                        {
                            mode = await _creationManager.AddControllerModeAsync(_controllerProfile, result.Result);
                        },
                        Translate("Creating"));

                    //await NavigationService.NavigateToAsync<SequenceEditorPageViewModel>(new NavigationParameters(("sequence", sequence)));
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
