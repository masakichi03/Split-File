using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplitFile
{
    public partial class SplitFile : Form
    {
        public SplitFile()
        {
            InitializeComponent();
        }

		// 分割サイズ
		private const int DIV_BYTE = 1024 * 1024 * 1024;
		// 読込バッファサイズ
		private const int READ_BYTE = DIV_BYTE / 10;
		private const string FILE_NAME = @"C:\hoge.bin";


		/// <summary> [分割]ボタン押下時の処理 </summary>
		/// <remarks> ファイル分割処理 </remarks>
		/// <returns>戻り値</returns>
		/// <param name="sample">引数例</param>
		private void button1_Click(object sender, EventArgs e)
        {
			string sOutputMessage = "";			// 結果出力
			string sFilePath = @textBox1.Text;  // 入力ファイルパス

			// パス入力チェック
			if ( sFilePath != "" ) {				
				// 分割サイズ入力チェック
				if ( textBox2.Text != "" ) {
					int nSplitSize = int.Parse( textBox2.Text );    // 分割バイト数
					int nReadSize = nSplitSize / 10;                // 読込バイト数
					// 分割サイズ0チェック
					if ( nSplitSize != 0 ) {

						// 分割元ファイルを開く
						using ( FileStream rf = new FileStream( sFilePath, FileMode.Open, FileAccess.Read ) ) {
							int nReadByte = 0;
							int nDivCount = 0;
							long lLeftByte = rf.Length;
							byte[] byReadBuf = new byte[nReadSize];
							// 分割バイト数と読込バイト数から読込マップを作成
							List<int> readMap = new List<int>();
							for ( int i = 0; i < nSplitSize / nReadSize; i++ ) {
								readMap.Add( nReadSize );
							}
							if ( nSplitSize % nReadSize > 0 ) {
								readMap.Add( nSplitSize % nReadSize );
							}
							// 読込が終わるまで繰り返す
							while ( lLeftByte > 0 ) {
								// 分割ファイル名を取得
								string sDivFileName = Path.GetDirectoryName( sFilePath ) + "\\" +
									Path.GetFileNameWithoutExtension( sFilePath ) + "_" +
									( ++nDivCount ).ToString() + Path.GetExtension( sFilePath );

								// 分割ファイルを開く
								using ( FileStream wf = new FileStream( sDivFileName, FileMode.Create, FileAccess.Write ) ) {
									foreach ( int mapByte in readMap ) {
										// 分割元ファイルから読み込む
										nReadByte = rf.Read( byReadBuf, 0, (int)Math.Min( mapByte, lLeftByte ) );
										// 分割ファイルに書きこむ
										wf.Write( byReadBuf, 0, nReadByte );
										// 読込情報の設定
										lLeftByte -= nReadByte;
										if ( lLeftByte == 0 )
											break;
									}
								}
							}
						}
						sOutputMessage = "分割完了";
					} else {
						sOutputMessage = "分割サイズ 0設定";
					}
                } else {
					sOutputMessage = "分割サイズ未設定";
				}
			} else {
				sOutputMessage = "対象ファイル未設定";
			}

			// 結果出力
			textBox3.Text = sOutputMessage;
		}

		/// <summary> [対象ファイル]テキストボックス ドラッグ時 </summary>
		private void TextBox_DragEnter( object sender, DragEventArgs e )
		{
			//ファイルがドラッグされている場合、カーソルを変更する
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) ) {
				e.Effect = DragDropEffects.Copy;
			}
		}

		/// <summary> [対象ファイル]テキストボックス ドロップ時 </summary>
		private void TextBox_DragDrop( object sender, DragEventArgs e )
		{
			//ドロップされたファイルの一覧を取得
			string[] fileName = (string[])e.Data.GetData( DataFormats.FileDrop, false );
			if ( fileName.Length <= 0 ) {
				return;
			}

			// ドロップ先がTextBoxであるかチェック
			TextBox txtTarget = sender as TextBox;
			if ( txtTarget == null ) {
				return;
			}

			//TextBoxの内容をファイル名に変更
			txtTarget.Text = fileName[0];
		}

		// 分割ファイルを結合 TODO:未作成
		private void button2_Click(object sender, EventArgs e)
		{
			// 分割ファイルをリストアップ
			List<string> divFiles = new List<string>();
			divFiles.Add(@"C:\hoge.bin1");
			divFiles.Add(@"C:\hoge.bin2");
			divFiles.Add(@"C:\hoge.bin3");
			// 結合先ファイルを開く
			using (FileStream wf = new FileStream(FILE_NAME + "_new", FileMode.Create, FileAccess.Write))
			{
				int readByte = 0;
				long leftByte = 0;
				byte[] readBuf = new byte[READ_BYTE];
				foreach (string divFile in divFiles)
				{
					// 分割ファイルを開く
					using (FileStream rf = new FileStream(divFile, FileMode.Open, FileAccess.Read))
					{
						leftByte = rf.Length;
						while (leftByte > 0)
						{
							// 分割ファイルから読み込む
							readByte = rf.Read(readBuf, 0, (int)Math.Min(READ_BYTE, leftByte));
							// 結合先ファイルに書きこむ
							wf.Write(readBuf, 0, readByte);
							// 読込情報の設定
							leftByte -= readByte;
						}
					}
				}
			}
		}

        private void button2_Click_1( object sender, EventArgs e )
        {
			textBox2.Text = "1024";
		}

        private void button3_Click( object sender, EventArgs e )
        {
			textBox2.Text = "1048576";
		}
    }
}
