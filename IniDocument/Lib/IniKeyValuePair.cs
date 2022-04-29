namespace Takap.Utility
{
    // INIフィあるのキーと値の組み合わせを表す
    public class IniKeyValuePair : IIniElement
    {
        public IniLineType Type { get; }
        public int Index { get; set; }
        public string Key { get; private set; }
        public string Value { get; set; }
        public IniKeyValuePair(string key, string initValue)
        {
            Type = IniLineType.KeyValue;
            Index = 0;
            Key = key;
            Value = initValue;
        }
        public override string ToString() => $"{Key}={Value}";
    }
}
