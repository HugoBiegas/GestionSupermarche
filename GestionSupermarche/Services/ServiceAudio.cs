using Plugin.Maui.Audio;


namespace GestionSupermarche.Services
{
    public class ServiceAudio
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _lecteurAudio;
        private bool _estEnLecture;
        private float _volumeDefaut = 0.5f;
        private string _nomFichierEnCours;

        public ServiceAudio(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _estEnLecture = false;
            InitialiserMusiqueFond();
        }

        private async void InitialiserMusiqueFond()
        {
            await DemarrerMusiqueFond("musique_fond.mp3");
        }

        public async Task DemarrerMusiqueFond(string nomFichier)
        {
            try
            {
                _nomFichierEnCours = nomFichier;
                if (_lecteurAudio == null)
                {
                    var flux = await FileSystem.OpenAppPackageFileAsync(nomFichier);
                    _lecteurAudio = _audioManager.CreatePlayer(flux);
                    _lecteurAudio.Volume = _volumeDefaut;

                    _lecteurAudio.PlaybackEnded += async (sender, args) =>
                    {
                        _estEnLecture = false;
                        _lecteurAudio.Dispose();
                        _lecteurAudio = null;
                        await DemarrerMusiqueFond(_nomFichierEnCours);
                    };
                }

                if (!_estEnLecture)
                {
                    _lecteurAudio.Play();
                    _estEnLecture = true;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur Audio", $"Impossible de lire le fichier audio : {ex.Message}", "OK");
            }
        }

        public async Task MettreEnPause()
        {
            if (_lecteurAudio != null && _estEnLecture)
            {
                _lecteurAudio.Pause();
                _estEnLecture = false;
            }
        }

        public async Task Reprendre()
        {
            if (_lecteurAudio != null && !_estEnLecture)
            {
                _lecteurAudio.Play();
                _estEnLecture = true;
            }
        }

        public void Arreter()
        {
            if (_lecteurAudio != null)
            {
                _lecteurAudio.Stop();
                _lecteurAudio.Dispose();
                _lecteurAudio = null;
                _estEnLecture = false;
            }
        }

        public void AjusterVolume(float volume)
        {
            if (_lecteurAudio != null)
            {
                _lecteurAudio.Volume = Math.Clamp(volume, 0f, 1f);
            }
        }

        public bool EstEnLecture()
        {
            return _estEnLecture;
        }
    }
}