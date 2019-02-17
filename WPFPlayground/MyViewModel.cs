namespace WPFPlayground
{
    public class MyViewModel
    {
        private readonly IMyService _service;

        public MyViewModel(IMyService service)
        {
            _service = service;
        }

        public string TextFromService => _service.GetText();
    }
}