using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class MonthCalendar : Control
    {
        private readonly Pen borderPen = new Pen(Color.Black);
        private readonly List<Control> _daysControls = new List<Control>();
        private string[] _daysShort;
        private DayOfWeek _firstDayOfWeek;
        private string[] _months;
        private DateTime _selectedDate;
        private bool _showToday;
        private DateTime _todayDate;
        private DateTime _value;

        private Button todayButton;

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public int CellWidth { get; set; }
        public string[] DaysShort
        {
            get { return _daysShort; }
            set
            {
                if (value == null) throw new NullReferenceException("value is null");
                if (value.Length != 7) throw new ArgumentException("value length is not equal 7");
                _daysShort = value;
            }
        }
        public DayOfWeek FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
            set
            {
                if (_firstDayOfWeek != value)
                {
                    _firstDayOfWeek = value;
                    SetDate(_selectedDate);
                }
            }
        }
        public string[] Months
        {
            get { return _months; }
            set
            {
                if (value == null) throw new NullReferenceException("value is null");
                if (value.Length != 12) throw new ArgumentException("value length is not equal 12");
                _months = value;
            }
        }
        public bool ShowToday
        {
            get { return _showToday; }
            set
            {
                if (_showToday != value)
                {
                    _showToday = value;
                    if (_showToday)
                    {
                        todayButton = new Button();
                        todayButton.Location = new Point(3 + 2 * CellWidth, 33 + 7 * 15);
                        todayButton.Text = "Today: " + TodayDate.ToShortDateString();
                        todayButton.TextAlign = ContentAlignment.MiddleLeft;
                        todayButton.Size = new Size(CellWidth * 4, 20);
                        todayButton.BackColor = Color.Transparent;
                        todayButton.HoverColor = Color.Transparent;
                        todayButton.BorderColor = Color.Transparent;
                        todayButton.BorderHoverColor = Color.Transparent;
                        todayButton.Click += (s, a) =>
                        {
                            Value = TodayDate;
                        };
                        Controls.Add(todayButton);
                    }
                    else
                    {
                        if (todayButton != null)
                            todayButton.Dispose();
                        todayButton = null;
                    }
                }
            }
        }
        public Color TitleBackColor { get; set; }
        public Color TitleForeColor { get; set; }
        public DateTime TodayDate
        {
            get { return _todayDate; }
            set
            {
                _todayDate = value;
                if (todayButton != null)
                    todayButton.Text = "Today: " + value.ToShortDateString();
            }
        }
        public DateTime Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetDate(_value);
                DateChanged(this, new DateRangeEventArgs(_value, _value));
            }
        }

        public MonthCalendar()
        {
            _firstDayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            BackColor = Color.White;
            CellWidth = 31;
            DaysShort = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
            Size = new Size(CellWidth * 7 + 6, 158);
            ShowToday = true;
            TitleBackColor = Color.Transparent;
            TitleForeColor = Color.FromArgb(42, 42, 42);
            TodayDate = DateTime.Now;
            Value = DateTime.Now;

            Button prevMonthButton = new Button();
            prevMonthButton.Image = ApplicationBehaviour.GdiImages.ArrowLeft;
            prevMonthButton.ImageColor = Color.FromArgb(48, 48, 48);
            prevMonthButton.Size = new Size(16, 16);
            prevMonthButton.Location = new Point(4, 8);
            prevMonthButton.BorderColor = Color.Transparent;
            prevMonthButton.BorderHoverColor = Color.Transparent;
            prevMonthButton.BackColor = Color.Transparent;
            prevMonthButton.Click += (s, a) => { SetDate(_selectedDate.AddMonths(-1)); };
            Controls.Add(prevMonthButton);

            Button nextMonthButton = new Button();
            nextMonthButton.Anchor = AnchorStyles.Right;
            nextMonthButton.Image = ApplicationBehaviour.GdiImages.ArrowRight;
            nextMonthButton.ImageColor = Color.FromArgb(48, 48, 48);
            nextMonthButton.Size = new Size(16, 16);
            nextMonthButton.Location = new Point(Width - nextMonthButton.Width - 4, 8);
            nextMonthButton.BorderColor = Color.Transparent;
            nextMonthButton.BorderHoverColor = Color.Transparent;
            nextMonthButton.BackColor = Color.Transparent;
            nextMonthButton.Click += (s, a) => { SetDate(_selectedDate.AddMonths(1)); };
            Controls.Add(nextMonthButton);

            _months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        }

        public void SetDate(DateTime date)
        {
            _selectedDate = date;

            // TODO: update only if month & year are changed.

            for (int i = 0; i < _daysControls.Count; i++)
                _daysControls[i].Dispose();
            _daysControls.Clear();

            var daysShort = DaysShort;
            List<string> daysShortList = new List<string>();
            for (int i = (int)(_firstDayOfWeek); i < 7; i++)
                daysShortList.Add(daysShort[i]);
            for (int i = 0; i < (int)_firstDayOfWeek; i++)
                daysShortList.Add(daysShort[i]);
            daysShort = daysShortList.ToArray();

            var monthStartDayOfWeek = new DateTime(date.Year, date.Month, 1).DayOfWeek;
            DateTime startDate = new DateTime(date.Year, date.Month, 1).AddDays(-(int)monthStartDayOfWeek + (int)_firstDayOfWeek);

            for (int row = 0; row < 7; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (row == 0) // Header.
                    {
                        Label labelDayOfWeek = new Label();
                        labelDayOfWeek.Font = new Font("Arial", 11);
                        labelDayOfWeek.Location = new Point(3 + CellWidth * column, 33);
                        labelDayOfWeek.Size = new Size(CellWidth, 20);
                        labelDayOfWeek.Text = daysShort[column];
                        labelDayOfWeek.TextAlign = ContentAlignment.TopCenter;
                        labelDayOfWeek.Padding = new Padding();
                        Controls.Add(labelDayOfWeek);

                        _daysControls.Add(labelDayOfWeek);
                    }
                    else
                    {
                        var dayColor = date.Month == startDate.Month ? Color.FromArgb(42, 42, 42) : Color.Gray;
                        var dayDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);

                        Button dayButton = new Button();
                        dayButton.ForeColor = dayColor;
                        dayButton.Size = new Size(CellWidth, 15);
                        dayButton.Location = new Point(3 + CellWidth * column, 33 + 15 * row);
                        dayButton.BackColor = Color.Transparent;
                        dayButton.BorderColor = Color.Transparent;
                        dayButton.BorderHoverColor = Color.FromArgb(112, 192, 231);
                        dayButton.Text = startDate.Day.ToString();
                        dayButton.Click += (s, a) =>
                        {
                            Value = dayDate;
                        };
                        Controls.Add(dayButton);

                        if (TodayDate.Year == startDate.Year && TodayDate.Month == startDate.Month && TodayDate.Day == startDate.Day)
                        {
                            dayButton.ForeColor = Color.FromArgb(0, 102, 204);
                            dayButton.BorderColor = dayButton.ForeColor;
                            dayButton.BorderHoverColor = dayButton.BorderColor;
                            dayButton.BackColor = Color.FromArgb(229, 243, 251);
                            dayButton.HoverColor = dayButton.BackColor;
                        }

                        if (Value.Year == dayDate.Year && Value.Month == dayDate.Month && Value.Day == dayDate.Day)
                        {
                            dayButton.BorderColor = Color.FromArgb(38, 160, 218);
                            dayButton.BorderHoverColor = dayButton.BorderColor;
                            dayButton.BackColor = Color.FromArgb(203, 232, 246);
                            dayButton.HoverColor = dayButton.BackColor;
                        }

                        startDate = startDate.AddDays(1);
                        _daysControls.Add(dayButton);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            // Header.
            e.Graphics.uwfFillRectangle(TitleBackColor, 0, 0, Width, 32);
            e.Graphics.uwfDrawString(_months[_selectedDate.Month - 1] + " " + _selectedDate.Year.ToString(), Font, TitleForeColor, 0, 0, Width, 32, ContentAlignment.MiddleCenter);

            if (ShowToday)
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(0, 102, 204)), CellWidth * 1, 141, CellWidth - 2, 13);

            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        public override void Refresh()
        {
            base.Refresh();
            SetDate(_selectedDate);

            if (todayButton != null)
            {
                todayButton.Location = new Point(3 + 2 * CellWidth, 33 + 7 * 15);
            }
        }

        public event DateRangeEventHandler DateChanged = delegate { };
    }
}
