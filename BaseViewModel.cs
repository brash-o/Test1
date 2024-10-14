 public abstract class BaseViewModel : PropertyChangedBase
    {
        protected BaseViewModel(IMainWindows windows)
        {
            _context = windows.Context;
            _windows = windows;
        }

        protected readonly IOTableDataContext _context;
        protected readonly IMainWindows _windows;
    }
