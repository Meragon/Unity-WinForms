namespace System.Windows.Forms
{
    public static class swfHelper
    {
        public static float GetDeltaTime()
        {
            if (ApiHolder.Timing == null)
                return 1f / 60f;
            return ApiHolder.Timing.DeltaTime;
        }
        public static void DoScroll(this ScrollBar sb, ScrollEventType type)
        {
            var value = sb.Value;
            int newValue = value;

            switch (type)
            {
                case ScrollEventType.First:
                    newValue = sb.Minimum;
                    break;

                case ScrollEventType.Last:
                    newValue = sb.Maximum - sb.LargeChange + 1;
                    break;

                case ScrollEventType.SmallDecrement:
                    newValue = Math.Max(value - sb.SmallChange, sb.Minimum);
                    break;

                case ScrollEventType.SmallIncrement:
                    newValue = Math.Min(value + sb.SmallChange, sb.Maximum - sb.LargeChange + 1);
                    break;

                case ScrollEventType.LargeDecrement:
                    newValue = Math.Max(value - sb.LargeChange, sb.Minimum);
                    break;

                case ScrollEventType.LargeIncrement:
                    newValue = Math.Min(value + sb.LargeChange, sb.Maximum - sb.LargeChange + 1);
                    break;

                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:

                    // not implemented yet.
                    break;
            }
            
            sb.Value = newValue;
        }
    }
}
