using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Jbh;
using Path = System.IO.Path;

namespace Santay;

public partial class JournalWindow
{
    
    public JournalWindow()
    {
        InitializeComponent();
        _logDictionary = new Dictionary<int, DayLog>();
        _tempMedicaments = new List<Medicament>();

        _currentDate = DateOnly.FromDateTime(DateTime.Today);
        _aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        _originDate = new DateOnly(2022, 7, 22);
    }

    private const string DayLogSeparator = "~";
    private const string ListSeparator = "$";
    private const string MedSeparator = "^";
    // TODO Ensure these are not entered in text boxes

    private readonly FontFamily _fixedFont = new("Consolas");
    
    private Dictionary<int, DayLog> _logDictionary;
    private readonly List<Medicament> _tempMedicaments;
    private bool _symptomsEdited;
    private bool _suppressDayLogUpdate;

    private readonly DateOnly _aujourdhui; // fixed per run
    private DateOnly _currentDate; // variable
    private readonly DateOnly _originDate; // fixed for all time

    private class DayLog : IComparable<DayLog>
    {
        public DayLog()
        {
            MedsTaken = new List<Medicament>();
            DateLabel = string.Empty;
            SymptomsAndActions = string.Empty;
            _jData = new PersonProfile();    
        }
        
        private readonly PersonProfile _jData;
        public bool Pooed { get; set; }
        public int DynamicChemCycleNumber { get; set; }
        public int DynamicChemCycleDay { get; set; }
        public int DynamicDaysSinceBpCheck { get; set; }
        public int DynamicDaysSinceBedChange { get; set; }
        public int DynamicDaysSinceExercise { get; set; }
        public bool DynamicBloodPressureMeasured => (_jData.DayTensions(DateOnly.FromDayNumber(DateIndex)).Count>0);
        public bool ChemoCycle { get; set; }
        public bool ChemoDayOne { get; set; }
        public bool BedChanged { get; set; }
        public bool Exercised { get; set; }
        public int DateIndex { get; set; } // int representing DateOnly by DayNumber (Key for dictionary) 
        public string DateLabel { get; set; }
        public List<Medicament> MedsTaken { get; private set; }

        public string SymptomsAndActions { get; set; }

        private string MedicamentListSpec
        {
            get
            {
                List<string> medSpecs = new List<string>();
                foreach (var medicament in MedsTaken)
                {
                    medSpecs.Add(medicament.Specification);
                }

                var ms = medSpecs.ToArray();
                return string.Join(ListSeparator, ms);
            }
            set
            {
                MedsTaken = new List<Medicament>();
                var meds = value.Split(ListSeparator.ToCharArray());
                foreach (var med in meds)
                {
                    var mt = new Medicament(med);
                    if (!string.IsNullOrWhiteSpace(mt.Name))
                    {
                        MedsTaken.Add(mt);
                    }
                }
            }
        }

        public string Specification
        {
            get
            {
                var parts = new string[9];
                parts[0] = $"{DateIndex}";
                parts[1] = DateLabel;
                parts[2] = $"{ChemoCycle}";
                parts[3] = $"{ChemoDayOne}";
                parts[4] = $"{Pooed}";
                parts[5] = $"{BedChanged}";
                parts[6] = $"{Exercised}";
                parts[7] = MedicamentListSpec;
                parts[8] = SymptomsAndActions;
                return string.Join(DayLogSeparator, parts);
            }
            set
            {
                var parts = value.Split(DayLogSeparator.ToCharArray());
                if (int.TryParse(parts[0], out var dt))
                {
                    DateIndex = dt;
                }

                DateLabel = parts[1];

                if (bool.TryParse(parts[2], out var chc))
                {
                    ChemoCycle = chc;
                }

                if (bool.TryParse(parts[3], out var chs))
                {
                    ChemoDayOne = chs;
                }

                if (bool.TryParse(parts[4], out var pd))
                {
                    Pooed = pd;
                }

                if (bool.TryParse(parts[5], out var bd))
                {
                    BedChanged = bd;
                }

                if (bool.TryParse(parts[6], out var gy))
                {
                    Exercised = gy;
                }

                MedicamentListSpec = parts[7];
                SymptomsAndActions = parts[8];
            }
        }
        
        public double MyWeight
        {
            get
            {
                var heavy = _jData.DayWeight(DateOnly.FromDayNumber(DateIndex));
                return heavy?.WgtKilograms ?? 0;
            }
        }
        
        public List<PersonProfile.Tension> MyBpList => _jData.DayTensions(DateOnly.FromDayNumber(DateIndex));

