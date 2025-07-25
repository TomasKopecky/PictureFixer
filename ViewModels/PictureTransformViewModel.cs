﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PictureFixer.Modals;
using PictureFixer.Services;
using PictureFixer.Validators;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui.Controls;

namespace PictureFixer.ViewModels
{
    public partial class PictureTransformViewModel : ObservableObject
    {
        private readonly IFolderPickerDialog _folderPickerDialog;
        private readonly IPictureValidator _pictureValidator;
        private readonly IPictureConverter _pictureConverter;
        private readonly IMessageBoxService _messageBoxService;

        private CancellationTokenSource _cts = new();
        public ObservableCollection<string> ConvertedFiles { get; } = new();
        public ObservableCollection<string> FailedFiles { get; } = new();

        [ObservableProperty]
        private string _sourceFolderName = string.Empty;

        [ObservableProperty]
        private string _destinationFolderName = string.Empty;

        [ObservableProperty]
        private double _currentProgressValue;

        [ObservableProperty]
        private double _maxProgressValue;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isStatusMessage;

        [ObservableProperty]
        private bool _isError;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private InfoBarSeverity _statusSeverity;

        [ObservableProperty]
        private string _progressText = string.Empty;

        public bool IsNotLoading => !IsLoading;

        public PictureTransformViewModel(IFolderPickerDialog folderPickerDialog, IPictureValidator pictureValidator, IPictureConverter pictureConverter, IMessageBoxService messageBoxService)
        {
            _folderPickerDialog = folderPickerDialog;
            _pictureValidator = pictureValidator;
            _pictureConverter = pictureConverter;
            _messageBoxService = messageBoxService;
        }

        partial void OnIsLoadingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotLoading));
        }

        [RelayCommand]
        private void BrowseSource()
        {
            SourceFolderName = _folderPickerDialog.PickFolder() ?? string.Empty;
        }

        [RelayCommand]
        private void BrowseDestination()
        {
            DestinationFolderName = _folderPickerDialog.PickFolder() ?? string.Empty;
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            var currentTheme = Wpf.Ui.Appearance.ApplicationThemeManager.GetAppTheme();
            var themeToToggle = currentTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark ? Wpf.Ui.Appearance.ApplicationTheme.Light : Wpf.Ui.Appearance.ApplicationTheme.Dark;

            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                  themeToToggle
            );
        }

        [RelayCommand]
        private void StopConversion()
        {
            _cts?.Cancel();
        }

        [RelayCommand]
        private async Task StartConversion()
        {
            IsError = false;

            ConvertedFiles.Clear();
            FailedFiles.Clear();
            CurrentProgressValue = 0;
            ProgressText = string.Empty;
            IsStatusMessage = false;

            if (!AreFoldersOk(SourceFolderName, DestinationFolderName))
            {
                IsLoading = false;
                return;
            }

            IEnumerable<string> files = Directory.GetFiles(SourceFolderName).Where(file => _pictureValidator.IsValid(file));

            MaxProgressValue = files.Count();

            if (MaxProgressValue == 0)
            {
                _messageBoxService.ShowError("Zdrojový adresář neobsahuje žádné obrázky");
                IsLoading = false;
                return;
            }

            IsLoading = true;
            _cts = new();

            var progress = new Progress<PictureConversionProgress>(info =>
            {
                if (info.IsSuccess)
                    ConvertedFiles.Add($"{info.File} - OK");
                else
                    FailedFiles.Add($"{info.File} - Chyba"/*: {info.ErrorMessage}"*/);

                CurrentProgressValue += 1;
                ProgressText = $"Zpracováno souborů: {CurrentProgressValue}/{MaxProgressValue}";
            });

            try
            {
                await Task.Run(() =>
                _pictureConverter.Convert(
                    files,
                    DestinationFolderName,
                    progress,
                    _cts.Token)
                );

                StatusSeverity = InfoBarSeverity.Informational;
                StatusMessage = $"Úspěšně zpracováno: {ConvertedFiles.Count}/{MaxProgressValue}, chyby: {FailedFiles.Count()}";
            }
            catch (OperationCanceledException)
            {
                // Cancellation detected here!
                StatusSeverity = InfoBarSeverity.Warning;
                StatusMessage = $"Operace byla zrušena uživatelem - úspěšně zpracováno: {ConvertedFiles.Count}/{MaxProgressValue}, chyby: {FailedFiles.Count()}";
            }
            catch (Exception)
            {
                IsError = true;
                _messageBoxService.ShowError("Došlo k chybě při převodu obrázků");
            }
            finally
            {
                _cts.Dispose();
                IsStatusMessage = true;
                IsLoading = false;
            }
        }

        private bool AreFoldersOk(string sourceFolder, string destinationFolder)
        {
            if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(destinationFolder))
            {
                _messageBoxService.ShowError("Musí být vyplněn zdrojová a cílový adresář");
                return false;
            }

            else if (!Directory.Exists(sourceFolder))
            {
                _messageBoxService.ShowError("Zdrojový adresář neexistuje");
                return false;
            }

            else if (!Directory.Exists(destinationFolder))
            {
                _messageBoxService.ShowError("Cílový adresář neexistuje");
                return false;
            }

            else if (!Directory.GetFiles(sourceFolder).Any())
            {
                _messageBoxService.ShowError("Zdrojový adresář neobsahuje žádné soubory");
                return false;
            }

            else if (sourceFolder == destinationFolder)
            {
                _messageBoxService.ShowError("Zdrojový i cílový adresář jsou totožné - zakázaný stav");
                return false;
            }

            return true;
        }
    }
}
