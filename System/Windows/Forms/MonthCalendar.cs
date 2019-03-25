namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    public class MonthCalendar : Control
    {
        internal CultureInfo uwfCultureInfo = Application.CurrentCulture;
        internal Padding uwfInnerPadding = new Padding(1);

        private readonly Pen borderPen = new Pen(SystemColors.ControlDark);
        private readonly List<Control> daysControls = new List<Control>();
        private readonly Pen todayPen = new Pen(SystemColors.HotTrack);

        private DayOfWeek firstDayOfWeek;
        private DateTime selectedDate;
        private bool showToday;
        private Button todayButton;
        private DateTime todayDate;
        private DateTime value;

        public MonthCalendar()
        {
            firstDayOfWeek = uwfCultureInfo.DateTimeFormat.FirstDayOfWeek;

            BackColor = Color.White;
            CellWidth = 22;
            ShowToday = true;
            TitleBackColor = Color.Transparent;
            TitleForeColor = Color.FromArgb(42, 42, 42);
            TodayDate = DateTime.Now;
            Value = DateTime.Now;

            var prevMonthButton = new RepeatButton();
            prevMonthButton.CooldownBetweenClicks = .4f;
            prevMonthButton.Image = uwfAppOwner.Resources.ArrowLeft;
            prevMonthButton.Size = new Size(16, 16);
            prevMonthButton.Location = new Point(4, 10);
            prevMonthButton.Click += (s, a) => { SetDate(selectedDate.AddMonths(-1)); };
            Controls.Add(prevMonthButton);

            var nextMonthButton = new RepeatButton();
            nextMonthButton.CooldownBetweenClicks = .4f;
            nextMonthButton.Anchor = AnchorStyles.Right;
            nextMonthButton.Image = uwfAppOwner.Resources.ArrowRight;
            nextMonthButton.Size = new Size(16, 16);
            nextMonthButton.Location = new Point(Width - nextMonthButton.Width - 4, 10);
            nextMonthButton.Click += (s, a) => { SetDate(selectedDate.AddMonths(1)); };
            Controls.Add(nextMonthButton);

            SetArrowButtonDefaultStyle(prevMonthButton);
            SetArrowButtonDefaultStyle(nextMonthButton);
        }

        public event DateRangeEventHandler DateChanged = delegate { };

        public int CellWidth { get; set; }
        public DayOfWeek FirstDayOfWeek
        {
            get { return firstDayOfWeek; }
            set
            {
                if (firstDayOfWeek != value)
                {
                    firstDayOfWeek = value;
                    SetDate(selectedDate);
                }
            }
        }
        public bool ShowToday
        {
            get { return showToday; }
            set
            {
                if (showToday != value)
                {
                    showToday = value;
                    if (showToday)
                        CreateTodayButton();
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
            get { return todayDate; }
            set
            {
                todayDate = value;
                UpdateTodayDate();
            }
        }
        public DateTime Value
        {
            get { return value; }
            set
            {
                this.value = value;
                SetDate(this.value);
                DateChanged(this, new DateRangeEventArgs(this.value, this.value));
            }
        }

        internal Color uwfBorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(164, 162);
            }
        }

        public void SetDate(DateTime date)
        {
            selectedDate = date;

            // TODO: update only if month & year are changed.

            for (int i = 0; i < daysControls.Count; i++)
                daysControls[i].Dispose();
            daysControls.Clear();

            var monthStartDayOfWeek = new DateTime(date.Year, date.Month, 1).DayOfWeek;
            var startDate = new DateTime(date.Year, date.Month, 1).AddDays(-(int)monthStartDayOfWeek + (int)firstDayOfWeek);
            var labelDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var labelFont = new Font("Arial", 11);

            var leftOffset = uwfInnerPadding.Left + 4; // 4 = 1 border + 3 pixels.

            for (int row = 0; row < 7; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (row == 0) // Header.
                    {
                        var labelDayOfWeek = new Label();
                        labelDayOfWeek.AutoSize = false;
                        labelDayOfWeek.Font = labelFont;
                        labelDayOfWeek.ForeColor = SystemColors.InfoText;
                        labelDayOfWeek.Location = new Point(leftOffset + CellWidth * column, 33);
                        labelDayOfWeek.Size = new Size(CellWidth, 20);
                        labelDayOfWeek.Text = uwfCultureInfo.DateTimeFormat.GetShortestDayName(labelDate.DayOfWeek);
                        labelDayOfWeek.TextAlign = ContentAlignment.TopCenter;
                        labelDayOfWeek.Padding = new Padding();
                        Controls.Add(labelDayOfWeek);

                        daysControls.Add(labelDayOfWeek);

                        labelDate = labelDate.AddDays(1);
                    }
                    else
                    {
                        var dayColor = date.Month == startDate.Month ? SystemColors.InfoText : SystemColors.GrayText;
                        var dayDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);

                        var dayButton = new Button();
                        dayButton.ForeColor = dayColor;
                        dayButton.Size = new Size(CellWidth, 15);
                        dayButton.Location = new Point(leftOffset + CellWidth * column, 33 + 15 * row);
                        dayButton.BackColor = Color.Transparent;
                        dayButton.uwfBorderColor = Color.Transparent;
                        dayButton.uwfBorderHoverColor = Color.FromArgb(112, 192, 231);
                        dayButton.uwfHoverColor = Color.FromArgb(229, 243, 251);
                        dayButton.Text = startDate.Day.ToString();
                        dayButton.Padding = new Padding();
                        dayButton.Click += (s, a) =>
                        {
                            Value = dayDate;
                        };
                        Controls.Add(dayButton);

                        if (TodayDate.Year == startDate.Year && TodayDate.Month == startDate.Month && TodayDate.Day == startDate.Day)
                        {
                            dayButton.ForeColor = Color.FromArgb(0, 102, 204);
                            dayButton.uwfBorderColor = dayButton.ForeColor;
                            dayButton.uwfBorderHoverColor = dayButton.uwfBorderColor;
                            dayButton.uwfHoverColor = dayButton.BackColor;
                        }

                        if (Value.Year == dayDate.Year && Value.Month == dayDate.Month && Value.Day == dayDate.Day)
                        {
                            dayButton.BackColor = Color.FromArgb(203, 232, 246);
                            dayButton.uwfBorderColor = Color.FromArgb(38, 160, 218);
                            dayButton.uwfBorderHoverColor = dayButton.uwfBorderColor;
                            dayButton.uwfHoverColor = dayButton.BackColor;
                        }

                        startDate = startDate.AddDays(1);
                        daysControls.Add(dayButton);
                    }
                }
            }
        }
        public override void Refresh()
        {
            base.Refresh();
            SetDate(selectedDate);

            if (todayButton != null)
            {
                todayButton.Location = new Point(3 + 2 * CellWidth, 33 + 7 * 15);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            var newDate = selectedDate.AddMonths(e.Delta > 0 ? -1 : 1);

            SetDate(newDate);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            // Header.
            e.Graphics.uwfFillRectangle(TitleBackColor, 0, 0, Width, 32);
            e.Graphics.uwfDrawString(uwfCultureInfo.DateTimeFormat.MonthNames[selectedDate.Month - 1] + " " + selectedDate.Year, Font, TitleForeColor, 0, 8, Width, 32, ContentAlignment.TopCenter);

            if (ShowToday)
                e.Graphics.DrawRectangle(todayPen, CellWidth * 1, 141, CellWidth - 2, 13);

            e.Graphics.DrawRectangle(borderPen, uwfInnerPadding.Left, uwfInnerPadding.Top, Width - uwfInnerPadding.Horizontal, Height - uwfInnerPadding.Vertical);
        }

        private static string GetTodayText(CultureInfo info)
        {
            if (info != null)
                switch (info.Name)
                {
                    case "af-ZA": return "Vandag";
                    case "ar-AE": return "اليوم";
                    case "eu-ES": return "Gaur";
                    case "be-BY": return "Сёння";
                    case "bg-BG": return "Днес";
                    case "ca-ES": return "Avui";
                    case "zh-CN":
                    case "zh-CHS":
                    case "zh-CHT": return "今天";
                    case "cs-CZ": return "Dnes";
                    case "da-DK": return "I dag";
                    case "nl-BE": return "Vandaag";
                    case "et-EE": return "Täna";
                    case "fo-FO": break;
                    case "fi-FI ": return "Tänään";
                    case "fr-FR": return "Aujourd'hui";
                    case "de-DE": return "Heute";
                    case "el-GR": return "Σήμερα";
                    case "he-IL": return "היום";
                    case "hu-HU": return "Ma";
                    case "is-IS": return "Í dag";
                    case "id-ID": return "Hari ini";
                    case "it-IT": return "Oggi";
                    case "ja-JP": return "今日";
                    case "ko-KR": return "오늘";
                    case "lv-LV": return "Šodien";
                    case "lt-LT": return "Šiandien";
                    case "nb-NO": return "I dag";
                    case "pl-PL": return "Dzisiaj";
                    case "pt-PT": return "Hoje";
                    case "ro-RO": return "Astăzi";
                    case "ru-RU": return "Сегодня";
                    case "Lt-sr-SP": return "Данас";
                    case "sk-SK": return "Dnes";
                    case "sl-SI": return "Danes";
                    case "es-ES": return "Hoy";
                    case "sv-SE": return "I dag";
                    case "th-TH": return "ในวันนี้";
                    case "tr-TR": return "Bugün";
                    case "uk-UA": return "Сьогодні";
                    case "vi-VN": return "Hôm nay";
                }

            return "Today";
        }
        private static void SetArrowButtonDefaultStyle(Button button)
        {
            button.BackColor = Color.Transparent;

            button.uwfBorderColor = Color.Transparent;
            button.uwfBorderHoverColor = Color.Transparent;
            button.uwfBorderSelectColor = Color.Transparent;
            button.uwfHoverColor = Color.Transparent;
            button.uwfImageColor = Color.FromArgb(48, 48, 48);
            button.uwfImageHoverColor = Color.FromArgb(0, 102, 204);
        }

        private void CreateTodayButton()
        {
            todayButton = new TodayButton();
            todayButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; 
            todayButton.BackColor = Color.Transparent;
            todayButton.Font = new Font("Arial", 11); // 12 is too big.
            todayButton.Location = new Point(3 + 2 * CellWidth, 33 + 7 * 15);
            todayButton.TextAlign = ContentAlignment.MiddleLeft;
            todayButton.Size = new Size(Width - uwfInnerPadding.Right - todayButton.Location.X - 4, 20);

            todayButton.uwfBorderColor = Color.Transparent;
            todayButton.uwfBorderHoverColor = Color.Transparent;
            todayButton.uwfBorderSelectColor = Color.Transparent;
            todayButton.uwfHoverColor = Color.Transparent;

            todayButton.Click += (s, a) =>
            {
                Value = TodayDate;
            };
            Controls.Add(todayButton);

            UpdateTodayDate();
        }
        private void UpdateTodayDate()
        {
            if (todayButton != null)
                todayButton.Text = GetTodayText(uwfCultureInfo) + ": " + todayDate.ToString(uwfCultureInfo.DateTimeFormat.ShortDatePattern);
        }

        /// <summary>
        /// With fore hover color.
        /// </summary>
        private class TodayButton : Button
        {
            public Color ForeBaseColor = SystemColors.InfoText;
            public Color ForeHoverColor = SystemColors.HotTrack;

            protected override void OnPaint(PaintEventArgs e)
            {
                if (hovered)
                    ForeColor = ForeHoverColor;
                else
                    ForeColor = ForeBaseColor;

                base.OnPaint(e);
            }
        }
    }
}
