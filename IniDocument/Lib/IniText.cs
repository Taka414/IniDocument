namespace Takap.Utility
{
    // コメントなどのテキスト行を表す
    public class IniText : IIniElement
    {
        public IniLineType Type { get; }
        public int Index { get; set; }
        public string Key { get; }
        public string Value { get; set; }
        public IniText(string initValue)
        {
            Type = IniLineType.Text;
            Index = 0;
            Key = "";
            Value = initValue;
        }
        public override string ToString() => Value;
    }
}
