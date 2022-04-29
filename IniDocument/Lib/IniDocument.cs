using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Takap.Utility
{
    // INIファイルを表します。
    public class IniDocument
    {
        //
        // Fields
        // - - - - - - - - - - - - - - - - - - - -

        // セクションのリスト
        private List<IniSection> _sections = new List<IniSection>();

        //
        // Props
        // - - - - - - - - - - - - - - - - - - - -

        // 保存するときのINIファイル改行文字の指定
        public string NewLine { get; set; } = "\r\n";

        //
        // 読み書き
        // - - - - - - - - - - - - - - - - - - - -

        // 指定したファイルパスに現在のオブジェクトの内容を保存する
        public async Task Save(string filePath, Encoding encoding)
        {
            using (var sw = new StreamWriter(filePath, false, encoding))
            {
                await Save(sw);
            }
        }

        // 指定したストリームに現在のオブジェクトの内容を保存する
        public async Task Save(StreamWriter sw)
        {
            await Task.Run(() =>
            {
                sw.NewLine = NewLine;
                for (int i = 0; i < _sections.Count; i++)
                {
                    IniSection section = _sections[i];
                    sw.Write(section.SerializeToIniString(NewLine));
                }
            });
        }

        // 指定したファイルパスを読み取って内容をロードします。
        public async Task Load(string filePath, Encoding encoding)
        {
            using (var sr = new StreamReader(filePath, encoding))
            {
                await Load(sr);
            }
        }

        // 指定したストリームを読み取って内容をロードします。
        public async Task Load(StreamReader sr)
        {
            await Task.Run(() =>
            {
                string[] lines = sr.ReadToEnd().Split(new string[] { NewLine }, StringSplitOptions.None);
                var currentSection = new IniSection(IniSection.UNDEF_SECTION);
                _sections.Add(currentSection);

                foreach (string line in lines)
                {
                    string _line = line.Trim();
                    if (_line.StartsWith("[") && _line.EndsWith("]"))
                    {
                        currentSection = new IniSection(_line.TrimStart('[').TrimEnd(']'));
                        _sections.Add(currentSection);
                    }
                    else
                    {
                        if (_line.Contains("="))
                        {
                            string[] items = _line.Split('=');
                            if (items.Length == 1)
                            {
                                currentSection.Items.Add(new IniKeyValuePair(items[0], ""));
                            }
                            else if (items.Length == 2)
                            {
                                currentSection.Items.Add(new IniKeyValuePair(items[0], items[1]));
                            }
                            else if (items.Length > 2)
                            {
                                int index = line.IndexOf('='); // この最初に見つかった'='以降は値扱い
                                string key = line.Substring(0, index);
                                string value = line.Substring(index + 1, line.Length - index - 1);
                                currentSection.Items.Add(new IniKeyValuePair(key, value));
                            }
                        }
                        else
                        {
                            currentSection.Items.Add(new IniText(line)); // そのまま
                        }
                    }
                }
            });
        }

        //
        // セクションの操作
        // - - - - - - - - - - - - - - - - - - - - -

        // セクションを列挙する
        public IEnumerable<IniSection> GetSections()
        {
            foreach (IniSection s in _sections)
            {
                yield return s;
            }
        }

        // セクションを取得する
        //  → 存在しない場合例外が出る
        public IniSection GetSection(string name)
        {
            if (!(TryGetSection(name, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={name}");
            return section;
        }

        // try - parse パターンでセクションを取得する
        public bool TryGetSection(string name, out IniSection section)
        {
            for (int i = 0; i < _sections.Count; i++)
            {
                IniSection s = _sections[i];
                if (s.Name == name)
                {
                    section = s;
                    return true;
                }
            }
            section = null;
            return false;
        }

        // セクションを追加する
        public IniSection CreateSection(string name)
        {
            if (TryGetSection(name, out IniSection section))
            {
                return section;
            }
            else
            {
                var s = new IniSection(name);
                _sections.Add(new IniSection(name));
                return s;
            }
        }

        // セクションを削除する
        //  → 存在しないセクションでも例外でない
        public void RemoveSection(string name)
        {
            IniSection s = _sections.Find(p => p.Name == name);
            if (s == null)
            {
                return;
            }
            _sections.Remove(s);
        }

        // セクションの位置を変更する
        public void ChangeIndex(string name, int newIndex)
        {
            _sections.ChangeOrder(p => p.Name == name, newIndex);
        }

        //
        // セクション内の操作
        // - - - - - - - - - - - - - - - - - - - - -        

        // セクション内の要素を全て取得する
        public IEnumerable<IIniElement> GetElements(string sectionName)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            for (int i = 0; i < section.Items.Count; i++)
                yield return section.Items[i];
        }

        // 指定した値を設定する
        //  → セクション・キーが無い場合末尾に新規作成して追加
        public void SetValue<T>(string sectionName, string key, T value)
        {
            IniSection s;
            if (!(TryGetSection(sectionName, out s)))
            {
                s = CreateSection(sectionName);
            }

            if (s.TryGetValue(key, out IIniElement elem))
            {
                elem.Value = value.ToString();
            }
            else
            {
                s.Items.Add(new IniKeyValuePair(key, value.ToString()));
            }
        }

        // コメントなどのテキストを追加する
        //  → このメソッドで key=value と入力してもこのオブジェクト中はテキスト扱いになる
        public void SetText(string sectionName, string text)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            section.Items.Add(new IniText(text));
        }

        // 途中に値を挿入する
        public void InsertValue(string sectionName, int index, string key, string value)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            if (section.TryGetValue(key, out IIniElement elem))
                throw new InvalidOperationException($"Already exists. key={key}");

            section.Items.Insert(index, new IniKeyValuePair(key, value));
        }

        // 途中にコメントなどのテキストを挿入する
        public void InsertText(string sectionName, int index, string text)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            section.Items.Insert(index, new IniText(text));
        }

        // 指定したキーの値を取得する
        //  → 存在しない場合例外が出る
        public string GetValue(string sectionName, string key)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            if (!(section.TryGetValue(key, out IIniElement elem)))
                throw new KeyNotFoundException($"Key not found. name={key}");

            return elem.Value;
        }

        // 値を try - parse パターンで取得する
        public bool TryGetValue(string sectionName, string key, out IIniElement elem)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            if (section.TryGetValue(key, out IIniElement _elem))
            {
                elem = _elem;
                return true;
            }

            elem = null;
            return false;
        }

        // 指定した位置のテキストを取得する
        //  → コメント行はキーが無いのでインデックスアクセスする
        public string GetText(string sectionName, int index)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            return section.Items[index].Value;
        }

        // 指定したキーを削除する
        public void RemoveValue(string sectionName, string key)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            var item = section.Items.Find(p => p is IniKeyValuePair pair && pair.Key == key);
            if (item != null) section.Items.Remove(item);
        }

        // 指定したインデックスの要素を削除します
        public void RemoveText(string sectionName, int index)
        {
            if (!(TryGetSection(sectionName, out IniSection section)))
                throw new KeyNotFoundException($"Section not found. name={sectionName}");

            section.Items.RemoveAt(index);
        }
    }
}
