using System.Collections.Generic;
using System.Text;

namespace Takap.Utility
{
    // INIファイル内のセクション
    public class IniSection
    {
        //
        // Fields
        // - - - - - - - - - - - - - - - - - - - -

        // ファイル先頭のコメント用の暗黙のセクション
        public const string UNDEF_SECTION = "___undef__section___";
        // 書き出すときに使うバッファー
        StringBuilder _sb;

        //
        // Props
        // - - - - - - - - - - - - - - - - - - - -

        // セクション名を取得する
        public string Name { get; private set; }

        // このセクションの中の要素のリストを取得する
        public List<IIniElement> Items { get; } = new List<IIniElement>();

        //
        // Constructors
        // - - - - - - - - - - - - - - - - - - - -

        public IniSection(string name)
        {
            this.Name = name;
        }

        //
        // Methods
        // - - - - - - - - - - - - - - - - - - - -

        // 現在のオブジェクトの内容を INI 形式の文字列に変換する
        public string SerializeToIniString(string newLine)
        {
            if (Items.Count == 0) return "";

            if (_sb == null) _sb = new StringBuilder();
            _sb.Clear();

            if (Name != UNDEF_SECTION)
            {
                _sb.Append("[");
                _sb.Append(Name);
                _sb.Append("]");
                _sb.Append(newLine);
            }
            for (int i = 0; i < Items.Count; i++)
            {
                _sb.Append(Items[i].ToString());
                _sb.Append(newLine);
            }

            //// セクションの末尾は空白行を1行追加する
            //IIniElement last = this.Items[this.Items.Count - 1];
            //if (last.Type != IniLineType.Text && string.IsNullOrEmpty(last.Value))
            //{
            //    _sb.AppendLine();
            //}

            return _sb.ToString();
        }

        // イデックスを全て更新する
        public void UpdateIndex()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is IInnerIniElement elem)
                {
                    elem.Index = i;
                }
            }
        }

        // try - parse パターンで要素を取得する
        public bool TryGetValue(string key, out IIniElement elem)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var _elem = Items[i];
                if (_elem.Key == key)
                {
                    elem = _elem;
                    return true;
                }
            }
            elem = null;
            return false;
        }

        //
        // InnerTypes
        // - - - - - - - - - - - - - - - - - - - -

        // インデックス更新用のインターフェース
        public interface IInnerIniElement
        {
            /// <summary>
            /// 要素の番号を取得します。
            /// </summary>
            int Index { get; set; }
        }
    }
}
