namespace Takap.Utility
{
    // INIファイル内の要素を表すインターフェース
    public interface IIniElement : IniSection.IInnerIniElement
    {
        // オブジェクトの種類を取得します。
        IniLineType Type { get; }

        // 要素の番号を取得します。
        int Index { get; }

        // この要素のキーを取得します。
        // テキスト要素の時は何も設定されません。
        string Key { get; }

        // 要素の値を設定または取得します。
        string Value { get; set; }
    }
}
