namespace Takap.Utility
{
    // INIファイル内の要素を表すインターフェース
    public interface IIniElement : IniSection.IInnerIniElement
    {
        /// <summary>
        /// オブジェクトの種類を取得します。
        /// </summary>
        IniLineType Type { get; }

        /// <summary>
        /// 要素の番号を取得します。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// この要素のキーを取得します。
        /// </summary>
        /// <remarks>
        /// テキスト要素の時は何も設定されません。
        /// </remarks>
        string Key { get; }

        /// <summary>
        /// 要素の値を設定または取得します。
        /// </summary>
        string Value { get; set; }
    }
}