        public int CompareTo(DayLog? other)
        {

            if (other is { })
            {
                return DateIndex.CompareTo(other.DateIndex);
            }

            return 1;
        }
    }

    private class Medicament : IComparable<Medicament>
    {
        public Medicament(string spec)
        {
            Name = string.Empty;
            Specification = spec;
        }

        public Medicament(string name, int number, TimeSpan tim)
        {
            Name = name;
            NumberOfDoses = number;
            Taken = tim;
        }

        public TimeSpan Taken { get; private init; }

        public string Name { get; private init; }

        public int NumberOfDoses { get; private init; }

        public string Specification
        {
            get
            {
                var parts = new string[3];
                parts[0] = $"{Taken.Ticks}";
                parts[1] = $"{NumberOfDoses}";
                parts[2] = $"{Name}";
                return string.Join(MedSeparator, parts);
            }
            private init
            {
                var parts = value.Split(MedSeparator.ToCharArray());
                if (parts.Length < 3)
                {
                    Taken = TimeSpan.Zero;
                    NumberOfDoses = 0;
                    Name = string.Empty;
                    return;
                }

                if (long.TryParse(parts[0], out var dt))
                {
                    Taken = TimeSpan.FromTicks(dt);
                }

                if (int.TryParse(parts[1], out var nd))
                {
                    NumberOfDoses = nd;
                }

                Name = parts[2];
            }
        }

        public int CompareTo(Medicament? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var takenComparison = Taken.CompareTo(other.Taken);
            if (takenComparison != 0) return takenComparison;
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }

    private void JournalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var scrX = SystemParameters.PrimaryScreenWidth;
        var scrY = SystemParameters.PrimaryScreenHeight;
        var winX = scrX * .98;
        var winY = scrY * .94;
        var xm = (scrX - winX) / 2;
        var ym = (scrY - winY) / 4;
        Width = winX;
        Height = winY;
        Left = xm;
        Top = ym;

        LoadData();
        FillMedsCombos();
    }

    private void FillMedsCombos()
    {
        MedicamentTimeCombo.Items.Clear();
        var increment = new TimeSpan(0, 30, 0);
        var current = TimeSpan.Zero;
        while (current.TotalHours < 24)
        {
            MedicamentTimeCombo.Items.Add(new ComboBoxItem()
            {
                Tag = current.Ticks
                , Content = new TextBlock()
                {
                    Text = $"{current:hh\\:mm}"
                    , Foreground = current.TotalHours < 12 ? Brushes.CornflowerBlue : Brushes.Tomato
                }
            });
            current = current.Add(increment);
        }

        MedicamentTimeCombo.SelectedIndex = 0;

        MedicamentQuantityCombo.Items.Clear();
        for (int t = 0; t < 12; t++)
        {
            MedicamentQuantityCombo.Items.Add(new ComboBoxItem()
                {Tag = t, Content = new TextBlock() {Text = t.ToString()}});
        }

        MedicamentQuantityCombo.SelectedIndex = 0;

        RefreshMedicamentNamesCombo();

    }

    private void RefreshMedicamentNamesCombo()
    {
        MedicamentNameCombo.Items.Clear();
        List<string> names = new List<string>();
        foreach (var value in _logDictionary.Values)
        {
            foreach (var drug in value.MedsTaken)
            {
                if (!names.Contains(drug.Name))
                {
                    names.Add(drug.Name);
                }
            }
        }

        names.Sort();
        foreach (var nm in names)
        {
            MedicamentNameCombo.Items.Add(new ComboBoxItem() {Tag = nm, Content = new TextBlock() {Text = nm}});
        }
    }

    private void LoadData()
    {
        // populate dictionary with a blank log for each day from origin to today
        var alpha = _originDate.DayNumber;
        var omega = _aujourdhui.DayNumber;
        _logDictionary = new Dictionary<int, DayLog>();
        for (var z = alpha; z <= omega; z++)
        {
            var log = new DayLog()
            {
                DateIndex = z
            };
            _logDictionary.Add(z, log);
        }

        // import previously saved logs
        var path = Path.Combine(AppManager.DataPath, "Data.journal");
        if (File.Exists(path))
        {
            using var reader = new StreamReader(path);
            int lastLoaded = 0;
            bool lastChemoValue = false;
            while (!reader.EndOfStream)
            {
                var j = reader.ReadLine();
                if (j is null) continue;
                var log = new DayLog() {Specification = j};
                if (_logDictionary.ContainsKey(log.DateIndex))
                {
                    DayLog foundLog = _logDictionary[log.DateIndex];
                    foundLog.Specification = j;
                }

                lastLoaded = log.DateIndex;
                lastChemoValue = log.ChemoCycle;
            }

            // Repeat last loaded 'in chemo cycle' value onto subsequent days
            if (lastLoaded > 0)
            {
                for (var z = lastLoaded + 1; z <= omega; z++)
                {
                    _logDictionary[z].ChemoCycle = lastChemoValue;
                }
            }

            ReallocateChemoNumbers();
        }
    }

