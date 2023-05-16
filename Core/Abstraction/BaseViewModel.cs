namespace GG40.Core.Abstraction
{
    public class BaseViewModel : BindableModel
    {
        private bool _isCreated;
        private bool _isLoading;

        public BaseViewModel()
        {

        }

        protected virtual void Create()
        {

        }

        protected virtual void Refresh()
        {

        }

        public void Start()
        {
            if (!_isCreated)
            {
                Create();

                _isCreated = true;
            }
            else
            {
                Refresh();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
    }
}
