using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Lab1.Core;
using Lab1.Util;

namespace Lab1.ViewModel;

public class MainViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string _keyValue = null!;
    private string _textInputValue = null!;
    private string _keyLengthWarning = null!;
    private string _outputValue = null!;
    private string _iv = null!;
    private KeyLenght _selectedKeySize = Util.KeyLenght.Aes128;
    
    private readonly ErrorsViewModel _errorsViewModel;
    
    public bool HasErrors => _errorsViewModel.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    
    public ICommand EncryptCommand { get; }
    public ICommand DecryptCommand { get; }
    
    public event PropertyChangedEventHandler? PropertyChanged = null!;

    public string TextInputValue
    {
        get => _textInputValue;
        set
        {
            _errorsViewModel!.ClearErrors(nameof(TextInputValue));
            if (String.IsNullOrEmpty(value))
            {
                _errorsViewModel.AddError(
                    nameof(TextInputValue),
                    "Field is required");
            }
            
            if (_textInputValue != value)
            {
                _textInputValue = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string KeyValue
    {
        get => _keyValue;
        set
        {
            KeyLengthWarning = "";
            _errorsViewModel!.ClearErrors(nameof(KeyValue));
            if (String.IsNullOrEmpty(value))
            {
                _errorsViewModel.AddError(
                    nameof(KeyValue),
                    "Field is required");
            }
            else if (value.Length < KeyLenght)
            {
                KeyLengthWarning = $"Warning: only {value.Length * 8} bits entered for {(int)SelectedKeySize} bit key";
            }
            if (value.Length > KeyLenght)
            {
                OnPropertyChanged();
            }
            else if (_keyValue != value)
            {
                _keyValue = value;
                OnPropertyChanged();
            }
        }
    }

    public string KeyLengthWarning
    {
        get => _keyLengthWarning;
        set
        {
            _keyLengthWarning = value;
            OnPropertyChanged();
        }
    }
    
    public KeyLenght SelectedKeySize
    {
        get => _selectedKeySize;
        set
        {
            if (_selectedKeySize != value)
            {
                _selectedKeySize = value;
                OnPropertyChanged();
                KeyValue = _keyValue;
            }
        }
    }

    public String OutputValue
    {
        get => _outputValue;
        set 
        { 
            _outputValue = value; 
            OnPropertyChanged(); 
        }
    }
    
    public String Iv
    {
        get => _iv;
        set 
        { 
            _iv = value; 
            OnPropertyChanged(); 
        }
    }

    private int KeyLenght => (int)SelectedKeySize / 8;
    
    public MainViewModel()
    {
        EncryptCommand = new RelayCommand(_ => EncryptText(), _ =>
        {
            _errorsViewModel!.ClearErrors(nameof(TextInputValue));
            _errorsViewModel!.ClearErrors(nameof(KeyValue));
            if (String.IsNullOrEmpty(TextInputValue))
            {
                _errorsViewModel.AddError(
                    nameof(TextInputValue),
                    "Field is required");
            }
            
            return !_errorsViewModel!.HasErrors;
        });
        DecryptCommand = new RelayCommand(_ => DecryptText(), _ =>
        {
            _errorsViewModel!.ClearErrors(nameof(TextInputValue));
            _errorsViewModel!.ClearErrors(nameof(KeyValue));
            if (String.IsNullOrEmpty(TextInputValue))
            {
                _errorsViewModel.AddError(
                    nameof(TextInputValue),
                    "Field is required");
            }

            return !_errorsViewModel!.HasErrors;
        });
        
        _errorsViewModel = new ErrorsViewModel();
        _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;
    }
    
    private void EncryptText()
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(TextInputValue);
        var key = Encoding.UTF8.GetBytes(KeyValue);
        
        var temp = Aes.Encrypt(plaintextBytes, key, SelectedKeySize);
        
        Iv = String.Join(" ", temp.iv.Select(x => x.ToString("X2")));
        OutputValue = String.Join(" ", temp.ciphertext.Select(x => x.ToString("X2")));
    }

    private byte[] HexStringToByteArray(string hex)
    {
        string[] hexValues = hex.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        byte[] byteArray = new byte[hexValues.Length];
        for (int i = 0; i < hexValues.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hexValues[i], 16);
        }

        return byteArray;
    }
    
    private void DecryptText()
    {
        var inputText  = HexStringToByteArray(TextInputValue);
        var key = Encoding.UTF8.GetBytes(KeyValue);
        var iv = HexStringToByteArray(Iv);
        
        var text = Aes.Decrypt(inputText, key, SelectedKeySize, iv);
        OutputValue = Encoding.UTF8.GetString(text);
    }
    
    public IEnumerable GetErrors(string propertyName)
    {
        return _errorsViewModel.GetErrors(propertyName);
    }
    
    private void ErrorsViewModel_ErrorsChanged(
        object sender, 
        DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(HasErrors));
    }
    
    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, 
            new PropertyChangedEventArgs(propertyName));
    }
}
