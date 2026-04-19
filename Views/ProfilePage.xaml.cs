using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class ProfilePage : ContentPage
{
    private static readonly string[] Avatars =
        ["♟","♛","♚","♜","♝","♞","🎯","🔥","💎","👑","🦁","🐉","⚡","🌟","🎭","🛡️"];

    private string _pendingAvatarPath = "";
    private string _pendingEmoji      = "";

    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var p = AppState.Current.Profile;

        NameEntry.Text    = p.Name;
        CountryEntry.Text = p.Country;
        StateEntry.Text   = p.State;

        _pendingAvatarPath = p.AvatarPath;
        _pendingEmoji      = p.Avatar;

        RefreshAvatarDisplay();
    }

    private void RefreshAvatarDisplay()
    {
        bool hasPhoto = !string.IsNullOrEmpty(_pendingAvatarPath) && File.Exists(_pendingAvatarPath);
        AvatarImage.IsVisible = hasPhoto;
        AvatarLabel.IsVisible = !hasPhoto;

        if (hasPhoto)
            AvatarImage.Source = ImageSource.FromFile(_pendingAvatarPath);
        else
            AvatarLabel.Text = string.IsNullOrEmpty(_pendingEmoji) ? "♟" : _pendingEmoji;
    }

    private async void OnPickEmojiClicked(object? sender, EventArgs e)
    {
        string? choice = await DisplayActionSheet("Escolha seu avatar", "Cancelar", null, Avatars);
        if (choice == null || choice == "Cancelar") return;

        _pendingEmoji      = choice;
        _pendingAvatarPath = "";          // limpa a foto se escolheu emoji
        RefreshAvatarDisplay();
    }

    private async void OnPickPhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Escolha uma foto"
            });

            if (result == null) return;

            var destPath = Path.Combine(FileSystem.AppDataDirectory, "avatar_photo.jpg");

            using var src  = await result.OpenReadAsync();
            using var dest = File.OpenWrite(destPath);
            await src.CopyToAsync(dest);

            _pendingAvatarPath = destPath;
            RefreshAvatarDisplay();
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permissão necessária",
                "Permita o acesso às fotos nas configurações do dispositivo.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar a foto: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Nome obrigatório", "Por favor, informe seu nome.", "OK");
            return;
        }

        var p        = AppState.Current.Profile;
        p.Name       = name;
        p.Country    = CountryEntry.Text?.Trim() ?? "";
        p.State      = StateEntry.Text?.Trim() ?? "";
        p.AvatarPath = _pendingAvatarPath;

        if (string.IsNullOrEmpty(_pendingAvatarPath))
            p.Avatar = string.IsNullOrEmpty(_pendingEmoji) ? "♟" : _pendingEmoji;

        await Shell.Current.GoToAsync("..");
    }
}
