using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace minipdv.Presentation.Desktop.Components.Controls;

public class SearchableComboBox : UserControl
{
    private readonly TextBox textBox;
    private readonly Form dropForm;
    private readonly ListBox listBox;
    private IList? _dataSource;
    private string _displayMember = "";
    private string _valueMember = "";
    private object? _selectedValue;
    private object? _selectedItem;
    private bool _suppressTextChange;
    private bool _dropdownVisible;
    private readonly System.Windows.Forms.Timer _filterTimer;

    public SearchableComboBox()
    {
        Height = 30;

        dropForm = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            ShowInTaskbar = false,
            TopMost = false,
            StartPosition = FormStartPosition.Manual,
            Size = new Size(200, 200),
            MinimumSize = new Size(50, 30)
        };

        listBox = new ListBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.None,
            IntegralHeight = false
        };
        dropForm.Controls.Add(listBox);

        textBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            AutoCompleteMode = AutoCompleteMode.None
        };
        Controls.Add(textBox);

        textBox.TextChanged += TextBox_TextChanged;
        textBox.KeyDown += TextBox_KeyDown;
        textBox.Enter += (_, _) => ShowDropdown();
        textBox.MouseClick += (_, _) => ShowDropdown();
        textBox.LostFocus += (_, _) =>
        {
            if (!dropForm.Focused && !listBox.Focused)
                HideDropdown();
        };

        listBox.Click += ListBox_Click;
        listBox.MouseDoubleClick += ListBox_DoubleClick;
        listBox.KeyDown += ListBox_KeyDown;

        dropForm.Deactivate += (_, _) => HideDropdown();

        _filterTimer = new System.Windows.Forms.Timer { Interval = 300 };
        _filterTimer.Tick += (_, _) =>
        {
            _filterTimer.Stop();
            ApplyFilter();
        };

        Disposed += (_, _) =>
        {
            _filterTimer.Dispose();
            dropForm.Dispose();
        };
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList? DataSource
    {
        get => _dataSource;
        set
        {
            _dataSource = value;
            listBox.DataSource = null;
            listBox.Items.Clear();
            if (_dataSource != null)
            {
                listBox.DataSource = _dataSource;
                listBox.DisplayMember = _displayMember;
                listBox.ValueMember = _valueMember;
            }
            if (!string.IsNullOrEmpty(textBox.Text))
                ApplyFilter();
        }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DisplayMember
    {
        get => _displayMember;
        set
        {
            _displayMember = value;
            listBox.DisplayMember = value;
        }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ValueMember
    {
        get => _valueMember;
        set
        {
            _valueMember = value;
            listBox.ValueMember = value;
        }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? SelectedValue
    {
        get => _selectedValue;
        set
        {
            _selectedValue = value;
            SyncTextBoxFromValue();
        }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            if (value != null)
            {
                _selectedValue = GetValueFromItem(value);
                SyncTextBoxFromItem(value);
            }
        }
    }

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PlaceholderText
    {
        get => textBox.PlaceholderText;
        set => textBox.PlaceholderText = value;
    }

    public event EventHandler? SelectedValueChanged;

    public void ClearSelection()
    {
        _selectedValue = null;
        _selectedItem = null;
        _suppressTextChange = true;
        textBox.Clear();
        _suppressTextChange = false;
    }

    private void ShowDropdown()
    {
        if (_dataSource == null || _dataSource.Count == 0) return;
        if (_dropdownVisible) return;
        _dropdownVisible = true;

        var screenLocation = PointToScreen(new Point(0, Height));
        dropForm.Location = screenLocation;
        dropForm.Width = Width;

        // Show all items when opening dropdown, regardless of current text
        var currentFilter = textBox.Text;
        _suppressTextChange = true;
        textBox.Clear();
        _suppressTextChange = false;

        ApplyFilter();

        _suppressTextChange = true;
        textBox.Text = currentFilter;
        textBox.Select(textBox.Text.Length, 0);
        _suppressTextChange = false;

        var itemCount = listBox.Items.Count;
        if (itemCount > 0)
        {
            dropForm.Height = Math.Min(200, itemCount * listBox.ItemHeight + 4);
            listBox.SelectedIndex = 0;
        }
        else
        {
            dropForm.Height = 30;
        }

        dropForm.Show(this);
        dropForm.BringToFront();
    }

    private void HideDropdown()
    {
        if (!_dropdownVisible) return;
        _dropdownVisible = false;
        dropForm.Hide();
    }

    private void TextBox_TextChanged(object? sender, EventArgs e)
    {
        if (_suppressTextChange) return;
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private void ApplyFilter()
    {
        if (_dataSource == null) return;

        var filter = textBox.Text.Trim();
        List<object> filtered;
        if (string.IsNullOrEmpty(filter))
        {
            filtered = _dataSource.Cast<object>().ToList();
        }
        else
        {
            filtered = _dataSource.Cast<object>()
                .Where(item => ItemMatchesFilter(item, filter))
                .ToList();
        }

        var prevSelected = listBox.SelectedItem;
        _suppressTextChange = true;
        listBox.DataSource = null;
        listBox.Items.Clear();

        if (filtered.Count > 0)
        {
            listBox.DataSource = filtered;
            listBox.DisplayMember = _displayMember;
            listBox.ValueMember = _valueMember;
        }

        if (prevSelected != null && listBox.Items.Contains(prevSelected))
            listBox.SelectedItem = prevSelected;
        else if (listBox.Items.Count > 0)
            listBox.SelectedIndex = 0;

        _suppressTextChange = false;

        if (filtered.Count > 0)
        {
            if (_dropdownVisible)
                dropForm.Height = Math.Min(200, filtered.Count * listBox.ItemHeight + 4);
        }
        else
        {
            HideDropdown();
        }
    }

    private bool ItemMatchesFilter(object item, string filter)
    {
        var display = GetDisplayFromItem(item);
        return display.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private string GetDisplayFromItem(object item)
    {
        if (string.IsNullOrEmpty(_displayMember))
            return item?.ToString() ?? "";
        var prop = item.GetType().GetProperty(_displayMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null)
            return prop.GetValue(item)?.ToString() ?? "";
        return item?.ToString() ?? "";
    }

    private object? GetValueFromItem(object item)
    {
        if (string.IsNullOrEmpty(_valueMember))
            return item;
        var prop = item.GetType().GetProperty(_valueMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return prop?.GetValue(item);
    }

    private void SelectCurrentItem()
    {
        if (listBox.SelectedItem == null && listBox.Items.Count > 0)
            listBox.SelectedIndex = 0;

        if (listBox.SelectedItem == null) return;

        _selectedItem = listBox.SelectedItem;
        _selectedValue = GetValueFromItem(_selectedItem);
        SyncTextBoxFromItem(_selectedItem);
        HideDropdown();
        SelectedValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SyncTextBoxFromItem(object item)
    {
        _suppressTextChange = true;
        textBox.Text = GetDisplayFromItem(item);
        textBox.Select(textBox.Text.Length, 0);
        _suppressTextChange = false;
    }

    private void SyncTextBoxFromValue()
    {
        if (_selectedValue == null || _dataSource == null)
        {
            _suppressTextChange = true;
            textBox.Clear();
            _suppressTextChange = false;
            return;
        }

        foreach (var item in _dataSource)
        {
            var val = GetValueFromItem(item);
            if (Equals(val, _selectedValue))
            {
                _selectedItem = item;
                SyncTextBoxFromItem(item);
                return;
            }
        }
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Down:
                if (!_dropdownVisible) ShowDropdown();
                else if (listBox.SelectedIndex < listBox.Items.Count - 1)
                    listBox.SelectedIndex++;
                e.SuppressKeyPress = true;
                break;
            case Keys.Up:
                if (listBox.SelectedIndex > 0)
                    listBox.SelectedIndex--;
                e.SuppressKeyPress = true;
                break;
            case Keys.Enter:
            case Keys.Tab:
                SelectCurrentItem();
                e.SuppressKeyPress = true;
                break;
            case Keys.Escape:
                HideDropdown();
                e.SuppressKeyPress = true;
                break;
        }
    }

    private void ListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Enter:
            case Keys.Tab:
                SelectCurrentItem();
                e.SuppressKeyPress = true;
                break;
            case Keys.Escape:
                HideDropdown();
                e.SuppressKeyPress = true;
                break;
        }
    }

    private void ListBox_Click(object? sender, EventArgs e)
    {
        SelectCurrentItem();
    }

    private void ListBox_DoubleClick(object? sender, MouseEventArgs e)
    {
        SelectCurrentItem();
    }
}
