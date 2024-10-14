public abstract class BaseDialogViewModel : BaseViewModel
{
    protected BaseDialogViewModel(IMainWindows windows) : base(windows)
    {
        _closeCommand = new OCommand(Close);
    }

    #region DialogResult

    public bool? DialogResult
    {
        get { return _dialogResult; }
        set
        {
            _dialogResult = value;
            OnPropertyChaned(() => DialogResult);
        }
    }

    bool? _dialogResult;

    #endregion

    #region CloseCommand

    public OCommand CloseCommand { get { return _closeCommand; } }
    private readonly OCommand _closeCommand;

    protected virtual void Close()
    {
        DialogResult = true;
    }

    protected virtual void OnClose() { }

    #endregion


}

public abstract class BaseRefreshViewModel : BaseDialogViewModel
{
    protected BaseRefreshViewModel(IMainWindows windows)
        : base(windows)
    {
        _refreshCommand = new OCommand(Refresh);
    }

    #region RefreshCommand

    public IOCommand RefreshCommand { get { return _refreshCommand; } }
    private readonly OCommand _refreshCommand;

    protected virtual void Refresh(){ }

    #endregion
}

public interface IBaseDialogViewModelSave
{
    IOCommand SaveCommand { get; }
}


public abstract class BaseDialogViewModel<T> : BaseRefreshViewModel, IBaseDialogViewModelSave where T : class,INotifyPropertyChanged, new()
{
    protected BaseDialogViewModel(T item, bool isnew, IMainWindows windows) : this(item, isnew, windows, false) { }

    protected BaseDialogViewModel(T item, bool isnew,IMainWindows windows, bool needRefresh):base(windows)
    {
        if (item == null)
            throw new NullReferenceException("item of type " + typeof(T).Name + " can't be null");
        _isnew = isnew;
        Item = item;
        var state = _context.GetEntityState(Item);
        if (needRefresh && state!=null)
            _context.Refresh(Item);
        _saveCommand = new OCommand(Save, CanSave);
    }

    protected virtual void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        HasChange = true;
        OnPropChange(e.PropertyName);
    }

    protected virtual void OnPropChange(string prop)
    {

    }

    #region HasChange

    public bool HasChange
    {
        get { return _hasChange; }
        set
        {
            _hasChange = value;
            OnPropertyChaned(() => HasChange);
            Raise();
        }
    }


    private bool _hasChange;

    #endregion

    #region IsNew

    private bool _isnew;

    public bool IsNew
    {
        get { return _isnew; }
        set
        {
            _isnew = value;
            OnPropertyChaned(() => IsNew);
        }
    }

    #endregion

    #region Item

    private T _item;

    public T Item
    {
        get { return _item; }
        set
        {
            if (_item != null && _item != value)
                _item.PropertyChanged -= Item_PropertyChanged;
            _item = value;
            if (_item != null )
                _item.PropertyChanged += Item_PropertyChanged;
            OnItemChanged();
            OnPropertyChaned(() => Item);
        }
    }

    protected virtual void OnItemChanged()
    {
    }

    #endregion

    protected virtual void Raise()
    {
        SaveCommand.RaiseCanExecuteChanged();
    }

    public virtual bool CanEdit
    {
        get { return false; }
    }

    protected override void Close()
    {
        if (HasChange && !_isnew)
        {
            var state = _context.GetEntityState(Item);
            if (state != null)
                _context.Refresh(Item);
        }
        OnClose();
        if (Item != null)
            Item.PropertyChanged -= Item_PropertyChanged;
        base.Close();
    }

    #region SaveCommand

    public IOCommand SaveCommand { get { return _saveCommand; } }
    private readonly OCommand _saveCommand;

    protected abstract void Save();
    protected abstract bool CanSave();

    #endregion

    protected override void Refresh()
    {
        if (IsNew && _context.ContainsInChangeSet(Item))
        {
            _context.DeleteOnSubmit(Item);
            _context.SubmitChanges();
        }
        if (HasChange && !IsNew)
        {
            _context.Refresh(RefreshMode.OverwriteCurrentValues, Item);
        }
    }
}
