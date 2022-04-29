using System;
using System.Text;
using System.Threading.Tasks;

namespace Takap.Utility
{
    internal class AppMain
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await SaveAndLoad();

                await CreateNew();
            });

            Console.ReadLine();
        }

        // 既存のファイルを読み取って値を変更する
        public async static Task SaveAndLoad()
        {
            // .NET Core以降だけ必要が必要以下をコメントイン
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // (1) オブジェクトの作成とファイルの読み取り
            var doc = new IniDocument();
            await doc.Load(@"d:\sample.ini", Encoding.GetEncoding("Shift-JIS"));

            // (2) 既存のセクションを取得
            IniSection s1 = doc.GetSection("aaa");

            // (3) セクションを全部取得
            foreach (IniSection s in doc.GetSections())
            {
                string name = s.Name;
                // aaa
                // bbb
            }

            // (4) 値の取得 & 書き換え
            string value1 = doc.GetValue("bbb", "a");
            doc.SetValue("bbb", "a", "999");
            string value2 = doc.GetValue("bbb", "a");
            // > value1=1
            // > value1=999

            // (5) コメントの追加
            doc.SetText("aaa", "-x-x- comment -x-x-x");

            // (6) ファイル保存
            await doc.Save(@"d:\sample2.ini", Encoding.GetEncoding("Shift-JIS"));
        }

        public async static Task CreateNew()
        {
            var doc = new IniDocument();

            // (1) セクションだけ追加
            doc.CreateSection("aaa");

            // (2) セクションと値の追加
            doc.SetValue("section", "b", 1);
            doc.SetValue("section", "c", "2");
            doc.SetText("section", ";This is sample key & value."); // コメントの追加
            doc.SetValue("section", "d", 4.25);
            doc.SetText("section", ""); // セクションの最後に空白行

            // (3) セクションと値の追加
            doc.SetValue("section2", "b", 1);
            doc.SetValue("section2", "c", "2");
            doc.SetText("section2", ";This is sample key & value."); // コメントの追加
            doc.SetValue("section2", "d", 4.25);
            doc.SetText("section2", ""); // セクションの最後に空白行

            // (4) ファイル保存
            await doc.Save(@"d:\sample3.ini", Encoding.GetEncoding("Shift-JIS"));
        }
    }
}