    private void ReallocateChemoNumbers()
    {
        // Dynamically allocate chemo cycle number and day of cycle number, and 'since' day counts
        var cycleIndex = 0;
        var cycleDay = 0;
        var depuisBp = 0;
        var depuisBed = 0;
        var depuisWalk = 0;
        List<int> clefs = _logDictionary.Keys.ToList();
        clefs.Sort();
        foreach (var k in clefs)
        {
            var foundLog = _logDictionary[k];
            if (foundLog.ChemoCycle)
            {
                if (foundLog.ChemoDayOne)
                {
                    cycleIndex++;
                    cycleDay = 1;
                }
                else
                {
                    cycleDay++;
                }

                foundLog.DynamicChemCycleNumber = cycleIndex;
                foundLog.DynamicChemCycleDay = cycleDay;
            }
            else
            {
                foundLog.DynamicChemCycleNumber = 0;
                foundLog.DynamicChemCycleDay = 0;
            }

            if (foundLog.DynamicBloodPressureMeasured)
            {
                depuisBp = 0;
            }
            else
            {
                depuisBp++;
            }

            if (foundLog.Exercised)
            {
                depuisWalk = 0;
            }
            else
            {
                depuisWalk++;
            }

            if (foundLog.BedChanged)
            {
                depuisBed = 0;
            }
            else
            {
                depuisBed++;
            }

            foundLog.DynamicDaysSinceBpCheck = depuisBp;
            foundLog.DynamicDaysSinceExercise = depuisWalk;
            foundLog.DynamicDaysSinceBedChange = depuisBed;
        }
    }

    private void SaveData()
    {
        var path = Path.Combine(AppManager.DataPath, "Data.journal");
        AppManager.CreateBackupDataFile(path);
        AppManager.PurgeOldBackups("journal", 10, 10);
        using var writer = new StreamWriter(path);
        // sort journal logs by date before saving
        List<int> clefs = _logDictionary.Keys.ToList();
        clefs.Sort();

        foreach (var clef in clefs)
        {
            var info = _logDictionary[clef];
            writer.WriteLine(info.Specification);
        }
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void JournalWindow_OnContentRendered(object sender, EventArgs e)
    {
        DateTextBlock.Text = $"{DateTime.Today:ddd MMM dd yyyy}";
        PopulateLeftPane();
        RefreshJournalListBox();
    }

    private void MinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentDate.Equals(_originDate))
        {
            return;
        } // Do not retract before start date of journal (22 07 2022)

        _currentDate = _currentDate.AddDays(-1);
        DateTextBlock.Text = $"{_currentDate:ddd MMM dd yyyy}";

