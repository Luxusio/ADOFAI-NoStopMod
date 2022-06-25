namespace NoStopMod.InputFixer
{
    public struct KeyEvent
    {
        public KeyEvent(long tick, ushort keyCode, bool press)
        {
            this.tick = tick;
            this.keyCode = keyCode;
            this.press = press;
        }

        public long tick;
        public ushort keyCode;
        public bool press;
    }
}