        PopulateLeftPane();
        RefreshJournalListBox();
    }

    private void PlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        // if (_currentDayEdited)
        // {
        //     MessageBoxResult answer = MessageBox.Show("The current day has been edited! Change date regardless?"
        //         , "Changing date"
        //         , MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //     if (answer == MessageBoxResult.Cancel)
        //     {
        //         return;
        //     }
        // }

        if (DaysAgo(_currentDate) < 1)
        {
            return;
        }

        _currentDate = _currentDate.AddDays(1);
        DateTextBlock.Text = $"{_currentDate:ddd MMM dd yyyy}";

        PopulateLeftPane();
        RefreshJournalListBox();
    }

    private int DaysAgo(DateOnly d)
    {
        DateOnly aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        int count = 0;
        while (!d.Equals(aujourdhui))
        {
            count++;
            d = d.AddDays(1);
        }

        return count;
    }

    // private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
    // {
    //     bool chemCycle = ChemoCheckBox.IsChecked ?? false;
    //     bool chemFirst = ChemoDayOneCheckBox.IsChecked ?? false;
    //
    //     if (chemFirst && !chemCycle)
    //     {
    //         MessageBox.Show("Chemo cycle Day One ticked without Chemo Cycle ticked", "Inconsistent", MessageBoxButton.OK
    //             , MessageBoxImage.Information);
    //         return;
    //     }
    //
    //     DayLog log = new DayLog
    //     {
    //         DateIndex = _currentDate.DayNumber, DateLabel = DateSignificanceBox.Text.Trim(), // e.g. Chemo Cycle 2 Day 4
    //         SymptomsAndActions = SymptomsAndActionsTextBox.Text.Trim()
    //         , Pooed = PooCheckBox.IsChecked ?? false
    //         , ChemoCycle = chemCycle
    //         , ChemoDayOne = chemFirst
    //         , Exercised = ExerciseCheckBox.IsChecked ?? false
    //         , BedChanged = BedCheckBox.IsChecked ?? false
    //     };
    //
    //     log.MedsTaken.Clear();
    //     foreach (var tempMedicament in _tempMedicaments)
    //     {
    //         log.MedsTaken.Add(tempMedicament);
    //     }
    //
    //     if (_logDictionary.ContainsKey(log.DateIndex))
    //     {
    //         DayLog existing = _logDictionary[log.DateIndex];
    //         if (existing.Specification != log.Specification)
    //         {
    //             _logDictionary[log.DateIndex].Specification = log.Specification;
    //         }
    //     }
    //     else
    //     {
    //         _logDictionary.Add(log.DateIndex, log);
    //     }
    //
    //     _currentDayEdited = false;
    //     WarningBorderTop.Background = WarningBorder.Background = Brushes.LightGreen;
    //     RefreshJournalListBox();
    //     RefreshMedicamentNamesCombo();
    // }
    
    private void UpdateCurrentDay()
    {
        if (_suppressDayLogUpdate) return; // avoid saving data from form while populating form!
        
        var chemCycle = ChemoCheckBox.IsChecked ?? false;
        var chemFirst = ChemoDayOneCheckBox.IsChecked ?? false;

        if (chemFirst && !chemCycle)
        {
            MessageBox.Show("Chemo cycle Day One ticked without Chemo Cycle ticked", "Inconsistent", MessageBoxButton.OK
                , MessageBoxImage.Information);
            return;
        }

        var log = new DayLog
        {
            DateIndex = _currentDate.DayNumber, DateLabel = DateSignificanceBox.Text.Trim(), // e.g. Chemo Cycle 2 Day 4
            SymptomsAndActions = SymptomsAndActionsTextBox.Text.Trim()
            , Pooed = PooCheckBox.IsChecked ?? false
            , ChemoCycle = chemCycle
            , ChemoDayOne = chemFirst
            , Exercised = ExerciseCheckBox.IsChecked ?? false
            , BedChanged = BedCheckBox.IsChecked ?? false
        };

        log.MedsTaken.Clear();
        foreach (var tempMedicament in _tempMedicaments)
        {
            log.MedsTaken.Add(tempMedicament);
        }

        if (_logDictionary.ContainsKey(log.DateIndex))
        {
            DayLog existing = _logDictionary[log.DateIndex];
            if (existing.Specification != log.Specification)
            {
                _logDictionary[log.DateIndex].Specification = log.Specification;
            }
        }
        else
        {
            _logDictionary.Add(log.DateIndex, log);
        }

        //WarningBorderTop.Background = WarningBorder.Background = Brushes.LightGreen;
        RefreshJournalListBox();
        RefreshMedicamentNamesCombo();
    }

    private void RefreshJournalListBox()
    {
        ReallocateChemoNumbers();
        LogsListBox.Items.Clear();
        int targetIndex = -1;
        List<int> dateIndices = _logDictionary.Keys.ToList();
        dateIndices.Sort();
        int i = -1;
        foreach (var l in dateIndices)
        {
            i++;
            var log = _logDictionary[l];
            StackPanel vPanel = new StackPanel() {Width = LogsListBox.ActualWidth - 44};
            var dateSeul = DateOnly.FromDayNumber(log.DateIndex);
            int ago = DaysAgo(dateSeul);
            Brush borderBrush = (ago < 1) ? Brushes.CornflowerBlue : Brushes.Black;
            bool w = dateSeul.Equals(_currentDate);
            Brush borderBack = w ? Brushes.MediumAquamarine : Brushes.Transparent;
            if (w) targetIndex = i;
            double borderThickness = ago < 1 ? 3 : 1;
            vPanel.Children.Add(new TextBlock()
            {
                Text = $"{dateSeul:ddd dd MMMM yyyy} ({dateSeul.DayNumber})", FontFamily = new FontFamily("Arial")
                , FontSize = 16
                , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateBlue
            });

            if (!string.IsNullOrWhiteSpace(log.DateLabel))
            {
                vPanel.Children.Add(new TextBlock()
                    {Text = log.DateLabel, FontWeight = FontWeights.Medium, Foreground = Brushes.Blue});
            }

            if (log.ChemoCycle)
            {
                vPanel.Children.Add(new TextBlock()
                {
                    Text = $"CHEMO Cycle {log.DynamicChemCycleNumber}: Day {log.DynamicChemCycleDay}"
                    , FontWeight = FontWeights.Medium, Foreground = Brushes.Crimson
                });

                if (log.DynamicChemCycleDay is > 4 and < 16)
                {
                    vPanel.Children.Add(new TextBlock()
                    {
                        Text = "ESPECIALLY VULNERABLE (days 5 to 15)"
                        , FontWeight = FontWeights.Bold, Foreground = Brushes.Red
                    });
                }
            }

            if (log.DynamicDaysSinceBpCheck > 1)
            {
                vPanel.Children.Add(new TextBlock()
                {
                    Text = $"{log.DynamicDaysSinceBpCheck} days since BP checked"
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.Crimson
                });
            }

            if (log.DynamicDaysSinceExercise > 1)
            {
                var sPanel = new StackPanel() {Orientation = Orientation.Horizontal};
                for (int j = 0; j < log.DynamicDaysSinceExercise; j++)
                {
                    sPanel.Children.Add(new Ellipse()
                        {Width = 12, Height = 12, Fill = Brushes.Red, Margin = new Thickness(2, 0, 0, 0)});
                }

                sPanel.Children.Add(new TextBlock()
                {
                    Text = $"{log.DynamicDaysSinceExercise} days since taken exercise"
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.Crimson, Margin = new Thickness(6, 0, 0, 0)
                });
                vPanel.Children.Add(sPanel);
            }

            if (log.DynamicDaysSinceBedChange > 6)
            {
                var sPanel = new StackPanel() {Orientation = Orientation.Horizontal};

                sPanel.Children.Add(new Rectangle()
                    {Height = 12, Width = 2 * log.DynamicDaysSinceBedChange, Fill = Brushes.Red});
                sPanel.Children.Add(new TextBlock()
                {
                    Text = $"{log.DynamicDaysSinceBedChange} days since bedding changed"
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.Crimson, Margin = new Thickness(6, 0, 0, 0)
                });
                vPanel.Children.Add(sPanel);
            }

            DockPanel tickedPanel = new DockPanel();
            DockPanel unTickedPanel = new DockPanel();
            Border tickedBorder = new Border()
            {
                Child = tickedPanel, CornerRadius = new CornerRadius(4), Margin = new Thickness(6, 0, 0, 0)
                , Padding = new Thickness(4, 4, 4, 4), BorderBrush = Brushes.DarkGreen
                , BorderThickness = new Thickness(1)
            };
            Border unTickedBorder = new Border()
            {
                Child = unTickedPanel, CornerRadius = new CornerRadius(4), Margin = new Thickness(12, 0, 0, 0)
                , Padding = new Thickness(4, 4, 4, 4), BorderBrush = Brushes.DarkRed, BorderThickness = new Thickness(1)
            };
            StackPanel stackPanel = new StackPanel() {Orientation = Orientation.Horizontal};
            stackPanel.Children.Add(tickedBorder);
            stackPanel.Children.Add(unTickedBorder);
            vPanel.Children.Add(stackPanel);
            BlockReportTicked("During chemo cycle", log.ChemoCycle, tickedPanel, unTickedPanel);
            BlockReportTicked("Day One of chemo cycle", log.ChemoDayOne, tickedPanel, unTickedPanel);
            BlockReportTicked("Pooed", log.Pooed, tickedPanel, unTickedPanel);
            BlockReportTicked("Exercised", log.Exercised, tickedPanel, unTickedPanel);
            BlockReportTicked("Bed changed", log.BedChanged, tickedPanel, unTickedPanel);

            if (log.MedsTaken.Count > 0)
            {

                vPanel.Children.Add(new TextBlock()
                {
                    Text = "OTHER MEDS TAKEN", FontFamily = new FontFamily("Arial"), FontSize = 14
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateBlue
                });

                List<string> mList = new List<string>();
                foreach (var medicine in log.MedsTaken)
                {
                    mList.Add(MedicamentSummaryString(medicine));
                }

                mList.Sort(); // sort by time taken

                foreach (var m in mList)
                {
                    vPanel.Children.Add(new TextBlock()
                    {
                        Text
                            = m
                        , TextWrapping = TextWrapping.Wrap, FontFamily = _fixedFont
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(log.SymptomsAndActions))
            {
                vPanel.Children.Add(new TextBlock()
                {
                    Text = "SYMPTOMS & ACTIONS", FontFamily = new FontFamily("Arial"), FontSize = 14
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateBlue
                });
                vPanel.Children.Add(new TextBlock()
                {
                    Text = log.SymptomsAndActions, FontFamily = _fixedFont, FontSize = 14
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateBlue
                    , TextWrapping = TextWrapping.Wrap
                });

            }

            List<PersonProfile.Tension> tense = log.MyBpList;
            if (tense.Count>0)
            {
                vPanel.Children.Add(new TextBlock()
                {
                    Text = "BLOOD PRESSURES", FontFamily = new FontFamily("Arial"), FontSize = 14
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateGray
                });
                foreach (var tension in tense)
                {
                    string pulse =tension.Pulse>0 ? $" (P={tension.Pulse})":string.Empty;
                    vPanel.Children.Add(new TextBlock()
                    {
                        Text
                            = $"{tension.BprWhen.ToShortTimeString()} {tension.BpSystolic}/{tension.BpDiastolic}{pulse}"
                        , FontFamily = _fixedFont, FontSize = 14
                        , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateGray
                    });
                }
            }
            
            double wt = log.MyWeight;
            if (wt>0)
            {
                vPanel.Children.Add(new TextBlock()
                {
                    Text = $"WEIGHT {wt:#0.0} Kg", FontFamily = new FontFamily("Arial"), FontSize = 14
                    , FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateGray
                });
            }
            
            var surround = new Border()
            {
                Child = vPanel, Padding = new Thickness(6, 4, 6, 4), BorderThickness = new Thickness(borderThickness)
                , BorderBrush = borderBrush, Background = borderBack, CornerRadius = new CornerRadius(3)
            };
            LogsListBox.Items.Add(new ListBoxItem() {Content = surround});
        }

        if (LogsListBox.Items.Count > 0)
        {
            if (targetIndex >= 0)
            {
                LogsListBox.ScrollIntoView(LogsListBox.Items[targetIndex]);
            }
        }

        ReportButton.IsEnabled = true;
    }

    private void NewMedButton_OnClick(object sender, RoutedEventArgs e)
    {
        var medName = MedicamentNameCombo.Text.Trim();
        var medNumber = 0;
        if (MedicamentQuantityCombo.SelectedItem is ComboBoxItem {Tag: int x})
        {
            medNumber = x;
        }

        TimeSpan medSpan = TimeSpan.Zero;
        if (MedicamentTimeCombo.SelectedItem is ComboBoxItem {Tag: long t})
        {
            medSpan = TimeSpan.FromTicks(t);
        }

        // wrong or missing data

        var mistake = ContainsProhibitedChar(medName) || medNumber < 1;

        if (mistake)
        {
            MessageBox.Show("Error in values entered, maybe including prohibited characters", "New medicament"
                , MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newMed = new Medicament(medName, medNumber, medSpan);
        _tempMedicaments.Add(newMed);
        RefreshMedicamentsList();
        UpdateCurrentDay();
    }

    private static bool ContainsProhibitedChar(string testString)
    {
        if (testString.Contains(MedSeparator))
        {
            return true;
        }

        if (testString.Contains(ListSeparator))
        {
            return true;
        }

        if (testString.Contains(DayLogSeparator))
        {
            return true;
        }

        return false;
    }

    private void RefreshMedicamentsList()
    {
        // Clear entry text and combo boxes
        MedicamentNameCombo.Text = string.Empty;
        MedicamentQuantityCombo.SelectedIndex = 0;
        MedicamentTimeCombo.SelectedIndex = 0;
        MedicamentsListBox.Items.Clear();
        MedDelButton.IsEnabled = false;
        
        _tempMedicaments.Sort();
        foreach (var tincture in _tempMedicaments)
        {
            MedicamentsListBox.Items.Add(new ListBoxItem()
            {
                Content = new TextBlock()
                {
                    Text = MedicamentSummaryString(tincture)
                    , TextWrapping = TextWrapping.Wrap, FontFamily = _fixedFont
                    , Foreground = tincture.Taken.TotalHours < 12 ? Brushes.CornflowerBlue : Brushes.Tomato
                }
            });
        }
    }

    private void MedDelButton_OnClick(object sender, RoutedEventArgs e)
    {
        var i = MedicamentsListBox.SelectedIndex;
        if (i < 0) return;
        _tempMedicaments.RemoveAt(i);
        UpdateCurrentDay();
        RefreshMedicamentsList();
    }

    private static string MedicamentSummaryString(Medicament m)
    {
        return $"{m.Taken:hh\\:mm}: {m.Name} x {m.NumberOfDoses}";
    }

    private void PopulateLeftPane()
    {
        _suppressDayLogUpdate = true;
        
        // Clear entry text and combo boxes
        MedicamentNameCombo.Text = string.Empty;
        MedicamentQuantityCombo.SelectedIndex = 0;
        MedicamentTimeCombo.SelectedIndex = 0;
        SymptomsAndActionsTextBox.Text = string.Empty;

        int ago = DaysAgo(_currentDate);
        WhenTextBlock.Text = (ago == 0) ? "TODAY" : ago == 1 ? "Yesterday" : $"{ago} days ago";
        WhenTextBlock.Foreground = (ago == 0) ? Brushes.Green : Brushes.OrangeRed;
        int code = _currentDate.DayNumber;
        if (_logDictionary.ContainsKey(code))
        {
            DayLog log = _logDictionary[code];
            DateSignificanceBox.Text = log.DateLabel;
            ChemoCheckBox.IsChecked = log.ChemoCycle;
            ChemoDayOneCheckBox.IsChecked = log.ChemoDayOne;
            PooCheckBox.IsChecked = log.Pooed;
            ExerciseCheckBox.IsChecked = log.Exercised;
            BedCheckBox.IsChecked = log.BedChanged;
            CycleStausTextBlock.Text = log.DynamicChemCycleNumber > 0 ? $"CYCLE {log.DynamicChemCycleNumber} DAY {log.DynamicChemCycleDay}" : string.Empty;
            BpWarningTextBlock.Visibility = log.MyBpList.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            WeightWarningTextBlock.Visibility = log.MyWeight > 0 ? Visibility.Hidden : Visibility.Visible;
            _tempMedicaments.Clear();
            foreach (var m in log.MedsTaken)
            {
                _tempMedicaments.Add(m);
            }
            RefreshMedicamentsList();
            SymptomsAndActionsTextBox.Text = log.SymptomsAndActions;
        }
        else
        {
            DateSignificanceBox.Text = string.Empty;
            SymptomsAndActionsTextBox.Text = string.Empty;
            PooCheckBox.IsChecked = false;
            _tempMedicaments.Clear();
            RefreshMedicamentsList();
        }

        _suppressDayLogUpdate = false;
        //WarningBorderTop.Background = WarningBorder.Background = Brushes.LightGreen;
    }

    // private void SetDayDataChanged()
    // {
    //     // _currentDayEdited = true;
    //     //WarningBorderTop.Background = WarningBorder.Background = Brushes.Red;
    //     UpdateCurrentDay();
    // }

    private void DayData_Changed(object sender, TextChangedEventArgs e)
    {
        UpdateCurrentDay();
    }

    private void PooCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        UpdateCurrentDay();
    }

    private void BlockReportTicked(string rubric, bool value, DockPanel trueFrame, DockPanel falseFrame)
    {
        Brush pinceau = value ? Brushes.DarkGreen : Brushes.DarkRed;
        char sign = value ? 'ü' : 'û';
        DockPanel frame = value ? trueFrame : falseFrame;
        TextBlock bloc = new TextBlock() {Foreground = pinceau};
        bloc.Inlines.Add(new Run() {FontFamily = new FontFamily("Wingdings"), Text = $"{sign} "});
        bloc.Inlines.Add(new Run() {FontFamily = _fixedFont, Text = rubric});
        DockPanel.SetDock(bloc, Dock.Top);
        frame.Children.Add(bloc);
    }

    private void SymptomsAndActionsTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _symptomsEdited = true;
    }

    private void ReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        var reportDrugs = new StringBuilder();
        var reportSymptoms = new StringBuilder();
        var reportWeight = new StringBuilder();
        var reportTension = new StringBuilder();
        
        reportDrugs.Append($"{Environment.NewLine}SPECIAL MEDS TAKEN{Environment.NewLine}{Environment.NewLine}");
        reportSymptoms.Append($"{Environment.NewLine}SYMPTOMS & ACTIONS{Environment.NewLine}{Environment.NewLine}");
        reportTension.Append($"{Environment.NewLine}TENSION{Environment.NewLine}{Environment.NewLine}");
        reportWeight.Append($"{Environment.NewLine}WEIGHT{Environment.NewLine}{Environment.NewLine}");
            
        var dateIndices = _logDictionary.Keys.ToList();
        dateIndices.Sort();
        
        foreach (var l in dateIndices)
        {
            var log = _logDictionary[l];
            var dateSeul = DateOnly.FromDayNumber(log.DateIndex);
            DaysAgo(dateSeul);
            
            
            if (log.ChemoCycle && log.ChemoDayOne) // clear data from before last chemo day one
            {
                reportDrugs.Clear(); 
                reportSymptoms.Clear(); 
                reportTension.Clear(); 
                reportWeight.Clear(); 
                reportDrugs.Append($"{Environment.NewLine}SPECIAL MEDS TAKEN{Environment.NewLine}{Environment.NewLine}");
                reportSymptoms.Append($"{Environment.NewLine}SYMPTOMS & ACTIONS{Environment.NewLine}{Environment.NewLine}");
                reportTension.Append($"{Environment.NewLine}TENSION{Environment.NewLine}{Environment.NewLine}");
                reportWeight.Append($"{Environment.NewLine}WEIGHT{Environment.NewLine}{Environment.NewLine}");
            }

            StringBuilder dateLineBuild = new StringBuilder();
            dateLineBuild.Append($"{Environment.NewLine}{dateSeul:ddd dd MMMM yyyy}{Environment.NewLine}");
            
            if (log.ChemoCycle)
            {
                dateLineBuild.Append($"    CHEMO Cycle {log.DynamicChemCycleNumber}: Day {log.DynamicChemCycleDay}{Environment.NewLine}");
            }
            
            if (!string.IsNullOrEmpty(log.DateLabel))
            {
                dateLineBuild.Append($"    {log.DateLabel}{Environment.NewLine}");    
            }
            dateLineBuild.Append($"{Environment.NewLine}");
            
            if (log.MedsTaken.Count > 0)
            {
                var mList = new List<string>();
                foreach (var medicine in log.MedsTaken)
                {
                    mList.Add(MedicamentSummaryString(medicine));
                }

                mList.Sort(); // sort by time taken

                reportDrugs.Append(dateLineBuild);
                foreach (var m in mList)
                {
                    reportDrugs.Append($"        ► {m}{Environment.NewLine}");
                }
            }

            if (!string.IsNullOrWhiteSpace(log.SymptomsAndActions))
            {
                reportSymptoms.Append(dateLineBuild);
                reportSymptoms.Append($"        ► {log.SymptomsAndActions}{Environment.NewLine}");
            }

            if (log.MyWeight > 0)
            {
                reportWeight.Append(dateLineBuild);
                reportWeight.Append($"        ► Weight: {log.MyWeight:#0.0} kg{Environment.NewLine}");
            }

            var tense = log.MyBpList;
            if (tense.Count>0)
            {
                reportTension.Append(dateLineBuild);
                reportTension.Append($"        BLOOD PRESSURES{Environment.NewLine}");
                foreach (var tension in tense)
                {
                    var pulse =tension.Pulse>0 ? $" (P={tension.Pulse})":string.Empty;
                    reportTension.Append($"        {tension.BprWhen.ToShortTimeString()} {tension.BpSystolic}/{tension.BpDiastolic}{pulse}{Environment.NewLine}");
                }
            }
        }

        var report = new StringBuilder();
        report.Append($"{reportDrugs.ToString()}{Environment.NewLine}{Environment.NewLine}");
        report.Append($"{reportSymptoms.ToString()}{Environment.NewLine}");
        report.Append($"{reportTension.ToString()}{Environment.NewLine}");
        report.Append($"{reportWeight.ToString()}{Environment.NewLine}");
        
        var filePath = Path.Combine(AppManager.DataPath, "Report.txt");
        using var writer = new StreamWriter(filePath);
        writer.WriteLine(report.ToString());
        ReportButton.IsEnabled = false;

        System.Diagnostics.ProcessStartInfo pinfo = new(filePath) { UseShellExecute = true };
        System.Diagnostics.Process.Start(pinfo);

    }

    private void MedicamentsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var i = MedicamentsListBox.SelectedIndex;
        if (i < 0) return;
        MedDelButton.IsEnabled = true;
        var selected = _tempMedicaments[i];
        MedicamentNameCombo.Text = selected.Name;
    }

    private void SymptomsAndActionsTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _symptomsEdited = false;
    }

    private void SymptomsAndActionsTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (_symptomsEdited)
        {
            UpdateCurrentDay();
        }
    }

    private void JournalWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        SaveData();
    }
    
}
